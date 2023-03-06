using System.Collections.Generic;
using CoreScripts.BattlePlayables;
using TalesOfRadiance.Scripts.Battle.BattleComponents;
using TalesOfRadiance.Scripts.Battle.BattleComponents.RuntimeClass;
using TalesOfRadiance.Scripts.Battle.Managers;
using TalesOfRadiance.Scripts.Character;
using UnityEngine;
using Types = TalesOfRadiance.Scripts.Battle.Constant.Types;

namespace TalesOfRadiance.Scripts.Battle.BattlePlayables
{
    public class BpDebut : ABattlePlayable
    {
        private int _heroId;
        private CharacterAnchor _anchor;
        private CharacterTeam _team;

        private bool _bringBackToLife;
        
#if UNITY_EDITOR
        public RuntimeHero GetHeroInstance()
        {
            return _anchor.Hero;
        }
#endif
        
        public BpDebut(int heroId, CharacterAnchor anchor, CharacterTeam team) : base((int)Types.PlayablePriority.Debut)
        {
            _heroId = heroId;
            _anchor = anchor;
            _team = team;
        }

        public BpDebut(RuntimeHero hero) : base((int)Types.PlayablePriority.Debut)
        {
            _heroId = hero.Template.Id;
            _anchor = hero.Anchor;
            _team = hero.Team;
            _bringBackToLife = true;
        }

        public override async void Execute()
        {
            if (_bringBackToLife)
            {
                _anchor.Hero.Properties.IsAlive = true;
                _anchor.gameObject.SetActive(true);
            }
            else
            {
                await _anchor.Init(_heroId, _team);
            }

            foreach (var runtimeSkill in _anchor.Hero.RuntimeSkillList)
            {
                if (runtimeSkill.Template.SkillType == Types.SkillType.Passive)
                {
                    List<RuntimeHero> targetHeroes = new List<RuntimeHero>();
                    var input = new SkillResult();
                    if (runtimeSkill.Template.OnLoadRequest == null)
                    {
                        targetHeroes.Add(null);
                    }
                    else
                    {
                        foreach (var func in runtimeSkill.Template.OnLoadRequest)
                        {
                            input = (SkillResult)await func(input, _anchor.Hero, runtimeSkill.Template);
                        }

                        targetHeroes = input.TargetHeroes;
                    }
                    foreach (var buffIndex in runtimeSkill.Template.PassiveSkillBuffList)
                    {
                        foreach (var hero in targetHeroes)
                        {
                            await BuffMgr.Instance.AddBuff(_anchor.Hero, hero, buffIndex, true);
                        }
                        
                    }
                }
            }

            
            await BuffMgr.Instance.ExecuteBuff(Constant.Constant.BuffEventKey.AfterDebut, new SkillResult(), _anchor.Hero);
            OnDestroy();
        }

        protected override void OnDestroy()
        {
            
            BattleMgr.Instance.BattlePlayableEnd();
        }
    }
}