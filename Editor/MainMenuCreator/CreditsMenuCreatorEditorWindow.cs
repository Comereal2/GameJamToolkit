using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;

namespace Editor.MainMenuCreator
{
    public class CreditsMenuCreatorEditorWindow : EditorWindow
    {
        public struct CreditsEntry
        {
            public string entry;
            public ReorderableList nameList;
            public List<string> names;

            public CreditsEntry(string entryName)
            {
                entry = entryName;
                names = new(){"Giles Corey"};
                nameList = new ReorderableList(names, typeof(string), true, true, true, true);
            }
        }

        private ReorderableList entryList;
        private List<CreditsEntry> entries = new();

        private void OnEnable()
        {
            entryList = new ReorderableList(entries, typeof(CreditsEntry), true, true, true, true);

            entryList.drawElementCallback = (rect, index, active, focused) =>
            {
                CreditsEntry entry = entries[index];
                float height = entry.names.Count * EditorGUIUtility.singleLineHeight;
            };
            
            entryList.drawHeaderCallback = rect =>
            {
                EditorGUI.LabelField(rect, "Credits entries");
            };

            entryList.onAddCallback = list =>
            {
                entries.Add(new CreditsEntry("Category Name"));
            };

            entryList.onRemoveCallback = list =>
            {
                entries.RemoveAt(list.index);
            };
        }
    }
}