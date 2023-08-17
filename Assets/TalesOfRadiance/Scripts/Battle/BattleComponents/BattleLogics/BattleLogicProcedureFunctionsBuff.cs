using System;
using System.Collections.Generic;
using CoreScripts.BattleComponents;
using Cysharp.Threading.Tasks;
using TalesOfRadiance.Scripts.Battle.BattleComponents.RuntimeClass;
using TalesOfRadiance.Scripts.Battle.BattlePlayables;
using TalesOfRadiance.Scripts.Battle.Managers;
using UnityEngine;
using UnityEngine.Windows;
using Types = TalesOfRadiance.Scripts.Battle.Constant.Types;

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
        
        public static readonly Func<IBattleEntity, IBattleEntity, ABuffRecorder, UniTask> ForBuffCancelTest = async (source, target, recorder) =>
        {
            var buffSource = (RuntimeHero)source;
            var buffTarget = (RuntimeHero)target;
            var buffRecorder = (BuffRecorder)recorder;
            await UniTask.Yield();
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
        
        public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ABuffRecorder, UniTask<ASkillResult>> ChangeAttack = async (input, source, target, recorder) =>
        {
            var buffSource = (RuntimeHero)source;
            var buffTarget = (RuntimeHero)target;
            var skillResult = (SkillResult)input;
            var buffRecorder = (BuffRecorder)recorder;
            buffTarget.Properties.Attack += (int)(buffTarget.Template.Attack * ((SkillTemplate)buffRecorder.Template).ValueChangeRate);

            await UniTask.Yield();
            return input;
        };

        public static readonly Func<IBattleEntity, IBattleEntity, ABuffRecorder, UniTask> UndoChangeAttack = async (source, target, recorder) =>
        {
            var buffSource = (RuntimeHero)source;
            var buffTarget = (RuntimeHero)target;
            var buffRecorder = (BuffRecorder)recorder;
            buffTarget.Properties.Attack = (int)(buffTarget.Template.Attack / (1f + ((SkillTemplate)buffRecorder.Template).ValueChangeRate));
            await UniTask.Yield();
        };
        
        public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ABuffRecorder, UniTask<ASkillResult>> ChangeDamageAvoid = async (input, source, target, recorder) =>
        {
            var buffSource = (RuntimeHero)source;
            var buffTarget = (RuntimeHero)target;
            var skillResult = (SkillResult)input;
            var buffRecorder = (BuffRecorder)recorder;
            buffTarget.Properties.DamageAvoid += (int)(buffTarget.Template.DamageAvoid * ((SkillTemplate)buffRecorder.Template).ValueChangeRate);

            await UniTask.Yield();
            return input;
        };

        public static readonly Func<IBattleEntity, IBattleEntity, ABuffRecorder, UniTask> UndoChangeDamageAvoid = async (source, target, recorder) =>
        {
            var buffSource = (RuntimeHero)source;
            var buffTarget = (RuntimeHero)target;
            var buffRecorder = (BuffRecorder)recorder;
            buffTarget.Properties.DamageAvoid = (int)(buffTarget.Template.DamageAvoid / (1f + ((SkillTemplate)buffRecorder.Template).ValueChangeRate));
            await UniTask.Yield();
        };
        
        public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ABuffRecorder, UniTask<ASkillResult>> ChangeCriticalRate = async (input, source, target, recorder) =>
        {
            var buffSource = (RuntimeHero)source;
            var buffTarget = (RuntimeHero)target;
            var skillResult = (SkillResult)input;
            var buffRecorder = (BuffRecorder)recorder;
            buffTarget.Properties.CriticalRate += (int)(buffTarget.Template.CriticalRate * ((SkillTemplate)buffRecorder.Template).ValueChangeRate);

            await UniTask.Yield();
            return input;
        };

        public static readonly Func<IBattleEntity, IBattleEntity, ABuffRecorder, UniTask> UndoChangeCriticalRate = async (source, target, recorder) =>
        {
            var buffSource = (RuntimeHero)source;
            var buffTarget = (RuntimeHero)target;
            var buffRecorder = (BuffRecorder)recorder;
            buffTarget.Properties.CriticalRate = (int)(buffTarget.Template.CriticalRate / (1f + ((SkillTemplate)buffRecorder.Template).ValueChangeRate));
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
            await TrySetTargetHpPassive(source, target, (SkillTemplate)recorder.Template, healHp, false);
            buffTarget.Anchor.ShowEffectText(buffRecorder.Template.Name, Color.green);
            return input;
        };
        
        public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ABuffRecorder, UniTask<ASkillResult>> Burnt = async (input, source, target, recorder) =>
        {
            var buffSource = (RuntimeHero)source;
            var buffTarget = (RuntimeHero)target;
            var skillResult = (SkillResult)input;
            var buffRecorder = (BuffRecorder)recorder;

            int damage = (int)(buffSource.Properties.Attack * ((SkillTemplate)buffRecorder.Template).PercentageDamageRate) + skillResult.Damage;
            await TrySetTargetHpPassive(source, target, (SkillTemplate)recorder.Template, -damage);
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
            await TrySetTargetHpPassive(source, target, (SkillTemplate)recorder.Template, damage);
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
            await BuffMgr.Instance.RemoveBuffBySource(buffSource, ConfigManager.Instance.GetBuffTemplateByID(8));
            await UniTask.Yield();
        };
        
        public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ABuffRecorder, UniTask<ASkillResult>> DawnProtect = async (input, source, target, recorder) =>
        {
            var buffSource = (RuntimeHero)source;
            var buffTarget = (RuntimeHero)target;
            var skillResult = (SkillResult)input;
            var buffRecorder = (BuffRecorder)recorder;


            if (skillResult.SkillTemplate.SkillType != Types.SkillType.Active)
            {
                return input;
            }
            
            if (skillResult.CurrentDamageDonePriority > Types.DamageDonePriority.DawnProtect)
            {
                return input;
            }
            
            

            skillResult.CurrentDamageDonePriority = Types.DamageDonePriority.DawnProtect;
            skillResult.DamageShouldBeDone = false;
            await TrySetTargetHpPassive(skillResult.SkillSource, source, (SkillTemplate)recorder.Template, (int)(skillResult.Damage * ((SkillTemplate)buffRecorder.Template).PercentageDamageRate));
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
                BattleMgr.Instance.AddBattleFaint(buffTarget);
            }
            await UniTask.Yield();
        };
        
        public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ABuffRecorder, UniTask<ASkillResult>> DamageToTeamHeal = async (input, source, target, recorder) =>
        {
            var buffSource = (RuntimeHero)source;
            var buffTarget = (RuntimeHero)target;
            var skillResult = (SkillResult)input;
            var buffRecorder = (BuffRecorder)recorder;
            
            if(((RuntimeHero)skillResult.SkillSource).Team == buffSource.Team)
            {
                SkillResult teammates = (SkillResult)await SelectAllTeammate(new SkillResult(), buffSource, null);
                foreach (var teammate in teammates.TargetHeroes)
                {
                    teammate.Anchor.ShowEffectText(buffRecorder.Template.Name, Color.green);
                    await TrySetTargetHpPassive(source, teammate, (SkillTemplate)recorder.Template, (int)(-skillResult.Damage * ((SkillTemplate)recorder.Template).PercentageDamageRate), false);
                }
            }

            return input;
        };
        
        public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ABuffRecorder, UniTask<ASkillResult>> BurntLotus1 = async (input, source, target, recorder) =>
        {
            var buffSource = (RuntimeHero)source;
            var buffTarget = (RuntimeHero)target;
            var skillResult = (SkillResult)input;
            var buffRecorder = (BuffRecorder)recorder;

            if (buffRecorder.Target == null)
                return input;
            
            if (((RuntimeHero)buffRecorder.Target).Team == buffSource.Team)
            {
                if (buffRecorder.Template.ID == 5)
                {
                    skillResult.Damage += (int) (buffSource.Properties.Attack * ((SkillTemplate)buffRecorder.Template).ValueChangeRate);
                }
            }

            await UniTask.Yield();

            return input;
        };
        
        public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ABuffRecorder, UniTask<ASkillResult>> BurntLotus2 = async (input, source, target, recorder) =>
        {
            var buffSource = (RuntimeHero)source;
            var buffTarget = (RuntimeHero)target;
            var skillResult = (SkillResult)input;
            var buffRecorder = (BuffRecorder)recorder;

            var enemyTeam = GetEnemyTeam(buffSource);

            int highestBuffCount = 0;

            foreach (var hero in enemyTeam.Heroes)
            {
                if (hero.Properties.IsAlive)
                {
                    int tempCount = 0;

                    tempCount += BuffMgr.Instance.GetBuffListByTargetAndBuffID(hero, 5).Count;
                    tempCount += BuffMgr.Instance.GetBuffListByTargetAndBuffID(hero, 15).Count;

                    if (tempCount > highestBuffCount)
                    {
                        highestBuffCount = tempCount;
                    }
                }
            }

            ASkillResult targets;

            if (highestBuffCount < 3)
            {
                return input;
            }
            
            if (highestBuffCount < 6)
            {
                targets = await SelectRandomEnemy(new SkillResult(), source, new SkillTemplate(){TargetCount = 3});
            } else if (highestBuffCount < 9)
            {
                targets = await SelectRandomEnemy(new SkillResult(), source, new SkillTemplate(){TargetCount = 4});
            }
            else
            {
                targets = await SelectAllEnemy(new SkillResult(), source, new SkillTemplate());
            }

            var newSkill = new BpSkill(buffSource, ConfigManager.Instance.GetSkillTemplateByID(15), (SkillResult)targets);
            BattleMgr.Instance.AddBattleSkill(newSkill);

            return input;
        };

        public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ABuffRecorder, UniTask<ASkillResult>> AccumulateDamageAddBuffInList = async (input, source, target, recorder) =>
        {
            var buffSource = (RuntimeHero)source;
            var buffTarget = (RuntimeHero)target;
            var skillResult = (SkillResult)input;
            var buffRecorder = (BuffRecorder)recorder;

            buffRecorder.AccumulateDamage += Math.Abs(skillResult.Damage);
            int loopTime = (int)(buffRecorder.AccumulateDamage / (buffTarget.Properties.MaxHealth * ((SkillTemplate)buffRecorder.Template).ValueChangeRate)); 
            for (int i = 0; i < loopTime; i++)
            {
                foreach (var indices in ((SkillTemplate)buffRecorder.Template).AddBuffIndex)
                {
                    foreach (var index in indices)
                    {
                        await TryAddBuff(buffSource, buffSource, index);
                    }
                }
            }

            if (loopTime != 0)
            {
                buffRecorder.AccumulateDamage = (int)(buffRecorder.AccumulateDamage % (buffTarget.Properties.MaxHealth * ((SkillTemplate)buffRecorder.Template).ValueChangeRate));
            }
            

            await UniTask.Yield();
            return input;
        };
        
        public static readonly Func<IBattleEntity, IBattleEntity, ABuffRecorder, UniTask> RemoveAllBuffInList = async (source, target, recorder) =>
        {
            var buffSource = (RuntimeHero)source;
            var buffTarget = (RuntimeHero)target;
            var buffRecorder = (BuffRecorder)recorder;
            foreach (var indices in ((SkillTemplate)buffRecorder.Template).AddBuffIndex)
            {
                foreach (var index in indices)
                {
                    await BuffMgr.Instance.RemoveBuffByTarget(buffTarget, ConfigManager.Instance.GetBuffTemplateByID(index));
                }
            }
            
        };
        
        public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ABuffRecorder, UniTask<ASkillResult>> CheckHeartLotusNumber = async (input, source, target, recorder) =>
        {
            var buffSource = (RuntimeHero)source;
            var buffTarget = (RuntimeHero)target;
            var skillResult = (SkillResult)input;
            var buffRecorder = (BuffRecorder)recorder;

            var buffList = BuffMgr.Instance.GetBuffListByTargetAndBuffID(buffSource, 19);
            if (buffList.Count != 9) 
                return input;
            await BuffMgr.Instance.RemoveBuffByTarget(buffSource, ConfigManager.Instance.GetBuffTemplateByID(19));
            await TryAddBuff(buffSource, buffSource, 17);
            return input;
        };
        
        public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ABuffRecorder, UniTask<ASkillResult>> NormalBecomeGod = async (input, source, target, recorder) =>
        {
            var buffSource = (RuntimeHero)source;
            var buffTarget = (RuntimeHero)target;
            var skillResult = (SkillResult)input;
            var buffRecorder = (BuffRecorder)recorder;

            if (((RuntimeHero)skillResult.SkillTarget).Team != buffSource.Team)
            {
                return input;
            }

            if (skillResult.SkillTemplate.SkillType != Types.SkillType.Buff && (skillResult.SkillTemplate.ID != 5 || skillResult.SkillTemplate.ID != 15))
            {
                return input;
            }

            skillResult.Damage = (int)((1 - ((SkillTemplate)buffRecorder.Template).ValueChangeRate) * skillResult.Damage);

            if (Equals((RuntimeHero)skillResult.SkillTarget, buffSource))
            {
                if (ProbTrigger(0.5f))
                {
                    await TryAddBuff(buffSource, buffSource, 19);
                }
            }
            return input;
        };
        
        public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ABuffRecorder, UniTask<ASkillResult>> EverythingUpdatePrefix = async (input, source, target, recorder) =>
        {
            var buffSource = (RuntimeHero)source;
            var buffTarget = (RuntimeHero)target;
            var skillResult = (SkillResult)input;
            var buffRecorder = (BuffRecorder)recorder;

            if (!BuffMgr.Instance.ExistActiveBuff(buffSource, ConfigManager.Instance.GetBuffTemplateByID(25)))
            {
                return input;
            }

            skillResult.Damage = (int)(skillResult.Damage * buffSource.Properties.CriticalDamage);

            await UniTask.Yield();
            
            return input;
        };

        public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ABuffRecorder, UniTask<ASkillResult>> EverythingUpdatePostFix = async (input, source, target, recorder) =>{
            
            var buffSource = (RuntimeHero)source;
            var buffTarget = (RuntimeHero)target;
            var skillResult = (SkillResult)input;
            var buffRecorder = (BuffRecorder)recorder;

            if (BuffMgr.Instance.ExistActiveBuff(target, ConfigManager.Instance.GetBuffTemplateByID(25)) && ProbTrigger(0.35f))
            {
                Debug.LogWarning("Continue Attack Effect");
                var damage = ApplyNormalDamage(buffTarget.Properties.Attack * 0.6f, (RuntimeHero)skillResult.SkillTarget).Damage;
                await TrySetTargetHpPassive(target, skillResult.SkillTarget, (SkillTemplate)recorder.Template, damage);
            }

            return input;
        };
        
        public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ABuffRecorder, UniTask<ASkillResult>> FeatherProtect = async (input, source, target, recorder) =>{
            
            var buffSource = (RuntimeHero)source;
            var buffTarget = (RuntimeHero)target;
            var skillResult = (SkillResult)input;
            var buffRecorder = (BuffRecorder)recorder;

            if (skillResult.Damage > buffTarget.Properties.MaxHealth * 0.15)
            {
                buffRecorder.BuffLayerCount -= 1;
                skillResult.Damage = (int)(skillResult.Damage * 0.65);
                await TrySetTargetHpPassive(target, target, (SkillTemplate)buffRecorder.Template, (int)(buffTarget.Properties.MaxHealth * 0.1), false);
            }

            if (buffRecorder.BuffLayerCount == 0)
            {
                await BuffMgr.Instance.RemoveBuffByTarget(target, buffRecorder.Template);
            }

            return input;
        };

    }
}