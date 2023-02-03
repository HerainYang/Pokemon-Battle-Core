using System;
using System.Collections.Generic;
using CoreScripts.BattlePlayables;
using CoreScripts.Managers;
using Cysharp.Threading.Tasks;
using Enum;
using TalesOfRadiance.Scripts.Battle.BattleComponents;
using TalesOfRadiance.Scripts.Battle.BattlePlayables;
using TalesOfRadiance.Scripts.Character;
using UnityEngine;

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
        
        public void BattlePlayableEnd()
        {
            Debug.Log("[BattleMgr] Current battle playable end");
            if (CurBattleRound.Status == BattleRoundStatus.Running)
                CurBattleRound.ExecuteBattleStage();
        }
        
        public override void EndOfCurRound()
        {
            LoadNextBattleRound();
        }
        
        private async void LoadNextBattleRound()
        {
            await UniTask.Delay(1000);
            Debug.Log("[BattleMgr] Start new round: " + RoundCount);
            var temp = new BattleRound(RoundCount, this);
            temp.Status = CurBattleRound.Status;
            CurBattleRound.OnDestroy();
            
            CurBattleRound = temp;
            CurBattleRound.AddBattlePlayables(new BpCommand());
            
            if (CurBattleRound.Status == BattleRoundStatus.Running)
                CurBattleRound.ExecuteBattleStage();
        }

        public void SentHeroOnStage(int heroId, CharacterAnchor characterAnchor, CharacterTeam team)
        {
            CurBattleRound.AddBattlePlayables(new BpDebut(heroId, characterAnchor, team));
        }

        public void AddBattleEntityMove(ABattleEntity entity)
        {
            CurBattleRound.AddBattlePlayables(new BpMove(entity));
        }

        public void TransferControlToPendingPlayable(ABattlePlayable playable)
        {
            CurBattleRound.TransferControlToPendingPlayable(playable);
        }
        
        public async void StartFirstRound()
        {
            CurBattleRound = new BattleRound(RoundCount, this);

            await OnStageTeam.Select(o => o.SentHeroOnStage());
            
            CurBattleRound.Status = BattleRoundStatus.Running;
            if(CurBattleRound.Status == BattleRoundStatus.Running) 
                CurBattleRound.ExecuteBattleStage();
        }
    }
}