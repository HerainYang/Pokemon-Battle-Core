using System;
using TalesOfRadiance.Scripts.Battle.BattleComponents.RuntimeClass;


namespace TalesOfRadiance.Scripts.Battle.BattleComponents.BattleLogics
{
    public static partial class BattleLogic
    {
        private static SkillResult ApplyNormalDamage(float attackDamageRough, RuntimeHero target)
        {
            float battleDefense = GetBattleDefense(target);
            float defenseCoe = ((battleDefense) / (battleDefense + 598 + 2 * 255));
            float damage = attackDamageRough * (1 - defenseCoe);

            return new SkillResult()
            {
                Damage = (int)-damage
            };
        }

        private static float GetBattleDefense(RuntimeHero hero)
        {
            float battleDefense = hero.Properties.Defence;
            return battleDefense;
        }

        
        private static bool ProbTrigger(float prob)
        {
            Random rand = new Random(Guid.NewGuid().GetHashCode());
            if (rand.NextDouble() < prob)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}