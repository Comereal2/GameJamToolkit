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
                            return null;
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
                            break;
                        default:
                            Debug.LogWarning("Could not set value: Invalid Type");
                            return;
                    }
                }
            }
            
            public Vector2 SliderBoundaries;
            
            public int DropdownAmount;
            public List<string> DropdownOptions;

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
                SliderBoundaries = Vector2.zero;
                DropdownAmount = 0;
                DropdownOptions = new();
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
                
                if (data.ControlType is Enums.SettingsControlType.None)
                {
                    settingsControlTypes[index] = data;
                    return;
                }
                
                if (data.ControlType is Enums.SettingsControlType.Button)
                {
                    settingsControlTypes[index] = data;
                    return;
                }

                if (data.ControlType is Enums.SettingsControlType.Dropdown)
                {
                    data.PlayerPrefsDataType = Enums.PlayerPrefsDataTypes.Int;
                    GUI.enabled = false;
                    EditorGUI.EnumPopup(new Rect(rect.x + halfWidth, rect.y, halfWidth, EditorGUIUtility.singleLineHeight), "Player Prefs Data Type", data.PlayerPrefsDataType);
                    GUI.enabled = true;
                    data.PlayerPrefsValue = EditorGUI.IntField(new Rect(rect.x + halfWidth, rect.y + EditorGUIUtility.singleLineHeight, halfWidth, EditorGUIUtility.singleLineHeight), "Default Selection Index", (int)data.PlayerPrefsValue);
                    data.DropdownAmount = EditorGUI.IntField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 2, halfWidth, EditorGUIUtility.singleLineHeight), "Dropdown options amount", data.DropdownAmount);
                    data.PlayerPrefsValue = Mathf.Min((int)data.PlayerPrefsValue, data.DropdownAmount - 1);
                    if (data.DropdownAmount > data.DropdownOptions.Count)
                    {
                        for (int i = data.DropdownOptions.Count; i < data.DropdownAmount; i++)
                        {
                            data.DropdownOptions.Add("");
                        }
                    }

                    float precalcWidth = rect.width * 0.7f;
                    float precalcOffset = rect.width - precalcWidth;
                    
                    for (int i = 0; i < data.DropdownAmount; i++)
                    {
                        string dropdownOptionText = "Dropdown option " + i;
                        if (i == (int)data.PlayerPrefsValue) dropdownOptionText += " - default";
                        EditorGUI.LabelField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * (3 + i), precalcOffset, EditorGUIUtility.singleLineHeight), dropdownOptionText);
                        data.DropdownOptions[i] = EditorGUI.TextField(new Rect(rect.x + precalcOffset, rect.y + EditorGUIUtility.singleLineHeight * (3 + i), precalcWidth, EditorGUIUtility.singleLineHeight), data.DropdownOptions[i]);
                    }
                }
                else if (data.ControlType is Enums.SettingsControlType.InputField)
                {
                    data.PlayerPrefsDataType = (Enums.PlayerPrefsDataTypes)EditorGUI.EnumPopup(new Rect(rect.x + halfWidth, rect.y, halfWidth, EditorGUIUtility.singleLineHeight), "Player Prefs Data Type", data.PlayerPrefsDataType);
                    
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
                }
                else if (data.ControlType is Enums.SettingsControlType.Slider)
                {
                    data.PlayerPrefsDataType = EditorGUI.Popup(new Rect(rect.x + halfWidth, rect.y, halfWidth, EditorGUIUtility.singleLineHeight), "Player Prefs Data Type", 
                        data.PlayerPrefsDataType is Enums.PlayerPrefsDataTypes.Int ? 0 : 1, new[] { "Int", "Float" }) == 0 ? Enums.PlayerPrefsDataTypes.Int : Enums.PlayerPrefsDataTypes.Float;
                    
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
                    
                    if (data.PlayerPrefsDataType is Enums.PlayerPrefsDataTypes.Int)
                    {
                        data.SliderBoundaries.x = EditorGUI.IntField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 2, halfWidth, EditorGUIUtility.singleLineHeight), "Lower bound", (int)data.SliderBoundaries.x);
                        data.SliderBoundaries.y = EditorGUI.IntField(new Rect(rect.x + halfWidth, rect.y + EditorGUIUtility.singleLineHeight * 2, halfWidth, EditorGUIUtility.singleLineHeight), "Upper bound", (int)data.SliderBoundaries.y);
                    }
                    else if (data.PlayerPrefsDataType is Enums.PlayerPrefsDataTypes.Float)
                    {
                        data.SliderBoundaries.x = EditorGUI.FloatField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 2, halfWidth, EditorGUIUtility.singleLineHeight), "Lower bound", data.SliderBoundaries.x);
                        data.SliderBoundaries.y = EditorGUI.FloatField(new Rect(rect.x + halfWidth, rect.y + EditorGUIUtility.singleLineHeight * 2, halfWidth, EditorGUIUtility.singleLineHeight), "Upper bound", data.SliderBoundaries.y);
                    }
                    else
                    {
                        data.PlayerPrefsDataType = Enums.PlayerPrefsDataTypes.Int;
                    }
                }
                else if (data.ControlType is Enums.SettingsControlType.Toggle)
                {
                    data.PlayerPrefsDataType = Enums.PlayerPrefsDataTypes.Int;
                    GUI.enabled = false;
                    EditorGUI.EnumPopup(new Rect(rect.x + halfWidth, rect.y, halfWidth, EditorGUIUtility.singleLineHeight), "Player Prefs Data Type", data.PlayerPrefsDataType);
                    GUI.enabled = true;
                    if ((int)data.PlayerPrefsValue != 0 && (int)data.PlayerPrefsValue != 1) data.PlayerPrefsValue = 0;
                    data.PlayerPrefsValue = EditorGUI.Toggle(new Rect(rect.x + halfWidth, rect.y + EditorGUIUtility.singleLineHeight, halfWidth, EditorGUIUtility.singleLineHeight), "Default Value", (int)data.PlayerPrefsValue == 1) ? 1 : 0;
                }
                
                data.PlayerPrefsKey = EditorGUI.TextField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight, halfWidth, EditorGUIUtility.singleLineHeight), "Player Prefs Key", data.PlayerPrefsKey);

                settingsControlTypes[index] = data;
            };

            settingsOptionsList.elementHeightCallback = index =>
            {
                switch (settingsControlTypes[index].ControlType)
                {
                    case Enums.SettingsControlType.InputField:
                    case Enums.SettingsControlType.Toggle:
                        return EditorGUIUtility.singleLineHeight * 2;
                    
                    case Enums.SettingsControlType.Slider:
                        return EditorGUIUtility.singleLineHeight * 3;
                    
                    case Enums.SettingsControlType.Dropdown:
                        return EditorGUIUtility.singleLineHeight * (3 + settingsControlTypes[index].DropdownAmount); 
                    
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

            if (saveMenuPressed)
            {
                HashSet<string> uniqueValues = new();
                foreach (var control in settingsControlTypes)
                {
                    if (uniqueValues.Add(control.PlayerPrefsKey)) continue;
                    EditorUtility.DisplayDialog("Settings Menu Creator", "You cannot have two of the same key for different playerprefs values. Please change one of them to a different one.", "Ok");
                    return;
                }
                CreatePrefab();
            }
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