using System.Collections.Generic;
using MainMenuLogic;
using MainMenuLogic.MenuObjectDetectorScripts;
using TMPro;
using UnityEditor;
using UnityEngine;
using Types;
using UnityEditorInternal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Editor.MainMenuCreator
{
    public class MainMenuCreatorEditorWindow : MenuWindowTemplate
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

        private string gameName = "Game";

        private SceneAsset playScene;
        private ReorderableList buttonList;
        
        private bool expandButtonSettings = false;
        private ButtonProperties combinedButtonProperties = new ButtonProperties(true, null, Color.white, Color.white, false, Color.black, 0.1f);
        private bool separateButtonSettings = false;
        
        private List<Enums.MainMenuButtonTypes> buttons = new();
        private List<ButtonProperties> buttonProperties = new()
        {
            new ButtonProperties(true, null, Color.white, Color.white, false, Color.black, 0.1f)
        };

        private List<string> customButtonNames = new();

        private bool requireLoadMenu = false;
        private bool requireSettingsMenu = false;
        private bool requireCreditsMenu = false;

        private void OnEnable()
        {
            if (buttons.Count == 0)
            {
                buttons.Add(Enums.MainMenuButtonTypes.Play);
                customButtonNames.Add(string.Empty);
                buttons.Add(Enums.MainMenuButtonTypes.Settings);
                customButtonNames.Add(string.Empty);
                buttons.Add(Enums.MainMenuButtonTypes.Credits);
                customButtonNames.Add(string.Empty);
                buttons.Add(Enums.MainMenuButtonTypes.Quit);
                customButtonNames.Add(string.Empty);
            }
            
            buttonList = new ReorderableList(buttons, typeof(Enums.MainMenuButtonTypes), true, true, true, true);
            
            buttonList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var button = buttons[index];
                float halfWidth = rect.width / 2;
                
                if (button != Enums.MainMenuButtonTypes.Play && button != Enums.MainMenuButtonTypes.NewGame)
                {
                    buttons[index] = (Enums.MainMenuButtonTypes)EditorGUI.EnumPopup(new Rect(rect.x, rect.y, halfWidth, EditorGUIUtility.singleLineHeight), button);
                    customButtonNames[index] = EditorGUI.TextField(new Rect(rect.x + halfWidth, rect.y, halfWidth, EditorGUIUtility.singleLineHeight), customButtonNames[index]);
                }
                else
                {
                    float thirdWidth = rect.width / 3;
                    buttons[index] = (Enums.MainMenuButtonTypes)EditorGUI.EnumPopup(new Rect(rect.x, rect.y, thirdWidth, EditorGUIUtility.singleLineHeight), button);
                    customButtonNames[index] = EditorGUI.TextField(new Rect(rect.x + thirdWidth, rect.y, thirdWidth, EditorGUIUtility.singleLineHeight), customButtonNames[index]);
                    playScene = (SceneAsset)EditorGUI.ObjectField(new Rect(rect.x + thirdWidth * 2, rect.y, thirdWidth, EditorGUIUtility.singleLineHeight), playScene, typeof(SceneAsset), true);
                }

                if (!separateButtonSettings) return;
                
                float yOffset = rect.y + EditorGUIUtility.singleLineHeight + 2;

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

            buttonList.drawHeaderCallback = (Rect rect) => EditorGUI.LabelField(rect, "Main Menu Buttons");

            buttonList.onAddCallback = (ReorderableList list) =>
            {
                buttons.Add(Enums.MainMenuButtonTypes.None);
                customButtonNames.Add(string.Empty);
            };

            buttonList.onRemoveCallback = (ReorderableList list) =>
            {
                buttons.RemoveAt(list.index);
                customButtonNames.RemoveAt(list.index);
            };

            buttonList.elementHeightCallback = index =>
            {
                if (!separateButtonSettings) return EditorGUIUtility.singleLineHeight;

                return buttonProperties[index].textHasOutline ? 140f : 100f;
            };
        }

        private void OnGUI()
        {
            gameName = EditorGUILayout.TextField("Game Name", gameName);
            
            DisplayStartSettings("Additional Game Name Settings");
            
            EditorGUILayout.Space(spacing);
            
            buttonList.DoLayoutList();

            if (!separateButtonSettings)
            {
                expandButtonSettings = EditorGUILayout.Foldout(expandButtonSettings, "Additional Button Settings", true);
                if (expandButtonSettings)
                {
                    combinedButtonProperties.isSolidColor = EditorGUILayout.Toggle("Is Button Solid Color", combinedButtonProperties.isSolidColor);
                    if (combinedButtonProperties.isSolidColor) combinedButtonProperties.buttonColor = EditorGUILayout.ColorField("Button Background Color", combinedButtonProperties.buttonColor);
                    else combinedButtonProperties.buttonImage = (Sprite)EditorGUILayout.ObjectField(combinedButtonProperties.buttonImage, typeof(Sprite), false);
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
                separateButtonSettings = EditorGUILayout.Toggle("Separate Button Settings", separateButtonSettings);
            }

            DisplayEndSettings();
            if(saveMenuPressed) CreatePrefab();
        }
        private void CreatePrefab()
        {
            string menuName = "MainMenuPrefab";
            CreateObjectBase<MainMenuScript>(Tags.mainMenuTag, gameName);
            
            Transform buttonHolder = new GameObject("Buttons").transform;
            buttonHolder.SetParent(menuCanvas);
            buttonHolder.localPosition = Vector3.zero;
            
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
                        SetupSlicedSprite(button.GetComponent<Image>(), Enums.SlicedSprite.UISprite);
                    }
                    else
                    {
                        button.GetComponent<Image>().sprite = buttonProperties[i].buttonImage;
                    }
                    
                    buttonText.color = buttonProperties[i].textColor;
                    if (buttonProperties[i].textHasOutline)
                    {
                        buttonText.fontMaterial = new Material(buttonText.fontMaterial);
                        buttonText.outlineColor = buttonProperties[i].textOutlineColor;
                        buttonText.outlineWidth = buttonProperties[i].textOutlineThickness;
                    }
                }
                else
                {
                    if (combinedButtonProperties.isSolidColor)
                    {
                        button.GetComponent<Image>().color = combinedButtonProperties.buttonColor;
                        SetupSlicedSprite(button.GetComponent<Image>(), Enums.SlicedSprite.UISprite);
                    }
                    else
                    {
                        button.GetComponent<Image>().sprite = combinedButtonProperties.buttonImage;
                    }
                    buttonText.color = combinedButtonProperties.textColor;
                    if (combinedButtonProperties.textHasOutline)
                    {
                        buttonText.fontMaterial = new Material(buttonText.fontMaterial);
                        buttonText.outlineColor = combinedButtonProperties.textOutlineColor;
                        buttonText.outlineWidth = combinedButtonProperties.textOutlineThickness;
                    }
                }
                
                int playSceneBuildIndex = -1;
                if (playScene)
                {
                    for (int j = 0; j < SceneManager.sceneCountInBuildSettings; j++)
                    {
                        if (SceneUtility.GetScenePathByBuildIndex(j).Contains(playScene.name))
                        {
                            playSceneBuildIndex = j;
                            break;
                        }
                    }
                }
                
                switch (buttons[i])
                {
                    case Enums.MainMenuButtonTypes.Play:
                        if (playSceneBuildIndex == -1)
                        {
                            Debug.LogError($"Scene of button {i} must be in Unity build order.");
                        }
                        else
                        {
                            UnityEditor.Events.UnityEventTools.AddIntPersistentListener(button.GetComponent<Button>().onClick, MenuManager.Play, playSceneBuildIndex);
                        }
                        buttonText.text += customButtonNames[i] == string.Empty ? "Play</size>" : $"{customButtonNames[i]}</size>";
                        break;
                    case Enums.MainMenuButtonTypes.NewGame:
                        if (playSceneBuildIndex == -1)
                        {
                            Debug.LogError($"Scene of button {i} must be in Unity build order.");
                        }
                        else
                        {
                            UnityEditor.Events.UnityEventTools.AddIntPersistentListener(button.GetComponent<Button>().onClick, MenuManager.Play, playSceneBuildIndex);
                        }
                        buttonText.text += customButtonNames[i] == string.Empty ? "New Game</size>" : $"{customButtonNames[i]}</size>";
                        break;
                    case Enums.MainMenuButtonTypes.LoadGame:
                        UnityEditor.Events.UnityEventTools.AddPersistentListener(button.GetComponent<Button>().onClick, MenuManager.OpenLoadMenu);
                        buttonText.text += customButtonNames[i] == string.Empty ? "Load Save</size>" : $"{customButtonNames[i]}</size>";
                        requireLoadMenu = true;
                        break;
                    case Enums.MainMenuButtonTypes.Settings:
                        UnityEditor.Events.UnityEventTools.AddPersistentListener(button.GetComponent<Button>().onClick, MenuManager.OpenSettings);
                        buttonText.text += customButtonNames[i] == string.Empty ? "Settings</size>" : $"{customButtonNames[i]}</size>";
                        requireSettingsMenu = true;
                        break;
                    case Enums.MainMenuButtonTypes.Credits:
                        UnityEditor.Events.UnityEventTools.AddPersistentListener(button.GetComponent<Button>().onClick, MenuManager.OpenCredits);
                        buttonText.text += customButtonNames[i] == string.Empty ? "Credits</size>" : $"{customButtonNames[i]}</size>";
                        requireCreditsMenu = true;
                        break;
                    case Enums.MainMenuButtonTypes.Quit:
                        UnityEditor.Events.UnityEventTools.AddPersistentListener(button.GetComponent<Button>().onClick, MenuManager.QuitGame);
                        buttonText.text += customButtonNames[i] == string.Empty ? "Quit</size>" : $"{customButtonNames[i]}</size>";
                        break;
                    case Enums.MainMenuButtonTypes.Custom:
                        buttonText.text += customButtonNames[i] == string.Empty ? "Custom</size>" : $"{customButtonNames[i]}</size>";
                        break;
                    default:
                        buttonText.text += "UNIMPLEMENTED</size>";
                        break;
                }
            }

            CreatePrefab(menuName);
            
            if (!FindFirstObjectByType<CreditsMenuScript>(FindObjectsInactive.Include) && requireCreditsMenu) GetWindow<CreditsMenuCreatorEditorWindow>("Credits Menu Creator").Show();
            if (!FindFirstObjectByType<SettingsMenuScript>(FindObjectsInactive.Include) && requireSettingsMenu) GetWindow<SettingsMenuCreatorEditorWindow>("Settings Menu Creator").Show();
            if (!FindFirstObjectByType<LoadGameMenuScript>(FindObjectsInactive.Include) && requireLoadMenu) GetWindow<LoadGameWindowEditorWindow>("Load Game Window Creator").Show();
        }
    }
}