using System;
using System.Collections.Generic;
using System.Linq;
using CoreScripts.BattleComponents;
using Cysharp.Threading.Tasks;
using PokemonDemo.Scripts.Managers;
using TalesOfRadiance.Scripts.Battle.BattleComponents.RuntimeClass;
using TalesOfRadiance.Scripts.Battle.BattlePlayables;
using TalesOfRadiance.Scripts.Battle.Managers;
using UnityEngine;
using BuffMgr = TalesOfRadiance.Scripts.Battle.Managers.BuffMgr;
using Types = TalesOfRadiance.Scripts.Battle.Constant.Types;

namespace TalesOfRadiance.Scripts.Battle.BattleComponents.BattleLogics
{
    public static partial class BattleLogic
    {
        public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> NormalAttack = async (input, entity, target, skillTemplate) =>
        {
            var skillResult = (SkillResult)input;
            var sourceEntity = (RuntimeHero)entity;
            var targetEntity = (RuntimeHero)target;
            var template = (SkillTemplate)skillTemplate;

            var damage = ApplyNormalDamage(sourceEntity.Properties.Attack * template.DamageIncreaseRate, targetEntity).Damage;

            if (await TrySetTargetHpActive(entity, target, template, damage) == false)
            {
                skillResult.ContinueProcedureFunction = false;
                return input;
            }
            
            EffectMgr.Instance.RenderLineFromTo(sourceEntity.Anchor.transform.position, targetEntity.Anchor.transform.position);
            await UniTask.Delay(BattleMgr.Instance.AnimationAwaitTime);
            skillResult.Damage = damage;
            return input;
        };

        public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> CleanAllNegativeBuff = async (input, entity, target, skillTemplate) =>
        {
            var skillResult = (SkillResult)input;
            var sourceEntity = (RuntimeHero)entity;
            var targetEntity = (RuntimeHero)target;
            var template = (SkillTemplate)skillTemplate;
            
            await BuffMgr.Instance.RemoveAllNegativeBuffByTarget(target, template.CurrentBuffRemovePriority);
            return input;
        };
        
        public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> HealWithExceedShield = async (input, entity, target, skillTemplate) =>
        {
            var skillResult = (SkillResult)input;
            var sourceEntity = (RuntimeHero)entity;
            var targetEntity = (RuntimeHero)target;
            var template = (SkillTemplate)skillTemplate;
            int health = (int)(template.PercentageDamageRate * targetEntity.Properties.MaxHealth);
            await targetEntity.SetTargetHp(health, true);
            await BuffMgr.Instance.ExecuteBuff(Constant.Constant.BuffEventKey.AfterHealActive, new SkillResult()
            {
                Damage = health,
                SkillSource = entity,
                SkillTarget = target
            }, targetEntity);
            return input;
        };
        
        
        
        public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> TryAddContinuousHealBuff = async (input, entity, target, skillTemplate) =>
        {
            var skillResult = (SkillResult)input;
            var sourceEntity = (RuntimeHero)entity;
            var targetEntity = (RuntimeHero)target;
            var template = (SkillTemplate)skillTemplate;
            await BuffMgr.Instance.AddBuff(sourceEntity, targetEntity, 4);
            return input;
        };
        
        public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> TryAddBuffForTest = async (input, entity, target, skillTemplate) =>
        {
            await BuffMgr.Instance.AddBuff(entity, target, 0);
            return input;
        };
        
        
        public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> TryApplyPercentageDamage = async (input, entity, target, skillTemplate) =>
        {
            var skillResult = (SkillResult)input;
            var sourceEntity = (RuntimeHero)entity;
            var targetEntity = (RuntimeHero)target;
            var template = (SkillTemplate)skillTemplate;
            int damage = (int)(template.PercentageDamageRate * targetEntity.Properties.MaxHealth);

            if (await TrySetTargetHpActive(entity, target, template, -damage) == false)
            {
                skillResult.ContinueProcedureFunction = false;
                return input;
            }
            
            skillResult.Damage = -damage;
            return input;
        };

        public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> TryAddBuffInBuffList = async (input, entity, target, skillTemplate) =>
        {
            var skillResult = (SkillResult)input;
            var sourceEntity = (RuntimeHero)entity;
            var targetEntity = (RuntimeHero)target;
            var template = (SkillTemplate)skillTemplate;

            for (int i = 0; i < template.AddBuffPossibility.Length; i++)
            {
                if (!ProbTrigger(template.AddBuffPossibility[i]))
                {
                    skillResult.ContinueProcedureFunction = false;
                    continue;
                }

                foreach (var index in template.AddBuffIndex[i])
                {
                    await TryAddBuff(sourceEntity, targetEntity, index);
                }
            }

            return input;
        };
        
        public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> TryAddBuffInInputStealBuffList = async (input, entity, target, skillTemplate) =>
        {
            var skillResult = (SkillResult)input;
            var sourceEntity = (RuntimeHero)entity;
            var targetEntity = (RuntimeHero)target;
            var template = (SkillTemplate)skillTemplate;
            

            foreach (var index in skillResult.StealBuffIDs)
            {
                await TryAddBuff(sourceEntity, targetEntity, index);
            }
            
            return input;
        };

        public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> TryBringBackToLife = async (input, entity, target, skillTemplate) =>
        {
            if (target == null)
                return input;

            var skillResult = (SkillResult)input;
            var sourceEntity = (RuntimeHero)entity;
            var targetEntity = (RuntimeHero)target;
            var template = (SkillTemplate)skillTemplate;

            BpDebut bpDebut = new BpDebut(targetEntity);

            var handler = EventMgr.Instance.ListenTo(CoreScripts.Constant.Constant.ListenToEvent.BattlePlayableReturnOwnerShip + skillResult.SelfPlayable.RuntimeID);
            BattleMgr.Instance.BorrowControlToPendingPlayable(skillResult.SelfPlayable, bpDebut);
            
            await UniTask.WaitUntil(() => handler.Complete);


            targetEntity.Properties.Hp = (int)(targetEntity.Properties.MaxHealth * 0.1);

            await UniTask.Delay(1000);

            return input;
        };

        public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> ExecuteNextSkillPlayable = async (input, entity, target, skillTemplate) =>
        {
            var skillResult = (SkillResult)input;
            var sourceEntity = (RuntimeHero)entity;
            var targetEntity = (RuntimeHero)target;
            var template = (SkillTemplate)skillTemplate;

            var nextSkillTemplate = ConfigManager.Instance.GetSkillTemplateByID(template.NextSkillID);
            var playable = await nextSkillTemplate.SendLoadSkillRequest(sourceEntity);
            
            skillResult.CallDefaultPlayableDestroyFunction = false;
            BattleMgr.Instance.TransferControlToPendingPlayable(playable);

            return input;
        };
        
        public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> TryHealByAttack = async (input, entity, target, skillTemplate) =>
        {
            var skillResult = (SkillResult)input;
            var sourceEntity = (RuntimeHero)entity;
            var targetEntity = (RuntimeHero)target;
            var template = (SkillTemplate)skillTemplate;
            int health = (int)(template.DamageIncreaseRate * sourceEntity.Properties.Attack);
            await TrySetTargetHpActive(entity, target, template, health, false);
            return input;
        };
        
        public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> StealOneBuff = async (input, entity, target, skillTemplate) =>
        {
            var skillResult = (SkillResult)input;
            var sourceEntity = (RuntimeHero)entity;
            var targetEntity = (RuntimeHero)target;
            var template = (SkillTemplate)skillTemplate;
            
            EffectMgr.Instance.RenderLineFromTo(sourceEntity.Anchor.transform.position, targetEntity.Anchor.transform.position);
            var buff = await BuffMgr.Instance.RemoveOnePositiveBuffByTarget(targetEntity, template.CurrentBuffRemovePriority);
            if(buff != null)
            {
                skillResult.StealBuffIDs.Add(buff.ID);
            }
            
            await UniTask.Delay(BattleMgr.Instance.AnimationAwaitTime);
            
            
            
            return input;
        };
        
        public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> RemoveAllPositiveBuffByChance = async (input, entity, target, skillTemplate) =>
        {
            var skillResult = (SkillResult)input;
            var sourceEntity = (RuntimeHero)entity;
            var targetEntity = (RuntimeHero)target;
            var template = (SkillTemplate)skillTemplate;

            if (!ProbTrigger(template.AddBuffPossibility[0]))
            {
                return input;
            }
            
            EffectMgr.Instance.RenderLineFromTo(sourceEntity.Anchor.transform.position, targetEntity.Anchor.transform.position);
            await BuffMgr.Instance.RemoveAllPositiveBuffByTarget(targetEntity, template.CurrentBuffRemovePriority);

            await UniTask.Delay(BattleMgr.Instance.AnimationAwaitTime);
            
            
            
            return input;
        };

        
        public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> LoopSkillNTime = async (input, entity, target, skillTemplate) =>
        {
            var skillResult = (SkillResult)input;
            var sourceEntity = (RuntimeHero)entity;
            var targetEntity = (RuntimeHero)target;
            var template = (SkillTemplate)skillTemplate;


            SkillTemplate loopSkillTemplate = ConfigManager.Instance.GetSkillTemplateByID(template.LoopSkillID);
            SkillResult prototype = new SkillResult();
            if (template.LoopLoadPreLoadData)
            {
                prototype = await loopSkillTemplate.GetPreloadData(sourceEntity);
            }
            else
            {
                prototype.TargetHeroes.Add(targetEntity);
            }
            
            for (int i = 0; i < template.LoopTime; i++)
            {
                SkillResult temp = prototype.Copy();
                BpSkill bpSkill = new BpSkill(sourceEntity, loopSkillTemplate, temp);
                
                var handler = EventMgr.Instance.ListenTo(CoreScripts.Constant.Constant.ListenToEvent.BattlePlayableReturnOwnerShip + skillResult.SelfPlayable.RuntimeID);
                BattleMgr.Instance.BorrowControlToPendingPlayable(skillResult.SelfPlayable, bpSkill);
                await UniTask.WaitUntil(() => handler.Complete);
            }
            
            
            
            return input;
        };

        public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> TryAddHolyFireWithCondition = async (input, entity, target, skillTemplate) =>
        {
            var skillResult = (SkillResult)input;
            var sourceEntity = (RuntimeHero)entity;
            var targetEntity = (RuntimeHero)target;
            var template = (SkillTemplate)skillTemplate;

            const float normalProb = 0.1f;
            const float specialProb = 0.5f;

            float prob;

            if (BuffMgr.Instance.ExistActiveBuff(targetEntity, ConfigManager.Instance.GetBuffTemplateByID(5)) || BuffMgr.Instance.ExistActiveBuff(targetEntity, ConfigManager.Instance.GetBuffTemplateByID(15)))
            {
                sourceEntity.Properties.CriticalRate *= 1.2f;
                skillResult.TemporaryChangeProperty = true;
                prob = normalProb;
            }
            else
            {
                prob = specialProb;
            }
            
            if (ProbTrigger(prob))
            {
                await TryAddBuff(sourceEntity, targetEntity, 15);
            }
            
            return input;
        };

        public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> ResetCriticalRate = async (input, entity, target, skillTemplate) =>
        {
            var skillResult = (SkillResult)input;
            var sourceEntity = (RuntimeHero)entity;
            var targetEntity = (RuntimeHero)target;
            var template = (SkillTemplate)skillTemplate;

            if (skillResult.TemporaryChangeProperty)
            {
                sourceEntity.Properties.CriticalRate /= 1.2f;
            }

            await UniTask.Yield();
            return input;
        };
        
        public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> SoulSpearCondition = async (input, entity, target, skillTemplate) =>
        {
            var skillResult = (SkillResult)input;
            var sourceEntity = (RuntimeHero)entity;
            var targetEntity = (RuntimeHero)target;
            var template = (SkillTemplate)skillTemplate;
            
            skillResult.CallDefaultPlayableDestroyFunction = false;

            var nextSkillTemplate = ConfigManager.Instance.GetSkillTemplateByID(BuffMgr.Instance.ExistActiveBuff(sourceEntity, ConfigManager.Instance.GetBuffTemplateByID(17)) ? 13 : 17);

            var playable = new BpSkill(sourceEntity, nextSkillTemplate, new SkillResult() { TargetHeroes = new List<RuntimeHero>(){targetEntity} });
            BattleMgr.Instance.TransferControlToPendingPlayable(playable);

            await UniTask.Yield();
            return input;
        };
        
        public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> ResetParentSkillCd = async (input, entity, target, skillTemplate) =>
        {
            var skillResult = (SkillResult)input;
            var sourceEntity = (RuntimeHero)entity;
            var targetEntity = (RuntimeHero)target;
            var template = (SkillTemplate)skillTemplate;

            foreach (var runtimeSkill in sourceEntity.RuntimeSkillList)
            {
                if (runtimeSkill.Template.ID == template.ParentSkillID)
                {
                    runtimeSkill.Cooldown = 0;
                    break;
                }
            }

            await UniTask.Yield();
            return input;
        };
        
        public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> MountainRiverCondition = async (input, entity, target, skillTemplate) =>
        {
            var skillResult = (SkillResult)input;
            var sourceEntity = (RuntimeHero)entity;
            var targetEntity = (RuntimeHero)target;
            var template = (SkillTemplate)skillTemplate;
            
            skillResult.CallDefaultPlayableDestroyFunction = false;

            SkillTemplate nextSkillTemplate;

            if (GetPosition(sourceEntity.Anchor.position) == Types.Position.Back)
            {
                nextSkillTemplate = ConfigManager.Instance.GetSkillTemplateByID(23);
            }
            else
            {
                nextSkillTemplate = ConfigManager.Instance.GetSkillTemplateByID(22);
            }


            var playable = await nextSkillTemplate.SendLoadSkillRequest(sourceEntity);
            BattleMgr.Instance.TransferControlToPendingPlayable(playable);

            await UniTask.Yield();
            return input;
        };


        public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> Mountain = async (input, entity, target, skillTemplate) =>
        {
            var skillResult = (SkillResult)input;
            var sourceEntity = (RuntimeHero)entity;
            var targetEntity = (RuntimeHero)target;
            var template = (SkillTemplate)skillTemplate;
            
            if (await TrySetTargetHpActive(entity, target, template, skillResult.Damage) == false)
            {
                skillResult.ContinueProcedureFunction = false;
                return input;
            }
            
            EffectMgr.Instance.RenderLineFromTo(sourceEntity.Anchor.transform.position, targetEntity.Anchor.transform.position);
            await UniTask.Delay(BattleMgr.Instance.AnimationAwaitTime);
            
            return input;
        };
        
        
        public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> StarSunCondition = async (input, entity, target, skillTemplate) =>
        {
            var skillResult = (SkillResult)input;
            var sourceEntity = (RuntimeHero)entity;
            var targetEntity = (RuntimeHero)target;
            var template = (SkillTemplate)skillTemplate;
            
            skillResult.CallDefaultPlayableDestroyFunction = false;

            SkillTemplate nextSkillTemplate;

            if (GetPosition(sourceEntity.Anchor.position) == Types.Position.Back)
            {
                nextSkillTemplate = ConfigManager.Instance.GetSkillTemplateByID(27);
            }
            else
            {
                nextSkillTemplate = ConfigManager.Instance.GetSkillTemplateByID(28);
            }


            var playable = await nextSkillTemplate.SendLoadSkillRequest(sourceEntity);
            BattleMgr.Instance.TransferControlToPendingPlayable(playable);

            await UniTask.Yield();
            return input;
        };
        
        public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> CopyNegativeBuffByTimes = async (input, entity, target, skillTemplate) =>
        {
            var skillResult = (SkillResult)input;
            var sourceEntity = (RuntimeHero)entity;
            var targetEntity = (RuntimeHero)target;
            var template = (SkillTemplate)skillTemplate;


            var negativeBuffList = BuffMgr.Instance.GetNegativeBuffListByTargetAndBuffID(target);
            var uniqueList = new List<int>();
            foreach (var buff in negativeBuffList)
            {
                if (uniqueList.Any(number => number == buff.Template.ID))
                {
                    continue;
                }
                uniqueList.Add(buff.Template.ID);
            }

            var top3 = uniqueList.Take(template.LoopTime);
            foreach (var id in top3)
            {
                for (int i = 0; i < template.BuffNumberLimit; i++)
                {
                    await TryAddBuff(sourceEntity, targetEntity, id);
                }
            }
            
            

            await UniTask.Yield();
            return input;
        };

    }
}