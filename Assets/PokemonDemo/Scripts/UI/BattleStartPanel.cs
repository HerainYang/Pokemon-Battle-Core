using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PokemonDemo.Scripts.BattleMgrComponents;
using PokemonDemo.Scripts.BattlePlayer;
using PokemonDemo.Scripts.Managers;
using PokemonDemo.Scripts.UI.BattleUIComponent;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace PokemonDemo.Scripts.UI
{
    public class BattleStartPanel : MonoBehaviour
    {
        [SerializeField] private Transform teamContent;
        [SerializeField] private Button startBtn;

        private Dictionary<int, List<APokemonBattlePlayer>> _teamInfoDic;
        private void Awake()
        {
            _teamInfoDic = new Dictionary<int, List<APokemonBattlePlayer>>();
            startBtn.onClick.AddListener(StartBtnBehavior);
            foreach (var pair in BattleMgr.Instance.PlayerInGame)
            {
                var player = pair.Value;
                if (!_teamInfoDic.ContainsKey(player.PlayerInfo.teamID))
                {
                    _teamInfoDic.Add(player.PlayerInfo.teamID, new List<APokemonBattlePlayer>());
                }
                _teamInfoDic[player.PlayerInfo.teamID].Add(player);
            }
            foreach (var pair in _teamInfoDic)
            {
                InitTeamDetails(pair.Value);
            }
        }

        private async void InitTeamDetails(List<APokemonBattlePlayer> infos)
        {
            var handler = Addressables.LoadAssetAsync<GameObject>("TeamDetail");
            await handler;
            var detail = Instantiate(handler.Result, teamContent);
            detail.GetComponent<TeamDetail>().InitTrainerDetail(infos);
        }

        private void StartBtnBehavior()
        {
            UIWindowsManager.Instance.ShowUIWindowAsync("BattleScenePanel").ContinueWith((_ =>
            {
                BattleMgr.Instance.StartFirstRound();
                UIWindowsManager.Instance.HideUIWindow("BattleStartPanel");
            }));
        }
    }
}
