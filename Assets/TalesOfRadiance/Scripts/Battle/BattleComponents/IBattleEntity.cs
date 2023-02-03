using Cysharp.Threading.Tasks;
using TalesOfRadiance.Scripts.Character;

namespace TalesOfRadiance.Scripts.Battle.BattleComponents
{
    public abstract class ABattleEntity
    {
        public readonly CharacterTeam Team;

        protected ABattleEntity(CharacterTeam team)
        {
            Team = team;
        }

        public abstract void LoadBattleMoveBp();

        public abstract SkillTemplate MakeBattleDecision();
    }
}