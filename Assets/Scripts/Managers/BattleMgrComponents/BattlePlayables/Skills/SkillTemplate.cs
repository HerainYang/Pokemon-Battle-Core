using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Enum;
using Managers.BattleMgrComponents.PokemonLogic;

namespace Managers.BattleMgrComponents.BattlePlayables.Skills
{
    public class SkillTemplate
    {
        public PokemonType Type;
        public SkillTargetType TargetType;
        public SkillType SkillType;
        public int Power;
        public int Accuracy;
        public int PowerPoint;
        public float SpecialEffectProb;
        public int CriticalRate;
        
        public bool MultiAttack;
        public int AttackTime;
        public int MultiAttackProb;
        
        public int[] PokemonStatPoint;
        public int PriorityLevel;
        public float PercentageDamage;
        public readonly string Name;
        public readonly int SkillID;
        public Weather WeatherType;
        public PokemonStat[] PokemonStatType;

        public Func<Pokemon, Pokemon, SkillTemplate, UniTask<bool>>[] ProcedureFunctions;

        public SkillTemplate()
        {
            
        }

        public SkillTemplate(string name, int skillID, PokemonType type, SkillType skillType, int priorityLevel, int powerPoint, SkillTargetType targetType, Func<Pokemon, Pokemon, SkillTemplate, UniTask<bool>>[] procedureFunctions)
        {
            Name = name;
            SkillID = skillID;
            ProcedureFunctions = procedureFunctions;
            TargetType = targetType;
            PowerPoint = powerPoint;
            Type = type;
            this.PriorityLevel = priorityLevel;
            SkillType = skillType;
        }
        public SkillTemplate(string name, int skillID, PokemonType type, SkillType skillType, int powerPoint, SkillTargetType targetType, Func<Pokemon, Pokemon, SkillTemplate, UniTask<bool>>[] procedureFunctions) : this(name, skillID, type, skillType, 0, powerPoint, targetType, procedureFunctions)
        {
        }

        public static SkillTemplate Copy(SkillTemplate prototype)
        {
            SkillTemplate skillTemplate = new SkillTemplate(prototype.Name, prototype.SkillID, prototype.Type, prototype.SkillType, prototype.PriorityLevel, prototype.PowerPoint, prototype.TargetType, null);
            skillTemplate.ProcedureFunctions = new Func<Pokemon, Pokemon, SkillTemplate, UniTask<bool>>[prototype.ProcedureFunctions.Length];
            Array.Copy(prototype.ProcedureFunctions, skillTemplate.ProcedureFunctions, prototype.ProcedureFunctions.Length);
            skillTemplate.Power = prototype.Power;
            skillTemplate.Accuracy = prototype.Accuracy;
            skillTemplate.PowerPoint = prototype.PowerPoint;
            skillTemplate.SpecialEffectProb = prototype.SpecialEffectProb;
            skillTemplate.CriticalRate = prototype.CriticalRate;
            skillTemplate.MultiAttack = prototype.MultiAttack;
            skillTemplate.AttackTime = prototype.AttackTime;
            skillTemplate.MultiAttackProb = prototype.MultiAttackProb;
            if (prototype.PokemonStatPoint != null)
            {
                skillTemplate.PokemonStatPoint = new int[prototype.PokemonStatPoint.Length];
                Array.Copy(prototype.PokemonStatPoint, skillTemplate.PokemonStatPoint, prototype.PokemonStatPoint.Length);
            }

            if (prototype.PokemonStatType != null)
            {
                skillTemplate.PokemonStatType = new PokemonStat[prototype.PokemonStatType.Length];
                Array.Copy(prototype.PokemonStatType, skillTemplate.PokemonStatType, prototype.PokemonStatType.Length);
            }

            
            skillTemplate.PercentageDamage = prototype.PercentageDamage;
            skillTemplate.WeatherType = prototype.WeatherType;

            return skillTemplate;
        }
    }
}