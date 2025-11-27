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
            public Structs.PlayerPrefData PlayerPref;
            public string DisplayText;
            
            public Vector2 SliderBoundaries;
            public int DropdownAmount;
            public List<string> DropdownOptions;

            public BaseSettingsControlData(Enums.SettingsControlType controlType, Structs.PlayerPrefData playerPref, string displayText)
            {
                ControlType = controlType;
                PlayerPref = playerPref;
                DisplayText = displayText;
                SliderBoundaries = Vector2.zero;
                DropdownAmount = 0;
                DropdownOptions = new();
            }

            public BaseSettingsControlData(Enums.SettingsControlType controlType, Enums.PlayerPrefsDataTypes dataType, string key, object value, string displayText)
            {
                ControlType = controlType;
                PlayerPref = new Structs.PlayerPrefData(dataType, key, value);
                DisplayText = displayText;
                SliderBoundaries = Vector2.zero;
                DropdownAmount = 0;
                DropdownOptions = new();
            }
        }
        
        private ReorderableList settingsOptionsList;

        private List<BaseSettingsControlData> settingsControlTypes = new();
        private float entrySpacing = 5f;
        
        private float settingsOptionsSpacing = 25;
        private static Vector2 defaultSettingsStartPos = new(0, 350f);

        private Sprite checkmark;
        private Sprite dropdownArrow;
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
                
                if(data.PlayerPref.DataType != Enums.PlayerPrefsDataTypes.None && data.ControlType is not (Enums.SettingsControlType.None or Enums.SettingsControlType.Button))
                    data.PlayerPref.Key = EditorGUI.TextField(new Rect(rect.x + halfWidth, rect.y + EditorGUIUtility.singleLineHeight, halfWidth, EditorGUIUtility.singleLineHeight), "Player Prefs Key", data.PlayerPref.Key);
                
                switch (data.ControlType)
                {
                    case Enums.SettingsControlType.None:
                    case Enums.SettingsControlType.Button:
                        break;
                    case Enums.SettingsControlType.Dropdown:
                    {
                        data.PlayerPref.DataType = Enums.PlayerPrefsDataTypes.Int;
                        GUI.enabled = false;
                        EditorGUI.EnumPopup(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight, halfWidth, EditorGUIUtility.singleLineHeight), "Player Prefs Data Type", data.PlayerPref.DataType);
                        GUI.enabled = true;
                        data.PlayerPref.Value = EditorGUI.IntField(new Rect(rect.x + halfWidth, rect.y + EditorGUIUtility.singleLineHeight * 2, halfWidth, EditorGUIUtility.singleLineHeight), "Default Selection Index", (int)data.PlayerPref.Value);
                        data.DropdownAmount = EditorGUI.IntField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 2, halfWidth, EditorGUIUtility.singleLineHeight), "Dropdown options amount", data.DropdownAmount);
                        data.PlayerPref.Value = Mathf.Min((int)data.PlayerPref.Value, data.DropdownAmount - 1);
                        if ((int)data.PlayerPref.Value < 0) data.PlayerPref.Value = 0;
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
                            if (i == (int)data.PlayerPref.Value) dropdownOptionText += " - default";
                            EditorGUI.LabelField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * (3 + i), precalcOffset, EditorGUIUtility.singleLineHeight), dropdownOptionText);
                            data.DropdownOptions[i] = EditorGUI.TextField(new Rect(rect.x + precalcOffset, rect.y + EditorGUIUtility.singleLineHeight * (3 + i), precalcWidth, EditorGUIUtility.singleLineHeight), data.DropdownOptions[i]);
                        }

                        break;
                    }
                    case Enums.SettingsControlType.InputField:
                        data.PlayerPref.DataType = (Enums.PlayerPrefsDataTypes)EditorGUI.EnumPopup(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight, halfWidth, EditorGUIUtility.singleLineHeight), 
                            "Player Prefs Data Type", data.PlayerPref.DataType);
                        
                        data.PlayerPref.Value = data.PlayerPref.DataType switch
                        {
                            Enums.PlayerPrefsDataTypes.Int => EditorGUI.IntField(
                                new Rect(rect.x + halfWidth, rect.y + EditorGUIUtility.singleLineHeight * 2, halfWidth, EditorGUIUtility.singleLineHeight), "Default Value",
                                (int)data.PlayerPref.Value),
                            Enums.PlayerPrefsDataTypes.Float => EditorGUI.FloatField(
                                new Rect(rect.x + halfWidth, rect.y + EditorGUIUtility.singleLineHeight * 2, halfWidth, EditorGUIUtility.singleLineHeight), "Default Value",
                                (float)data.PlayerPref.Value),
                            Enums.PlayerPrefsDataTypes.String => EditorGUI.TextField(
                                new Rect(rect.x + halfWidth, rect.y + EditorGUIUtility.singleLineHeight * 2, halfWidth, EditorGUIUtility.singleLineHeight), "Default Value",
                                (string)data.PlayerPref.Value),
                            _ => data.PlayerPref.Value
                        };
                        break;
                    case Enums.SettingsControlType.Slider:
                    {
                        data.PlayerPref.DataType = EditorGUI.Popup(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight, halfWidth, EditorGUIUtility.singleLineHeight), "Player Prefs Data Type", 
                            data.PlayerPref.DataType is Enums.PlayerPrefsDataTypes.Int ? 0 : 1, new[] { "Int", "Float" }) == 0 ? Enums.PlayerPrefsDataTypes.Int : Enums.PlayerPrefsDataTypes.Float;
                    
                        data.PlayerPref.Value = data.PlayerPref.DataType switch
                        {
                            Enums.PlayerPrefsDataTypes.Int => EditorGUI.IntField(
                                new Rect(rect.x + halfWidth, rect.y + EditorGUIUtility.singleLineHeight * 2, halfWidth, EditorGUIUtility.singleLineHeight), "Default Value",
                                (int)data.PlayerPref.Value),
                            Enums.PlayerPrefsDataTypes.Float => EditorGUI.FloatField(
                                new Rect(rect.x + halfWidth, rect.y + EditorGUIUtility.singleLineHeight * 2, halfWidth, EditorGUIUtility.singleLineHeight), "Default Value",
                                (float)data.PlayerPref.Value),
                            Enums.PlayerPrefsDataTypes.String => EditorGUI.TextField(
                                new Rect(rect.x + halfWidth, rect.y + EditorGUIUtility.singleLineHeight * 2, halfWidth, EditorGUIUtility.singleLineHeight), "Default Value",
                                (string)data.PlayerPref.Value),
                            _ => data.PlayerPref.Value
                        };
                    
                        if (data.PlayerPref.DataType is Enums.PlayerPrefsDataTypes.Int)
                        {
                            data.SliderBoundaries.x = EditorGUI.IntField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 2, halfWidth, EditorGUIUtility.singleLineHeight), "Lower bound", (int)data.SliderBoundaries.x);
                            data.SliderBoundaries.y = EditorGUI.IntField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 3, halfWidth, EditorGUIUtility.singleLineHeight), "Upper bound", (int)data.SliderBoundaries.y);
                            if ((int)data.PlayerPref.Value < data.SliderBoundaries.x) data.PlayerPref.Value = (int)data.SliderBoundaries.x;
                            else if ((int)data.PlayerPref.Value > data.SliderBoundaries.y) data.PlayerPref.Value = (int)data.SliderBoundaries.y;
                        }
                        else if (data.PlayerPref.DataType is Enums.PlayerPrefsDataTypes.Float)
                        {
                            data.SliderBoundaries.x = EditorGUI.FloatField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 2, halfWidth, EditorGUIUtility.singleLineHeight), "Lower bound", data.SliderBoundaries.x);
                            data.SliderBoundaries.y = EditorGUI.FloatField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 3, halfWidth, EditorGUIUtility.singleLineHeight), "Upper bound", data.SliderBoundaries.y);
                            if ((float)data.PlayerPref.Value < data.SliderBoundaries.x) data.PlayerPref.Value = data.SliderBoundaries.x;
                            else if ((float)data.PlayerPref.Value > data.SliderBoundaries.y) data.PlayerPref.Value = data.SliderBoundaries.y;
                        }
                        else
                        {
                            data.PlayerPref.DataType = Enums.PlayerPrefsDataTypes.Int;
                        }

                        if (data.SliderBoundaries.y < data.SliderBoundaries.x) data.SliderBoundaries.y = data.SliderBoundaries.x;
                        
                        break;
                    }
                    case Enums.SettingsControlType.Toggle:
                    {
                        data.PlayerPref.DataType = Enums.PlayerPrefsDataTypes.Int;
                        GUI.enabled = false;
                        EditorGUI.EnumPopup(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight, halfWidth, EditorGUIUtility.singleLineHeight), "Player Prefs Data Type", data.PlayerPref.DataType);
                        GUI.enabled = true;
                        if ((int)data.PlayerPref.Value != 0 && (int)data.PlayerPref.Value != 1) data.PlayerPref.Value = 0;
                        data.PlayerPref.Value = EditorGUI.Toggle(new Rect(rect.x + halfWidth, rect.y + EditorGUIUtility.singleLineHeight * 2, halfWidth, EditorGUIUtility.singleLineHeight), "Default Value", (int)data.PlayerPref.Value == 1) ? 1 : 0;
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
                    if (uniqueValues.Add(control.PlayerPref.Key)) continue;
                    EditorUtility.DisplayDialog("Settings Menu Creator", "You cannot have two of the same key for different playerprefs values. Please change one of them to a different one.", "Ok");
                    return;
                }
                CreatePrefab();
            }
        }

        private void CreatePrefab()
        {
            if (!checkmark) checkmark = AssetDatabase.GetBuiltinExtraResource<Sprite>(Consts.CheckmarkSpritePath);
            if (!knob) knob = AssetDatabase.GetBuiltinExtraResource<Sprite>(Consts.KnobSpritePath);
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
                        con.gameObject.AddComponent<CanvasRenderer>();
                        con.gameObject.AddComponent<Button>();
                        
                        SetupSlicedSprite(con.gameObject.AddComponent<Image>(), Enums.SlicedSprite.UISprite);
                        
                        TMP_Text buttonText = Instantiate(title.gameObject, con.transform).GetComponent<TMP_Text>();
                        RectTransform buttonTextRect = buttonText.GetComponent<RectTransform>();
                        buttonTextRect.sizeDelta = con.sizeDelta;
                        buttonTextRect.anchoredPosition = Vector2.zero;
                        buttonText.richText = true;
                        buttonText.alignment = TextAlignmentOptions.Center;
                        buttonText.text = $"<size=24>{control.DisplayText}</size>";
                        buttonText.color = Color.black;
                        
                        con.sizeDelta = Consts.ButtonSize;
                        con.localScale = Consts.ButtonScale;
                        offset += Consts.ButtonSize.y * Consts.ButtonScale.y;
                        
                        break;
                    case Enums.SettingsControlType.Dropdown:
                        con.gameObject.AddComponent<CanvasRenderer>();
                        
                        SetupSlicedSprite(con.gameObject.AddComponent<Image>(), Enums.SlicedSprite.UISprite);
                        
                        TMP_Dropdown dDropdown = con.gameObject.AddComponent<TMP_Dropdown>();
                        dDropdown.AddOptions(control.DropdownOptions);

                        RectTransform dLabel = new GameObject("Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI)).GetComponent<RectTransform>();
                        dLabel.SetParent(con);
                        dDropdown.captionText = dLabel.GetComponent<TMP_Text>();
                        dLabel.anchorMin = Vector2.zero;
                        dLabel.anchorMax = Consts.Anchor_MaxMax;
                        dLabel.anchoredPosition = new Vector2(10, 7);
                        dLabel.sizeDelta = new Vector2(25, 6);

                        RectTransform dArrow = new GameObject("Arrow", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image)).GetComponent<RectTransform>();
                        dArrow.SetParent(con);
                        dArrow.GetComponent<Image>().sprite = dropdownArrow;
                        dArrow.anchorMin = Consts.Anchor_MaxHalf;
                        dArrow.anchorMax = Consts.Anchor_MaxHalf;
                        dArrow.anchoredPosition = new Vector2(-15, 0);
                        dArrow.sizeDelta = new Vector2(20, 20);

                        RectTransform dTemplate = new GameObject("Template", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(ScrollRect)).GetComponent<RectTransform>();
                        dTemplate.SetParent(con);
                        SetupSlicedSprite(dTemplate.GetComponent<Image>(), Enums.SlicedSprite.UISprite);
                        dTemplate.gameObject.SetActive(false);
                        dDropdown.template = dTemplate;
                        dTemplate.anchorMin = Vector2.zero;
                        dTemplate.anchorMax = Consts.Anchor_MaxHalf;
                        dTemplate.anchoredPosition = new Vector2(0, 2);
                        dTemplate.sizeDelta = new Vector2(0, 150);
                        ScrollRect dTemplateScrollRect = dTemplate.GetComponent<ScrollRect>();

                        RectTransform dViewport = new GameObject("Viewport", typeof(RectTransform), typeof(Mask), typeof(CanvasRenderer), typeof(Image)).GetComponent<RectTransform>();
                        dViewport.SetParent(dTemplate);
                        SetupSlicedSprite(dViewport.GetComponent<Image>(), Enums.SlicedSprite.UIMask);
                        dViewport.anchorMin = Vector2.zero;
                        dViewport.anchorMax = Consts.Anchor_MaxMax;
                        dTemplateScrollRect.viewport = dViewport;

                        RectTransform dContent = new GameObject("Content", typeof(RectTransform)).GetComponent<RectTransform>();
                        dContent.SetParent(dViewport);
                        dContent.anchorMin = Vector2.up;
                        dContent.anchorMax = Consts.Anchor_MaxMax;
                        dTemplateScrollRect.content = dContent;

                        RectTransform dItem = new GameObject("Item", typeof(RectTransform), typeof(Toggle)).GetComponent<RectTransform>();
                        dItem.SetParent(dContent);
                        dItem.anchorMin = Consts.Anchor_ZeroHalf;
                        dItem.anchorMax = Consts.Anchor_MaxHalf;
                        Toggle dItemToggle = dItem.GetComponent<Toggle>();
                        dItemToggle.isOn = true;

                        RectTransform dItemBackground = new GameObject("Item Background", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image)).GetComponent<RectTransform>();
                        dItemBackground.SetParent(dItem);
                        dItemToggle.targetGraphic = dItemBackground.GetComponent<Image>();
                        dItemBackground.anchorMin = Vector2.zero;
                        dItemBackground.anchorMax = Consts.Anchor_MaxMax;

                        RectTransform dItemCheckmark = new GameObject("Item Checkmark", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image)).GetComponent<RectTransform>();
                        dItemCheckmark.SetParent(dItem);
                        dItemToggle.graphic = dItemCheckmark.GetComponent<Image>();
                        dItemCheckmark.GetComponent<Image>().sprite = checkmark;
                        dItemCheckmark.anchorMin = Consts.Anchor_ZeroHalf;
                        dItemCheckmark.anchorMax = Consts.Anchor_ZeroHalf;

                        RectTransform dItemLabel = new GameObject("Item Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI)).GetComponent<RectTransform>();
                        dItemLabel.SetParent(dItem);
                        dItemLabel.anchorMin = Vector2.zero;
                        dItemLabel.anchorMax = Consts.Anchor_MaxMax;
                        dDropdown.itemText = dItemLabel.GetComponent<TMP_Text>();

                        RectTransform dScrollbar = new GameObject("Scrollbar", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Scrollbar)).GetComponent<RectTransform>();
                        dScrollbar.SetParent(dTemplate);
                        dScrollbar.anchorMin = Vector2.right;
                        dScrollbar.anchorMax = Consts.Anchor_MaxMax;
                        SetupSlicedSprite(dScrollbar.GetComponent<Image>(), Enums.SlicedSprite.Background);
                        Scrollbar dScrollbarScrollbar = dScrollbar.GetComponent<Scrollbar>();
                        dTemplateScrollRect.verticalScrollbar = dScrollbarScrollbar;

                        RectTransform dSlidingArea = new GameObject("Sliding Area", typeof(RectTransform)).GetComponent<RectTransform>();
                        dSlidingArea.SetParent(dScrollbar);
                        dSlidingArea.anchorMin = Vector2.zero;
                        dSlidingArea.anchorMax = Consts.Anchor_MaxMax;

                        RectTransform dHandle = new GameObject("Handle", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image)).GetComponent<RectTransform>();
                        dHandle.SetParent(dScrollbar);
                        dHandle.anchorMin = Vector2.zero;
                        dHandle.anchorMax = new Vector2(1, 0.2f);
                        SetupSlicedSprite(dHandle.GetComponent<Image>(), Enums.SlicedSprite.UISprite);
                        dScrollbarScrollbar.handleRect = dHandle;
                        dScrollbarScrollbar.targetGraphic = dHandle.GetComponent<Image>();
                        
                        con.sizeDelta = Consts.DropdownSize;
                        con.localScale = Consts.DropdownScale;
                        offset += Consts.DropdownSize.y * Consts.DropdownScale.y;
                        
                        break;
                    case Enums.SettingsControlType.InputField:
                        con.gameObject.AddComponent<CanvasRenderer>();
                        con.gameObject.AddComponent<Image>();
                        con.gameObject.AddComponent<TMP_InputField>();

                        RectTransform iTextArea = new GameObject("Text Area", typeof(RectTransform), typeof(RectMask2D)).GetComponent<RectTransform>();
                        iTextArea.SetParent(con);

                        RectTransform iPlaceholder = new GameObject("Placeholder", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI), typeof(LayoutElement)).GetComponent<RectTransform>();
                        iPlaceholder.SetParent(iTextArea);

                        RectTransform iText = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI)).GetComponent<RectTransform>();
                        iText.SetParent(iTextArea);
                        
                        con.sizeDelta = Consts.InputFieldSize;
                        con.localScale = Consts.InputFieldScale;
                        offset += Consts.InputFieldSize.y * Consts.InputFieldScale.y;
                        
                        break;
                    case Enums.SettingsControlType.Slider:
                        Slider slider = con.gameObject.AddComponent<Slider>();
                        
                        RectTransform sBackground = new GameObject("Background", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image)).GetComponent<RectTransform>();
                        sBackground.SetParent(con);
                        SetupSlicedSprite(sBackground.GetComponent<Image>(), Enums.SlicedSprite.Background);
                        sBackground.anchorMin = Consts.Anchor_ZeroQuarter;
                        sBackground.anchorMax = Consts.Anchor_MaxThreeQuarters;
                        sBackground.anchoredPosition = Vector2.zero;
                        sBackground.sizeDelta = Vector2.zero;
                        
                        RectTransform sFillArea = new GameObject("Fill Area", typeof(RectTransform)).GetComponent<RectTransform>();
                        sFillArea.SetParent(con);
                        sFillArea.anchorMin = Consts.Anchor_ZeroQuarter;
                        sFillArea.anchorMax = Consts.Anchor_MaxThreeQuarters;
                        sFillArea.offsetMin = new Vector2(5, 0);
                        sFillArea.offsetMax = new Vector2(15, 0);
                        
                        RectTransform sFill = new GameObject("Fill", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image)).GetComponent<RectTransform>();
                        sFill.SetParent(sFillArea);
                        SetupSlicedSprite(sFill.GetComponent<Image>(), Enums.SlicedSprite.UISprite);
                        sFill.anchorMin = Vector2.zero;
                        sFill.anchorMax = Vector2.up;
                        sFill.offsetMin = Vector2.zero;
                        sFill.offsetMax = new Vector2(10, 0);
                        
                        RectTransform sHandleSlideArea = new GameObject("Handle Slide Area", typeof(RectTransform)).GetComponent<RectTransform>();
                        sHandleSlideArea.SetParent(con);
                        sHandleSlideArea.anchorMin = Vector2.zero;
                        sHandleSlideArea.anchorMax = Consts.Anchor_MaxMax;
                        sHandleSlideArea.offsetMin = new Vector2(10, 0);
                        sHandleSlideArea.offsetMax = new Vector2(10, 0);
                        
                        RectTransform sHandle = new GameObject("Handle", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image)).GetComponent<RectTransform>();
                        sHandle.SetParent(sHandleSlideArea);
                        sHandle.GetComponent<Image>().sprite = knob;
                        sHandle.offsetMin = Vector2.zero;
                        sHandle.offsetMax = new Vector2(20, 0);
                        
                        slider.fillRect = sFill;
                        slider.handleRect = sHandle;
                        slider.targetGraphic = sHandle.GetComponent<Image>();
                        slider.minValue = control.SliderBoundaries.x;
                        slider.maxValue = control.SliderBoundaries.y;
                        if (control.PlayerPref.DataType == Enums.PlayerPrefsDataTypes.Int)
                        {
                            slider.wholeNumbers = true;
                            slider.value = (int)control.PlayerPref.Value;
                        }
                        else slider.value = (float)control.PlayerPref.Value;
                        
                        con.sizeDelta = Consts.SliderSize;
                        con.localScale = Consts.SliderScale;
                        offset += Consts.SliderSize.y * Consts.SliderScale.y;
                        
                        break;
                    case Enums.SettingsControlType.Toggle:
                        Toggle t = con.gameObject.AddComponent<Toggle>();
                        t.isOn = ((int)control.PlayerPref.Value) == 1;
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
                        
                        con.sizeDelta = Consts.ToggleSize;
                        con.localScale = Consts.ToggleScale;
                        offset += Consts.ToggleSize.y * Consts.ToggleScale.y;
                        
                        break;
                    case Enums.SettingsControlType.None:
                    default:
                        Debug.LogWarning("Settings type not recognized - Instantiation");
                        break;
                }

                offset += settingsOptionsSpacing;
                MenuManager.AddSettingsOption(con.gameObject, control.ControlType, control.PlayerPref);
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