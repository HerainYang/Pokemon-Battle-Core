using System.Collections.Generic;
using System.Linq;
using CoreScripts.BattleComponents;
using CoreScripts.Managers;
using Cysharp.Threading.Tasks;
using TalesOfRadiance.Scripts.Battle.BattleComponents;
using TalesOfRadiance.Scripts.Battle.BattleComponents.RuntimeClass;
using TalesOfRadiance.Scripts.Battle.Constant;
using Unity.VisualScripting;

namespace TalesOfRadiance.Scripts.Battle.Managers
{
    public class BuffMgr : ABuffMgr
    {
        private static BuffMgr _instance;
        public static BuffMgr Instance
        {
            get { return _instance ??= new BuffMgr(); }
        }
        
#if UNITY_EDITOR
        public Dictionary<string, List<ABuffRecorder>> GetAllBuff()
        {
            return Listeners;
        }
#endif
        
        public override async UniTask<ABuffRecorder> AddBuff(IBattleEntity source, IBattleEntity target, int buffKey, bool isAttribute)
        {
            var template = ConfigManager.Instance.GetBuffTemplateByID(buffKey);
            Listeners.TryGetValue(template.BuffTriggerEvent, out var listener);
            if (listener == null)
            {
                listener = new List<ABuffRecorder>();
                Listeners.Add(template.BuffTriggerEvent, listener);
            }

            BuffRecorder recorder = new BuffRecorder(source, target, template, isAttribute);
            listener.Add(recorder);

            if (recorder.Template.BuffTriggerEvent == Constant.Constant.BuffEventKey.AfterAddThisBuff)
            {
                await ExecuteBuffByRecorder(recorder, new SkillResult(), target);
            }

            await ExecuteBuff(Constant.Constant.BuffEventKey.AfterAddBuff, new SkillResult(), target);
            
            return recorder;
        }

        public async UniTask RemoveAllNegativeBuffByTarget(IBattleEntity target, Types.BuffRemovePriority buffRemovePriority = Types.BuffRemovePriority.Highest)
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
                    if (recorder.DeletePending || recorder.Target == null || !recorder.Target.Equals(target) || recorder.Template is not SkillTemplate template)
                        continue;
                    if (template.BuffType != Types.BuffType.Negative)
                    {
                        continue;
                    }
                    if (buffRemovePriority < template.CurrentBuffRemovePriority)
                    {
                        continue;
                    }
                    if (recorder.Template.OnDestroyCallBacks != null)
                    {
                        await recorder.Template.OnDestroyCallBacks(recorder.Source, recorder.Target, recorder);
                    }

                    recorder.DeletePending = true;
                }
            }
        }
        
        public async UniTask RemoveAllPositiveBuffByTarget(IBattleEntity target, Types.BuffRemovePriority buffRemovePriority = Types.BuffRemovePriority.Highest)
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
                    if (recorder.DeletePending || recorder.Target == null || !recorder.Target.Equals(target) || recorder.Template is not SkillTemplate template)
                        continue;
                    if (template.BuffType != Types.BuffType.Positive)
                    {
                        continue;
                    }
                    if (buffRemovePriority < template.CurrentBuffRemovePriority)
                    {
                        continue;
                    }
                    if (recorder.Template.OnDestroyCallBacks != null)
                    {
                        await recorder.Template.OnDestroyCallBacks(recorder.Source, recorder.Target, recorder);
                    }

                    recorder.DeletePending = true;
                }
            }
        }

        
        public async UniTask<SkillTemplate> RemoveOnePositiveBuffByTarget(IBattleEntity target, Types.BuffRemovePriority buffRemovePriority = Types.BuffRemovePriority.Highest)
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
                    if (recorder.DeletePending || recorder.Target == null || !recorder.Target.Equals(target) || recorder.IsAttribute || recorder.Template is not SkillTemplate template)
                    {
                        continue;
                    }
                    if (template.BuffType != Types.BuffType.Positive)
                    {
                        continue;
                    }
                    if (buffRemovePriority < template.CurrentBuffRemovePriority)
                    {
                        continue;
                    }
                    if (template.OnDestroyCallBacks != null)
                    {
                        await template.OnDestroyCallBacks(recorder.Source, recorder.Target, recorder);
                    }

                    recorder.DeletePending = true;
                    return template;
                }
            }

            return null;
        }
        
        public List<ABuffRecorder> GetBuffListByTargetAndBuffID(IBattleEntity target, int buffID)
        {
            List<ABuffRecorder> buffRecorders = new List<ABuffRecorder>();
            var buffTemplate = ConfigManager.Instance.GetBuffTemplateByID(buffID);
            Listeners.TryGetValue(buffTemplate.BuffTriggerEvent, out var listener);
            if (listener == null)
            {
                return buffRecorders;
            }

            buffRecorders.AddRange(listener.Where(buffRecorder => !buffRecorder.DeletePending && buffRecorder.Template.ID == buffID && Equals(buffRecorder.Target, target)));

            return buffRecorders;
        }
        
        public List<ABuffRecorder> GetNegativeBuffListByTargetAndBuffID(IBattleEntity target)
        {
            return (from pair in Listeners from recorder in pair.Value where !recorder.DeletePending && ((SkillTemplate)recorder.Template).BuffType == Types.BuffType.Negative && Equals(recorder.Target, target) select recorder).ToList();
        }
        
        public List<ABuffRecorder> GetPositiveBuffListByTargetAndBuffID(IBattleEntity target)
        {
            return (from pair in Listeners from recorder in pair.Value where !recorder.DeletePending && ((SkillTemplate)recorder.Template).BuffType == Types.BuffType.Positive && Equals(recorder.Target, target) select recorder).ToList();
        }


        public override async UniTask<ASkillResult> ExecuteBuff(string evt, ASkillResult input, IBattleEntity target)
        {
            if (evt != Constant.Constant.BuffEventKey.BeforeExecuteBuff)
            {
                input = await ExecuteBuff(Constant.Constant.BuffEventKey.BeforeExecuteBuff, input, target);
            }
            return await base.ExecuteBuff(evt, input, target);
        }
    }
}