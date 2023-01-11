using Cysharp.Threading.Tasks;
using Managers.BattleMgrComponents;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace UI.BattleUIComponent
{
    public class TrainerDetail : MonoBehaviour
    {
        public async void InitBasicInfo(BasicPlayerInfo info)
        {
            var handler = Addressables.LoadAssetAsync<GameObject>("BasicInfo");
            await handler;
            
            var trainerName = Instantiate(handler.Result, transform);
            trainerName.GetComponent<Text>().text = info.name;
            
            var trainerTeam = Instantiate(handler.Result, transform);
            trainerTeam.GetComponent<Text>().text = "team: " + info.teamID;
            
            var isAI = Instantiate(handler.Result, transform);
            isAI.GetComponent<Text>().text = info.isAI ? "Control By AI" : "Control By Player";

            foreach (var id in info.pokemonIDs)
            {
                var pokemon = Instantiate(handler.Result, transform);
                pokemon.GetComponent<Text>().text = "Pokemon: " + id;
            }
        }
    }
}
