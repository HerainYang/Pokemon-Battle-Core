using System;
using Enum;

namespace Managers.BattleMgrComponents.PokemonLogic
{
    public class BuffTemplate
    {
        public int EffectRound;
        
        public readonly Delegate Callback;
        public readonly Delegate OnDestroyCallBack;
        
        public string TriggerEvent;
        
        public PokemonType Type;
        public SkillTargetType TargetType;
        public int Power;
        public int Accuracy;
        public int PowerPoint;
        public float SpecialEffectProb;
        public float PercentageDamage;
        public string Message;


        public float ChangePercentage;
        
        public readonly string Name;
        public readonly int BuffID;
        
        public int TargetSkillID;

        public PokemonStat ChangeStat;

        public BuffTemplate(int buffID, string name, int effectRound, Delegate callback, Delegate onDestroyCallBack, string triggerEvent)
        {
            EffectRound = effectRound;
            Callback = callback;
            TriggerEvent = triggerEvent;
            BuffID = buffID;
            Name = name;
            OnDestroyCallBack = onDestroyCallBack;
        }
    }
}