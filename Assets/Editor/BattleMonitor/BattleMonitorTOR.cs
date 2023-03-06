using CoreScripts.BattleComponents;
using CoreScripts.BattlePlayables;
using TalesOfRadiance.Scripts.Battle.BattleComponents.RuntimeClass;
using TalesOfRadiance.Scripts.Battle.BattlePlayables;
using TalesOfRadiance.Scripts.Battle.Managers;
using UnityEditor;
using UnityEngine;

namespace Editor.BattleMonitor
{
    public partial class BattleMonitor : EditorWindow
    {
        private void RenderTorDebut(ABattlePlayable playable)
        {
            BpDebut debut = (BpDebut)playable;
            EditorGUILayout.LabelField("Debut: ", GUILayout.MinWidth(ItemMinWidth - 50), GUILayout.Height(ItemHeight));
            if (debut.GetHeroInstance() != null)
            {
                if (GUILayout.Button(debut.GetHeroInstance().Template.Name, GUILayout.MinWidth(ItemMinWidth), GUILayout.Height(ItemHeight)))
                {
                    DisplayRuntimeHeroInfo(debut.GetHeroInstance());
                }
            }
        }

        private void RenderTorMove(ABattlePlayable playable)
        {
            BpMove move = (BpMove)playable;
            EditorGUILayout.LabelField("Move: ", GUILayout.MinWidth(ItemMinWidth - 50), GUILayout.Height(ItemHeight));
            if (move.Source != null)
            {
                if (move.Source is RuntimeHero hero)
                {
                    if (GUILayout.Button(hero.Template.Name, GUILayout.MinWidth(ItemMinWidth), GUILayout.Height(ItemHeight)))
                    {
                        DisplayRuntimeHeroInfo(hero);
                    }
                }
            }
        }

        private void RenderTorSkill(ABattlePlayable playable)
        {
            BpSkill skill = (BpSkill)playable;
            if (GUILayout.Button(skill.GetSkillTemplate().Name, GUILayout.MinWidth(ItemMinWidth), GUILayout.Height(ItemHeight)))
            {
            }

            if (skill.Source != null)
            {
                if (skill.Source is RuntimeHero hero)
                {
                    if (GUILayout.Button("Source: " + hero.Template.Name, GUILayout.MinWidth(ItemMinWidth), GUILayout.Height(ItemHeight)))
                    {
                        DisplayRuntimeHeroInfo(hero);
                    }
                }
                else
                {
                    GUILayout.Button("Not hero source", GUILayout.MinWidth(ItemMinWidth), GUILayout.Height(ItemHeight));
                }
            }
            else
            {
                GUILayout.Button("No Source", GUILayout.MinWidth(ItemMinWidth), GUILayout.Height(ItemHeight));
            }

            if (skill.GetPreLoadData() != null)
            {
                foreach (var hero in skill.GetPreLoadData().TargetHeroes)
                {
                    if (hero != null)
                    {
                        if (GUILayout.Button("Target: " + hero.Template.Name, GUILayout.MinWidth(ItemMinWidth), GUILayout.Height(ItemHeight)))
                        {
                            DisplayRuntimeHeroInfo(hero);
                        }
                    }
                    else
                    {
                        GUILayout.Button("Invalid Target", GUILayout.MinWidth(ItemMinWidth), GUILayout.Height(ItemHeight));
                    }
                }
            }
        }

        private void RenderTorFaint(ABattlePlayable playable)
        {
            BpFaint faint = (BpFaint)playable;
            EditorGUILayout.LabelField("Faint: ", GUILayout.MinWidth(ItemMinWidth - 50), GUILayout.Height(ItemHeight));
            if (faint.GetHeroInstance() != null)
            {
                if (GUILayout.Button(faint.GetHeroInstance().Template.Name, GUILayout.MinWidth(ItemMinWidth), GUILayout.Height(ItemHeight)))
                {
                    DisplayRuntimeHeroInfo(faint.GetHeroInstance());
                }
            }
        }

        private void DisplayRuntimeHeroInfo(RuntimeHero hero)
        {
            RuntimeHeroMonitor.ShowWindow(hero);
        }

        private void RenderTorBuffManager()
        {
            foreach (var listener in BuffMgr.Instance.GetAllBuff())
            {
                if (listener.Value == null || listener.Value.Count == 0)
                    continue;
                EditorGUILayout.LabelField("Trigger on " + listener.Key, EditorStyles.boldLabel);

                foreach (var buffRecorder in listener.Value)
                {
                    EditorGUILayout.BeginHorizontal();
                    RenderSingleBuff((BuffRecorder)buffRecorder);
                    EditorGUILayout.EndHorizontal();
                }
            }
        }

        private void RenderTorBuff(ABuffRecorder aBuffRecorder)
        {
            BuffRecorder buffRecorder = (BuffRecorder)aBuffRecorder;
            if (buffRecorder.IsAttribute)
            {
                EditorGUILayout.LabelField("Attribute: " + buffRecorder.Template.Name, GUILayout.MinWidth(ItemMinWidth), GUILayout.MaxWidth(ItemMinWidth * 5), GUILayout.Height(ItemHeight));
            }
            else
            {
                EditorGUILayout.LabelField(buffRecorder.Template.Name, GUILayout.MinWidth(ItemMinWidth), GUILayout.MaxWidth(ItemMinWidth * 5), GUILayout.Height(ItemHeight));
            }
            
            EditorGUILayout.LabelField(buffRecorder.DeletePending.ToString(), GUILayout.MinWidth(ItemMinWidth), GUILayout.MaxWidth(ItemMinWidth * 5), GUILayout.Height(ItemHeight));

            if (buffRecorder.Source != null)
            {
                if (GUILayout.Button(((RuntimeHero)buffRecorder.Source).Template.Name, GUILayout.MinWidth(ItemMinWidth), GUILayout.MaxWidth(ItemMinWidth * 5), GUILayout.Height(ItemHeight)))
                {
                    DisplayRuntimeHeroInfo(((RuntimeHero)buffRecorder.Source));
                }
            }
            else
            {
                EditorGUILayout.LabelField("No Source", GUILayout.MinWidth(ItemMinWidth), GUILayout.MaxWidth(ItemMinWidth * 5), GUILayout.Height(ItemHeight));
            }

            if (buffRecorder.Target != null)
            {
                if (GUILayout.Button(((RuntimeHero)buffRecorder.Target).Template.Name, GUILayout.MinWidth(ItemMinWidth), GUILayout.MaxWidth(ItemMinWidth * 5), GUILayout.Height(ItemHeight)))
                {
                    DisplayRuntimeHeroInfo(((RuntimeHero)buffRecorder.Target));
                }
            }
            else
            {
                EditorGUILayout.LabelField("No Target", GUILayout.MinWidth(ItemMinWidth), GUILayout.MaxWidth(ItemMinWidth * 5), GUILayout.Height(ItemHeight));
            }

            EditorGUILayout.LabelField(buffRecorder.EffectLastRound.ToString(), GUILayout.MinWidth(ItemMinWidth), GUILayout.MaxWidth(ItemMinWidth * 5), GUILayout.Height(ItemHeight));
        }
    }
}