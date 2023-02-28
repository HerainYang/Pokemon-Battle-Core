using System;
using System.Collections.Generic;
using CoreScripts.BattleComponents;
using CoreScripts.Managers;
using Cysharp.Threading.Tasks;
using PokemonDemo.Scripts.BattlePlayables.Skills;
using PokemonDemo.Scripts.PokemonLogic;
using PokemonDemo.Scripts.PokemonLogic.BuffResults;
using PokemonDemo.Scripts.PokemonLogic.PokemonData;
using Unity.VisualScripting;
using UnityEngine;

namespace PokemonDemo.Scripts.Managers
{
    /*
     * Buff rules:
     * 1. Callback function must have Unitask<T> as return type
     * 2. Use skillTemplate to store data, but not calling execute function, call callback function instead
     * 3, When a buff affects pokemon A, its target is pokemon A
     * 4, Warning: Do not remove buff in a buff, create a new battle playable to do so
     * 5, It is safe to add buff in a buff
     */
    public class BuffMgr : ABuffMgr
    {
        private static BuffMgr _instance;

#if UNITY_EDITOR
        public Dictionary<string, List<ABuffRecorder>> GetAllBuff()
        {
            return Listeners;
        }
#endif

        public static BuffMgr Instance
        {
            get { return _instance ??= new BuffMgr(); }
        }

        public async UniTask<PokemonBuffRecorder> AddBuff(Pokemon source, Pokemon target, int buffKey, bool isAttribute, bool isWeather)
        {
            var template = PokemonMgr.Instance.GetBuffTemplateByID(buffKey);
            Debug.Log("[BuffMgr] Receive new buff " + template.Name + " trigger on " + template.BuffTriggerEvent);
            Listeners.TryGetValue(template.BuffTriggerEvent, out var listener);
            if (listener == null)
            {
                listener = new List<ABuffRecorder>();
                Listeners.Add(template.BuffTriggerEvent, listener);
            }

            PokemonCommonResult result = new PokemonCommonResult();
            result.BuffKey = buffKey;
            result = (PokemonCommonResult)await ExecuteBuff(Constant.BuffExecutionTimeKey.BeforeAddBuff, result, target);
            if (!result.CanAddBuff)
            {
                return null;
            }

            var newRecorder = new PokemonBuffRecorder(source, target, template, isAttribute, isWeather);
            listener.Add(newRecorder);

            result = new PokemonCommonResult();
            result.BuffKey = buffKey;
            await ExecuteBuff(Constant.BuffExecutionTimeKey.OnAddBuff, result, target);

            return newRecorder;
        }

        // the follow two won't delete weather buff
        public override void RemoveBuffByTarget(IBattleEntity target, ASkillTemplate template)
        {
            Listeners.TryGetValue(template.BuffTriggerEvent, out var listener);
            if (listener == null)
            {
                return;
            }

            listener.RemoveAll(r => ((PokemonBuffRecorder)r).Target != null && ((PokemonBuffRecorder)r).Template == template && ((PokemonBuffRecorder)r).Target.Equals(target) && !((PokemonBuffRecorder)r).IsWeather);
        }

        public override void RemoveBuffBySource(IBattleEntity source, ASkillTemplate template)
        {
            Listeners.TryGetValue(template.BuffTriggerEvent, out var listener);
            if (listener == null)
            {
                return;
            }

            int i = listener.RemoveAll(r => ((PokemonBuffRecorder)r).Source != null && ((PokemonBuffRecorder)r).Template == template && ((PokemonBuffRecorder)r).Source.Equals(source) && !((PokemonBuffRecorder)r).IsWeather);
            Debug.Log("[BuffMgr] " + i + "buffs are removed by source");
        }

        public void RemoveAllWeatherBuff()
        {
            foreach (var pair in Listeners)
            {
                var list = pair.Value;
                list.RemoveAll(r => ((PokemonBuffRecorder)r).IsWeather);
            }
        }

        public override async UniTask<ABuffRecorder> AddBuff(IBattleEntity source, IBattleEntity target, int buffKey, bool isAttribute)
        {
            return await AddBuff(((Pokemon)source), ((Pokemon)target), buffKey, isAttribute, false);
        }
    }
}