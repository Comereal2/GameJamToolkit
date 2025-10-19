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

            public BaseSettingsControlData(Enums.SettingsControlType controlType, Enums.PlayerPrefsDataTypes playerPrefsDataType, string playerPrefsKey, object playerPrefsValue)
            {
                ControlType = controlType;
                PlayerPrefsDataType = playerPrefsDataType;
                PlayerPrefsKey = playerPrefsKey;
                playerPrefsFloatValue = 0f;
                playerPrefsIntValue = 0;
                playerPrefsStringValue = "";
                PlayerPrefsValue = playerPrefsValue;
            }
        }
        
        private ReorderableList settingsOptionsList;

        private List<BaseSettingsControlData> settingsControlTypes = new();

        private void OnEnable()
        {
            settingsOptionsList = new ReorderableList(settingsControlTypes, typeof(BaseSettingsControlData), true, true, true, true);

            settingsOptionsList.drawElementCallback = (rect, index, active, focused) =>
            {
                var data = settingsControlTypes[index];

                float halfWidth = rect.width / 2;
                
                data.ControlType = (Enums.SettingsControlType)EditorGUI.EnumPopup(new Rect(rect.x, rect.y, halfWidth, EditorGUIUtility.singleLineHeight), "Control Type", data.ControlType);
                if (data.ControlType is Enums.SettingsControlType.None or Enums.SettingsControlType.Button) return;
                data.PlayerPrefsDataType = (Enums.PlayerPrefsDataTypes)EditorGUI.EnumPopup(new Rect(rect.x + halfWidth, rect.y, halfWidth, EditorGUIUtility.singleLineHeight), "Player Prefs Data Type", data.PlayerPrefsDataType);
                data.PlayerPrefsKey = EditorGUI.TextField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight, halfWidth, EditorGUIUtility.singleLineHeight), "Player Prefs Key", data.PlayerPrefsKey);
                data.PlayerPrefsValue = data.PlayerPrefsDataType switch
                {
                    Enums.PlayerPrefsDataTypes.Int => EditorGUI.IntField(
                        new Rect(rect.x + halfWidth, rect.y + EditorGUIUtility.singleLineHeight, halfWidth, EditorGUIUtility.singleLineHeight), "Default Value",
                        (int)data.PlayerPrefsValue),
                    Enums.PlayerPrefsDataTypes.Float => EditorGUI.FloatField(
                        new Rect(rect.x + halfWidth, rect.y + EditorGUIUtility.singleLineHeight, halfWidth, EditorGUIUtility.singleLineHeight), "Default Value",
                        (float)data.PlayerPrefsValue),
                    Enums.PlayerPrefsDataTypes.String => EditorGUI.TextField(
                        new Rect(rect.x + halfWidth, rect.y + EditorGUIUtility.singleLineHeight, halfWidth, EditorGUIUtility.singleLineHeight), "Default Value",
                        (string)data.PlayerPrefsValue),
                    _ => data.PlayerPrefsValue
                };

                settingsControlTypes[index] = data;
            };

            settingsOptionsList.elementHeightCallback = index =>
            {
                switch (settingsControlTypes[index].ControlType)
                {
                    case Enums.SettingsControlType.Dropdown:
                    case Enums.SettingsControlType.InputField:
                    case Enums.SettingsControlType.Slider:
                    case Enums.SettingsControlType.Toggle:
                        return EditorGUIUtility.singleLineHeight * 2;
                    
                    case Enums.SettingsControlType.Button:
                    case Enums.SettingsControlType.None:
                    default:
                        return EditorGUIUtility.singleLineHeight;
                }
            };
            
            settingsOptionsList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Options");

            settingsOptionsList.onAddCallback = list => settingsControlTypes.Add(new BaseSettingsControlData(Enums.SettingsControlType.None, Enums.PlayerPrefsDataTypes.None, "defalutKey", null));

            settingsOptionsList.onRemoveCallback = list => settingsControlTypes.RemoveAt(list.index);
        }

        private void OnGUI()
        {
            DisplayStartSettings("Settings Title Settings");
            
            settingsOptionsList.DoLayoutList();
            
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