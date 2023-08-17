using CoreScripts.BattleComponents;
using CoreScripts.BattlePlayables;
using CoreScripts.Constant;
using PokemonDemo.Scripts.Enum;

namespace CoreScripts.Managers
{
    public abstract class ABattleMgr
    {
        protected int RoundCount;

        //This variable is for buffs that might kill battle entity in their onDestroy callback, the idea is we only update buff manager once a round,
        //but we might check multiple time to ensure there is no remaining playable (for example bpFaint) to execute.
        /* Example of use
         
            public override async void EndOfCurRound()
            {
                if (UpdatedRoundCount != RoundCount)
                {
                    await BuffMgr.Instance.Update();
                    UpdatedRoundCount = RoundCount;
                }
    
                if (CurBattleRound.GetRemainingPlayables().Count != 0)
                {
                    CurBattleRound.ExecuteBattleStage();
                }
    
                await SetCommandText(" ");
                LoadNextBattleRound();
            }
        
         */
        protected int UpdatedRoundCount;
        
        protected BattleRound CurBattleRound;

        //For editor only
#if UNITY_EDITOR
        public BattleRound GetCurBattleRound()
        {
            return CurBattleRound;
        }

        public void SetBattleRoundStatus(Types.BattleRoundStatus status)
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

        protected abstract void LoadNextBattleRound();
        public abstract void StartFirstRound();
        
        public void BattlePlayableEnd()
        {
            if (CurBattleRound.Status == Types.BattleRoundStatus.Running)
                CurBattleRound.ExecuteBattleStage();
        }

        public void RemoveRunTimeSkillBySource(IBattleEntity source)
        {
            CurBattleRound.RemoveRunTimeSkill(source);
        }
        
        public void TransferControlToPendingPlayable(ABattlePlayable playable)
        {
            CurBattleRound.TransferControlToPendingPlayable(playable);
        }
    }
}