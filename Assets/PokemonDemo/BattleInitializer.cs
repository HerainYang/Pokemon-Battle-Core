using Managers;
using Managers.BattleMgrComponents;
using UnityEngine;

namespace PokemonDemo
{
    public class BattleInitializer : MonoBehaviour
    {
        public BasicPlayerInfo[] playerInfos;
        private void Start()
        {
            _ = UIWindowsManager.Instance.ShowUIWindowAsync("BattleStartPanel");
            BattleMgr.Instance.InitData(playerInfos, 2);
        }
    }
}
