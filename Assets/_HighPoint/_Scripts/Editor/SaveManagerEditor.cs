using UnityEditor;
using UnityEngine;

namespace Systems.Persistence.Editor
{
    [CustomEditor(typeof(SaveLoadSystem))]
    public class SaveManagerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            SaveLoadSystem saveLoadSystem = (SaveLoadSystem)target;

            DrawDefaultInspector();

            if (GUILayout.Button("Save Game"))
            {
                saveLoadSystem.SaveGame();
            }

            if (GUILayout.Button("Load Game"))
            {
                saveLoadSystem.LoadGame();
            }

            if (GUILayout.Button("Delete Game"))
            {
                saveLoadSystem.DeleteGame();
            }
        }
    }
}