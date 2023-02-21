using System;
using TalesOfRadiance.Scripts.Battle;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using TalesOfRadiance.Scripts.Battle.Constant;
using TalesOfRadiance.Scripts.Battle.Managers;
using TalesOfRadiance.Scripts.Character;
using Unity.VisualScripting.ReorderableList;

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
            _target.uiCamera = (Camera)EditorGUILayout.ObjectField("UI Camera", _target.uiCamera, typeof(Camera),false);
            _target.cameraSpaceCanvas = (Canvas)EditorGUILayout.ObjectField("Camera Space Canvas", _target.cameraSpaceCanvas, typeof(Canvas), false);
            
            EditorGUILayout.LabelField("Player Info:", EditorStyles.boldLabel);
            RenderInfo(_target.player);
            
            EditorGUILayout.LabelField("Enemy Info:", EditorStyles.boldLabel);
            RenderInfo(_target.enemy);


            EditorGUILayout.LabelField("Team Prefab:", EditorStyles.boldLabel);
            _target.playerTeam = (CharacterTeam)EditorGUILayout.ObjectField("Player:", _target.playerTeam, typeof(CharacterTeam), true);
            _target.enemyTeam = (CharacterTeam)EditorGUILayout.ObjectField("Enemy:", _target.enemyTeam, typeof(CharacterTeam), true);

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(_target);
            }
            serializedObject.Update();
            serializedObject.ApplyModifiedProperties();
        }

        private void RenderInfo(BattleTeamInfo info)
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

            int index = 0;
            for (int i = 0; i < 3; i++)
            {
                EditorGUILayout.BeginHorizontal();
                for (int j = 0; j < 3; j++)
                {
                    if (ConfigManager.Instance.GetSquadTypeByID(info.squadTypeID)[i][j] == 1)
                    {
                        info.squadInfoByIndex[index] = Int32.Parse(EditorGUILayout.TextField(info.squadInfoByIndex[index].ToString(), GUILayout.Width(EditorGUIUtility.currentViewWidth/3)));
                        // info.SquadInfoByIndex[index] = -1;
                    }
                    else
                    {
                        GUILayout.Label("X", "textfield", GUILayout.Width(EditorGUIUtility.currentViewWidth/3));
                        info.squadInfoByIndex[index] = -1;
                    }

                    index++;
                }
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}