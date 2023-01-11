using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Managers.BattleMgrComponents.BattlePlayables.Skills;
using Managers.BattleMgrComponents.PokemonLogic.BuffResults;
using UnityEngine;

namespace Managers.BattleMgrComponents.PokemonLogic
{
    /*
     * Buff rules:
     * 1. Callback function must have Unitask<T> as return type
     * 2. Use skillTemplate to store data, but not calling execute function, call callback function instead
     * 3, When a buff affects pokemon A, its target is pokemon A
     * 4, Warning: Do not remove buff in a buff, create a new battle playable to do so
     * 5, It is safe to add buff in a buff
     */
    public class BuffMgr
    {
        private static BuffMgr _instance;
        private readonly Dictionary<string, List<BuffRecorder>> _listeners = new Dictionary<string, List<BuffRecorder>>();

#if UNITY_EDITOR
        public Dictionary<string, List<BuffRecorder>> GetAllBuff()
        {
            return _listeners;
        } 
#endif

        public static BuffMgr Instance
        {
            get { return _instance ??= new BuffMgr(); }
        }

        public async UniTask<BuffRecorder> AddBuff(Pokemon source, Pokemon target, int buffKey, bool isAttribute, bool isWeather)
        {
            var template = PokemonMgr.Instance.GetBuffTemplateByID(buffKey);
            Debug.Log("[BuffMgr] Receive new buff " + template.Name + " trigger on " + template.TriggerEvent);
            _listeners.TryGetValue(template.TriggerEvent, out var listener);
            if (listener == null)
            {
                listener = new List<BuffRecorder>();
                _listeners.Add(template.TriggerEvent, listener);
            }

            CommonResult result = new CommonResult();
            result.BuffKey = buffKey;
            result = await ExecuteBuff(Constant.BuffExecutionTimeKey.BeforeAddBuff, result, target);
            if (!result.CanAddBuff)
            {
                return null;
            }
            
            var newRecorder = new BuffRecorder(source, target, template, isAttribute, isWeather);
            listener.Add(newRecorder);

            result = new CommonResult();
            result.BuffKey = buffKey;
            await ExecuteBuff(Constant.BuffExecutionTimeKey.OnAddBuff, result, target);
            
            return newRecorder;
        }

        public async UniTask<BuffRecorder> AddBuff(Pokemon source, Pokemon target, int buffKey, bool isAttribute)
        {
            return await AddBuff(source, target, buffKey, isAttribute, false);
        }

        public async UniTask<BuffRecorder> AddBuff(Pokemon source, Pokemon target, int buffKey)
        {
            return await AddBuff(source, target, buffKey, false);
        }

        
        public async UniTask RemoveAllBuffByTarget(Pokemon target)
        {
            foreach (var pair in _listeners)
            {
                var list = pair.Value;
                if (list.Count == 0)
                {
                    continue;
                }
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    // should remove weather
                    if (list[i].Target != null && list[i].Target.Equals(target))
                    {
                        if (list[i].Template.OnDestroyCallBack != null)
                        {
                            await ((Func<Pokemon, Pokemon, BuffRecorder, UniTask>)list[i].Template.OnDestroyCallBack)(list[i].Source, list[i].Target, list[i]);
                        }
                        list.RemoveAt(i);
                    }
                }
            }
        } 
        
        // the follow two won't delete weather buff
        public void RemoveBuffByTarget(Pokemon target, int buffKey)
        {
            var template = PokemonMgr.Instance.GetBuffTemplateByID(buffKey);
            _listeners.TryGetValue(template.TriggerEvent, out var listener);
            if (listener == null)
            {
                return;
            }

            listener.RemoveAll(r => r.Target != null && r.Template == template && r.Target.Equals(target) && !r.IsWeather);
        }
        
        

        public void RemoveBuffBySource(Pokemon source, int buffKey)
        {
            var template = PokemonMgr.Instance.GetBuffTemplateByID(buffKey);
            _listeners.TryGetValue(template.TriggerEvent, out var listener);
            if (listener == null)
            {
                return;
            }

            int i = listener.RemoveAll(r => r.Source != null && r.Template == template && r.Source.Equals(source) && !r.IsWeather);
            Debug.Log("[BuffMgr] " + i + "buffs are removed by source");
        }

        public void RemoveAllWeatherBuff()
        {
            foreach (var pair in _listeners)
            {
                var list = pair.Value;
                list.RemoveAll(r => r.IsWeather);
            }
        }


        public bool Exist(Pokemon target, int buffKey)
        {
            var buff = PokemonMgr.Instance.GetBuffTemplateByID(buffKey);
            _listeners.TryGetValue(buff.TriggerEvent, out var listener);
            if (listener == null)
                return false;
            return listener.Exists(r => r.Target.Equals(target) && r.Template.BuffID == buffKey);
        }

        public async UniTask Update()
        {
            foreach (var pair in _listeners)
            {
                foreach (var buffRecorder in pair.Value)
                {
                    if (!buffRecorder.IsAttribute)
                    {
                        buffRecorder.EffectLastRound -= 1;
                    }
                }

                foreach (var recorder in pair.Value.FindAll(recorder => recorder.EffectLastRound <= 0))
                {
                    if (recorder.Template.OnDestroyCallBack != null)
                    {
                        await ((Func<Pokemon, Pokemon, BuffRecorder, UniTask>)recorder.Template.OnDestroyCallBack)(recorder.Source, recorder.Target, recorder);
                    }
                }

                var count = pair.Value.RemoveAll(buffRecorder => buffRecorder.EffectLastRound <= 0);
                Debug.Log("[BuffMgr] Remove " + count + " buffs from " + pair.Key );
            }
        }

        // Here the pokemon is the target of buff, it is not the target of a specific attack
        // For example, when A attack B, a buff triggered of B, now the param pokemon is B, not A, if you want it to be A, pass it as another arg
        public async UniTask<TResultType> ExecuteBuff<TResultType>(string evt, TResultType input)
        {
            TResultType result = await ExecuteBuff(evt, input, null);
            return result;
        }

        // if targetpokemon is null, check all buffs, if targetpokemon is provided, check buffs target on this pokemon
        public async UniTask<TResultType> ExecuteBuff<TResultType>(string evt, TResultType input, Pokemon targetPokemon)
        {
            TResultType result = input;
            _listeners.TryGetValue(evt, out var recordList);
            if (recordList == null)
            {
                return result;
            }

            for (int i = 0; i < recordList.Count; i++)
            {
                var recorder = recordList[i];
                if (targetPokemon == null || recorder.Target == null || recorder.Target.Equals(targetPokemon))
                {
                    result = await ((Func<TResultType, Pokemon, Pokemon, BuffRecorder, UniTask<TResultType>>)recorder.Template.Callback)(result, recorder.Source, recorder.Target, recorder);
                }
            }

            return result;
        }
    }
}