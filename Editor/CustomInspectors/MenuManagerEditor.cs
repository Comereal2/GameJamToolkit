using System.Collections.Generic;
using MainMenuLogic;
using UnityEditor;
using UnityEngine;

namespace Editor.CustomInspectors
{
    [CustomEditor(typeof(MenuManager))]
    public class MenuManagerEditor : UnityEditor.Editor
    {
        private bool removingPlayerPref;

        private List<string> playerPrefKeys = new();
        private int selectedKeyIndex = 0;
        
        public override void OnInspectorGUI()
        {
            var menu = (MenuManager)target;
            
            base.OnInspectorGUI();

            if (GUILayout.Button("Revalidate Menus"))
            {
                menu.RevalidateInstances();
            }
            
            if (menu.settingsMenu || menu.GetAllKeys().Count > 0)
            {
                if (removingPlayerPref)
                {
                
                    if(playerPrefKeys.Count == 0) RefreshList(menu);
                
                    if (GUILayout.Button("Cancel")) removingPlayerPref = false;
                    selectedKeyIndex = EditorGUILayout.Popup("Select Player Pref", selectedKeyIndex, playerPrefKeys.ToArray());
                    if (GUILayout.Button("Force Refresh List")) RefreshList(menu);
                
                    if (GUILayout.Button("Remove") && selectedKeyIndex != 0)
                    {
                        removingPlayerPref = false;
                        PlayerPrefs.DeleteKey(playerPrefKeys[selectedKeyIndex]);
                        menu.RemovePlayerPref(selectedKeyIndex - 1);
                        RefreshList(menu);
                    }
                }
                else
                {
                    if(GUILayout.Button("Clear Player Prefs")) MenuManager.ClearPlayerPrefs();
                    if (GUILayout.Button("Remove Player Pref")) removingPlayerPref = true;
                }
            }
        }

        private void RefreshList(MenuManager menu)
        {
            playerPrefKeys.Clear();
            playerPrefKeys.Add("None");
            foreach (var val in menu.GetAllKeys())
            {
                playerPrefKeys.Add(val.Item1);
            }
        }
    }
}