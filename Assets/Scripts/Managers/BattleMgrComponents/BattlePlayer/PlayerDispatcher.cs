namespace Managers.BattleMgrComponents.BattlePlayer
{
    public class PlayerDispatcher
    {
        public static ABattlePlayer InitPlayer(BasicPlayerInfo info)
        {
            if (info.isAI)
            {
                return new PlayerAI(info);
            }

            return new PlayerUser(info);
        }
    }
}