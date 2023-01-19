using System;
using Managers.BattleMgrComponents.PokemonLogic;
using PokemonLogic;
using PokemonLogic.PokemonData;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class PokemonInfoMonitor : EditorWindow
    {
        private Pokemon _targetPokemon;
        private static EditorWindow _thisWindow;

        public static void ShowWindow(Pokemon pokemon)
        {
            if (!_thisWindow)
            {
                _thisWindow = EditorWindow.GetWindow(typeof(PokemonInfoMonitor));
            }
            ((PokemonInfoMonitor)_thisWindow)._targetPokemon = pokemon;
            _thisWindow.Focus();
        }

        private void OnGUI()
        {
            if(_targetPokemon == null)
                return;
            EditorGUILayout.LabelField(_targetPokemon.Name,  EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Lv."+ _targetPokemon.Level);
            EditorGUILayout.LabelField("Runtime ID: "+ _targetPokemon.RuntimeID);
            EditorGUILayout.LabelField("Trainer ID: "+ _targetPokemon.TrainerID);
            EditorGUILayout.LabelField("Hp: " + _targetPokemon.GetHp() + "/" + _targetPokemon.HpMax);
            
            EditorGUILayout.LabelField("On Stage: "+ _targetPokemon.OnStage);
            EditorGUILayout.LabelField("Is Faint: "+ _targetPokemon.IsFaint);
            EditorGUILayout.LabelField("Attribute: "+ _targetPokemon.Attribute.Name);
            
            EditorGUILayout.LabelField("Abilities",  EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Attack "+ _targetPokemon.Attack);
            EditorGUILayout.LabelField("Defence "+ _targetPokemon.Defence);
            EditorGUILayout.LabelField("Special Attack "+ _targetPokemon.SpecialAttack);
            EditorGUILayout.LabelField("Special Defense "+ _targetPokemon.SpecialDefence);
            EditorGUILayout.LabelField("Speed "+ _targetPokemon.Speed);
            EditorGUILayout.LabelField("Type "+ _targetPokemon.Type);
            
            EditorGUILayout.LabelField("Status Change",  EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Attack "+ _targetPokemon.GetAllStatus()[0]);
            EditorGUILayout.LabelField("Defence "+  _targetPokemon.GetAllStatus()[1]);
            EditorGUILayout.LabelField("Special Attack "+  _targetPokemon.GetAllStatus()[2]);
            EditorGUILayout.LabelField("Special Defense "+  _targetPokemon.GetAllStatus()[3]);
            EditorGUILayout.LabelField("Speed "+  _targetPokemon.GetAllStatus()[4]);
            EditorGUILayout.LabelField("Accuracy "+  _targetPokemon.GetAllStatus()[5]);
            EditorGUILayout.LabelField("Evasion "+  _targetPokemon.GetAllStatus()[6]);
            EditorGUILayout.LabelField("CriticalHit "+  _targetPokemon.GetAllStatus()[7]);
            
            EditorGUILayout.LabelField("On Stage: "+ _targetPokemon.OnStage);
            
            EditorGUILayout.LabelField("Skills",  EditorStyles.boldLabel);
            foreach (var skill in _targetPokemon.RuntimeSkillList)
            {
                if (GUILayout.Button(skill.SkillTemplate.Name))
                {
                    SkillInfoMonitor.ShowWindow(skill.SkillTemplate);
                }
            }
        }
    }
}