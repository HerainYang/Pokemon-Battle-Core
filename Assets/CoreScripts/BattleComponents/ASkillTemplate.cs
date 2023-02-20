using System;
using Cysharp.Threading.Tasks;

namespace CoreScripts.BattleComponents
{
    public abstract class ASkillTemplate
    {
        public string Name;
        public int ID;
        public string BuffTriggerEvent;
        public Func<ASkillResult, IBattleEntity, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>>[] ProcedureFunctions;
        public Func<ASkillResult, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>>[] OnLoadRequest;
        public Func<IBattleEntity, IBattleEntity, ABuffRecorder, UniTask> OnDestroyCallBack;

        public Func<ASkillResult, IBattleEntity, IBattleEntity, ABuffRecorder, UniTask<ASkillResult>> BuffCallBack;
    }
}