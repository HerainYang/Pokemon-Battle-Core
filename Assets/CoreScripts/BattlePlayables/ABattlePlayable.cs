using System;
using CoreScripts.BattleComponents;

namespace CoreScripts.BattlePlayables
{
    public abstract class ABattlePlayable
    {
        // higher the priority value 
        public int Priority;
        public bool Available;

        public IBattleEntity Source;
        
        public abstract void Execute();
        protected abstract void OnDestroy();

        public readonly Guid RuntimeID = Guid.NewGuid();

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