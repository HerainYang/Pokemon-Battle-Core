using Managers.BattleMgrComponents.PokemonLogic;
using UnityEngine;

namespace Managers.BattleMgrComponents.BattlePlayer
{
    public class PlayerUser : ABattlePlayer
    {
        protected override void SendCommandRequest(Pokemon pokemon)
        {
            BattleMgr.Instance.BattleScenePanelTwoPlayerUI.StartCommandStage(this, pokemon);
        }

        protected override void SendPokemonForceAddRequest(int onStagePosition)
        {
            BattleMgr.Instance.BattleScenePanelTwoPlayerUI.StartForcePokemonSelect(this, onStagePosition);
        }

        public override void TestHeartBeat()
        {
            EventMgr.Instance.Dispatch(Constant.EventKey.HeartBeatSent, this);
        }

        public PlayerUser(BasicPlayerInfo info) : base(info)
        {
        }
    }
}