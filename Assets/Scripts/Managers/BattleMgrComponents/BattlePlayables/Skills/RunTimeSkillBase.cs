using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Enum;
using Managers.BattleMgrComponents.BattlePlayer;
using Managers.BattleMgrComponents.PokemonLogic;
using Managers.BattleMgrComponents.PokemonLogic.BuffResults;
using Unity.VisualScripting;
using UnityEngine;

namespace Managers.BattleMgrComponents.BattlePlayables.Skills
{
    public class RunTimeSkillBase : ABattlePlayable
    {
        public Pokemon Source;
        public int[] TargetIndices;
        public CommonSkillTemplate Template;
        public List<Func<Pokemon, Pokemon, CommonSkillTemplate, UniTask<bool>>> Procedure;

        public RunTimeSkillBase(CommonSkillTemplate template, Pokemon source, int[] targetIndices) : base(BattleLogic.GetPokemonSpeed(source))
        {
            Procedure = new List<Func<Pokemon, Pokemon, CommonSkillTemplate, UniTask<bool>>>();
            Template = template;
            Source = source;
            TargetIndices = targetIndices;
        }

        public override async void Execute()
        {
            var item = new BattleStackItem();
            item.Source = Source;
            item.Template = Template;
            BattleMgr.Instance.BattleStack.Add(item);
            
            if(Source.OnStage && !Source.IsFaint) 
                await BattleMgr.Instance.SetCommandText(BattleMgr.Instance.PlayerInGame[Source.TrainerID].PlayerInfo.name + " asks " + Source.GetName() + " to use " + Template.Name);
            if (Available == false)
            {
                if (Source.OnStage && !Source.IsFaint)
                {
                    await BattleMgr.Instance.SetCommandText("But failed!");
                }
                OnDestroy();
                return;
            }
            BattleLogic.CheckSkillValidation(this);
            await ExecuteList();
            
            OnDestroy();
        }

        protected override void OnDestroy()
        {
            BattleMgr.Instance.BattlePlayableEnd();
        }

        private async UniTask ExecuteList()
        {
            List<Pokemon> targets = await BattleLogic.FindTarget(TargetIndices);
            CommonResult result = new CommonResult();
            result.CanMove = true;
            result.Priority = (int)MovePriority.Normal;
            result.STemplate = Template;
            result.TargetsList = targets;
            result = await BuffMgr.Instance.ExecuteBuff(Constant.BuffExecutionTimeKey.BeforeMove, result, Source);
            
            if (!result.CanMove)
            {
                if (result.Message != null)
                {
                    await BattleMgr.Instance.SetCommandText(result.Message);
                }
                return;
            }
            if (targets == null || targets.Count == 0)
            {
                await BattleMgr.Instance.SetCommandText("But there is no target!");
                return;
            }

            BattleMgr.Instance.BattleStack[BattleMgr.Instance.BattleStack.Count - 1].Targets = targets;

            foreach (var target in targets)
            {
                bool funcResult;
                for (int i = 0; i < Procedure.Count; i++)
                {
                    funcResult = await Procedure[i](Source, target, Template);
                    if (!funcResult)
                    {
                        break;
                    }
                }
            }
        }
    }
}