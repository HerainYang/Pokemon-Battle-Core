using System;
using System.Collections.Generic;
using CoreScripts.BattleComponents;
using Cysharp.Threading.Tasks;

namespace CoreScripts.Managers
{
    public abstract class ABuffMgr
    {
        protected readonly Dictionary<string, List<ABuffRecorder>> Listeners = new Dictionary<string, List<ABuffRecorder>>();

        public abstract UniTask<ABuffRecorder> AddBuff(IBattleEntity source, IBattleEntity target, int buffKey, bool isAttribute);

        public async UniTask<ABuffRecorder> AddBuff(IBattleEntity source, IBattleEntity target, int buffKey)
        {
            return await AddBuff(source, target, buffKey, false);
        }
        
        public async UniTask RemoveAllBuffByTarget(IBattleEntity target)
        {
            foreach (var pair in Listeners)
            {
                var list = pair.Value;
                if (list.Count == 0)
                {
                    continue;
                }
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    ABuffRecorder recorder = list[i];
                    if (recorder.Target != null && recorder.Target.Equals(target))
                    {
                        if (recorder.Template.OnDestroyCallBack != null)
                        {
                            await recorder.Template.OnDestroyCallBack(recorder.Source, recorder.Target, recorder);
                        }
                        list.RemoveAt(i);
                    }
                }
            }
        } 
        
        public async UniTask Update()
        {
            foreach (var pair in Listeners)
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
                        await recorder.Template.OnDestroyCallBack(recorder.Source, recorder.Target, recorder);
                    }
                }

                pair.Value.RemoveAll(buffRecorder => buffRecorder.EffectLastRound <= 0);
            }
        }
        
        public virtual void RemoveBuffByTarget(IBattleEntity target, ASkillTemplate buffTemplate)
        {
            Listeners.TryGetValue(buffTemplate.BuffTriggerEvent, out var listener);
            if (listener == null)
            {
                return;
            }

            listener.RemoveAll(r => r.Target != null && r.Template == buffTemplate && r.Target.Equals(target));
        }
        
        public virtual void RemoveBuffBySource(IBattleEntity source, ASkillTemplate buffTemplate)
        {
            Listeners.TryGetValue(buffTemplate.BuffTriggerEvent, out var listener);
            if (listener == null)
            {
                return;
            }

            listener.RemoveAll(r => r.Source != null && r.Template == buffTemplate && r.Source.Equals(source));
        }
        
        public bool Exist(IBattleEntity target, ASkillTemplate buffTemplate)
        {
            Listeners.TryGetValue(buffTemplate.BuffTriggerEvent, out var listener);
            if (listener == null)
                return false;
            return listener.Exists(r => r.Target.Equals(target) && r.Template == buffTemplate);
        }
        
        // Here the pokemon is the target of buff, it is not the target of a specific attack
        public async UniTask<ASkillResult> ExecuteBuff(string evt, ASkillResult input)
        {
            ASkillResult result = await ExecuteBuff(evt, input, null);
            return result;
        }

        // if targetpokemon is null, check all buffs, if targetpokemon is provided, check buffs target on this pokemon
        // For example, when A attack B, a buff triggered of B, now the param pokemon is B, not A, if you want it to be A, pass it as another arg
        public async UniTask<ASkillResult> ExecuteBuff(string evt, ASkillResult input, IBattleEntity targetPokemon)
        {
            ASkillResult result = input;
            Listeners.TryGetValue(evt, out var recordList);
            if (recordList == null)
            {
                return result;
            }

            foreach (var recorder in recordList)
            {
                if (targetPokemon == null || recorder.Target == null || recorder.Target.Equals(targetPokemon))
                {
                    result = await (recorder.Template).BuffCallBack(result, recorder.Source, recorder.Target, recorder);
                }
            }

            return result;
        }
    }
}