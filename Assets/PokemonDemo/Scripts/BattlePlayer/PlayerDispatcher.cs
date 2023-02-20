using Managers.BattleMgrComponents;
using PokemonDemo.Scripts.BattleMgrComponents;

namespace PokemonDemo.Scripts.BattlePlayer
{
    public static class PlayerDispatcher
    {
        public static APokemonBattlePlayer InitPlayer(BasicPlayerInfo info)
        {
            if (info.isAI)
            {
                return new PlayerAI(info);
            }

            return new PlayerUser(info);
        }
    }
}