using System;
using TalesOfRadiance.Scripts.Character;
using UnityEditor;
using UnityEngine;

namespace TalesOfRadiance.Editor
{
    [CustomEditor(typeof(CharacterAnchor))]
    public class CharacterAnchorInspector : UnityEditor.Editor
    {
        private void OnSceneGUI()
        {
            Color newColor = Color.cyan;
            newColor.a = 0.5f;
            Handles.color = newColor;
            Handles.SphereHandleCap(0, ((CharacterAnchor)target).transform.position, ((CharacterAnchor)target).transform.rotation, 0.1f, EventType.Repaint);
        }
    }
}