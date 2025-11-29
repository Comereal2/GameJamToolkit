using System.IO;
using System.Linq;
using PremadeComponents.ScriptableObjects;
using UnityEditor;
using UnityEngine;

namespace Editor.CustomInspectors
{
    [CustomEditor(typeof(AudioClipDatabase))]
    public class AudioClipDatabaseEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Load from folder"))
            {
                string path = EditorUtility.OpenFolderPanel("Select Audio Files Folder", Application.dataPath, "");
                SearchFolder(path);
            }
        }

        private void SearchFolder(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;

            GetAudioClipsFromPath(path);
            string[] subDirectories = Directory.GetDirectories(path);
            foreach (string dir in subDirectories)
            {
                SearchFolder(dir);
            }
        }

        private void GetAudioClipsFromPath(string path)
        {
            string[] audioFiles = Directory.GetFiles(path, "*.wav")
                .Concat(Directory.GetFiles(path, "*.mp3"))
                .Concat(Directory.GetFiles(path, "*.ogg"))
                .ToArray();

            foreach (string audioFile in audioFiles)
            {
                string relativePath = "Assets" + audioFile.Substring(Application.dataPath.Length);
                AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(relativePath);
                ((AudioClipDatabase)target).Editor_Add(clip);
            }
        }
    }
}