using UnityEditor;
using UnityEngine;
using System.IO;

namespace Platformer.EditorTools
{
    public static class FileStructureGenerator
    {
        // Adds a menu item under Tools with shortcut Ctrl/Cmd + Shift + G
        [MenuItem("Tools/Generate Project File Structure %#g")]
        public static void GenerateStructure()
        {
            string[] folders = new string[]
            {
                "_Project",
                "_Project/Scripts",
                "_Project/Scripts/Core",
                "_Project/Scripts/Characters",
                "_Project/Scripts/Abilities",
                "_Project/Scripts/States",
                "_Project/Scripts/AI",
                "_Project/Scripts/Networking",
                "_Project/Scripts/Utilities",
                "_Project/Art",
                "_Project/Art/Models",
                "_Project/Art/Animations",
                "_Project/Art/Materials",
                "_Project/Art/Textures",
                "_Project/Audio",
                "_Project/Audio/Music",
                "_Project/Audio/SFX",
                "_Project/Audio/VO",
                "_Project/Prefabs",
                "_Project/Prefabs/Characters",
                "_Project/Prefabs/Environment",
                "_Project/Prefabs/UI",
                "_Project/Prefabs/Gameplay",
                "_Project/Scenes",
                "_Project/Scenes/Prototype",
                "_Project/Scenes/MainMenu",
                "_Project/Scenes/Maps",
                "_Project/UI",
                "_Project/UI/Screens",
                "_Project/UI/Widgets",
                "_Project/UI/Icons",
                "_Project/Resources",
                "Plugins",
                "Plugins/Photon",
                "Plugins/InputSystem",
                "Plugins/AINavigation",
                "Gizmos"
            };

            int foldersCreated = 0;

            foreach (string folder in folders)
            {
                string fullPath = Path.Combine("Assets", folder);

                if (!Directory.Exists(fullPath))
                {
                    Directory.CreateDirectory(fullPath);
                    Debug.Log($"📁 Created folder: {fullPath}");
                    foldersCreated++;
                }
            }

            AssetDatabase.Refresh();

            if (foldersCreated > 0)
            {
                EditorUtility.DisplayDialog(
                    "✅ Project Structure",
                    $"Successfully created {foldersCreated} folder(s).",
                    "Nice!"
                );
            }
            else
            {
                EditorUtility.DisplayDialog(
                    "📂 Project Structure",
                    "All folders already exist. No changes made.",
                    "Cool"
                );
            }
        }
    }
}
