using Managers.BattleMgrComponents.BattlePlayables.Skills;

namespace CoreScripts.BattlePlayables
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

        public virtual bool Compare(ABattlePlayable playable)
        {
            return Priority > playable.Priority;
        }
    }
}