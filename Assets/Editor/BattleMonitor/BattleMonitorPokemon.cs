using CoreScripts;
using CoreScripts.BattleComponents;
using CoreScripts.BattlePlayables;
using PokemonDemo.Scripts.BattleMgrComponents;
using PokemonDemo.Scripts.BattlePlayables.Skills;
using PokemonDemo.Scripts.BattlePlayables.Stages;
using PokemonDemo.Scripts.Managers;
using PokemonDemo.Scripts.PokemonLogic;
using PokemonDemo.Scripts.PokemonLogic.PokemonData;
using UnityEditor;
using UnityEngine;
using Types = CoreScripts.Constant.Types;

namespace Editor.BattleMonitor
{
    public partial class BattleMonitor : EditorWindow
    {

        private void RenderPokemonRunTimeSkillBase(ABattlePlayable playable)
        {
            RunTimeSkillBase skillBase = (RunTimeSkillBase)playable;
            EditorGUILayout.LabelField(skillBase.RuntimeParam.RunTimeSkillBaseIsItem ? "Item: " : "Skill: ", GUILayout.MinWidth(ItemMinWidth - 50), GUILayout.Height(ItemHeight));

            if (GUILayout.Button(skillBase.Template.Name, GUILayout.MinWidth(ItemMinWidth), GUILayout.Height(ItemHeight)))
            {
                DisplayPokemonSkillInfo(skillBase.Template);
            }

            if (skillBase.PokemonSource != null)
            {
                if (GUILayout.Button("Source: " + skillBase.PokemonSource.Name, GUILayout.MinWidth(ItemMinWidth), GUILayout.Height(ItemHeight)))
                {
                    DisplayPokemonInfo(skillBase.PokemonSource);
                }
            }
            else
            {
                GUILayout.Button("No Source", GUILayout.MinWidth(ItemMinWidth), GUILayout.Height(ItemHeight));
            }

            if (skillBase.RuntimeParam.TargetsByIndices != null)
            {
                foreach (var index in skillBase.RuntimeParam.TargetsByIndices)
                {
                    if (BattleMgr.Instance.OnStagePokemon[index] != null)
                    {
                        if (GUILayout.Button("Target: " + BattleMgr.Instance.OnStagePokemon[index].Name, GUILayout.MinWidth(ItemMinWidth), GUILayout.Height(ItemHeight)))
                        {
                            DisplayPokemonInfo(BattleMgr.Instance.OnStagePokemon[index]);
                        }
                    }
                    else
                    {
                        GUILayout.Button("Invalid Target", GUILayout.MinWidth(ItemMinWidth), GUILayout.Height(ItemHeight));
                    }
                }
            }

            if (skillBase.GetRunTimeTarget() != null)
            {
                foreach (var pokemon in skillBase.GetRunTimeTarget())
                {
                    if (pokemon != null)
                    {
                        if (GUILayout.Button("Target: " + pokemon.Name, GUILayout.MinWidth(ItemMinWidth), GUILayout.Height(ItemHeight)))
                        {
                            DisplayPokemonInfo(pokemon);
                        }
                    }
                    else
                    {
                        GUILayout.Button("Invalid Target", GUILayout.MinWidth(ItemMinWidth), GUILayout.Height(ItemHeight));
                    }
                }
            }
        }

        private void RenderPokemonDebut(ABattlePlayable playable)
        {
            BpDebut debut = (BpDebut)playable;
            EditorGUILayout.LabelField("Debut: ", GUILayout.MinWidth(ItemMinWidth - 50), GUILayout.Height(ItemHeight));
            if (GUILayout.Button(debut.GetPokemonInstance().Name, GUILayout.MinWidth(ItemMinWidth), GUILayout.Height(ItemHeight)))
            {
                DisplayPokemonInfo(debut.GetPokemonInstance());
            }
        }

        private void DisplayPokemonInfo(Pokemon pokemon)
        {
            PokemonInfoMonitor.ShowWindow(pokemon);
        }

        private void DisplayPokemonSkillInfo(CommonSkillTemplate template)
        {
            SkillInfoMonitor.ShowWindow(template);
        }

        private void RenderPokemonBuff(ABuffRecorder buffRecorder)
        {
            PokemonBuffRecorder pokemonBuffRecorder = (PokemonBuffRecorder)buffRecorder;
            if (pokemonBuffRecorder.IsAttribute)
            {
                EditorGUILayout.LabelField("Attribute: " + pokemonBuffRecorder.Template.Name, GUILayout.MinWidth(ItemMinWidth), GUILayout.MaxWidth(ItemMinWidth * 5), GUILayout.Height(ItemHeight));
            }
            else if (pokemonBuffRecorder.IsWeather)
            {
                EditorGUILayout.LabelField("Weather: " + pokemonBuffRecorder.Template.Name, GUILayout.MinWidth(ItemMinWidth), GUILayout.MaxWidth(ItemMinWidth * 5), GUILayout.Height(ItemHeight));
            }
            else
            {
                EditorGUILayout.LabelField(pokemonBuffRecorder.Template.Name, GUILayout.MinWidth(ItemMinWidth), GUILayout.MaxWidth(ItemMinWidth * 5), GUILayout.Height(ItemHeight));
            }
            
            EditorGUILayout.LabelField(pokemonBuffRecorder.DeletePending.ToString(), GUILayout.MinWidth(ItemMinWidth), GUILayout.MaxWidth(ItemMinWidth * 5), GUILayout.Height(ItemHeight));

            if (pokemonBuffRecorder.Source != null)
            {
                if (GUILayout.Button(((Pokemon)pokemonBuffRecorder.Source).Name, GUILayout.MinWidth(ItemMinWidth), GUILayout.MaxWidth(ItemMinWidth * 5), GUILayout.Height(ItemHeight)))
                {
                    DisplayPokemonInfo(((Pokemon)pokemonBuffRecorder.Source));
                }
            }
            else
            {
                EditorGUILayout.LabelField("No Source", GUILayout.MinWidth(ItemMinWidth), GUILayout.MaxWidth(ItemMinWidth * 5), GUILayout.Height(ItemHeight));
            }

            if (pokemonBuffRecorder.Target != null)
            {
                if (GUILayout.Button(((Pokemon)pokemonBuffRecorder.Target).Name, GUILayout.MinWidth(ItemMinWidth), GUILayout.MaxWidth(ItemMinWidth * 5), GUILayout.Height(ItemHeight)))
                {
                    DisplayPokemonInfo(((Pokemon)pokemonBuffRecorder.Target));
                }
            }
            else
            {
                EditorGUILayout.LabelField("No Target", GUILayout.MinWidth(ItemMinWidth), GUILayout.MaxWidth(ItemMinWidth * 5), GUILayout.Height(ItemHeight));
            }

            EditorGUILayout.LabelField(pokemonBuffRecorder.EffectLastRound.ToString(), GUILayout.MinWidth(ItemMinWidth), GUILayout.MaxWidth(ItemMinWidth * 5), GUILayout.Height(ItemHeight));
        }

        private void RenderPokemonBuffManager()
        {
            foreach (var listener in BuffMgr.Instance.GetAllBuff())
            {
                if (listener.Value == null || listener.Value.Count == 0)
                    continue;
                EditorGUILayout.LabelField("Trigger on " + listener.Key, EditorStyles.boldLabel);

                foreach (var buffRecorder in listener.Value)
                {
                    EditorGUILayout.BeginHorizontal();
                    RenderSingleBuff((PokemonBuffRecorder)buffRecorder);
                    EditorGUILayout.EndHorizontal();
                }
            }
        }
    }
}