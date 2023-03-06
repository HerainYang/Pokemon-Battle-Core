using System;
using System.Collections.Generic;
using System.Linq;
using CoreScripts.BattleComponents;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CoreScripts.Managers
{
    /*
     * todo:
     * new delete machinism: mark then delete
     */
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
                    if (!recorder.DeletePending && recorder.Target != null && recorder.Target.Equals(target))
                    {
                        recorder.DeletePending = true;
                        if (recorder.Template.OnDestroyCallBacks != null)
                        {
                            await recorder.Template.OnDestroyCallBacks(recorder.Source, recorder.Target, recorder);
                        }
                    }
                }
            }
        }

        public async UniTask RemoveAllAttributeBySource(IBattleEntity source)
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
                    if (!recorder.DeletePending && recorder.IsAttribute && recorder.Source != null && recorder.Source.Equals(source))
                    {
                        recorder.DeletePending = true;
                        if (recorder.Template.OnDestroyCallBacks != null)
                        {
                            await recorder.Template.OnDestroyCallBacks(recorder.Source, recorder.Target, recorder);
                        }
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
                
                // delete all buff to be expire
                var destroyCallBackList = pair.Value.FindAll(recorder => recorder.EffectLastRound <= 0 && !recorder.DeletePending);
                pair.Value.RemoveAll(buffRecorder => buffRecorder.EffectLastRound <= 0 && !buffRecorder.DeletePending);
                foreach (var recorder in destroyCallBackList)
                {
                    if (recorder.Template.OnDestroyCallBacks != null)
                    {
                        await recorder.Template.OnDestroyCallBacks(recorder.Source, recorder.Target, recorder);
                    }
                }
                
                //delete all buff in the delete pending list, callback function should be already called, so do nothing here
                pair.Value.RemoveAll(recorder => recorder.DeletePending);

                //delete here
            }
        }
        
        public virtual async UniTask RemoveBuffByTarget(IBattleEntity target, ASkillTemplate buffTemplate)
        {
            Listeners.TryGetValue(buffTemplate.BuffTriggerEvent, out var listener);
            if (listener == null)
            {
                return;
            }

            foreach (var buffRecorder in listener.Where(buffRecorder => !buffRecorder.DeletePending && buffRecorder.Target != null && buffRecorder.Template == buffTemplate && buffRecorder.Target.Equals(target)))
            {
                if(buffRecorder.Template.OnDestroyCallBacks != null) 
                    await buffRecorder.Template.OnDestroyCallBacks(buffRecorder.Source, buffRecorder.Target, buffRecorder);
                buffRecorder.DeletePending = true;
            }
        }
        
        public virtual async UniTask RemoveBuffBySource(IBattleEntity source, ASkillTemplate buffTemplate)
        {
            Listeners.TryGetValue(buffTemplate.BuffTriggerEvent, out var listener);
            if (listener == null)
            {
                return;
            }
            
            foreach (var buffRecorder in listener.Where(buffRecorder => !buffRecorder.DeletePending && buffRecorder.Source != null && buffRecorder.Template == buffTemplate && buffRecorder.Source.Equals(source)))
            {
                if(buffRecorder.Template.OnDestroyCallBacks != null) 
                    await buffRecorder.Template.OnDestroyCallBacks(buffRecorder.Source, buffRecorder.Target, buffRecorder);
                buffRecorder.DeletePending = true;
            }
        }

        public bool ExistActiveBuff(IBattleEntity target, ASkillTemplate buffTemplate)
        {
            Listeners.TryGetValue(buffTemplate.BuffTriggerEvent, out var listener);
            if (listener == null)
                return false;
            return listener.Exists(r => r.Target != null && r.Target.Equals(target) && r.Template == buffTemplate && !r.DeletePending);
        }
        
        // Here the pokemon is the target of buff, it is not the target of a specific attack
        public async UniTask<ASkillResult> ExecuteBuff(string evt, ASkillResult input)
        {
            ASkillResult result = await ExecuteBuff(evt, input, null);
            return result;
        } 

        // if target is null, check all buffs, if target is provided, check buffs target on this pokemon
        // For example, when A attack B, a buff triggered of B, now the param pokemon is B, not A, if you want it to be A, pass it as another arg
        public virtual async UniTask<ASkillResult> ExecuteBuff(string evt, ASkillResult input, IBattleEntity target)
        {
            ASkillResult result = input;
            Listeners.TryGetValue(evt, out var recordList);
            if (recordList == null)
            {
                return result;
            }
            
            
            Debug.Log($"[BuffMgr] Trigger event {evt}");
            // it has to be like this way because we might add something to list during buff execution
            for (int i = 0; i < recordList.Count; i++)
            {
                var recorder = recordList[i];
                input = await ExecuteBuffByRecorder(recorder, input, target);
            }

            return result;
        }

        public async UniTask<ASkillResult> ExecuteBuffByRecorder(ABuffRecorder recorder, ASkillResult input, IBattleEntity target)
        {
            
            if (!recorder.DeletePending && (target == null || recorder.Target == null || recorder.Target.Equals(target)))
            {
                input = await (recorder.Template).BuffCallBacks(input, recorder.Source, recorder.Target, recorder);
            }

            return input;
        }
    }
}