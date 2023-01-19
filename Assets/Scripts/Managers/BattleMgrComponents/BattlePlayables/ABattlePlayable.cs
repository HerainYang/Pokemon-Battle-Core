
using Managers.BattleMgrComponents.BattlePlayables.Skills;
using Managers.BattleMgrComponents.BattlePlayer;

namespace Managers.BattleMgrComponents.BattlePlayables
{
    public abstract class ABattlePlayable
    {
        // higher the priority value 
        public int Priority;
        public bool Available;
        public abstract void Execute();
        protected abstract void OnDestroy();

        protected ABattlePlayable(int priority)
        {
            Priority = priority;
            Available = true;
        }

        //Should playable a earlier than playable b?
        public static bool operator <(ABattlePlayable playableA, ABattlePlayable playableB)
        {
            if (!(playableA is RunTimeSkillBase) || !(playableB is RunTimeSkillBase))
            {
                return playableA.Priority < playableB.Priority;
            }
            else
            {
                RunTimeSkillBase skillA = (RunTimeSkillBase)playableA;
                RunTimeSkillBase skillB = (RunTimeSkillBase)playableB;
                if (skillA.Template.PriorityLevel == skillB.Template.PriorityLevel)
                {
                    return playableA.Priority < playableB.Priority;
                }
                else
                {
                    return skillA.Template.PriorityLevel < skillB.Template.PriorityLevel;
                }
            }
        }

        public static bool operator >(ABattlePlayable playableA, ABattlePlayable playableB)
        {
            if (!(playableA is RunTimeSkillBase) || !(playableB is RunTimeSkillBase))
            {
                return playableA.Priority > playableB.Priority;
            }
            else
            {
                RunTimeSkillBase skillA = (RunTimeSkillBase)playableA;
                RunTimeSkillBase skillB = (RunTimeSkillBase)playableB;
                if (skillA.Template.PriorityLevel == skillB.Template.PriorityLevel)
                {
                    return playableA.Priority > playableB.Priority;
                }
                else
                {
                    return skillA.Template.PriorityLevel > skillB.Template.PriorityLevel;
                }
            }
        }
    }
}