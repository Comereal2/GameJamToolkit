using System;
using System.Collections.Generic;
using System.Reflection;
using Editor.MainMenu.MainMenuLogic;
using TMPro;
using UnityEditor;
using UnityEngine;
using Types;
using UnityEditorInternal;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace Editor.MainMenu.MainMenuCreator
{
    public class MainMenuCreatorEditorWindow : EditorWindow
    {
        public struct ButtonProperties
        {
            public bool isSolidColor;        
            public Sprite buttonImage;       
            public Color buttonColor;        
            public Color textColor;          
            public bool textHasOutline;      
            public Color textOutlineColor;   
            public float textOutlineThickness; 
            
            public ButtonProperties(bool isSolidColor, Sprite buttonImage, Color buttonColor, Color textColor, bool textHasOutline, Color textOutlineColor, float textOutlineThickness)
            {
                this.isSolidColor = isSolidColor;
                this.buttonImage = buttonImage;
                this.buttonColor = buttonColor;
                this.textColor = textColor;
                this.textHasOutline = textHasOutline;
                this.textOutlineColor = textOutlineColor;
                this.textOutlineThickness = textOutlineThickness;
            }

            public ButtonProperties(ButtonProperties p)
            {
                isSolidColor = p.isSolidColor;
                buttonImage = p.buttonImage;
                buttonColor = p.buttonColor;
                textColor = p.textColor;
                textHasOutline = p.textHasOutline;
                textOutlineColor = p.textOutlineColor;
                textOutlineThickness = p.textOutlineThickness;
            }
        }
        
        private float spacing = 15f;
        
        private bool keepObject = false;
        
        private string gameName = "Game";
        private bool expandGameNameSettings = false;
        private Color gameNameColor = Color.white;
        private bool gameNameHasOutline = true;
        private Color gameNameOutlineColor = Color.black;
        [Range(0, 1)] private float gameNameOutlineThickness = 0.1f;
        private int gameNameFontSize = 120;

        private SceneAsset playScene;
        private ReorderableList buttonList;
        
        private bool expandButtonSettings = false;
        private ButtonProperties combinedButtonProperties = new ButtonProperties(true, null, Color.white, Color.white, true, Color.black, 0.1f);
        private float propertiesHeight = 120f;
        private bool separateButtonSettings = false;
        
        private List<Enums.MainMenuButtonTypes> buttons = new();
        private List<ButtonProperties> buttonProperties = new()
        {
            new ButtonProperties(true, null, Color.white, Color.white, true, Color.black, 0.1f)
        };

        private bool isBackgroundSolidColor = true;
        private Sprite backgroundImage;
        private Color backgroundColor = Color.black;

        private int outlineColorId = Shader.PropertyToID("_OutlineColor");
        private int outlineThicknessId = Shader.PropertyToID("_OutlineWidth");

        private void OnEnable()
        {
            if (buttons.Count == 0)
            {
                buttons.Add(Enums.MainMenuButtonTypes.Play);
                buttons.Add(Enums.MainMenuButtonTypes.Settings);
                buttons.Add(Enums.MainMenuButtonTypes.Credits);
                buttons.Add(Enums.MainMenuButtonTypes.Quit);
            }
            
            buttonList = new ReorderableList(buttons, typeof(Enums.MainMenuButtonTypes), true, true, true, true);

            buttonList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var button = buttons[index];

                if (!separateButtonSettings)
                {
                    if (button != Enums.MainMenuButtonTypes.Play && button != Enums.MainMenuButtonTypes.NewGame)
                    {
                        buttons[index] = (Enums.MainMenuButtonTypes)EditorGUI.EnumPopup(
                            new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), button);
                    }
                    else
                    {
                        float halfWidth = rect.width / 2;
                        buttons[index] = (Enums.MainMenuButtonTypes)EditorGUI.EnumPopup(
                            new Rect(rect.x, rect.y, halfWidth, EditorGUIUtility.singleLineHeight), button);
                        playScene = (SceneAsset)EditorGUI.ObjectField(
                            new Rect(rect.x + halfWidth, rect.y, halfWidth, EditorGUIUtility.singleLineHeight), 
                            playScene, typeof(SceneAsset), true);
                    }

                    return;
                }
                
                if (button != Enums.MainMenuButtonTypes.Play && button != Enums.MainMenuButtonTypes.NewGame)
                {
                    buttons[index] = (Enums.MainMenuButtonTypes)EditorGUI.EnumPopup(
                        new Rect(rect.x, rect.y + index * propertiesHeight, rect.width, EditorGUIUtility.singleLineHeight), button);
                }
                else
                {
                    float halfWidth = rect.width / 2;
                    buttons[index] = (Enums.MainMenuButtonTypes)EditorGUI.EnumPopup(
                        new Rect(rect.x, rect.y + index * propertiesHeight, halfWidth, EditorGUIUtility.singleLineHeight), button);
                    playScene = (SceneAsset)EditorGUI.ObjectField(
                        new Rect(rect.x + halfWidth, rect.y + index * propertiesHeight, halfWidth, EditorGUIUtility.singleLineHeight), 
                        playScene, typeof(SceneAsset), true);
                }
                
                float yOffset = rect.y + EditorGUIUtility.singleLineHeight + 2 + index * propertiesHeight;

                ButtonProperties properties;

                if (index >= buttonProperties.Count)
                {
                    properties = new ButtonProperties(combinedButtonProperties);
                }
                else
                {
                    properties = buttonProperties[index];
                }
                
                properties.isSolidColor = EditorGUI.Toggle(
                    new Rect(rect.x, yOffset, rect.width, EditorGUIUtility.singleLineHeight), 
                    "Is Button Solid Color", properties.isSolidColor);
                
                yOffset += EditorGUIUtility.singleLineHeight + 2;
                
                if (properties.isSolidColor)
                {
                    properties.buttonColor = EditorGUI.ColorField(
                        new Rect(rect.x, yOffset, rect.width, EditorGUIUtility.singleLineHeight), 
                        "Button Background Color", properties.buttonColor);
                    yOffset += EditorGUIUtility.singleLineHeight + 2;
                }
                else
                {
                    properties.buttonImage = (Sprite)EditorGUI.ObjectField(
                        new Rect(rect.x, yOffset, rect.width, EditorGUIUtility.singleLineHeight), 
                        "Button Image", properties.buttonImage, typeof(Sprite), false);
                    yOffset += EditorGUIUtility.singleLineHeight + 2;
                }

                properties.textColor = EditorGUI.ColorField(
                    new Rect(rect.x, yOffset, rect.width, EditorGUIUtility.singleLineHeight), 
                    "Button Text Color", properties.textColor);
                yOffset += EditorGUIUtility.singleLineHeight + 2;

                properties.textHasOutline = EditorGUI.Toggle(
                    new Rect(rect.x, yOffset, rect.width, EditorGUIUtility.singleLineHeight), 
                    "Has Outline", properties.textHasOutline);
                yOffset += EditorGUIUtility.singleLineHeight + 2;

                if (properties.textHasOutline)
                {
                    properties.textOutlineColor = EditorGUI.ColorField(
                        new Rect(rect.x, yOffset, rect.width, EditorGUIUtility.singleLineHeight), 
                        "Outline Color", properties.textOutlineColor);
                    yOffset += EditorGUIUtility.singleLineHeight + 2;

                    properties.textOutlineThickness = EditorGUI.Slider(
                        new Rect(rect.x, yOffset, rect.width, EditorGUIUtility.singleLineHeight), 
                        "Outline Thickness", properties.textOutlineThickness, 0, 1);
                    yOffset += EditorGUIUtility.singleLineHeight + 2;
                }

                if (index >= buttonProperties.Count)
                {
                    buttonProperties.Add(properties);
                }
                else
                {
                    buttonProperties[index] = properties;
                }
            };

            buttonList.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Main Menu Buttons");
            };

            buttonList.onAddCallback = (ReorderableList list) =>
            {
                buttons.Add(Enums.MainMenuButtonTypes.None);
            };

            buttonList.onRemoveCallback = (ReorderableList list) =>
            {
                buttons.RemoveAt(list.index);
            };
        }

        private void OnGUI()
        {
            gameName = EditorGUILayout.TextField("Game Name", gameName);

            expandGameNameSettings = EditorGUILayout.Foldout(expandGameNameSettings, "Additional Game Name Settings", true);

            if (expandGameNameSettings)
            {
                gameNameFontSize = EditorGUILayout.IntField("Font Size", gameNameFontSize);
                gameNameColor = EditorGUILayout.ColorField("Font Color", gameNameColor);
                gameNameHasOutline = EditorGUILayout.Toggle("Has Outline", gameNameHasOutline);
                if (gameNameHasOutline)
                {
                    gameNameOutlineColor = EditorGUILayout.ColorField("Outline Color", gameNameOutlineColor);
                    gameNameOutlineThickness = EditorGUILayout.Slider("Outline Thickness", gameNameOutlineThickness, 0, 1);
                }
            }
            
            EditorGUILayout.Space(spacing);
            
            buttonList.DoLayoutList();

            if (!separateButtonSettings)
            {
                expandButtonSettings = EditorGUILayout.Foldout(expandButtonSettings, "Additional Button Settings", true);
                if (expandButtonSettings)
                {
                    combinedButtonProperties.isSolidColor = EditorGUILayout.Toggle("Is Button Solid Color", combinedButtonProperties.isSolidColor);
                    if (combinedButtonProperties.isSolidColor) combinedButtonProperties.buttonColor = EditorGUILayout.ColorField("Button Background Color", combinedButtonProperties.buttonColor);
                    else combinedButtonProperties.buttonImage = (Sprite)EditorGUILayout.ObjectField(backgroundImage, typeof(Sprite), false);
                    combinedButtonProperties.textColor = EditorGUILayout.ColorField("Button Text Color", combinedButtonProperties.textColor);
                    combinedButtonProperties.textHasOutline = EditorGUILayout.Toggle("Has Outline", combinedButtonProperties.textHasOutline);
                    if (combinedButtonProperties.textHasOutline)
                    {
                        combinedButtonProperties.textOutlineColor = EditorGUILayout.ColorField("Outline Color", combinedButtonProperties.textOutlineColor);
                        combinedButtonProperties.textOutlineThickness = EditorGUILayout.Slider("Outline Thickness", combinedButtonProperties.textOutlineThickness, 0, 1);
                    }
                    separateButtonSettings = EditorGUILayout.Toggle("Separate Button Settings", separateButtonSettings);
                }
            }
            else
            {
                EditorGUILayout.Space(propertiesHeight * buttonProperties.Count);
                separateButtonSettings = EditorGUILayout.Toggle("Separate Button Settings", separateButtonSettings);
            }
            
            EditorGUILayout.Space(spacing);
            
            isBackgroundSolidColor = EditorGUILayout.Toggle("Is Background Solid Color", isBackgroundSolidColor);

            if (isBackgroundSolidColor) backgroundColor = EditorGUILayout.ColorField(backgroundColor);
            else backgroundImage = (Sprite)EditorGUILayout.ObjectField(backgroundImage, typeof(Sprite), false);
            
            EditorGUILayout.Space(spacing);
            
            keepObject = GUILayout.Toggle(keepObject, "Keep On Scene");
            
            if (GUILayout.Button("Save Main Menu"))
            {
                for (int i = 0; i < buttons.Count; i++)
                {
                    if (buttons[i] == Enums.MainMenuButtonTypes.None)
                    {
                        buttons.RemoveAt(i);
                        i--;
                    }
                }
                CreateMainMenuPrefab();
            }
        }

        private void CreateMainMenuPrefab()
        {
            if (!TryGetSelectedFolderPath(out string path)) return;
            path += "/MainMenu.prefab";
            GameObject leftoverMenu = GameObject.Find("MenuPrefab");
            if(leftoverMenu) DestroyImmediate(leftoverMenu);
            Transform menuPrefab = new GameObject("MenuPrefab").transform;
            Transform eventSystem = new GameObject("EventSystem", typeof(EventSystem)).transform;
            Transform menuCanvas = new GameObject("Menu Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster)).transform;
            Transform title = new GameObject("Title", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI)).transform;
            Transform buttonHolder = new GameObject("Buttons").transform;
            Transform background = new GameObject("Background", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image)).transform;
            
            eventSystem.SetParent(menuPrefab);
            menuCanvas.SetParent(menuPrefab);
            background.SetParent(menuCanvas);
            title.SetParent(menuCanvas);
            buttonHolder.SetParent(menuCanvas);
            #if ENABLE_INPUT_SYSTEM
            eventSystem.gameObject.AddComponent<InputSystemUIInputModule>();
            #endif
            #if !ENABLE_INPUT_SYSTEM
            eventSystem.gameObject.AddComponent<StandaloneInputModule>();
            #endif

            TMP_Text titleText = title.GetComponent<TMP_Text>();
            titleText.text = $"<b><size={gameNameFontSize}>{gameName}</size></b>";
            titleText.richText = true;
            titleText.color = gameNameColor;
            if (gameNameHasOutline)
            {
                Material newTitleTextMat = new Material(titleText.fontMaterial);
                newTitleTextMat.SetColor(outlineColorId, gameNameOutlineColor);
                newTitleTextMat.SetFloat(outlineThicknessId, gameNameOutlineThickness);
                titleText.fontMaterial = newTitleTextMat;
            }
            RectTransform titleRect = title.GetComponent<RectTransform>();
            titleRect.sizeDelta = new Vector2(1800, 50);
            titleRect.anchoredPosition = new Vector2(0, 475);
            
            menuCanvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = menuCanvas.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);

            background.GetComponent<RectTransform>().sizeDelta = new Vector2(1920, 1080);
            if (isBackgroundSolidColor)
            {
                background.GetComponent<Image>().color = backgroundColor;
            }
            else
            {
                background.GetComponent<Image>().sprite = backgroundImage;
            }

            float colSize = 740;
            float buttonToSpacingRatio = 0.3f;
            //float buttonWidthToHeight = 2.57f;
            int buttonHeight = Mathf.RoundToInt(colSize / (buttonToSpacingRatio * (buttons.Count - 1) + buttons.Count));
            //int buttonWidth = Mathf.RoundToInt(buttonHeight * buttonWidthToHeight);
            int buttonWidth = 340;
            float startingPos = 300;

            Vector2 buttonSize = new Vector2(buttonWidth, buttonHeight);
            
            for (int i = 0; i < buttons.Count; i++)
            {
                RectTransform button = new GameObject($"{buttons[i]} Button", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button)).GetComponent<RectTransform>();
                button.SetParent(buttonHolder);
                Vector2 buttonPos = new Vector2(-900f + buttonWidth / 2, startingPos - Mathf.RoundToInt(buttonHeight * (i * (1 + buttonToSpacingRatio) + 0.5f)));
                button.anchoredPosition = buttonPos;
                button.sizeDelta = buttonSize;
                
                TMP_Text buttonText = Instantiate(title.gameObject, button.transform).GetComponent<TMP_Text>();
                RectTransform buttonTextRect = buttonText.GetComponent<RectTransform>();
                buttonTextRect.sizeDelta = buttonSize;
                buttonTextRect.anchoredPosition = Vector2.zero;
                buttonText.richText = true;
                buttonText.alignment = TextAlignmentOptions.Center;
                buttonText.text = $"<size={buttonHeight * 9 / 16}>";
                
                if (separateButtonSettings)
                {
                    if (buttonProperties[i].isSolidColor)
                    {
                        button.GetComponent<Image>().color = buttonProperties[i].buttonColor;
                    }
                    else
                    {
                        button.GetComponent<Image>().sprite = buttonProperties[i].buttonImage;
                    }
                    buttonText.color = buttonProperties[i].textColor;
                    if (buttonProperties[i].textHasOutline)
                    {
                        Material newButtonTextMat = new Material(buttonText.fontMaterial);
                        newButtonTextMat.SetColor(outlineColorId, buttonProperties[i].textOutlineColor);
                        newButtonTextMat.SetFloat(outlineThicknessId, buttonProperties[i].textOutlineThickness);
                        buttonText.fontMaterial = newButtonTextMat;
                    }
                }
                else
                {
                    if (combinedButtonProperties.isSolidColor)
                    {
                        button.GetComponent<Image>().color = combinedButtonProperties.buttonColor;
                    }
                    else
                    {
                        button.GetComponent<Image>().sprite = combinedButtonProperties.buttonImage;
                    }
                    buttonText.color = combinedButtonProperties.textColor;
                    if (combinedButtonProperties.textHasOutline)
                    {
                        Material newButtonTextMat = new Material(buttonText.fontMaterial);
                        newButtonTextMat.SetColor(outlineColorId, combinedButtonProperties.textOutlineColor);
                        newButtonTextMat.SetFloat(outlineThicknessId, combinedButtonProperties.textOutlineThickness);
                        buttonText.fontMaterial = newButtonTextMat;
                    }
                }
                
                
                switch (buttons[i])
                {
                    case Enums.MainMenuButtonTypes.Play:
                        button.GetComponent<Button>().onClick.AddListener(() => MainMenuButtonOnClicks.Play(playScene));
                        buttonText.text += "Play</size>";
                        break;
                    case Enums.MainMenuButtonTypes.NewGame:
                        button.GetComponent<Button>().onClick.AddListener(() => MainMenuButtonOnClicks.Play(playScene));
                        buttonText.text += "New Game</size>";
                        break;
                    case Enums.MainMenuButtonTypes.LoadGame:
                        button.GetComponent<Button>().onClick.AddListener(MainMenuButtonOnClicks.OpenLoadMenu);
                        buttonText.text += "Load Save</size>";
                        break;
                    case Enums.MainMenuButtonTypes.Settings:
                        button.GetComponent<Button>().onClick.AddListener(MainMenuButtonOnClicks.OpenSettings);
                        buttonText.text += "Settings</size>";
                        break;
                    case Enums.MainMenuButtonTypes.Credits:
                        button.GetComponent<Button>().onClick.AddListener(MainMenuButtonOnClicks.OpenCredits);
                        buttonText.text += "Credits</size>";
                        break;
                    case Enums.MainMenuButtonTypes.Quit:
                        button.GetComponent<Button>().onClick.AddListener(MainMenuButtonOnClicks.QuitGame);
                        buttonText.text += "Quit</size>";
                        break;
                    default:
                        buttonText.text += "UNIMPLEMENTED</size>";
                        break;
                }
            }

            PrefabUtility.SaveAsPrefabAsset(menuPrefab.gameObject, path);
            if (!keepObject) DestroyImmediate(menuPrefab.gameObject);
        }

        private static bool TryGetSelectedFolderPath(out string path)
        {
            var tryGetActiveFolderPath = typeof(ProjectWindowUtil).GetMethod("TryGetActiveFolderPath", BindingFlags.Static | BindingFlags.NonPublic);

            object[] args = new object[] { null };
            bool found = (bool)tryGetActiveFolderPath.Invoke(null, args);
            path = (string)args[0];

            return found;
        }
    }
}