using System;
using System.Collections.Generic;
using CoreScripts.BattleComponents;
using Cysharp.Threading.Tasks;
using TalesOfRadiance.Scripts.Battle.BattlePlayables;
using TalesOfRadiance.Scripts.Battle.Managers;
using TalesOfRadiance.Scripts.Character;
using UnityEditor;
using UnityEngine;
using Types = TalesOfRadiance.Scripts.Battle.Constant.Types;

namespace TalesOfRadiance.Scripts.Battle.BattleComponents.RuntimeClass
{
    public class RuntimeHeroProperties
    {
        // Attributes
        // Basic
        public int Attack;
        public int MaxHealth;
        public int Defence;
        public int Speed;

        //Special
        public float CriticalRate;
        public float CriticalDamage;
        public float ControlRate;
        public float AntiControl;
        public float AntiCritical;
        public float Accuracy;
        public float DamageAvoid;
        public float DodgeRate;
        public float HealRate;
        public float GetHealRate;
        public float DamageIncrease;
        public float PhysicalDamageIncrease;
        public float SpecialDamageIncrease;
        public float PhysicalDamageAvoid;
        public float SpecialDamageAvoid;
        public float SustainDamageIncrease;
        public float SustainDamageAvoid;
        

        //Local Properties
        public int Hp;
        public int HealShield;
        public bool IsAlive;

        public RuntimeHeroProperties(HeroTemplate template)
        {
            // Attributes
            // Basic
            Attack = template.Attack;
            MaxHealth = template.MaxHealth;
            Defence = template.Defence;
            Speed = template.Speed;

            //Special
            CriticalRate = template.CriticalRate;
            CriticalDamage = template.CriticalDamage;
            ControlRate = template.ControlRate;
            AntiControl = template.AntiControl;
            AntiCritical = template.AntiCritical;
            Accuracy = template.Accuracy;
            DamageAvoid = template.DamageAvoid;
            DodgeRate = template.DodgeRate;
            HealRate = template.HealRate;
            GetHealRate = template.GetHealRate;
            DamageIncrease = template.DamageIncrease;
            PhysicalDamageIncrease = template.PhysicalDamageIncrease;
            SpecialDamageIncrease = template.SpecialDamageIncrease;
            PhysicalDamageAvoid = template.PhysicalDamageAvoid;
            SpecialDamageAvoid = template.SpecialDamageAvoid;
            SustainDamageIncrease = template.SustainDamageIncrease;
            SustainDamageAvoid = template.SustainDamageAvoid;


            Hp = MaxHealth;
            IsAlive = true;
        }
    }

    public class RuntimeHero : AtorBattleEntity
    {
        public readonly HeroTemplate Template;
        public readonly Guid RuntimeID;
        public readonly CharacterAnchor Anchor;

        public RuntimeHeroProperties Properties;

        public RuntimeHero(HeroTemplate template, CharacterTeam team, CharacterAnchor anchor) : base(team)
        {
            Template = template;
            RuntimeID = Guid.NewGuid();
            Team.Heroes.Add(this);
            Anchor = anchor;

            RuntimeSkillList = new List<RuntimeSkill>();
            if (template.SkillIndices != null)
            {
                foreach (var skillId in Template.SkillIndices)
                {
                    RuntimeSkillList.Add(new RuntimeSkill(skillId));
                }
            }

            // Add normal attack
            RuntimeSkillList.Add(new RuntimeSkill(0));

            Properties = new RuntimeHeroProperties(Template);
        }

        public override void LoadBattleMoveBp()
        {
            BattleMgr.Instance.AddBattleEntityMove(this);
        }

        public override SkillTemplate MakeBattleDecision()
        {
            foreach (var runtimeSkill in RuntimeSkillList)
            {
                if (runtimeSkill.Template.SkillType == Types.SkillType.Active && runtimeSkill.Cooldown == 0)
                {
                    return runtimeSkill.ExecuteSkill();
                }
            }

            throw new Exception("It should be impossible to reach here since there is a normal attack");
        }

        public override bool Equals(object obj)
        {
            if (obj is not RuntimeHero)
                return false;
            return ((RuntimeHero)obj).RuntimeID == RuntimeID;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        // by default, heal doesn't recover healShield, damage doesn't not go through healShield
        public async UniTask SetTargetHp(int changeValue)
        {
            if (changeValue > 0)
            {
                await SetTargetHp(changeValue, false);
            }
            else
            {
                await SetTargetHp(changeValue, true);
            }
        }

        public async UniTask SetTargetHp(int changeValue, bool healShield)
        {
            if (healShield)
            {
                if (changeValue < 0)
                {
                    if (Properties.HealShield > 0)
                    {
                        int actualDamage = Properties.HealShield + changeValue;
                        Properties.HealShield = actualDamage > 0 ? actualDamage : 0;
                        changeValue = actualDamage < 0 ? actualDamage : 0;
                    }
                }
                else
                {
                    int shieldHealPoint = (Properties.Hp + changeValue) - Properties.MaxHealth;
                    if (shieldHealPoint > 0)
                    {
                        Properties.HealShield += shieldHealPoint;
                        changeValue -= shieldHealPoint;
                    }
                }
            }

            
            Properties.Hp += changeValue;
            if (Properties.Hp < 0)
            {
                BattleMgr.Instance.AddBattleFaint(this);
                return;
            }

            if (Properties.Hp > Properties.MaxHealth)
            {
                Properties.Hp = Properties.MaxHealth;
            }
            Anchor.SetTargetHp((Properties.Hp * 1f) / Properties.MaxHealth);

            await UniTask.Yield();
        }
    }
}