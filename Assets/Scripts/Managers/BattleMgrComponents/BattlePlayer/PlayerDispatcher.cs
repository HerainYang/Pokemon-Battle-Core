namespace Managers.BattleMgrComponents.BattlePlayer
{
    public class PlayerDispatcher
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