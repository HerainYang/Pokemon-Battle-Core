using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Managers;
using Managers.BattleMgrComponents;
using Managers.BattleMgrComponents.BattlePlayer;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace UI
{
    public class BattleStartPanel : MonoBehaviour
    {
        [SerializeField] private Transform teamContent;
        [SerializeField] private Button startBtn;

        private Dictionary<int, List<ABattlePlayer>> _teamInfoDic;
        private void Awake()
        {
            _teamInfoDic = new Dictionary<int, List<ABattlePlayer>>();
            startBtn.onClick.AddListener(StartBtnBehavior);
            foreach (var pair in BattleMgr.Instance.PlayerInGame)
            {
                var player = pair.Value;
                if (!_teamInfoDic.ContainsKey(player.PlayerInfo.teamID))
                {
                    _teamInfoDic.Add(player.PlayerInfo.teamID, new List<ABattlePlayer>());
                }
                _teamInfoDic[player.PlayerInfo.teamID].Add(player);
            }
            foreach (var pair in _teamInfoDic)
            {
                InitTeamDetails(pair.Value);
            }
        }

        private async void InitTeamDetails(List<ABattlePlayer> infos)
        {
            var handler = Addressables.LoadAssetAsync<GameObject>("TeamDetail");
            await handler;
            var detail = Instantiate(handler.Result, teamContent);
            detail.GetComponent<TeamDetail>().InitTrainerDetail(infos);
        }

        private void StartBtnBehavior()
        {
            UIWindowsManager.Instance.ShowUIWindowAsync("BattleScenePanel").ContinueWith((o =>
            {
                BattleScenePanelTwoPlayer battleScenePanelTwoPlayer = o.GetComponent<BattleScenePanelTwoPlayer>();
                battleScenePanelTwoPlayer.SetPlayerInfo();
                UIWindowsManager.Instance.HideUIWindow("BattleStartPanel");
            }));
        }
    }
}
