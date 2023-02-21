using TalesOfRadiance.Scripts.Battle.BattleComponents.RuntimeClass;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class RuntimeHeroMonitor : EditorWindow
    {
        private RuntimeHero _targetHero;
        private static EditorWindow _thisWindow;

        public static void ShowWindow(RuntimeHero hero)
        {
            if (!_thisWindow)
            {
                _thisWindow = EditorWindow.GetWindow(typeof(RuntimeHeroMonitor));
            }

            ((RuntimeHeroMonitor)_thisWindow)._targetHero = hero;
            _thisWindow.Focus();
        }

        private void OnGUI()
        {
            if (_targetHero == null)
                return;
            EditorGUILayout.LabelField(_targetHero.Template.Name, EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Runtime ID: " + _targetHero.RuntimeID);

            EditorGUILayout.LabelField("Skills", EditorStyles.boldLabel);
            foreach (var skill in _targetHero.RuntimeSkillList)
            {
                if (GUILayout.Button(skill.Template.Name + ", CD: " + skill.Cooldown))
                {
                    
                }
            }
        }
    }
}