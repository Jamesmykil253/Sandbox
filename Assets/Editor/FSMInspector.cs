using UnityEditor;
using UnityEngine;
using Platformer;

namespace Platformer.EditorTools
{
    [CustomEditor(typeof(StateMachine), true)]
    public class FSMInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            // Draw default inspector properties
            DrawDefaultInspector();

            GUILayout.Space(10);
            EditorGUILayout.LabelField("🧠 FSM Debug Tools", EditorStyles.boldLabel);

            // Toggles from GlobalEditorSettings
            GlobalEditorSettings.DrawDebugGizmos = EditorGUILayout.Toggle("Draw Gizmos", GlobalEditorSettings.DrawDebugGizmos);
            GlobalEditorSettings.ShowStateMachineOverlay = EditorGUILayout.Toggle("Show State Overlay", GlobalEditorSettings.ShowStateMachineOverlay);
            GlobalEditorSettings.LockSelectionOnPlayer = EditorGUILayout.Toggle("Lock Player Selection", GlobalEditorSettings.LockSelectionOnPlayer);

            // Show current state in inspector
            var stateMachine = (StateMachine)target;
            EditorGUILayout.LabelField("Current FSM State:", stateMachine.DebugCurrentState);
        }
    }
}
