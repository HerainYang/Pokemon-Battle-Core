using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Enum;
using Managers;
using Managers.BattleMgrComponents;
using Managers.BattleMgrComponents.PokemonLogic;
using PokemonDemo;
using UnityEngine;
using Random = System.Random;

namespace PokemonLogic.PokemonData
{
    public class Pokemon : PokemonBasicInfo
    {
        private int _hp;
        public int Level;
        public string TrainerID;
        public readonly Guid RuntimeID;
        public AttributesTemplate Attribute;

        public bool IsFaint;
        public bool OnStage;

        private int[] _pokemonStatsChange;

        public List<PokemonRuntimeSkillData> RuntimeSkillList;


        public Pokemon(PokemonBasicInfo template, string trainerID) : base(template)
        {
            TrainerID = trainerID;
            _hp = template.GetHpMax();
            RuntimeID = Guid.NewGuid();
            Random r = new Random(RuntimeID.GetHashCode());
            int index = r.Next(0, Abilities.Length);
            Attribute = PokemonMgr.Instance.GetAttributeByID(Abilities[index]);
            Level = 100;
            OnStage = false;
            IsFaint = false;
            RuntimeSkillList = new List<PokemonRuntimeSkillData>();
            _pokemonStatsChange = new int[8];

            for (int i = 0; i < template.SkillList.Length; i++)
            {
                RuntimeSkillList.Add(new PokemonRuntimeSkillData()
                {
                    SkillTemplate = PokemonMgr.Instance.GetSkillTemplateByID(SkillList[i]),
                    Pp = PokemonMgr.Instance.GetSkillTemplateByID(SkillList[i]).PowerPoint
                });
            }
        }

        public string GetAttribute()
        {
            return Attribute.Name;
        }

        public int GetStatusChange(PokemonStat statType)
        {
            return _pokemonStatsChange[(int)statType];
        }

        public int[] GetAllStatus()
        {
            return _pokemonStatsChange;
        }

        public void ChangeAllStatus(int[] status)
        {
            for (int i = 0; i < _pokemonStatsChange.Length; i++)
            {
                _pokemonStatsChange[i] = status[i];
            }
        }

        public void SetStatusChange(PokemonStat statType, int changeValue)
        {
            _pokemonStatsChange[(int)statType] += changeValue;
            if (statType == PokemonStat.CriticalHit)
            {
                _pokemonStatsChange[(int)statType] = _pokemonStatsChange[(int)statType] > 3 ? 3 : (_pokemonStatsChange[(int)statType] < 0 ? 0 : _pokemonStatsChange[(int)statType]);
            }
            else
            {
                _pokemonStatsChange[(int)statType] = _pokemonStatsChange[(int)statType] > 6 ? 6 : (_pokemonStatsChange[(int)statType] < -6 ? -6 : _pokemonStatsChange[(int)statType]);
            }
        }

        public bool CanUseSkillByIndex(int index)
        {
            return RuntimeSkillList[index].Pp != 0;
        }

        public void ConsumePpBySkillID(int skillID)
        {
            int index = -1;
            for (int i = 0; i < RuntimeSkillList.Count; i++)
            {
                if (RuntimeSkillList[i].SkillTemplate.ID == skillID)
                {
                    index = i;
                    break;
                }
            }
            if (index == -1)
                return;
            
            if (RuntimeSkillList[index].Pp == 0)
            {
                Debug.LogError(index + " skills has no PP! It should not happen!");
            }

            RuntimeSkillList[index].Pp -= 1;
        }

        public async UniTask<bool> SetPpByIndex(PokemonRuntimeSkillData skillData, int value)
        {
            if (!RuntimeSkillList.Contains(skillData))
            {
                Debug.LogError("receive skill that doesn't be long to " + Name);
            }

            int actualValue = value;
            if (IsFaint)
            {
                await BattleMgr.Instance.BattleScenePanelTwoPlayerUI.SetCommandText("But have no effect");
                return false;
            }

            if (skillData.Pp + value <= 0)
            {
                actualValue = -skillData.Pp;
            }

            if (skillData.Pp + value >= skillData.SkillTemplate.PowerPoint)
            {
                actualValue = skillData.SkillTemplate.PowerPoint - skillData.Pp;
            }

            skillData.Pp += actualValue;
            await BattleMgr.Instance.SetCommandText(Name + "'s " + skillData.SkillTemplate.Name + "'s PP change by " + actualValue);
            return true;
        }

        public async UniTask<bool> ChangeHp(int value)
        {
            Debug.Log("[Pokemon] " + Name + " Hp Change: damage " + value);
            int actualValue = value;
            if (IsFaint)
            {
                await BattleMgr.Instance.BattleScenePanelTwoPlayerUI.SetCommandText("But have no effect");
                return false;
            }

            if (_hp + value <= 0)
            {
                actualValue = -_hp;
                IsFaint = true;
                EventMgr.Instance.Dispatch(Constant.EventKey.PokemonFaint, this);
                Debug.Log("[Pokemon] Pokemon should faint");
            }

            if (_hp + value >= HpMax)
            {
                actualValue = HpMax - _hp;
            }

            _hp += actualValue;
            Debug.Log("[Pokemon] Hp Change: Hp remain " + _hp + ", change value: " + actualValue);
            EventMgr.Instance.Dispatch(Constant.EventKey.HpChange, _hp, this);
            return true;
        }

        public int GetHp()
        {
            return _hp;
        }

        public bool HealFaint()
        {
            if (IsFaint)
            {
                IsFaint = false;
                return true;
            }

            return false;
        }

        public override bool Equals(object obj)
        {
            return ((((Pokemon)obj)!).RuntimeID) == RuntimeID;
        }

        public override int GetHashCode()
        {
            return RuntimeID.GetHashCode();
        }
    }
}