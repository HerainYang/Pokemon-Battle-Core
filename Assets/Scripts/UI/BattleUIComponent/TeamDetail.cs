using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Managers.BattleMgrComponents;
using Managers.BattleMgrComponents.BattlePlayer;
using UI.BattleUIComponent;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class TeamDetail : MonoBehaviour
{
    [SerializeField] private Transform trainerContent;

    public async void InitTrainerDetail(List<ABattlePlayer> infos)
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
