using System;
using System.Collections.Generic;
using CoreScripts.BattlePlayables;
using CoreScripts.Managers;
using Cysharp.Threading.Tasks;
using TalesOfRadiance.Scripts.Battle.BattleComponents;
using TalesOfRadiance.Scripts.Battle.BattleComponents.RuntimeClass;
using TalesOfRadiance.Scripts.Battle.BattlePlayables;
using TalesOfRadiance.Scripts.Character;
using UnityEngine;
using Types = CoreScripts.Constant.Types;

namespace TalesOfRadiance.Scripts.Battle.Managers
{
    public class BattleMgr : ABattleMgr
    {
        private static BattleMgr _instance;
        public static BattleMgr Instance
        {
            get { return _instance ??= new BattleMgr(); }
        }

        public List<CharacterTeam> OnStageTeam = new List<CharacterTeam>();

        public Camera UICamera;
        public Canvas CameraSpaceCanvas;

        public int AnimationAwaitTime = 1000;


        public void Init(Camera uiCamera, Canvas cameraSpaceCanvas, params CharacterTeam[] teams)
        {
            UICamera = uiCamera;
            CameraSpaceCanvas = cameraSpaceCanvas;
            
            foreach (var team in teams)
            {
                OnStageTeam.Add(team);
            }

            StartFirstRound();
        }

        public override async void EndOfCurRound()
        {
            if (UpdatedRoundCount != RoundCount)
            {
                await BuffMgr.Instance.Update();
                foreach (var team in OnStageTeam)
                {
                    team.UpdateHeroCooldown();
                }

                UpdatedRoundCount = RoundCount;
            }
            
            if (CurBattleRound.GetRemainingPlayables().Count != 0)
            {
                CurBattleRound.ExecuteBattleStage();
                return;
            }

            LoadNextBattleRound();
        }

        protected override async void LoadNextBattleRound()
        {
            await UniTask.Delay(1000);
            Debug.Log("[BattleMgr] Start new round: " + RoundCount);
            var temp = new BattleRound(RoundCount + 1, this)
            {
                Status = CurBattleRound.Status
            };
            CurBattleRound.OnDestroy();
            
            CurBattleRound = temp;
            CurBattleRound.AddBattlePlayables(new BpCommand());
            CurBattleRound.AddBattlePlayables(new BpEndOfRound());

            await BuffMgr.Instance.ExecuteBuff(Constant.Constant.BuffEventKey.BeforeRound, new SkillResult());
            if (CurBattleRound.Status == Types.BattleRoundStatus.Running)
                CurBattleRound.ExecuteBattleStage();
        }

        public void SentHeroOnStage(int heroId, CharacterAnchor characterAnchor, CharacterTeam team)
        {
            CurBattleRound.AddBattlePlayables(new BpDebut(heroId, characterAnchor, team));
        }

        public void AddBattleEntityMove(AtorBattleEntity entity)
        {
            CurBattleRound.AddBattlePlayables(new BpMove(entity));
        }
        
        public void AddBattleFaint(RuntimeHero hero)
        {
            CurBattleRound.AddBattlePlayables(new BpFaint(hero));
        }

        public void AddBattleSkill(BpSkill skill)
        {
            CurBattleRound.AddBattlePlayables(skill);
        }

        public void TransferControlToPendingPlayable(ABattlePlayable playable)
        {
            CurBattleRound.TransferControlToPendingPlayable(playable);
        }

        public void BorrowControlToPendingPlayable(ABattlePlayable self, ABattlePlayable targetPlayable)
        {
            CurBattleRound.BorrowControlToPendingPlayable(self, targetPlayable);
        }

        public async UniTask HeroDead(RuntimeHero hero)
        {
            hero.Properties.IsAlive = false;
            hero.Anchor.gameObject.SetActive(false);
            CurBattleRound.RemoveRunTimeSkill(hero);
            await BuffMgr.Instance.RemoveAllBuffByTarget(hero);
            await BuffMgr.Instance.RemoveAllAttributeBySource(hero);
        }

        public override async void StartFirstRound()
        {
            CurBattleRound = new BattleRound(RoundCount, this);

            await OnStageTeam.Select(o => o.InitTeamAnchor());
            
            CurBattleRound.Status = Types.BattleRoundStatus.Running;
            if(CurBattleRound.Status == Types.BattleRoundStatus.Running) 
                CurBattleRound.ExecuteBattleStage();
        }
    }
}