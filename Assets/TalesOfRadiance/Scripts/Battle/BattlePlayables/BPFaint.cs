using CoreScripts.BattlePlayables;
using TalesOfRadiance.Scripts.Battle.BattleComponents;
using TalesOfRadiance.Scripts.Battle.BattleComponents.RuntimeClass;
using TalesOfRadiance.Scripts.Battle.Managers;
using UnityEngine;
using Types = TalesOfRadiance.Scripts.Battle.Constant.Types;

namespace TalesOfRadiance.Scripts.Battle.BattlePlayables
{
    public class BpFaint : ABattlePlayable
    {
        private RuntimeHero _hero;
        public BpFaint(RuntimeHero hero) : base((int)Types.PlayablePriority.Faint)
        {
            _hero = hero;
        }
        
#if UNITY_EDITOR
        public RuntimeHero GetHeroInstance()
        {
            return _hero;
        }
#endif

        public override async void Execute()
        {
            SkillResult result = (SkillResult)await BuffMgr.Instance.ExecuteBuff(Constant.Constant.BuffEventKey.BeforeFaint, new SkillResult(), _hero);
            if(result.HeroShouldFaint)
            {
                _hero.Properties.Hp = 0;
                await BattleMgr.Instance.HeroDead(_hero);
            }
            OnDestroy();
        }

        protected override void OnDestroy()
        {
            BattleMgr.Instance.BattlePlayableEnd();
        }
    }
}