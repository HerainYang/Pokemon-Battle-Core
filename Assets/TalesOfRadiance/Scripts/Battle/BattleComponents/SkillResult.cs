using System.Collections.Generic;
using CoreScripts.BattleComponents;
using TalesOfRadiance.Scripts.Battle.BattleComponents.RuntimeClass;

namespace TalesOfRadiance.Scripts.Battle.BattleComponents
{
    public class SkillResult : ASkillResult
    {
        public int Damage;

        public List<RuntimeHero> TargetHeroes = new List<RuntimeHero>();
    }
}