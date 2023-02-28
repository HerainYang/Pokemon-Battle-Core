using System.Collections.Generic;
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

            await UniTask.Yield();
            
            return recorder;
        }

        public async UniTask RemoveAllNegativeBuffByTarget(IBattleEntity target)
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
                    if (recorder.Target != null && recorder.Target.Equals(target) && recorder.Template is SkillTemplate template)
                    {
                        if (template.BuffType != Types.BuffType.Negative)
                        {
                            continue;
                        }
                        if (recorder.Template.OnDestroyCallBacks != null)
                        {
                            await recorder.Template.OnDestroyCallBacks(recorder.Source, recorder.Target, recorder);
                        }
                        list.RemoveAt(i);
                    }
                }
            }
        }

        
        // todo: change to remove
        public async UniTask<SkillTemplate> GetOnePositiveBuffByTarget(IBattleEntity target)
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
                    if (recorder.Target != null && recorder.Target.Equals(target) && !recorder.IsAttribute && recorder.Template is SkillTemplate template)
                    {
                        if (template.BuffType == Types.BuffType.Positive)
                        {
                            return template;
                        }
                    }
                }
            }

            return null;
        }
    }
}