using System.Collections.Generic;
using CoreScripts.BattleComponents;
using Cysharp.Threading.Tasks;
using TalesOfRadiance.Scripts.Battle.BattleComponents.RuntimeClass;
using TalesOfRadiance.Scripts.Character;

namespace TalesOfRadiance.Scripts.Battle.BattleComponents
{
    public abstract class AtorBattleEntity : IBattleEntity
    {
        public readonly CharacterTeam Team;
        public List<RuntimeSkill> RuntimeSkillList;

        protected AtorBattleEntity(CharacterTeam team)
        {
            Team = team;
        }

        public abstract void LoadBattleMoveBp();

        public abstract SkillTemplate MakeBattleDecision();
    }
}