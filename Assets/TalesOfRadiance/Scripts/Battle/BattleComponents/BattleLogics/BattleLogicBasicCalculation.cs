using TalesOfRadiance.Scripts.Battle.BattleComponents.RuntimeClass;

namespace TalesOfRadiance.Scripts.Battle.BattleComponents.BattleLogics
{
    public static partial class BattleLogic
    {
        public static SkillResult ApplyNormalDamage(float attackDamageRough, RuntimeHero target)
        {
            float battleDefense = GetBattleDefense(target);
            float defenseCoe = ((battleDefense) / (battleDefense + 598 + 2 * 255));
            float damage = attackDamageRough * (1 - defenseCoe);

            return new SkillResult()
            {
                Damage = (int)-damage
            };
        }

        public static float GetBattleDefense(RuntimeHero hero)
        {
            float battleDefense = hero.Properties.Defence;
            return battleDefense;
        }
    }
}