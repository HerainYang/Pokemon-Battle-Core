using System;
using System.Collections.Generic;
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
        public Func<List<Tuple<IBattleEntity, ASkillResult>>, ASkillResult, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>>[] OnProcedureFunctionsEndCallBacks;

        public Func<IBattleEntity, IBattleEntity, ABuffRecorder, UniTask> OnDestroyCallBacks;

        public Func<ASkillResult, IBattleEntity, IBattleEntity, ABuffRecorder, UniTask<ASkillResult>> BuffCallBacks;
    }
}