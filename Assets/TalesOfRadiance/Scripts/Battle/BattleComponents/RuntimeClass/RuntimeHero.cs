using System;
using Cysharp.Threading.Tasks;
using TalesOfRadiance.Scripts.Battle.CoreClass;
using TalesOfRadiance.Scripts.Battle.Managers;
using TalesOfRadiance.Scripts.Character;
using UnityEditor;
using UnityEngine;

namespace TalesOfRadiance.Scripts.Battle.BattleComponents.RuntimeClass
{
    public class RuntimeHero : ABattleEntity
    {
        public readonly HeroTemplate Template;
        public readonly Guid RuntimeID;
        public readonly CharacterAnchor Anchor;
        
        public RuntimeHero(HeroTemplate template, CharacterTeam team, CharacterAnchor anchor) : base(team)
        {
            Template = template;
            RuntimeID = Guid.NewGuid();
            Team.Heroes.Add(this);
            Anchor = anchor;
        }
        
        public override void LoadBattleMoveBp()
        {
            BattleMgr.Instance.AddBattleEntityMove(this);
        }

        public override SkillTemplate MakeBattleDecision()
        {
            // go through all possible skill
            return ConfigManager.Instance.GetSkillTemplateByID(0);
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
    }
}