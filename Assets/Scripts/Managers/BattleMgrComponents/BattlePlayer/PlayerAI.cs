using Cysharp.Threading.Tasks;
using Managers.BattleMgrComponents.PokemonLogic;
using UnityEngine;

namespace Managers.BattleMgrComponents.BattlePlayer
{
    public class PlayerAI : ABattlePlayer
    {
        private bool _turn = false;
        protected override void SendCommandRequest(Pokemon pokemon)
        {
            EventMgr.Instance.Dispatch(Constant.EventKey.RequestLoadPokemonSkill, pokemon, !_turn ? 1 : 2);

            _turn = !_turn;
        }

        protected override void SendPokemonForceAddRequest(int onStagePosition)
        {
            //replace as first available
            EventMgr.Instance.Dispatch(Constant.EventKey.RequestSentPokemonOnStage, -1, onStagePosition);
            // EventMgr.Instance.Dispatch(Constant.EventKey.BattlePokemonForceChangeCommandSent, this);
        }

        public override void TestHeartBeat()
        {
            EventMgr.Instance.Dispatch(Constant.EventKey.HeartBeatSent, this);
        }

        public PlayerAI(BasicPlayerInfo info) : base(info)
        {
        }
    }
}