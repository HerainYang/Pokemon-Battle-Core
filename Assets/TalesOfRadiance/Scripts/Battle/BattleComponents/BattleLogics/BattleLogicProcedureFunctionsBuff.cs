using System;
using CoreScripts.BattleComponents;
using Cysharp.Threading.Tasks;
using TalesOfRadiance.Scripts.Battle.BattleComponents.RuntimeClass;
using TalesOfRadiance.Scripts.Battle.Managers;
using UnityEngine;
using UnityEngine.Windows;

namespace TalesOfRadiance.Scripts.Battle.BattleComponents.BattleLogics
{
    public partial class BattleLogic
    {
        public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ABuffRecorder, UniTask<ASkillResult>> ForBuffTest = async (input, source, target, recorder) =>
        {
            var buffSource = (RuntimeHero)source;
            var buffTarget = (RuntimeHero)target;
            var skillResult = (SkillResult)input;
            var buffRecorder = (BuffRecorder)recorder;
            await UniTask.Yield();
            return input;
        };

        public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ABuffRecorder, UniTask<ASkillResult>> ChangeHpMax = async (input, source, target, recorder) =>
        {
            var buffSource = (RuntimeHero)source;
            var buffTarget = (RuntimeHero)target;
            var skillResult = (SkillResult)input;
            var buffRecorder = (BuffRecorder)recorder;
            buffTarget.Properties.MaxHealth = (int)((1f + ((SkillTemplate)buffRecorder.Template).ValueChangeRate) * buffTarget.Template.MaxHealth);
            if (((SkillTemplate)buffRecorder.Template).RecoverHp)
            {
                buffTarget.Properties.Hp = buffTarget.Properties.MaxHealth;
            }

            await UniTask.Yield();
            return input;
        };

        public static readonly Func<IBattleEntity, IBattleEntity, ABuffRecorder, UniTask> UndoChangeHpMax = async (source, target, recorder) =>
        {
            var buffSource = (RuntimeHero)source;
            var buffTarget = (RuntimeHero)target;
            var buffRecorder = (BuffRecorder)recorder;
            buffTarget.Properties.MaxHealth = (int)(buffTarget.Template.MaxHealth / (1f + ((SkillTemplate)buffRecorder.Template).ValueChangeRate));
            if (buffTarget.Properties.Hp > buffTarget.Properties.MaxHealth)
            {
                buffTarget.Properties.Hp = buffTarget.Properties.MaxHealth;
            }
            await UniTask.Yield();
        };

        public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ABuffRecorder, UniTask<ASkillResult>> ChangeDefense = async (input, source, target, recorder) =>
        {
            var buffSource = (RuntimeHero)source;
            var buffTarget = (RuntimeHero)target;
            var skillResult = (SkillResult)input;
            var buffRecorder = (BuffRecorder)recorder;
            buffTarget.Properties.Defence += (int)(buffTarget.Template.Defence * ((SkillTemplate)buffRecorder.Template).ValueChangeRate);

            await UniTask.Yield();
            return input;
        };

        public static readonly Func<IBattleEntity, IBattleEntity, ABuffRecorder, UniTask> UndoChangeDefense = async (source, target, recorder) =>
        {
            var buffSource = (RuntimeHero)source;
            var buffTarget = (RuntimeHero)target;
            var buffRecorder = (BuffRecorder)recorder;
            buffTarget.Properties.Defence = (int)(buffTarget.Template.Defence / (1f + ((SkillTemplate)buffRecorder.Template).ValueChangeRate));
            await UniTask.Yield();
        };

        public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ABuffRecorder, UniTask<ASkillResult>> IncreaseDamageAvoidWhenHpDecrease = async (input, source, target, recorder) =>
        {
            var buffSource = (RuntimeHero)source;
            var buffTarget = (RuntimeHero)target;
            var skillResult = (SkillResult)input;
            var buffRecorder = (BuffRecorder)recorder;
            var differentRate = (buffTarget.Properties.MaxHealth - buffTarget.Properties.Hp) / (float)buffTarget.Properties.MaxHealth;
            var increaseRate = differentRate / ((SkillTemplate)buffRecorder.Template).ValueChangeRate;
            buffTarget.Properties.DamageAvoid += buffTarget.Template.DamageAvoid * increaseRate;
            await UniTask.Yield();
            buffTarget.Anchor.ShowEffectText(buffRecorder.Template.Name, Color.green);
            return input;
        };
        
        public static readonly Func<IBattleEntity, IBattleEntity, ABuffRecorder, UniTask> UndoIncreaseDamageAvoidWhenHpDecrease = async (source, target, recorder) =>
        {
            var buffSource = (RuntimeHero)source;
            var buffTarget = (RuntimeHero)target;
            var buffRecorder = (BuffRecorder)recorder;
            var differentRate = (buffTarget.Properties.MaxHealth - buffTarget.Properties.Hp) / (float)buffTarget.Properties.MaxHealth;
            var increaseRate = differentRate / ((SkillTemplate)buffRecorder.Template).ValueChangeRate;
            buffTarget.Properties.DamageAvoid -= buffTarget.Template.DamageAvoid * increaseRate;
            await UniTask.Yield();
        };
        
        public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ABuffRecorder, UniTask<ASkillResult>> ContinueHeal = async (input, source, target, recorder) =>
        {
            var buffSource = (RuntimeHero)source;
            var buffTarget = (RuntimeHero)target;
            var skillResult = (SkillResult)input;
            var buffRecorder = (BuffRecorder)recorder;
            int healHp = (int)(buffTarget.Properties.MaxHealth * ((SkillTemplate)buffRecorder.Template).PercentageDamageRate);
            await buffTarget.SetTargetHp(healHp);
            buffTarget.Anchor.ShowEffectText(buffRecorder.Template.Name, Color.green);
            return input;
        };
        
        public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ABuffRecorder, UniTask<ASkillResult>> Burnt = async (input, source, target, recorder) =>
        {
            var buffSource = (RuntimeHero)source;
            var buffTarget = (RuntimeHero)target;
            var skillResult = (SkillResult)input;
            var buffRecorder = (BuffRecorder)recorder;

            int damage = (int)(buffSource.Properties.Attack * ((SkillTemplate)buffRecorder.Template).PercentageDamageRate);
            await buffTarget.SetTargetHp(-damage);
            buffTarget.Anchor.ShowEffectText(buffRecorder.Template.Name, Color.red);
            return input;
        };
        
        public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ABuffRecorder, UniTask<ASkillResult>> FireJudgement = async (input, source, target, recorder) =>
        {
            var buffSource = (RuntimeHero)source;
            var buffTarget = (RuntimeHero)target;
            var skillResult = (SkillResult)input;
            var buffRecorder = (BuffRecorder)recorder;


            int damage = (int)(skillResult.Damage * 0.5f);
            await buffTarget.SetTargetHp(damage);
            buffTarget.Anchor.ShowEffectText(buffRecorder.Template.Name, Color.red);
            
            return input;
        };
        
        public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ABuffRecorder, UniTask<ASkillResult>> DawnProtectPrefix = async (input, source, target, recorder) =>
        {
            var buffSource = (RuntimeHero)source;
            var buffTarget = (RuntimeHero)target;
            var skillResult = (SkillResult)input;
            var buffRecorder = (BuffRecorder)recorder;

            var allTeammate = await SelectAllTeammate(new SkillResult(), target, buffRecorder.Template);
            var allTeammateExceptSelf = await DeselectSelfFromPrevious(allTeammate, target, buffRecorder.Template);
            var lowest = await SelectLowestHpFromPrevious(allTeammateExceptSelf, target, buffRecorder.Template);
            RuntimeHero lowestHero = ((SkillResult)lowest).TargetHeroes[0];

            if (lowestHero.Properties.Hp < (lowestHero.Properties.MaxHealth / 2))
            {
                await BuffMgr.Instance.AddBuff(buffTarget, lowestHero, 8);
            }

            return input;
        };
        
        public static readonly Func<IBattleEntity, IBattleEntity, ABuffRecorder, UniTask> UndoDawnProtect = async (source, target, recorder) =>
        {
            var buffSource = (RuntimeHero)source;
            var buffTarget = (RuntimeHero)target;
            var buffRecorder = (BuffRecorder)recorder;
            // All protect effect withdraw
            BuffMgr.Instance.RemoveBuffBySource(buffSource, ConfigManager.Instance.GetBuffTemplateByID(8));
            await UniTask.Yield();
        };
        
        public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ABuffRecorder, UniTask<ASkillResult>> DawnProtect = async (input, source, target, recorder) =>
        {
            var buffSource = (RuntimeHero)source;
            var buffTarget = (RuntimeHero)target;
            var skillResult = (SkillResult)input;
            var buffRecorder = (BuffRecorder)recorder;

            skillResult.DamageShouldBeDone = false;
            await buffSource.SetTargetHp((int)(skillResult.Damage * ((SkillTemplate)buffRecorder.Template).PercentageDamageRate));
            EffectMgr.Instance.RenderLineFromTo(((RuntimeHero)skillResult.SkillSource).Anchor.transform.position, buffSource.Anchor.transform.position);
            buffSource.Anchor.ShowEffectText(buffRecorder.Template.Name, Color.green);
            ((RuntimeHero)skillResult.SkillTarget).Anchor.ShowEffectText(buffRecorder.Template.Name, Color.green);
            await UniTask.Delay(1000);
            
            return input;
        };
        
        public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ABuffRecorder, UniTask<ASkillResult>> Immortal = async (input, source, target, recorder) =>
        {
            var buffSource = (RuntimeHero)source;
            var buffTarget = (RuntimeHero)target;
            var skillResult = (SkillResult)input;
            var buffRecorder = (BuffRecorder)recorder;
            skillResult.HeroShouldFaint = false;

            await UniTask.Yield();
            
            return input;
        };
        
        public static readonly Func<IBattleEntity, IBattleEntity, ABuffRecorder, UniTask> UndoImmortal = async (source, target, recorder) =>
        {
            var buffSource = (RuntimeHero)source;
            var buffTarget = (RuntimeHero)target;
            var buffRecorder = (BuffRecorder)recorder;
            // All protect effect withdraw
            if (buffTarget.Properties.Hp <= 0)
            {
                Debug.LogWarning("Add one " + buffTarget.Properties.Hp);
                BattleMgr.Instance.AddBattleFaint(buffTarget);
            }
            await UniTask.Yield();
        };
    }
}