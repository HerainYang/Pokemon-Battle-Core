using CoreScripts.BattlePlayables;
using Enum;

namespace CoreScripts.Managers
{
    public abstract class ABattleMgr
    {
        protected int RoundCount;
        
        protected BattleRound CurBattleRound;
        
        //For editor only
#if UNITY_EDITOR
        public BattleRound GetCurBattleRound()
        {
            return CurBattleRound;
        }

        public void SetBattleRoundStatus(BattleRoundStatus status)
        {
            CurBattleRound.Status = status;
        }
#endif
        
        public void UpdateRoundCount()
        {
            RoundCount++;
        }
        
        public int GetRoundCount()
        {
            return RoundCount;
        }

        protected ABattleMgr()
        {
            RoundCount = 0;
            RuntimeLog.CurrentBattleManager = this;
        }

        public abstract void EndOfCurRound();
    }
}