using System;
using System.Collections.Generic;
using TMPro;
using Types;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Editor.MainMenuCreator
{
    public class SettingsMenuCreatorEditorWindow : MenuWindowTemplate
    {
        private struct BaseSettingsControlData
        {
            public Enums.SettingsControlType ControlType;
            public Enums.PlayerPrefsDataTypes PlayerPrefsDataType;
            public string PlayerPrefsKey;

            public object PlayerPrefsValue
            {
                get
                {
                    switch (PlayerPrefsDataType)
                    {
                        case Enums.PlayerPrefsDataTypes.Int:
                            return playerPrefsIntValue;
                        case Enums.PlayerPrefsDataTypes.Float:
                            return playerPrefsFloatValue;
                        case Enums.PlayerPrefsDataTypes.String:
                            return playerPrefsStringValue;
                        case Enums.PlayerPrefsDataTypes.None:
                        default:
                            Debug.LogWarning("Could not retrieve value: Invalid Type");
                            return null;
                    }
                }
                set
                {
                    switch (PlayerPrefsDataType)
                    {
                        case Enums.PlayerPrefsDataTypes.Int:
                            playerPrefsIntValue = (int)value;
                            break;
                        case Enums.PlayerPrefsDataTypes.Float:
                            playerPrefsFloatValue = (float)value;
                            break;
                        case Enums.PlayerPrefsDataTypes.String:
                            playerPrefsStringValue = (string)value;
                            break;
                        case Enums.PlayerPrefsDataTypes.None:
                        default:
                            Debug.LogWarning("Could not set value: Invalid Type");
                            break;
                    }
                }
            }

            private float playerPrefsFloatValue;
            private int playerPrefsIntValue;
            private string playerPrefsStringValue;
        }
        
        private ReorderableList settingsOptionsList;

        private List<BaseSettingsControlData> settingsControlTypes;

        private void OnEnable()
        {
            settingsOptionsList = new ReorderableList(settingsControlTypes, typeof(BaseSettingsControlData), true, true, true, true);

            settingsOptionsList.drawElementCallback = (rect, index, active, focused) =>
            {
                
            };
            
            settingsOptionsList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Options");

            settingsOptionsList.onAddCallback = list => settingsControlTypes.Add(new BaseSettingsControlData());

            settingsOptionsList.onRemoveCallback = list => settingsControlTypes.RemoveAt(list.index);
        }

        private void OnGUI()
        {
            DisplayStartSettings("Settings Title Settings");
            
            DisplayEndSettings();
            
            if(saveMenuPressed) CreatePrefab();
        }

        private void CreatePrefab()
        {
            string menuName = "SettingsMenuPrefab";
            CreateObjectBase(Tags.settingsMenuTag, "Settings");
            title.GetComponent<TMP_Text>().alignment = TextAlignmentOptions.Center;
            
            RectTransform button = CreateBackButton();
            button.SetParent(menuCanvas);
            button.gameObject.SetActive(false);
            button.anchoredPosition = new Vector2(0, -300f);
            
            CreatePrefab(menuName);
        }
    }
}