using System.Collections.Generic;
using TalesOfRadiance.Scripts.Battle.BattleComponents.RuntimeClass;

namespace TalesOfRadiance.Scripts.Battle.BattleComponents
{
    public class SkillResult
    {
        public int SkillID;

        public List<RuntimeHero> TargetHeroes = new List<RuntimeHero>();
    }
}