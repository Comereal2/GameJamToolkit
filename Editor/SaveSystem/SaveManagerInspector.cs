/*using SaveSystem;
using UnityEditor;
using UnityEngine;

namespace Editor.SaveSystem
{
    [CustomEditor(typeof(SaveManager))]
    public class SaveManagerInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Revalidate monitored scripts"))
            {
                if (EditorUtility.DisplayDialog("Save Manager", "This will wipe all currently monitored scripts and/or variables. Are you sure?", "Yes", "No"))
                {
                    ((SaveManager)target).ReValidateMonitoredScripts();
                }
            }
            
            if (!EditorApplication.isPlaying) return;
            if (GUILayout.Button("Revalidate monitored variables"))
            {
                if (EditorUtility.DisplayDialog("Save Manager", "This will look through all of your scripts for monitored variables. Your editor might freeze depending on the size of the project. Are you sure?", "Yes", "No"))
                {
                    ((SaveManager)target).ReValidateMonitoredVariables();
                }
            }
        }
    }
}*/