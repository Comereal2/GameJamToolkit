﻿using System.Collections.Generic;
using TMPro;
using Types;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

namespace Editor.MainMenuCreator
{
    public class SettingsMenuCreatorEditorWindow : MenuWindowTemplate
    {
        private struct BaseSettingsControlData
        {
            public Enums.SettingsControlType ControlType;
            public Enums.PlayerPrefsDataTypes PlayerPrefsDataType;
            public string PlayerPrefsKey;
            public string DisplayText;

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

            public BaseSettingsControlData(Enums.SettingsControlType controlType, Enums.PlayerPrefsDataTypes playerPrefsDataType, string playerPrefsKey, string displayText, object playerPrefsValue)
            {
                ControlType = controlType;
                PlayerPrefsDataType = playerPrefsDataType;
                PlayerPrefsKey = playerPrefsKey;
                DisplayText = displayText;
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
        private float entrySpacing = 5f;

        private float settingsOptionsSpacing = 15f;
        private Vector2 defaultSettingsStartPos = new Vector2(0, 350f);
        private Vector2 buttonSize = new(300f, 100f);

        private Sprite checkmark;
        private Sprite knob;

        private void OnEnable()
        {
            settingsOptionsList = new ReorderableList(settingsControlTypes, typeof(BaseSettingsControlData), true, true, true, true);

            settingsOptionsList.drawElementCallback = (rect, index, active, focused) =>
            {
                var data = settingsControlTypes[index];

                float halfWidth = rect.width / 2;
                
                if(index != 0) EditorGUI.LabelField(new Rect(rect.x, rect.y - ((EditorGUIUtility.singleLineHeight + entrySpacing + GUI.skin.horizontalSlider.fixedHeight) / 2), rect.width, 0.1f), "", GUI.skin.horizontalSlider);
                
                data.ControlType = (Enums.SettingsControlType)EditorGUI.EnumPopup(new Rect(rect.x, rect.y, halfWidth, EditorGUIUtility.singleLineHeight), "Control Type", data.ControlType);
                data.DisplayText = EditorGUI.TextField(new Rect(rect.x + halfWidth, rect.y, halfWidth, EditorGUIUtility.singleLineHeight), "Display Text", data.DisplayText);
                
                if(data.PlayerPrefsDataType != Enums.PlayerPrefsDataTypes.None && data.ControlType is not (Enums.SettingsControlType.None or Enums.SettingsControlType.Button))
                    data.PlayerPrefsKey = EditorGUI.TextField(new Rect(rect.x + halfWidth, rect.y + EditorGUIUtility.singleLineHeight, halfWidth, EditorGUIUtility.singleLineHeight), "Player Prefs Key", data.PlayerPrefsKey);
                
                switch (data.ControlType)
                {
                    case Enums.SettingsControlType.None:
                    case Enums.SettingsControlType.Button:
                        break;
                    case Enums.SettingsControlType.Dropdown:
                    {
                        data.PlayerPrefsDataType = Enums.PlayerPrefsDataTypes.Int;
                        GUI.enabled = false;
                        EditorGUI.EnumPopup(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight, halfWidth, EditorGUIUtility.singleLineHeight), "Player Prefs Data Type", data.PlayerPrefsDataType);
                        GUI.enabled = true;
                        data.PlayerPrefsValue = EditorGUI.IntField(new Rect(rect.x + halfWidth, rect.y + EditorGUIUtility.singleLineHeight * 2, halfWidth, EditorGUIUtility.singleLineHeight), "Default Selection Index", (int)data.PlayerPrefsValue);
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

                        break;
                    }
                    case Enums.SettingsControlType.InputField:
                        data.PlayerPrefsDataType = (Enums.PlayerPrefsDataTypes)EditorGUI.EnumPopup(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight, halfWidth, EditorGUIUtility.singleLineHeight), "Player Prefs Data Type", data.PlayerPrefsDataType);
                        
                        data.PlayerPrefsValue = data.PlayerPrefsDataType switch
                        {
                            Enums.PlayerPrefsDataTypes.Int => EditorGUI.IntField(
                                new Rect(rect.x + halfWidth, rect.y + EditorGUIUtility.singleLineHeight * 2, halfWidth, EditorGUIUtility.singleLineHeight), "Default Value",
                                (int)data.PlayerPrefsValue),
                            Enums.PlayerPrefsDataTypes.Float => EditorGUI.FloatField(
                                new Rect(rect.x + halfWidth, rect.y + EditorGUIUtility.singleLineHeight * 2, halfWidth, EditorGUIUtility.singleLineHeight), "Default Value",
                                (float)data.PlayerPrefsValue),
                            Enums.PlayerPrefsDataTypes.String => EditorGUI.TextField(
                                new Rect(rect.x + halfWidth, rect.y + EditorGUIUtility.singleLineHeight * 2, halfWidth, EditorGUIUtility.singleLineHeight), "Default Value",
                                (string)data.PlayerPrefsValue),
                            _ => data.PlayerPrefsValue
                        };
                        break;
                    case Enums.SettingsControlType.Slider:
                    {
                        data.PlayerPrefsDataType = EditorGUI.Popup(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight, halfWidth, EditorGUIUtility.singleLineHeight), "Player Prefs Data Type", 
                            data.PlayerPrefsDataType is Enums.PlayerPrefsDataTypes.Int ? 0 : 1, new[] { "Int", "Float" }) == 0 ? Enums.PlayerPrefsDataTypes.Int : Enums.PlayerPrefsDataTypes.Float;
                    
                        data.PlayerPrefsValue = data.PlayerPrefsDataType switch
                        {
                            Enums.PlayerPrefsDataTypes.Int => EditorGUI.IntField(
                                new Rect(rect.x + halfWidth, rect.y + EditorGUIUtility.singleLineHeight * 2, halfWidth, EditorGUIUtility.singleLineHeight), "Default Value",
                                (int)data.PlayerPrefsValue),
                            Enums.PlayerPrefsDataTypes.Float => EditorGUI.FloatField(
                                new Rect(rect.x + halfWidth, rect.y + EditorGUIUtility.singleLineHeight * 2, halfWidth, EditorGUIUtility.singleLineHeight), "Default Value",
                                (float)data.PlayerPrefsValue),
                            Enums.PlayerPrefsDataTypes.String => EditorGUI.TextField(
                                new Rect(rect.x + halfWidth, rect.y + EditorGUIUtility.singleLineHeight * 2, halfWidth, EditorGUIUtility.singleLineHeight), "Default Value",
                                (string)data.PlayerPrefsValue),
                            _ => data.PlayerPrefsValue
                        };
                    
                        if (data.PlayerPrefsDataType is Enums.PlayerPrefsDataTypes.Int)
                        {
                            data.SliderBoundaries.x = EditorGUI.IntField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 2, halfWidth, EditorGUIUtility.singleLineHeight), "Lower bound", (int)data.SliderBoundaries.x);
                            data.SliderBoundaries.y = EditorGUI.IntField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 3, halfWidth, EditorGUIUtility.singleLineHeight), "Upper bound", (int)data.SliderBoundaries.y);
                        }
                        else if (data.PlayerPrefsDataType is Enums.PlayerPrefsDataTypes.Float)
                        {
                            data.SliderBoundaries.x = EditorGUI.FloatField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 2, halfWidth, EditorGUIUtility.singleLineHeight), "Lower bound", data.SliderBoundaries.x);
                            data.SliderBoundaries.y = EditorGUI.FloatField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 3, halfWidth, EditorGUIUtility.singleLineHeight), "Upper bound", data.SliderBoundaries.y);
                        }
                        else
                        {
                            data.PlayerPrefsDataType = Enums.PlayerPrefsDataTypes.Int;
                        }

                        if (data.SliderBoundaries.y < data.SliderBoundaries.x) data.SliderBoundaries.y = data.SliderBoundaries.x;
                        
                        break;
                    }
                    case Enums.SettingsControlType.Toggle:
                    {
                        data.PlayerPrefsDataType = Enums.PlayerPrefsDataTypes.Int;
                        GUI.enabled = false;
                        EditorGUI.EnumPopup(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight, halfWidth, EditorGUIUtility.singleLineHeight), "Player Prefs Data Type", data.PlayerPrefsDataType);
                        GUI.enabled = true;
                        if ((int)data.PlayerPrefsValue != 0 && (int)data.PlayerPrefsValue != 1) data.PlayerPrefsValue = 0;
                        data.PlayerPrefsValue = EditorGUI.Toggle(new Rect(rect.x + halfWidth, rect.y + EditorGUIUtility.singleLineHeight * 2, halfWidth, EditorGUIUtility.singleLineHeight), "Default Value", (int)data.PlayerPrefsValue == 1) ? 1 : 0;
                        break;
                    }
                    default:
                        Debug.LogWarning("Control type not detected - settings creator window");
                        break;
                }
                
                settingsControlTypes[index] = data;
            };

            settingsOptionsList.elementHeightCallback = index =>
            {
                switch (settingsControlTypes[index].ControlType)
                {
                    case Enums.SettingsControlType.InputField:
                    case Enums.SettingsControlType.Toggle:
                        return EditorGUIUtility.singleLineHeight * 3 + entrySpacing;
                    
                    case Enums.SettingsControlType.Slider:
                        return EditorGUIUtility.singleLineHeight * 4 + entrySpacing;
                    
                    case Enums.SettingsControlType.Dropdown:
                        return EditorGUIUtility.singleLineHeight * (3 + settingsControlTypes[index].DropdownAmount) + entrySpacing; 
                    
                    case Enums.SettingsControlType.Button:
                    case Enums.SettingsControlType.None:
                    default:
                        return EditorGUIUtility.singleLineHeight + entrySpacing;
                }
            };
            
            settingsOptionsList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Options");

            settingsOptionsList.onAddCallback = list => settingsControlTypes.Add(new BaseSettingsControlData(Enums.SettingsControlType.None, Enums.PlayerPrefsDataTypes.None, "defaultKey", "Setting", null));

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
                    if (control.ControlType is Enums.SettingsControlType.Button or Enums.SettingsControlType.None) continue;
                    if (uniqueValues.Add(control.PlayerPrefsKey)) continue;
                    EditorUtility.DisplayDialog("Settings Menu Creator", "You cannot have two of the same key for different playerprefs values. Please change one of them to a different one.", "Ok");
                    return;
                }
                CreatePrefab();
            }
        }

        private void CreatePrefab()
        {
            if (!checkmark) checkmark = (Sprite)AssetDatabase.LoadAssetAtPath(Consts.CheckmarkSpritePath, typeof(Sprite));
            if (!knob) knob = (Sprite)AssetDatabase.LoadAssetAtPath(Consts.KnobSpritePath, typeof(Sprite));
            string menuName = "SettingsMenuPrefab";
            CreateObjectBase(Tags.settingsMenuTag, "Settings");
            title.GetComponent<TMP_Text>().alignment = TextAlignmentOptions.Center;

            float offset = 0f;
            foreach (var control in settingsControlTypes)
            {
                if (control.ControlType == Enums.SettingsControlType.None) continue;

                RectTransform con = new GameObject($"{control.ControlType} settings panel", typeof(RectTransform)).GetComponent<RectTransform>();
                con.SetParent(menuCanvas);

                switch (control.ControlType)
                {
                    case Enums.SettingsControlType.Button:
                        con.gameObject.AddComponent<CanvasRenderer>();
                        con.gameObject.AddComponent<Image>();
                        con.gameObject.AddComponent<Button>();
                        con.sizeDelta = buttonSize;
                        con.anchoredPosition = new Vector2(defaultSettingsStartPos.x, defaultSettingsStartPos.y - offset);
                        offset += buttonSize.y + settingsOptionsSpacing;
                        TMP_Text buttonText = Instantiate(title.gameObject, con.transform).GetComponent<TMP_Text>();
                        RectTransform buttonTextRect = buttonText.GetComponent<RectTransform>();
                        buttonTextRect.sizeDelta = con.sizeDelta;
                        buttonTextRect.anchoredPosition = Vector2.zero;
                        buttonText.richText = true;
                        buttonText.alignment = TextAlignmentOptions.Center;
                        buttonText.text = $"<size=36>{control.DisplayText}</size>";
                        buttonText.color = Color.black;
                        break;
                    case Enums.SettingsControlType.Dropdown:
                        break;
                    case Enums.SettingsControlType.InputField:
                        break;
                    case Enums.SettingsControlType.Slider:
                        Slider slider = con.gameObject.AddComponent<Slider>();
                        slider.minValue = control.SliderBoundaries.x;
                        slider.maxValue = control.SliderBoundaries.y;
                        if (control.PlayerPrefsDataType == Enums.PlayerPrefsDataTypes.Int)
                        {
                            slider.wholeNumbers = true;
                            slider.value = (int)control.PlayerPrefsValue;
                        }
                        else slider.value = (float)control.PlayerPrefsValue;
                        
                        RectTransform sBackground = new GameObject("Background", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image)).GetComponent<RectTransform>();
                        sBackground.SetParent(con);
                        
                        Transform sFillArea = new GameObject("Fill Area", typeof(RectTransform)).transform;
                        RectTransform sFill = new GameObject("Fill", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image)).GetComponent<RectTransform>();
                        sFillArea.SetParent(con);
                        sFill.SetParent(sFillArea);
                        slider.fillRect = sFill;
                        
                        Transform sHandleSlideArea = new GameObject("Handle Slide Area", typeof(RectTransform)).transform;
                        RectTransform sHandle = new GameObject("Handle", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image)).GetComponent<RectTransform>();
                        sHandleSlideArea.SetParent(con);
                        sHandle.SetParent(sHandleSlideArea);
                        sHandle.GetComponent<Image>().sprite = knob;
                        slider.handleRect = sHandle;
                        break;
                    case Enums.SettingsControlType.Toggle:
                        con.gameObject.AddComponent<Toggle>().isOn = ((int)control.PlayerPrefsValue) == 1;
                        RectTransform tBackground = new GameObject("Background", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image)).GetComponent<RectTransform>();
                        RectTransform tCheckmark = new GameObject("Checkmark", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image)).GetComponent<RectTransform>();
                        tCheckmark.SetParent(tBackground);
                        tCheckmark.GetComponent<Image>().sprite = checkmark;
                        tBackground.SetParent(con);
                        TMP_Text tLabel = Instantiate(title.gameObject, con.transform).GetComponent<TMP_Text>();
                        tLabel.text = control.DisplayText;
                        tLabel.transform.SetParent(con);
                        tLabel.rectTransform.anchoredPosition = new Vector2(150f, 0);
                        con.sizeDelta = buttonSize;
                        con.anchoredPosition = new Vector2(defaultSettingsStartPos.x, defaultSettingsStartPos.y - offset);
                        offset += buttonSize.y + settingsOptionsSpacing;
                        break;
                    default:
                        Debug.LogWarning("Settings type not recognized - Instantiation");
                        break;
                }
            }
            
            RectTransform button = CreateBackButton();
            button.SetParent(menuCanvas);
            button.sizeDelta = buttonSize;
            button.anchoredPosition = new Vector2(0, -450f);
            button.GetComponentInChildren<TMP_Text>().text = "<size=36>Back</size>";
            
            menuParent.gameObject.SetActive(false);
            
            CreatePrefab(menuName);
        }
    }
}