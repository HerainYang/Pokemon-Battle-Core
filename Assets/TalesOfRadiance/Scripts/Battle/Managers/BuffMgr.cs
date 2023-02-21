using System.Collections.Generic;
using CoreScripts.BattleComponents;
using CoreScripts.Managers;
using Cysharp.Threading.Tasks;
using TalesOfRadiance.Scripts.Battle.BattleComponents;
using TalesOfRadiance.Scripts.Battle.BattleComponents.RuntimeClass;

namespace TalesOfRadiance.Scripts.Battle.Managers
{
    public class BuffMgr : ABuffMgr
    {
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
    }
}