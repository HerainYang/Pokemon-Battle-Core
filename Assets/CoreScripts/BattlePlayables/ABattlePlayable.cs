using Managers.BattleMgrComponents.BattlePlayables.Skills;
using TalesOfRadiance.Scripts.Battle.BattleComponents;

namespace CoreScripts.BattlePlayables
{
    public abstract class ABattlePlayable
    {
        // higher the priority value 
        public int Priority;
        public bool Available;

        public ABattleEntity Source;
        
        public abstract void Execute();
        protected abstract void OnDestroy();

        protected ABattlePlayable(int priority)
        {
            Priority = priority;
            Available = true;
        }

        public virtual bool Compare(ABattlePlayable playable)
        {
            return Priority > playable.Priority;
        }
    }
}