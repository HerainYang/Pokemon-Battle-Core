using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PokemonDemo.Scripts.BattlePlayer;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace PokemonDemo.Scripts.UI.BattleUIComponent
{
    public class TeamDetail : MonoBehaviour
    {
        [SerializeField] private Transform trainerContent;

        public async void InitTrainerDetail(List<APokemonBattlePlayer> infos)
        {
            foreach (var basicPlayerInfo in infos)
            {
                var handler = Addressables.LoadAssetAsync<GameObject>("TrainerDetail");
                await handler;
                var detail = Instantiate(handler.Result, trainerContent);
                detail.GetComponent<TrainerDetail>().InitBasicInfo(basicPlayerInfo.PlayerInfo);
            }
        }
    }
}
