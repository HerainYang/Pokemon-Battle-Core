using System.Text;
using TalesOfRadiance.Scripts.Battle.BattleComponents;
using TalesOfRadiance.Scripts.Battle.BattleComponents.RuntimeClass;
using UnityEditor;
using UnityEngine;
using Types = TalesOfRadiance.Scripts.Battle.Constant.Types;

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
            
            EditorGUILayout.LabelField("Remain HP: " + _targetHero.Properties.Hp + "/" + _targetHero.Properties.MaxHealth);
            EditorGUILayout.LabelField("Heal Shield: " + _targetHero.Properties.HealShield);
            
            EditorGUILayout.LabelField("Runtime ID: " + _targetHero.RuntimeID);

            EditorGUILayout.LabelField("Skills", EditorStyles.boldLabel);
            foreach (RuntimeSkill skill in _targetHero.RuntimeSkillList)
            {
                if (skill.Template.SkillType == Types.SkillType.Active)
                {
                    if (GUILayout.Button(skill.Template.Name + ", CD: " + skill.Cooldown))
                    {
                    
                    }
                }
                else
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    for (int i = 0; i < skill.Template.PassiveSkillBuffList.Length; i++)
                    {
                        stringBuilder.Append(skill.Template.PassiveSkillBuffList[i]);
                        if (i != skill.Template.PassiveSkillBuffList.Length - 1)
                        {
                            stringBuilder.Append(", ");
                        }
                        
                    }
                    if (GUILayout.Button(skill.Template.Name + ", Buff list: " + stringBuilder))
                    {
                    
                    }
                }

            }
        }
    }
}