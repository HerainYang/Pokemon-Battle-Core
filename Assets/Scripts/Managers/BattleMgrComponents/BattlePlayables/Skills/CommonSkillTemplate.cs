using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Enum;
using Managers.BattleMgrComponents.BattlePlayer;
using Managers.BattleMgrComponents.PokemonLogic;
using Managers.BattleMgrComponents.PokemonLogic.BuffResults;
using PokemonDemo;
using PokemonLogic;
using PokemonLogic.BuffResults;
using PokemonLogic.PokemonData;
using UnityEngine;

namespace Managers.BattleMgrComponents.BattlePlayables.Skills
{
    public class CommonSkillTemplate
    {
        //Common Properties:
        public float PercentageDamage;
        public PokemonType Type;
        public readonly string Name;
        public readonly int ID;
        public int[] TargetSkillID;
        public Func<CommonResult, Pokemon, Pokemon, CommonSkillTemplate, UniTask<CommonResult>>[] ProcedureFunctions;
        public Func<CommonResult, Pokemon, CommonSkillTemplate, UniTask<CommonResult>>[] OnLoadRequest;

        //Skill properties:
        public int Power;
        public int Accuracy;
        public readonly SkillTargetType TargetType;
        public readonly SkillType SkillType;

        public int PowerPoint;
        public float SpecialEffectProb;
        public int CriticalRate;
        
        public readonly int PriorityLevel;
        
        public int AddBuffID;
        public Weather WeatherType;
        
        public int[] PokemonStatPoint;
        public PokemonStat[] PokemonStatType;


        
        //Buff properties:
        public int EffectRound;
        public readonly Delegate Callback;
        public string TriggerEvent;
        public readonly Delegate OnDestroyCallBack;
        public MovePriority MovePriority;
        public string Message;
        public float PokemonDataChangePercentage;
        public PokemonStat ChangeStat;
        public DamageApplyPriority BuffDamageApplyPriority;
        
        //Item properties:
        public bool IsItem = false;
        public int RecoveryPoint;
        public int NeedToSelectSkillCount;
        public int[] PpChangeTargetSkills;

        public CommonSkillTemplate()
        {
            
        }

        //skill constructor
        public CommonSkillTemplate(int id, string name, PokemonType type, SkillType skillType, int priorityLevel, int powerPoint, SkillTargetType targetType, Func<CommonResult, Pokemon, Pokemon, CommonSkillTemplate, UniTask<CommonResult>>[] procedureFunctions, Func<CommonResult, Pokemon, CommonSkillTemplate, UniTask<CommonResult>>[] funcOnLoad = null)
        {
            Name = name;
            ID = id;
            ProcedureFunctions = procedureFunctions;
            TargetType = targetType;
            PowerPoint = powerPoint;
            Type = type;
            PriorityLevel = priorityLevel;
            SkillType = skillType;
            OnLoadRequest = funcOnLoad;
        }
        public CommonSkillTemplate(int id, string name, PokemonType type, SkillType skillType, int powerPoint, SkillTargetType targetType, Func<CommonResult, Pokemon, Pokemon, CommonSkillTemplate, UniTask<CommonResult>>[] procedureFunctions, Func<CommonResult, Pokemon, CommonSkillTemplate, UniTask<CommonResult>>[] funcOnLoad = null) : this(id, name, type, skillType, 0, powerPoint, targetType, procedureFunctions, funcOnLoad)
        {
        }
        
        // buff constructor
        public CommonSkillTemplate(int id, string name, int effectRound, Delegate callback, Delegate onDestroyCallBack, string triggerEvent)
        {
            EffectRound = effectRound;
            Callback = callback;
            TriggerEvent = triggerEvent;
            ID = id;
            Name = name;
            OnDestroyCallBack = onDestroyCallBack;
        }

        public CommonSkillTemplate(int id, string name, SkillTargetType targetType, Func<CommonResult, Pokemon, Pokemon, CommonSkillTemplate, UniTask<CommonResult>>[] procedureFunctions, Func<CommonResult, Pokemon, CommonSkillTemplate, UniTask<CommonResult>>[] funcOnLoad = null)
        {
            ID = id;
            Name = name;
            TargetType = targetType;
            ProcedureFunctions = procedureFunctions;
            OnLoadRequest = funcOnLoad;
            IsItem = true;
        }
        
        

        public static CommonSkillTemplate CopySkill(CommonSkillTemplate prototype)
        {
            CommonSkillTemplate commonSkillTemplate = new CommonSkillTemplate(prototype.ID, prototype.Name, prototype.Type, prototype.SkillType, prototype.PriorityLevel, prototype.PowerPoint, prototype.TargetType, null);
            commonSkillTemplate.ProcedureFunctions = new Func<CommonResult, Pokemon, Pokemon, CommonSkillTemplate, UniTask<CommonResult>>[prototype.ProcedureFunctions.Length];
            Array.Copy(prototype.ProcedureFunctions, commonSkillTemplate.ProcedureFunctions, prototype.ProcedureFunctions.Length);
            if (prototype.OnLoadRequest != null)
            {
                commonSkillTemplate.OnLoadRequest = new Func<CommonResult, Pokemon, CommonSkillTemplate, UniTask<CommonResult>>[prototype.OnLoadRequest.Length];
                Array.Copy(prototype.OnLoadRequest, commonSkillTemplate.OnLoadRequest, prototype.OnLoadRequest.Length);
            }

            commonSkillTemplate.IsItem = prototype.IsItem;
            commonSkillTemplate.Power = prototype.Power;
            commonSkillTemplate.Accuracy = prototype.Accuracy;
            commonSkillTemplate.PowerPoint = prototype.PowerPoint;
            commonSkillTemplate.SpecialEffectProb = prototype.SpecialEffectProb;
            commonSkillTemplate.CriticalRate = prototype.CriticalRate;
            if (prototype.PokemonStatPoint != null)
            {
                commonSkillTemplate.PokemonStatPoint = new int[prototype.PokemonStatPoint.Length];
                Array.Copy(prototype.PokemonStatPoint, commonSkillTemplate.PokemonStatPoint, prototype.PokemonStatPoint.Length);
            }

            if (prototype.PokemonStatType != null)
            {
                commonSkillTemplate.PokemonStatType = new PokemonStat[prototype.PokemonStatType.Length];
                Array.Copy(prototype.PokemonStatType, commonSkillTemplate.PokemonStatType, prototype.PokemonStatType.Length);
            }

            
            commonSkillTemplate.PercentageDamage = prototype.PercentageDamage;
            commonSkillTemplate.WeatherType = prototype.WeatherType;

            commonSkillTemplate.RecoveryPoint = prototype.RecoveryPoint;
            commonSkillTemplate.PpChangeTargetSkills = prototype.PpChangeTargetSkills;
            if (prototype.PpChangeTargetSkills != null)
            {
                commonSkillTemplate.PpChangeTargetSkills = new int[prototype.PpChangeTargetSkills.Length];
                Array.Copy(prototype.PpChangeTargetSkills, commonSkillTemplate.PpChangeTargetSkills, prototype.PpChangeTargetSkills.Length);
            }


            return commonSkillTemplate;
        }

        public async UniTask SendLoadSkillRequest(Pokemon curPokemon, CommonResult input = null)
        {
            if (input == null)
                input = new CommonResult();
            input.SkillID = ID;
            input.RunTimeSkillBaseIsItem = IsItem;
            if (OnLoadRequest == null)
            {
                input = await BattleLogic.PreLoadProcedure.SelectIndicesTarget(input, curPokemon, this);
                if(input == null)
                    return;
            }
            else
            {
                foreach (var func in OnLoadRequest)
                {
                    var result = await func(input, curPokemon, this);
                    if(result == null)
                        return;
                    input = result;
                }
            }

            EventMgr.Instance.Dispatch(Constant.EventKey.RequestLoadPokemonSkill, curPokemon, this, input, IsItem ? PlayablePriority.Item : PlayablePriority.None);
        }
    }
}