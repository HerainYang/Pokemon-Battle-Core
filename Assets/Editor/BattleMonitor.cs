using System;
using CoreScripts;
using CoreScripts.BattlePlayables;
using Enum;
using Managers.BattleMgrComponents;
using Managers.BattleMgrComponents.BattlePlayables;
using Managers.BattleMgrComponents.BattlePlayables.Skills;
using Managers.BattleMgrComponents.BattlePlayables.Stages;
using Managers.BattleMgrComponents.PokemonLogic;
using PokemonLogic;
using PokemonLogic.PokemonData;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class BattleMonitor : EditorWindow
    {
        private static EditorWindow _thisWindow;

        private string[] _monitorOption =
        {
            "Current round battle playables",
            "Current Buff",
        };

        private int _optionChoice = 0;

        private float _itemHeight = 20;
        private float _itemMinWidth = 100;

        private static GUIStyle _attribute = new GUIStyle();
        private static GUIStyle _weather = new GUIStyle();

        [MenuItem("Tools/BattleMonitor")]
        public static void ShowWindow()
        {
            if (!_thisWindow)
            {
                _thisWindow = EditorWindow.GetWindow(typeof(BattleMonitor));
                _attribute.normal.textColor = Color.red;
                _weather.normal.textColor = Color.green;
            }
        }

        private void OnGUI()
        {
            RenderMonitor();
            // if (Application.isPlaying)
            // {
            //     RenderMonitor();
            // }
            // else
            // {
            //     EditorGUILayout.LabelField("Please run the game first");
            // }
        }

        private void RenderMonitor()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Select target data:");
            _optionChoice = EditorGUILayout.Popup(_optionChoice, _monitorOption);

            if (RuntimeLog.CurrentBattleManager.GetCurBattleRound() != null)
            {
                if (RuntimeLog.CurrentBattleManager.GetCurBattleRound().Status == BattleRoundStatus.Running)
                {
                    if (GUILayout.Button("Pause auto play battle round"))
                    {
                        RuntimeLog.CurrentBattleManager.SetBattleRoundStatus(BattleRoundStatus.Pause);
                    }
                }

                if (RuntimeLog.CurrentBattleManager.GetCurBattleRound().Status == BattleRoundStatus.Pause)
                {
                    if (GUILayout.Button("Continue auto play battle round"))
                    {
                        RuntimeLog.CurrentBattleManager.SetBattleRoundStatus(BattleRoundStatus.Running);
                        RuntimeLog.CurrentBattleManager.GetCurBattleRound().ExecuteBattleStage();
                    }

                    if (GUILayout.Button("Run next playables"))
                    {
                        RuntimeLog.CurrentBattleManager.GetCurBattleRound().ExecuteBattleStage();
                    }
                }
            }

            EditorGUILayout.EndVertical();


            Rect rect = EditorGUILayout.GetControlRect(false, _thisWindow.position.height, GUILayout.Width(1));
            EditorGUI.DrawRect(rect, new Color(1f, 1f, 1f, 1));

            EditorGUILayout.BeginVertical();
            RenderContent();
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }

        private void RenderContent()
        {
            switch (_optionChoice)
            {
                case 0:
                    RenderBattlePlayables();
                    break;
                case 1:
                    RenderBuffManager();
                    break;
            }
        }

        private void RenderBattlePlayables()
        {
            var battleRound = RuntimeLog.CurrentBattleManager.GetCurBattleRound();
            if (battleRound == null)
                return;
            EditorGUILayout.LabelField("Playables list:");
            foreach (var playable in battleRound.GetPlayables())
            {
                RenderSinglePlayable(playable);
            }

            EditorGUILayout.LabelField("Remaining playables list:");

            foreach (var playable in battleRound.GetRemainingPlayables())
            {
                RenderSinglePlayable(playable);
            }
        }

        private void RenderSinglePlayable(ABattlePlayable playable)
        {
            EditorGUILayout.BeginHorizontal();
            switch (playable)
            {
                case RunTimeSkillBase runTimeSkillBase:
                {
                    RunTimeSkillBase skillBase = runTimeSkillBase;
                    EditorGUILayout.LabelField(skillBase.RuntimeParam.RunTimeSkillBaseIsItem ? "Item: " : "Skill: ", GUILayout.MinWidth(_itemMinWidth - 50), GUILayout.Height(_itemHeight));

                    if (GUILayout.Button(skillBase.Template.Name, GUILayout.MinWidth(_itemMinWidth), GUILayout.Height(_itemHeight)))
                    {
                        DisplaySkillInfo(skillBase.Template);
                    }

                    if (skillBase.Source != null)
                    {
                        if (GUILayout.Button("Source: " + skillBase.Source.Name, GUILayout.MinWidth(_itemMinWidth), GUILayout.Height(_itemHeight)))
                        {
                            DisplayPokemonInfo(skillBase.Source);
                        }
                    }
                    else
                    {
                        GUILayout.Button("No Source", GUILayout.MinWidth(_itemMinWidth), GUILayout.Height(_itemHeight));
                    }

                    if (skillBase.RuntimeParam.TargetsByIndices != null)
                    {
                        foreach (var index in skillBase.RuntimeParam.TargetsByIndices)
                        {
                            if (BattleMgr.Instance.OnStagePokemon[index] != null)
                            {
                                if (GUILayout.Button("Target: " + BattleMgr.Instance.OnStagePokemon[index].Name, GUILayout.MinWidth(_itemMinWidth), GUILayout.Height(_itemHeight)))
                                {
                                    DisplayPokemonInfo(BattleMgr.Instance.OnStagePokemon[index]);
                                }
                            }
                            else
                            {
                                GUILayout.Button("Invalid Target", GUILayout.MinWidth(_itemMinWidth), GUILayout.Height(_itemHeight));
                            }
                        }
                    }

                    if (skillBase.GetRunTimeTarget() != null)
                    {
                        foreach (var pokemon in skillBase.GetRunTimeTarget())
                        {
                            if (pokemon != null)
                            {
                                if (GUILayout.Button("Target: " + pokemon.Name, GUILayout.MinWidth(_itemMinWidth), GUILayout.Height(_itemHeight)))
                                {
                                    DisplayPokemonInfo(pokemon);
                                }
                            }
                            else
                            {
                                GUILayout.Button("Invalid Target", GUILayout.MinWidth(_itemMinWidth), GUILayout.Height(_itemHeight));
                            }
                        }
                    }


                    break;
                }
                case BpDebut debut:
                {
                    EditorGUILayout.LabelField("Debut: ", GUILayout.MinWidth(_itemMinWidth - 50), GUILayout.Height(_itemHeight));
                    if (GUILayout.Button(debut.GetPokemonInstance().Name, GUILayout.MinWidth(_itemMinWidth), GUILayout.Height(_itemHeight)))
                    {
                        DisplayPokemonInfo(debut.GetPokemonInstance());
                    }

                    break;
                }
                default:
                    EditorGUILayout.LabelField(playable.ToString());
                    break;
            }

            EditorGUILayout.EndHorizontal();
        }

        private void RenderBuffManager()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Buff", GUILayout.MinWidth(_itemMinWidth), GUILayout.MaxWidth(_itemMinWidth * 5), GUILayout.Height(_itemHeight));
            EditorGUILayout.LabelField("Source", GUILayout.MinWidth(_itemMinWidth), GUILayout.MaxWidth(_itemMinWidth * 5), GUILayout.Height(_itemHeight));
            EditorGUILayout.LabelField("Target", GUILayout.MinWidth(_itemMinWidth), GUILayout.MaxWidth(_itemMinWidth * 5), GUILayout.Height(_itemHeight));
            EditorGUILayout.LabelField("Remain Round", GUILayout.MinWidth(_itemMinWidth), GUILayout.MaxWidth(_itemMinWidth * 5), GUILayout.Height(_itemHeight));
            EditorGUILayout.EndHorizontal();
            foreach (var listener in BuffMgr.Instance.GetAllBuff())
            {
                if (listener.Value == null || listener.Value.Count == 0)
                    continue;
                EditorGUILayout.LabelField("Trigger on " + listener.Key, EditorStyles.boldLabel);

                foreach (var buffRecorder in listener.Value)
                {
                    EditorGUILayout.BeginHorizontal();
                    RenderSingleBuff(buffRecorder);
                    EditorGUILayout.EndHorizontal();
                }
            }
        }

        private void RenderSingleBuff(BuffRecorder buffRecorder)
        {
            EditorGUILayout.BeginHorizontal();
            if (buffRecorder.IsAttribute)
            {
                EditorGUILayout.LabelField("Attribute: " + buffRecorder.Template.Name, GUILayout.MinWidth(_itemMinWidth), GUILayout.MaxWidth(_itemMinWidth * 5), GUILayout.Height(_itemHeight));
            }
            else if (buffRecorder.IsWeather)
            {
                EditorGUILayout.LabelField("Weather: " + buffRecorder.Template.Name, GUILayout.MinWidth(_itemMinWidth), GUILayout.MaxWidth(_itemMinWidth * 5), GUILayout.Height(_itemHeight));
            }
            else
            {
                EditorGUILayout.LabelField(buffRecorder.Template.Name, GUILayout.MinWidth(_itemMinWidth), GUILayout.MaxWidth(_itemMinWidth * 5), GUILayout.Height(_itemHeight));
            }

            if (buffRecorder.Source != null)
            {
                if (GUILayout.Button(buffRecorder.Source.Name, GUILayout.MinWidth(_itemMinWidth), GUILayout.MaxWidth(_itemMinWidth * 5), GUILayout.Height(_itemHeight)))
                {
                    DisplayPokemonInfo(buffRecorder.Source);
                }
            }
            else
            {
                EditorGUILayout.LabelField("No Source", GUILayout.MinWidth(_itemMinWidth), GUILayout.MaxWidth(_itemMinWidth * 5), GUILayout.Height(_itemHeight));
            }

            if (buffRecorder.Target != null)
            {
                if (GUILayout.Button(buffRecorder.Target.Name, GUILayout.MinWidth(_itemMinWidth), GUILayout.MaxWidth(_itemMinWidth * 5), GUILayout.Height(_itemHeight)))
                {
                    DisplayPokemonInfo(buffRecorder.Target);
                }
            }
            else
            {
                EditorGUILayout.LabelField("No Target", GUILayout.MinWidth(_itemMinWidth), GUILayout.MaxWidth(_itemMinWidth * 5), GUILayout.Height(_itemHeight));
            }
            
            EditorGUILayout.LabelField(buffRecorder.EffectLastRound.ToString(), GUILayout.MinWidth(_itemMinWidth), GUILayout.MaxWidth(_itemMinWidth * 5), GUILayout.Height(_itemHeight));

            EditorGUILayout.EndHorizontal();
        }

        private void DisplayPokemonInfo(Pokemon pokemon)
        {
            PokemonInfoMonitor.ShowWindow(pokemon);
        }

        private void DisplaySkillInfo(CommonSkillTemplate template)
        {
            SkillInfoMonitor.ShowWindow(template);
        }
    }
}