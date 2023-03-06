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
            buffTarget.Properties.Attack += (int)(buffTarget.Template.Defence * ((SkillTemplate)buffRecorder.Template).ValueChangeRate);

            await UniTask.Yield();
            return input;
        };

        public static readonly Func<IBattleEntity, IBattleEntity, ABuffRecorder, UniTask> UndoChangeAttack = async (source, target, recorder) =>
        {
            var buffSource = (RuntimeHero)source;
            var buffTarget = (RuntimeHero)target;
            var buffRecorder = (BuffRecorder)recorder;
            buffTarget.Properties.Attack = (int)(buffTarget.Template.Defence / (1f + ((SkillTemplate)buffRecorder.Template).ValueChangeRate));
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

            int damage = (int)(buffSource.Properties.Attack * ((SkillTemplate)buffRecorder.Template).PercentageDamageRate) + skillResult.Damage;
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
            await BuffMgr.Instance.RemoveBuffBySource(buffSource, ConfigManager.Instance.GetBuffTemplateByID(8));
            await UniTask.Yield();
        };
        
        public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ABuffRecorder, UniTask<ASkillResult>> DawnProtect = async (input, source, target, recorder) =>
        {
            var buffSource = (RuntimeHero)source;
            var buffTarget = (RuntimeHero)target;
            var skillResult = (SkillResult)input;
            var buffRecorder = (BuffRecorder)recorder;


            if (skillResult.CurrentDamageDonePriority > Types.DamageDonePriority.DawnProtect)
            {
                return input;
            }

            skillResult.CurrentDamageDonePriority = Types.DamageDonePriority.DawnProtect;
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
                    await teammate.SetTargetHp((int)(-skillResult.Damage * ((SkillTemplate)recorder.Template).PercentageDamageRate));
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
    }
}