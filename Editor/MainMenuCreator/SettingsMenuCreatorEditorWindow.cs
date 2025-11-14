using System.Collections.Generic;
using MainMenuLogic;
using MainMenuLogic.MenuObjectDetectorScripts;
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
        
        private float settingsOptionsSpacing = 25;
        private static Vector2 defaultSettingsStartPos = new(0, 350f);
        
        private Sprite backgroundSprite;
        private Sprite checkmark;
        private Sprite dropdownArrow;
        private Sprite knob;
        private Sprite uiSprite;
        
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
                        if ((int)data.PlayerPrefsValue < 0) data.PlayerPrefsValue = 0;
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
                            if ((int)data.PlayerPrefsValue < data.SliderBoundaries.x) data.PlayerPrefsValue = data.SliderBoundaries.x;
                            else if ((int)data.PlayerPrefsValue > data.SliderBoundaries.y) data.PlayerPrefsValue = data.SliderBoundaries.y;
                        }
                        else if (data.PlayerPrefsDataType is Enums.PlayerPrefsDataTypes.Float)
                        {
                            data.SliderBoundaries.x = EditorGUI.FloatField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 2, halfWidth, EditorGUIUtility.singleLineHeight), "Lower bound", data.SliderBoundaries.x);
                            data.SliderBoundaries.y = EditorGUI.FloatField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 3, halfWidth, EditorGUIUtility.singleLineHeight), "Upper bound", data.SliderBoundaries.y);
                            if ((float)data.PlayerPrefsValue < data.SliderBoundaries.x) data.PlayerPrefsValue = data.SliderBoundaries.x;
                            else if ((float)data.PlayerPrefsValue > data.SliderBoundaries.y) data.PlayerPrefsValue = data.SliderBoundaries.y;
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
            if (!backgroundSprite) backgroundSprite = AssetDatabase.GetBuiltinExtraResource<Sprite>(Consts.BackgroundSpritePath);
            if (!checkmark) checkmark = AssetDatabase.GetBuiltinExtraResource<Sprite>(Consts.CheckmarkSpritePath);
            if (!knob) knob = AssetDatabase.GetBuiltinExtraResource<Sprite>(Consts.KnobSpritePath);
            if (!uiSprite) uiSprite = AssetDatabase.GetBuiltinExtraResource<Sprite>(Consts.UISpriteSpritePath);
            if (!dropdownArrow) dropdownArrow = AssetDatabase.GetBuiltinExtraResource<Sprite>(Consts.DropdownArrowSpritePath);
            
            string menuName = "SettingsMenuPrefab";
            CreateObjectBase<SettingsMenuScript>(Tags.settingsMenuTag, "Settings");
            title.GetComponent<TMP_Text>().alignment = TextAlignmentOptions.Center;

            float offset = 0f;
            foreach (var control in settingsControlTypes)
            {
                if (control.ControlType == Enums.SettingsControlType.None) continue;

                RectTransform con = new GameObject($"{control.ControlType} settings panel", typeof(RectTransform)).GetComponent<RectTransform>();
                con.SetParent(menuCanvas);
                
                con.localPosition = new Vector2(defaultSettingsStartPos.x, defaultSettingsStartPos.y - offset);

                switch (control.ControlType)
                {
                    case Enums.SettingsControlType.Button:
                        con.sizeDelta = Consts.ButtonSize;
                        con.localScale = Consts.ButtonScale;
                        offset += Consts.ButtonSize.y * Consts.ButtonScale.y;
                        
                        con.gameObject.AddComponent<CanvasRenderer>();
                        con.gameObject.AddComponent<Image>();
                        con.gameObject.AddComponent<Button>();
                        TMP_Text buttonText = Instantiate(title.gameObject, con.transform).GetComponent<TMP_Text>();
                        RectTransform buttonTextRect = buttonText.GetComponent<RectTransform>();
                        buttonTextRect.sizeDelta = con.sizeDelta;
                        buttonTextRect.anchoredPosition = Vector2.zero;
                        buttonText.richText = true;
                        buttonText.alignment = TextAlignmentOptions.Center;
                        buttonText.text = $"<size=24>{control.DisplayText}</size>";
                        buttonText.color = Color.black;
                        break;
                    case Enums.SettingsControlType.Dropdown:
                        con.sizeDelta = Consts.DropdownSize;
                        con.localScale = Consts.DropdownScale;
                        offset += Consts.DropdownSize.y * Consts.DropdownScale.y;
                        
                        con.gameObject.AddComponent<CanvasRenderer>();
                        con.gameObject.AddComponent<Image>().sprite = uiSprite;
                        TMP_Dropdown dDropdown = con.gameObject.AddComponent<TMP_Dropdown>();

                        RectTransform dLabel = new GameObject("Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI)).GetComponent<RectTransform>();
                        dLabel.SetParent(con);
                        dDropdown.captionText = dLabel.GetComponent<TMP_Text>();
                        dLabel.anchorMin = Vector2.zero;
                        dLabel.anchorMax = new Vector2(1, 1);
                        dLabel.anchoredPosition = new Vector2(10, 7);
                        dLabel.sizeDelta = new Vector2(25, 6);

                        RectTransform dArrow = new GameObject("Arrow", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image)).GetComponent<RectTransform>();
                        dArrow.SetParent(con);
                        dArrow.GetComponent<Image>().sprite = dropdownArrow;
                        dArrow.anchorMin = new Vector2(1, 0.5f);
                        dArrow.anchorMax = new Vector2(1, 0.5f);
                        dArrow.anchoredPosition = new Vector2(-15, 0);
                        dArrow.sizeDelta = new Vector2(20, 20);

                        RectTransform dTemplate = new GameObject("Template", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(ScrollRect)).GetComponent<RectTransform>();
                        dTemplate.SetParent(con);
                        dTemplate.gameObject.SetActive(false);
                        dTemplate.GetComponent<Image>().sprite = uiSprite;
                        dTemplate.anchorMin = Vector2.zero;
                        dTemplate.anchorMax = new Vector2(1, 0.5f);
                        dArrow.anchoredPosition = new Vector2(0, 2);
                        dArrow.sizeDelta = new Vector2(0, 150);
                        
                        break;
                    case Enums.SettingsControlType.InputField:
                        con.sizeDelta = Consts.InputFieldSize;
                        con.localScale = Consts.InputFieldScale;
                        offset += Consts.InputFieldSize.y * Consts.InputFieldScale.y;
                        
                        con.gameObject.AddComponent<CanvasRenderer>();
                        con.gameObject.AddComponent<Image>();
                        con.gameObject.AddComponent<TMP_InputField>();

                        RectTransform iTextArea = new GameObject("Text Area", typeof(RectTransform), typeof(RectMask2D)).GetComponent<RectTransform>();
                        iTextArea.SetParent(con);

                        RectTransform iPlaceholder = new GameObject("Placeholder", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI), typeof(LayoutElement)).GetComponent<RectTransform>();
                        iPlaceholder.SetParent(iTextArea);

                        RectTransform iText = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI)).GetComponent<RectTransform>();
                        iText.SetParent(iTextArea);
                        break;
                    case Enums.SettingsControlType.Slider:
                        con.sizeDelta = Consts.SliderSize;
                        con.localScale = Consts.SliderScale;
                        offset += Consts.SliderSize.y * Consts.SliderScale.y;
                        
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
                        sBackground.GetComponent<Image>().sprite = backgroundSprite;
                        sBackground.anchorMin = new Vector2(0, 0.25f);
                        sBackground.anchorMax = new Vector2(1, 0.75f);
                        sBackground.anchoredPosition = Vector2.zero;
                        sBackground.sizeDelta = Vector2.zero;
                        
                        RectTransform sFillArea = new GameObject("Fill Area", typeof(RectTransform)).GetComponent<RectTransform>();
                        sFillArea.SetParent(con);
                        sFillArea.anchorMin = new Vector2(0, 0.25f);
                        sFillArea.anchorMax = new Vector2(1, 0.75f);
                        sFillArea.anchoredPosition = Vector2.zero;
                        
                        RectTransform sFill = new GameObject("Fill", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image)).GetComponent<RectTransform>();
                        sFill.SetParent(sFillArea);
                        slider.fillRect = sFill;
                        sFill.anchorMin = Vector2.zero;
                        sFill.anchorMax = Vector2.up;
                        sFill.anchoredPosition = Vector2.zero;
                        sFill.GetComponent<Image>().sprite = uiSprite;
                        
                        RectTransform sHandleSlideArea = new GameObject("Handle Slide Area", typeof(RectTransform)).GetComponent<RectTransform>();
                        sHandleSlideArea.SetParent(con);
                        sHandleSlideArea.anchorMin = Vector2.zero;
                        sHandleSlideArea.anchorMax = new Vector2(1, 1);
                        sHandleSlideArea.anchoredPosition = Vector2.zero;
                        
                        RectTransform sHandle = new GameObject("Handle", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image)).GetComponent<RectTransform>();
                        Image sHandleImage = sHandle.GetComponent<Image>();
                        sHandle.SetParent(sHandleSlideArea);
                        slider.handleRect = sHandle;
                        slider.targetGraphic = sHandleImage;
                        sHandleImage.sprite = knob;
                        sHandle.anchoredPosition = Vector2.zero;
                        
                        break;
                    case Enums.SettingsControlType.Toggle:
                        con.sizeDelta = Consts.ToggleSize;
                        con.localScale = Consts.ToggleScale;
                        offset += Consts.ToggleSize.y * Consts.ToggleScale.y;
                        
                        Toggle t = con.gameObject.AddComponent<Toggle>();
                        t.isOn = ((int)control.PlayerPrefsValue) == 1;
                        con.localPosition = new Vector2(con.localPosition.x - 100f, con.localPosition.y);
                        
                        RectTransform tBackground = new GameObject("Background", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image)).GetComponent<RectTransform>();
                        tBackground.SetParent(con);
                        tBackground.localPosition = Vector2.zero;
                        
                        RectTransform tCheckmark = new GameObject("Checkmark", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image)).GetComponent<RectTransform>();
                        tCheckmark.SetParent(tBackground);
                        tCheckmark.anchoredPosition = Vector2.zero;
                        Image tCheckmarkImage = tCheckmark.GetComponent<Image>();
                        tCheckmarkImage.sprite = checkmark;
                        t.graphic = tCheckmarkImage;
                        
                        TMP_Text tLabel = Instantiate(title.gameObject, con.transform).GetComponent<TMP_Text>();
                        tLabel.text = control.DisplayText;
                        tLabel.transform.SetParent(con);
                        tLabel.rectTransform.localPosition = new Vector2(100f, 0);
                        break;
                    default:
                        Debug.LogWarning("Settings type not recognized - Instantiation");
                        break;
                }

                offset += settingsOptionsSpacing;
                MenuManager.AddSettingsOption(con.gameObject, control.PlayerPrefsKey, control.ControlType, control.PlayerPrefsDataType);
            }
            
            RectTransform button = CreateBackButton();
            button.SetParent(menuCanvas);
            button.anchoredPosition = new Vector2(0, -450f);
            button.GetComponentInChildren<TMP_Text>().text = "<size=24>Back</size>";
            
            menuParent.gameObject.SetActive(false);
            
            CreatePrefab(menuName);
        }
    }
}