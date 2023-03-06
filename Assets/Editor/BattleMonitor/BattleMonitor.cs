using CoreScripts;
using CoreScripts.BattleComponents;
using CoreScripts.BattlePlayables;

using UnityEditor;
using UnityEngine;
using Types = CoreScripts.Constant.Types;

namespace Editor.BattleMonitor
{
    public partial class BattleMonitor : EditorWindow
    {
        private static EditorWindow _thisWindow;

        private string[] _monitorOption =
        {
            "Current round battle playables",
            "Current Buff",
        };

        private int _optionChoice = 0;

        private const float ItemHeight = 20;
        private const float ItemMinWidth = 100;

        private static readonly GUIStyle Attribute = new GUIStyle();
        private static readonly GUIStyle Weather = new GUIStyle();

        [MenuItem("Tools/BattleMonitor")]
        public static void ShowWindow()
        {
            if (!_thisWindow)
            {
                _thisWindow = GetWindow(typeof(BattleMonitor));
                Attribute.normal.textColor = Color.red;
                Weather.normal.textColor = Color.green;
            }
        }

        private void OnGUI()
        {
            RenderMonitor();
        }

        private void RenderMonitor()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Select target data:");
            _optionChoice = EditorGUILayout.Popup(_optionChoice, _monitorOption);

            if (RuntimeLog.CurrentBattleManager.GetCurBattleRound() != null)
            {
                if (RuntimeLog.CurrentBattleManager.GetCurBattleRound().Status == CoreScripts.Constant.Types.BattleRoundStatus.Running)
                {
                    if (GUILayout.Button("Pause auto play battle round"))
                    {
                        RuntimeLog.CurrentBattleManager.SetBattleRoundStatus(CoreScripts.Constant.Types.BattleRoundStatus.Pause);
                    }
                }

                if (RuntimeLog.CurrentBattleManager.GetCurBattleRound().Status == CoreScripts.Constant.Types.BattleRoundStatus.Pause)
                {
                    if (GUILayout.Button("Continue auto play battle round"))
                    {
                        RuntimeLog.CurrentBattleManager.SetBattleRoundStatus(Types.BattleRoundStatus.Running);
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
                case PokemonDemo.Scripts.BattlePlayables.Skills.RunTimeSkillBase:
                {
                    RenderPokemonRunTimeSkillBase(playable);
                    break;
                }
                case PokemonDemo.Scripts.BattlePlayables.Stages.BpDebut:
                {
                    RenderPokemonDebut(playable);
                    break;
                }
                case TalesOfRadiance.Scripts.Battle.BattlePlayables.BpDebut:
                {
                    RenderTorDebut(playable);
                    break;
                }
                case TalesOfRadiance.Scripts.Battle.BattlePlayables.BpMove:
                {
                    RenderTorMove(playable);
                    break;
                }
                case TalesOfRadiance.Scripts.Battle.BattlePlayables.BpSkill:
                {
                    RenderTorSkill(playable);
                    break;
                }
                case TalesOfRadiance.Scripts.Battle.BattlePlayables.BpFaint:
                {
                    RenderTorFaint(playable);
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
            EditorGUILayout.LabelField("Buff", GUILayout.MinWidth(ItemMinWidth), GUILayout.MaxWidth(ItemMinWidth * 5), GUILayout.Height(ItemHeight));
            EditorGUILayout.LabelField("To Delete", GUILayout.MinWidth(ItemMinWidth), GUILayout.MaxWidth(ItemMinWidth * 5), GUILayout.Height(ItemHeight));
            EditorGUILayout.LabelField("Source", GUILayout.MinWidth(ItemMinWidth), GUILayout.MaxWidth(ItemMinWidth * 5), GUILayout.Height(ItemHeight));
            EditorGUILayout.LabelField("Target", GUILayout.MinWidth(ItemMinWidth), GUILayout.MaxWidth(ItemMinWidth * 5), GUILayout.Height(ItemHeight));
            EditorGUILayout.LabelField("Remain Round", GUILayout.MinWidth(ItemMinWidth), GUILayout.MaxWidth(ItemMinWidth * 5), GUILayout.Height(ItemHeight));
            EditorGUILayout.EndHorizontal();
            if (RuntimeLog.CurrentBattleManager is PokemonDemo.Scripts.BattleMgrComponents.BattleMgr)
            {
                RenderPokemonBuffManager();
            } else if (RuntimeLog.CurrentBattleManager is TalesOfRadiance.Scripts.Battle.Managers.BattleMgr)
            {
                RenderTorBuffManager();
            }
        }

        private void RenderSingleBuff(ABuffRecorder buffRecorder)
        {
            EditorGUILayout.BeginHorizontal();
            if (buffRecorder is PokemonDemo.Scripts.PokemonLogic.PokemonBuffRecorder)
            {
                RenderPokemonBuff(buffRecorder);
            } else if (buffRecorder is TalesOfRadiance.Scripts.Battle.BattleComponents.RuntimeClass.BuffRecorder)
            {
                RenderTorBuff(buffRecorder);
            }

            EditorGUILayout.EndHorizontal();
        }






    }
}