using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Enum;
using Managers;
using Managers.BattleMgrComponents;
using Managers.BattleMgrComponents.BattlePlayables.Skills;
using Managers.BattleMgrComponents.BattlePlayer;
using PokemonDemo;
using PokemonLogic;
using PokemonLogic.PokemonData;
using UnityEngine;
using UnityEngine.UI;

namespace UI.BattleUIComponent
{
    public class ItemSelectPanel : MonoBehaviour
    {
        [SerializeField] private GameObject itemButtonPrefab;
        [SerializeField] private Button backBtn;
        [SerializeField] private Transform content;

        private ABattlePlayer _curPlayer;
        private Pokemon _curPokemon;
        private int _onStagePosition;

        private void OnEnable()
        {
            backBtn.onClick.AddListener(BackBehavior);
        }

        private void OnDestroy()
        {
            backBtn.onClick.RemoveAllListeners();
        }

        private void BackBehavior()
        {
            foreach (Transform obj in content)
            {
                Destroy(obj.gameObject);
            }
            EventMgr.Instance.Dispatch(Constant.UIEventKey.CloseSkillSelectWindow, (List<PokemonRuntimeSkillData>)null);
            UIWindowsManager.Instance.HideUIWindow("ItemSelectPanel");
        }

        private void ItemButtonBehavior(int index)
        {
            CommonSkillTemplate item = PokemonMgr.Instance.GetItemTemplateByID(index);
            _ = item.SendLoadSkillRequest(_curPokemon);
            BackBehavior();
        }

        private void SelectSkillAndSendLoadRequest(List<PokemonRuntimeSkillData> skillIndex)
        {
            EventMgr.Instance.Dispatch(Constant.UIEventKey.CloseSkillSelectWindow, skillIndex);
            BackBehavior();
        }

        public void ShowItemList(int onStagePosition)
        {
            _curPokemon = BattleMgr.Instance.OnStagePokemon[onStagePosition];
            _curPlayer = BattleMgr.Instance.PlayerInGame[_curPokemon.TrainerID];
            _onStagePosition = onStagePosition;
            foreach (var item in _curPlayer.Items)
            {
                GameObject o = Instantiate(itemButtonPrefab, content);
                o.transform.GetChild(0).GetComponent<Text>().text = item.Key.Name;
                o.GetComponent<Button>().interactable = (item.Value != 0);
                o.GetComponent<Button>().onClick.AddListener(delegate { ItemButtonBehavior(item.Key.ID); });
                o.SetActive(true);
            }
        }

        public void ShowSkillList(Pokemon target)
        {
            for (int i = 0; i < target.RuntimeSkillList.Count; i++)
            {
                GameObject o = Instantiate(itemButtonPrefab, content);
                var skill = target.RuntimeSkillList[i].SkillTemplate;
                o.transform.GetChild(0).GetComponent<Text>().text = skill.Name + " (" + target.RuntimeSkillList[i].Pp + "/" + skill.PowerPoint + ")";
                o.GetComponent<Button>().interactable = (target.RuntimeSkillList[i].Pp != skill.PowerPoint);
                var i1 = i;
                o.GetComponent<Button>().onClick.AddListener(delegate { SelectSkillAndSendLoadRequest(new List<PokemonRuntimeSkillData>() { target.RuntimeSkillList[i1] }); });
                o.SetActive(true);
            }
        }
        
    }
}