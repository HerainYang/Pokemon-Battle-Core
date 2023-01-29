using System;
using TalesOfRadiance.Scripts.Battle;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using TalesOfRadiance.Scripts.Battle;
using TalesOfRadiance.Scripts.Battle.Constant;
using TalesOfRadiance.Scripts.Battle.Managers;

namespace TalesOfRadiance.Editor
{
    [CustomEditor(typeof(BattleInitializer))]
    public class BattleInitializerInspector : UnityEditor.Editor
    {

        private BattleInitializer _target;

        public override VisualElement CreateInspectorGUI()
        {
            _target = ((BattleInitializer)target);
            return base.CreateInspectorGUI();
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.LabelField("Player Info:", EditorStyles.boldLabel);
            RenderInfo(_target.player);
            
            EditorGUILayout.LabelField("Enemy Info:", EditorStyles.boldLabel);
            RenderInfo(_target.enemy);
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(_target);
            }
        }

        private void RenderInfo(BattleInitInfo info)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("PlayerID:");
            info.playerID = EditorGUILayout.TextField(info.playerID);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("神器:");
            info.godWeaponID = EditorGUILayout.Popup(info.godWeaponID, Constant.GodWeapon);
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("阵型:");
            info.squadTypeID = EditorGUILayout.Popup(info.squadTypeID, Constant.SquidTypeStrings);
            EditorGUILayout.EndHorizontal();

            for (int i = 0; i < 3; i++)
            {
                EditorGUILayout.BeginHorizontal();
                for (int j = 0; j < 3; j++)
                {
                    if (ConfigManager.Instance.SquadTypeConfig[info.squadTypeID][i][j] == 1)
                    {
                        info.SquadInfoByIndex[i][j] = Int32.Parse(EditorGUILayout.TextField(info.SquadInfoByIndex[i][j].ToString(), GUILayout.Width(EditorGUIUtility.currentViewWidth/3)));
                    }
                    else
                    {
                        GUILayout.Label("X", "textfield", GUILayout.Width(EditorGUIUtility.currentViewWidth/3));
                    }
                    
                }
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}