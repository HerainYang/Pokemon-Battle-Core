using Managers.BattleMgrComponents.BattlePlayables.Skills;
using Managers.BattleMgrComponents.PokemonLogic;
using UnityEditor;

namespace Editor
{
    public class SkillInfoMonitor : EditorWindow
    {
        public SkillTemplate TargetSkill;
        private static EditorWindow _thisWindow;

        public static void ShowWindow(SkillTemplate skill)
        {
            if (!_thisWindow)
            {
                _thisWindow = EditorWindow.GetWindow<SkillInfoMonitor>(typeof(PokemonInfoMonitor));
            }
            ((SkillInfoMonitor)_thisWindow).TargetSkill = skill;
            _thisWindow.Focus();
        }

        private void OnGUI()
        {
            if(TargetSkill == null)
                return;
            EditorGUILayout.LabelField(TargetSkill.Name,  EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Type: "+ TargetSkill.Type);
            EditorGUILayout.LabelField("Skill target type: "+ TargetSkill.TargetType);
            
            EditorGUILayout.LabelField("Priority level "+ TargetSkill.PriorityLevel);

            EditorGUILayout.LabelField("Power " + TargetSkill.Power);
            EditorGUILayout.LabelField("Accuracy "+ TargetSkill.Accuracy);
            EditorGUILayout.LabelField("Powerpoint "+ TargetSkill.PowerPoint);
            EditorGUILayout.LabelField("Special effect trigger prob "+ TargetSkill.SpecialEffectProb);
            EditorGUILayout.LabelField("Critical rate "+ TargetSkill.CriticalRate);
            EditorGUILayout.LabelField("Percentage damage "+ TargetSkill.PercentageDamage * 100 + "%");
            EditorGUILayout.LabelField("Weather type "+ TargetSkill.WeatherType);

            
            EditorGUILayout.LabelField("Status Change",  EditorStyles.boldLabel);
            if(TargetSkill.PokemonStatType == null)
                return;
            for (int i = 0; i < TargetSkill.PokemonStatType.Length; i++)
            {
                EditorGUILayout.LabelField(TargetSkill.PokemonStatType[i] + " change " + TargetSkill.PokemonStatPoint[i]);
            }
        }
    }
}