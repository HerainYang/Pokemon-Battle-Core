using System.Collections.Generic;
using CoreScripts.BattleComponents;
using CoreScripts.BattlePlayables;
using TalesOfRadiance.Scripts.Battle.BattleComponents.RuntimeClass;

namespace TalesOfRadiance.Scripts.Battle.BattleComponents
{
    public class SkillResult : ASkillResult
    {
        public int Damage;
        public bool DamageShouldBeDone = true;
        public IBattleEntity SkillSource;
        public IBattleEntity SkillTarget;
        public ABattlePlayable SelfPlayable;
        public bool CallDefaultPlayableDestroyFunction = true;
        public bool HeroShouldFaint = true;
        public bool ContinueProcedureFunction = true;

        public List<RuntimeHero> TargetHeroes = new List<RuntimeHero>();

        public SkillResult Copy()
        {
            var output = new SkillResult
            {
                Damage = Damage,
                DamageShouldBeDone = DamageShouldBeDone,
                SkillSource = SkillSource,
                SkillTarget = SkillTarget,
                SelfPlayable = SelfPlayable,
                CallDefaultPlayableDestroyFunction = CallDefaultPlayableDestroyFunction,
                HeroShouldFaint = HeroShouldFaint,
                ContinueProcedureFunction = ContinueProcedureFunction,
                TargetHeroes = TargetHeroes
            };
            return output;
        }
    }
}