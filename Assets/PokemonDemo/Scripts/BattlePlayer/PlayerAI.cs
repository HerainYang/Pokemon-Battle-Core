using Managers;
using Managers.BattleMgrComponents;
using PokemonDemo.Scripts.BattleMgrComponents;
using PokemonDemo.Scripts.Managers;
using PokemonDemo.Scripts.PokemonLogic.PokemonData;

namespace PokemonDemo.Scripts.BattlePlayer
{
    public class PlayerAI : APokemonBattlePlayer
    {
        private bool _turn = false;
        protected override void SendCommandRequest(Pokemon pokemon)
        {
            int skillID = !_turn ? 1 : 2;
            
            _ = pokemon.RuntimeSkillList[skillID].SkillTemplate.SendLoadSkillRequest(pokemon);
            // SelectOnePokemonToSend(BattleMgr.Instance.GetPokemonOnstagePosition(pokemon), true);

            _turn = !_turn;
        }

        protected override void SendPokemonForceAddRequest(int onStagePosition)
        {
            //replace as first available
            SelectOnePokemonToSend(onStagePosition, true);
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