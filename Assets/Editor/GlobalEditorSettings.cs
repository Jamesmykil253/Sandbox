using UnityEngine;
using UnityEditor;

namespace Platformer.EditorTools
{
    /// <summary>
    /// Global flags and constants for editor-only settings and debug views.
    /// </summary>
    [InitializeOnLoad]
    public static class GlobalEditorSettings
    {
        // Toggle gizmo drawing in FSMs, AI, etc.
        public static bool DrawDebugGizmos { get; set; } = true;

        // Toggle FSM state name overlay
        public static bool ShowStateMachineOverlay { get; set; } = true;

        // Locks selection on player prefab (optional)
        public static bool LockSelectionOnPlayer { get; set; } = false;

        // Consistent editor UI color scheme
        public static readonly Color PrimaryColor = new Color(0.2f, 0.6f, 1f);
        public static readonly Color ErrorColor = new Color(1f, 0.3f, 0.3f);

        // This runs once when the editor loads
        static GlobalEditorSettings()
        {
            Debug.Log("✅ GlobalEditorSettings initialized.");
        }
    }
}
