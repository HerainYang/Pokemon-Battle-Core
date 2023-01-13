using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Enum;
using Managers.BattleMgrComponents.PokemonLogic;

namespace Managers.BattleMgrComponents.BattlePlayables.Skills
{
    public class CommonSkillTemplate
    {
        //Common Properties:
        public float PercentageDamage;
        public PokemonType Type;
        
        //Skill properties:
        public int Power;
        public int Accuracy;
        public readonly SkillTargetType TargetType;
        public readonly SkillType SkillType;

        public int PowerPoint;
        public float SpecialEffectProb;
        public int CriticalRate;


        public readonly int PriorityLevel;

        public readonly string Name;
        public readonly int SkillID;
        public int AddBuffID;
        public Weather WeatherType;
        
        public int[] PokemonStatPoint;
        public PokemonStat[] PokemonStatType;

        public Func<Pokemon, Pokemon, CommonSkillTemplate, UniTask<bool>>[] ProcedureFunctions;
        
        //Buff properties:
        public int BuffID;
        public int EffectRound;
        public readonly Delegate Callback;
        public string TriggerEvent;
        public readonly Delegate OnDestroyCallBack;
        public int TargetSkillID;
        public MovePriority MovePriority;
        public string Message;
        public float PokemonDataChangePercentage;
        public PokemonStat ChangeStat;
        public DamageApplyPriority BuffDamageApplyPriority;
        

        public CommonSkillTemplate()
        {
            
        }

        //skill constructor
        public CommonSkillTemplate(string name, int skillID, PokemonType type, SkillType skillType, int priorityLevel, int powerPoint, SkillTargetType targetType, Func<Pokemon, Pokemon, CommonSkillTemplate, UniTask<bool>>[] procedureFunctions)
        {
            Name = name;
            SkillID = skillID;
            ProcedureFunctions = procedureFunctions;
            TargetType = targetType;
            PowerPoint = powerPoint;
            Type = type;
            PriorityLevel = priorityLevel;
            SkillType = skillType;
        }
        public CommonSkillTemplate(string name, int skillID, PokemonType type, SkillType skillType, int powerPoint, SkillTargetType targetType, Func<Pokemon, Pokemon, CommonSkillTemplate, UniTask<bool>>[] procedureFunctions) : this(name, skillID, type, skillType, 0, powerPoint, targetType, procedureFunctions)
        {
        }
        
        public CommonSkillTemplate(int addBuffID, string name, int effectRound, Delegate callback, Delegate onDestroyCallBack, string triggerEvent)
        {
            EffectRound = effectRound;
            Callback = callback;
            TriggerEvent = triggerEvent;
            BuffID = addBuffID;
            Name = name;
            OnDestroyCallBack = onDestroyCallBack;
        }
        
        

        public static CommonSkillTemplate CopySkill(CommonSkillTemplate prototype)
        {
            CommonSkillTemplate commonSkillTemplate = new CommonSkillTemplate(prototype.Name, prototype.SkillID, prototype.Type, prototype.SkillType, prototype.PriorityLevel, prototype.PowerPoint, prototype.TargetType, null);
            commonSkillTemplate.ProcedureFunctions = new Func<Pokemon, Pokemon, CommonSkillTemplate, UniTask<bool>>[prototype.ProcedureFunctions.Length];
            Array.Copy(prototype.ProcedureFunctions, commonSkillTemplate.ProcedureFunctions, prototype.ProcedureFunctions.Length);
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


            return commonSkillTemplate;
        }
    }
}