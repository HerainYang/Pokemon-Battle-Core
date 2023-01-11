using System;
using Managers.BattleMgrComponents.PokemonLogic;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class PokemonInfoMonitor : EditorWindow
    {
        public Pokemon TargetPokemon;
        private static EditorWindow _thisWindow;

        public static void ShowWindow(Pokemon pokemon)
        {
            if (!_thisWindow)
            {
                _thisWindow = EditorWindow.GetWindow(typeof(PokemonInfoMonitor));
            }
            ((PokemonInfoMonitor)_thisWindow).TargetPokemon = pokemon;
            _thisWindow.Focus();
        }

        private void OnGUI()
        {
            if(TargetPokemon == null)
                return;
            EditorGUILayout.LabelField(TargetPokemon.Name,  EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Lv."+ TargetPokemon.Level);
            EditorGUILayout.LabelField("Runtime ID: "+ TargetPokemon.RuntimeID);
            EditorGUILayout.LabelField("Hp: " + TargetPokemon.GetHp() + "/" + TargetPokemon.HpMax);
            
            EditorGUILayout.LabelField("On Stage: "+ TargetPokemon.OnStage);
            EditorGUILayout.LabelField("Is Faint: "+ TargetPokemon.IsFaint);
            EditorGUILayout.LabelField("Attribute: "+ TargetPokemon.Attribute.Name);
            
            EditorGUILayout.LabelField("Abilities",  EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Attack "+ TargetPokemon.Attack);
            EditorGUILayout.LabelField("Defence "+ TargetPokemon.Defence);
            EditorGUILayout.LabelField("Special Attack "+ TargetPokemon.SpecialAttack);
            EditorGUILayout.LabelField("Special Defense "+ TargetPokemon.SpecialDefence);
            EditorGUILayout.LabelField("Speed "+ TargetPokemon.Speed);
            EditorGUILayout.LabelField("Type "+ TargetPokemon.Type);
            
            EditorGUILayout.LabelField("Status Change",  EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Attack "+ TargetPokemon.GetAllStatus()[0]);
            EditorGUILayout.LabelField("Defence "+  TargetPokemon.GetAllStatus()[1]);
            EditorGUILayout.LabelField("Special Attack "+  TargetPokemon.GetAllStatus()[2]);
            EditorGUILayout.LabelField("Special Defense "+  TargetPokemon.GetAllStatus()[3]);
            EditorGUILayout.LabelField("Speed "+  TargetPokemon.GetAllStatus()[4]);
            EditorGUILayout.LabelField("Accuracy "+  TargetPokemon.GetAllStatus()[5]);
            EditorGUILayout.LabelField("Evasion "+  TargetPokemon.GetAllStatus()[6]);
            EditorGUILayout.LabelField("CriticalHit "+  TargetPokemon.GetAllStatus()[7]);
            
            EditorGUILayout.LabelField("On Stage: "+ TargetPokemon.OnStage);
            
            EditorGUILayout.LabelField("Skills",  EditorStyles.boldLabel);
            foreach (var skill in TargetPokemon.SkillList)
            {
                if (GUILayout.Button(PokemonMgr.Instance.GetSkillTemplateByID(skill).Name))
                {
                    SkillInfoMonitor.ShowWindow(PokemonMgr.Instance.GetSkillTemplateByID(skill));
                }
            }
        }
    }
}