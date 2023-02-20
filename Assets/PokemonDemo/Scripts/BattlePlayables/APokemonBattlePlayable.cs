using CoreScripts.BattlePlayables;
using PokemonDemo.Scripts.BattlePlayables.Skills;

namespace Managers.BattleMgrComponents.BattlePlayables
{
    public abstract class APokemonBattlePlayable : ABattlePlayable
    {
        protected APokemonBattlePlayable(int priority) : base(priority) { }

        public override bool Compare(ABattlePlayable playable)
        {
            if (!(this is RunTimeSkillBase) || !(playable is RunTimeSkillBase))
            {
                return base.Compare(playable);
            }
            else
            {
                RunTimeSkillBase skillA = (RunTimeSkillBase)this;
                RunTimeSkillBase skillB = (RunTimeSkillBase)playable;
                if (skillA.Template.PriorityLevel == skillB.Template.PriorityLevel)
                {
                    return skillA.Priority > skillB.Priority;
                }
                else
                {
                    return skillA.Template.PriorityLevel > skillB.Template.PriorityLevel;
                }
            }
        }
    }
}