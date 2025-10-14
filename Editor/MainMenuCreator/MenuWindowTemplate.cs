﻿using System.Reflection;
using MainMenuLogic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace Editor.MainMenuCreator
{
    public class MenuWindowTemplate : EditorWindow
    {
        public struct TextData
        {
            public bool ExpandSettings;
            public Color TextColor;
            public bool TextHasOutline;
            public Color TextOutlineColor;
            public float TextOutlineThickness;
            public int TextFontSize;

            public TextData(bool expandSettings = false, Color textColor = default, bool textHasOutline = true, Color textOutlineColor = default, float textOutlineThickness = 0.1f, int textFontSize = 120)
            {
                ExpandSettings = expandSettings;
                TextColor = textColor;
                TextHasOutline = textHasOutline;
                TextOutlineColor = textOutlineColor;
                TextOutlineThickness = textOutlineThickness;
                TextFontSize = textFontSize;
            }

            public TextData(TextData d)
            {
                ExpandSettings = d.ExpandSettings;
                TextColor = d.TextColor;
                TextHasOutline = d.TextHasOutline;
                TextOutlineColor = d.TextOutlineColor;
                TextOutlineThickness = d.TextOutlineThickness;
                TextFontSize = d.TextFontSize;
            }
        }
        
        protected float spacing = 15f;

        protected TextData titleData = new(textColor: Color.white, textOutlineColor: Color.black);
        
        private bool isBackgroundSolidColor = true;
        private Sprite backgroundImage;
        private Color backgroundColor = Color.black;
        private bool keepOnScene = false;

        private Vector2 scrollPos;
        
        protected bool saveMenuPressed = false;
        
        protected int outlineColorId = Shader.PropertyToID("_OutlineColor");
        protected int outlineThicknessId = Shader.PropertyToID("_OutlineWidth");

        protected Transform menuParent;
        protected Transform eventSystem;
        protected Transform menuCanvas;
        protected Transform title;

        protected void CreateObjectBase(string menuTag, string titleText)
        {
            if (!MenuManager.instance) MenuManager.instance = new GameObject("Menu Manager", typeof(MenuManager)).GetComponent<MenuManager>();
            
            GameObject leftoverMenu = GameObject.Find(menuTag);
            if(leftoverMenu) DestroyImmediate(leftoverMenu);
            menuParent = new GameObject(menuTag).transform;
            if (!FindAnyObjectByType<EventSystem>())
            {
                eventSystem = new GameObject("EventSystem", typeof(EventSystem)).transform;
#if ENABLE_INPUT_SYSTEM
                eventSystem.gameObject.AddComponent<InputSystemUIInputModule>();
#endif
#if !ENABLE_INPUT_SYSTEM
            eventSystem.gameObject.AddComponent<StandaloneInputModule>();
#endif
            }
            else eventSystem = FindAnyObjectByType<EventSystem>().transform;
            menuCanvas = new GameObject("Menu Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster)).transform;
            title = new GameObject("Title", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI)).transform;
            
            eventSystem.SetParent(menuParent);
            menuCanvas.SetParent(menuParent);
            CreateBackground();
            title.SetParent(menuCanvas);
            
            menuCanvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = menuCanvas.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            
            SetTextProperties(titleText, titleData, title.GetComponent<TMP_Text>());
            
            RectTransform textRect = title.gameObject.GetComponent<RectTransform>();
            textRect.sizeDelta = new Vector2(1800, 50);
            textRect.anchoredPosition = new Vector2(0, 475);
        }

        protected void CreatePrefab(string menuName)
        {
            if (!TryGetSelectedFolderPath(out string path)) return;
            path += $"/{menuName}.prefab";
            
            PrefabUtility.SaveAsPrefabAsset(menuParent.gameObject, path);
            if (!keepOnScene) DestroyImmediate(menuParent.gameObject);
        }

        protected void DisplayStartSettings(string titleExpandName)
        {
            EditorGUILayout.BeginHorizontal();
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            DisplayTextSettings(titleExpandName, ref titleData);
        }

        protected void DisplayTextSettings(string expandName, ref TextData data)
        {
            data.ExpandSettings = EditorGUILayout.Foldout(data.ExpandSettings, expandName, true);
            if (data.ExpandSettings)
            {
                data.TextFontSize = EditorGUILayout.IntField("Font Size", data.TextFontSize);
                data.TextColor = EditorGUILayout.ColorField("Font Color", data.TextColor);
                data.TextHasOutline = EditorGUILayout.Toggle("Has Outline", data.TextHasOutline);
                if (data.TextHasOutline)
                {
                    data.TextOutlineColor = EditorGUILayout.ColorField("Outline Color", data.TextOutlineColor);
                    data.TextOutlineThickness = EditorGUILayout.Slider("Outline Thickness", data.TextOutlineThickness, 0, 1);
                }
            }
        }

        protected void DisplayEndSettings()
        {
            EditorGUILayout.Space(spacing);
            
            isBackgroundSolidColor = EditorGUILayout.Toggle("Is Background Solid Color", isBackgroundSolidColor);

            if (isBackgroundSolidColor) backgroundColor = EditorGUILayout.ColorField(backgroundColor);
            else backgroundImage = (Sprite)EditorGUILayout.ObjectField(backgroundImage, typeof(Sprite), false);
            
            EditorGUILayout.Space(spacing);
            
            keepOnScene = GUILayout.Toggle(keepOnScene, "Keep On Scene");

            saveMenuPressed = false;
            if (GUILayout.Button("Save Menu"))
            {
                saveMenuPressed = true;
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndHorizontal();
        }

        protected void SetTextProperties(string text, TextData data, TMP_Text textComp)
        {
            textComp.text = $"<size={data.TextFontSize}>{text}</size>";
            textComp.richText = true;
            textComp.color = data.TextColor;
            if (data.TextHasOutline)
            {
                Material newTitleTextMat = new Material(textComp.fontMaterial);
                newTitleTextMat.SetColor(outlineColorId, data.TextOutlineColor);
                newTitleTextMat.SetFloat(outlineThicknessId, data.TextOutlineThickness);
                textComp.fontMaterial = newTitleTextMat;
            }
        }

        protected RectTransform CreateBackButton()
        {
            RectTransform button = new GameObject("Back Button", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button)).GetComponent<RectTransform>();
            button.GetComponent<Button>().onClick.AddListener(MenuManager.instance.BackButton);
            TMP_Text buttonText = Instantiate(title.gameObject, button.transform).GetComponent<TMP_Text>();
            RectTransform buttonTextRect = buttonText.GetComponent<RectTransform>();
            buttonTextRect.sizeDelta = button.sizeDelta;
            buttonTextRect.anchoredPosition = Vector2.zero;
            buttonText.richText = true;
            buttonText.alignment = TextAlignmentOptions.Center;
            buttonText.text = $"<size=24>Back</size>";
            buttonText.color = Color.black;
            return button;
        }

        private void CreateBackground()
        {
            Transform background = new GameObject("Background", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image)).transform;
            background.SetParent(menuCanvas);
            background.GetComponent<RectTransform>().sizeDelta = new Vector2(1920, 1080);
            if (isBackgroundSolidColor)
            {
                background.GetComponent<Image>().color = backgroundColor;
            }
            else
            {
                background.GetComponent<Image>().sprite = backgroundImage;
            }
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