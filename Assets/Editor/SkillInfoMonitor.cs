using Managers.BattleMgrComponents.BattlePlayables.Skills;
using Managers.BattleMgrComponents.PokemonLogic;
using UnityEditor;

namespace Editor
{
    public class SkillInfoMonitor : EditorWindow
    {
        public CommonSkillTemplate TargetCommonSkill;
        private static EditorWindow _thisWindow;

        public static void ShowWindow(CommonSkillTemplate commonSkill)
        {
            if (!_thisWindow)
            {
                _thisWindow = EditorWindow.GetWindow<SkillInfoMonitor>(typeof(PokemonInfoMonitor));
            }
            ((SkillInfoMonitor)_thisWindow).TargetCommonSkill = commonSkill;
            _thisWindow.Focus();
        }

        private void OnGUI()
        {
            if(TargetCommonSkill == null)
                return;
            EditorGUILayout.LabelField(TargetCommonSkill.Name,  EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Type: "+ TargetCommonSkill.Type);
            EditorGUILayout.LabelField("Skill target type: "+ TargetCommonSkill.TargetType);
            
            EditorGUILayout.LabelField("Priority level "+ TargetCommonSkill.PriorityLevel);

            EditorGUILayout.LabelField("Power " + TargetCommonSkill.Power);
            EditorGUILayout.LabelField("Accuracy "+ TargetCommonSkill.Accuracy);
            EditorGUILayout.LabelField("Powerpoint "+ TargetCommonSkill.PowerPoint);
            EditorGUILayout.LabelField("Special effect trigger prob "+ TargetCommonSkill.SpecialEffectProb);
            EditorGUILayout.LabelField("Critical rate "+ TargetCommonSkill.CriticalRate);
            EditorGUILayout.LabelField("Percentage damage "+ TargetCommonSkill.PercentageDamage * 100 + "%");
            EditorGUILayout.LabelField("Weather type "+ TargetCommonSkill.WeatherType);

            
            EditorGUILayout.LabelField("Status Change",  EditorStyles.boldLabel);
            if(TargetCommonSkill.PokemonStatType == null)
                return;
            for (int i = 0; i < TargetCommonSkill.PokemonStatType.Length; i++)
            {
                EditorGUILayout.LabelField(TargetCommonSkill.PokemonStatType[i] + " change " + TargetCommonSkill.PokemonStatPoint[i]);
            }
        }
    }
}