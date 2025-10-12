using System;
using System.Reflection;
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
        protected float spacing = 15f;
        
        protected bool expandTitleSettings = false;
        protected Color titleColor = Color.white;
        protected bool titleHasOutline = true;
        protected Color titleOutlineColor = Color.black;
        [Range(0, 1)] protected float titleOutlineThickness = 0.1f;
        protected int titleFontSize = 120;
        
        private bool isBackgroundSolidColor = true;
        private Sprite backgroundImage;
        private Color backgroundColor = Color.black;
        private bool keepOnScene = false;
        
        protected bool saveMenuPressed = false;
        
        protected int outlineColorId = Shader.PropertyToID("_OutlineColor");
        protected int outlineThicknessId = Shader.PropertyToID("_OutlineWidth");

        protected Transform menuParent;
        protected Transform eventSystem;
        protected Transform menuCanvas;
        protected Transform title;
        protected void OnGUI()
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
        }

        protected void CreateObjectBase(string menuName, string titleText)
        {
            GameObject leftoverMenu = GameObject.Find(menuName);
            if(leftoverMenu) DestroyImmediate(leftoverMenu);
            menuParent = new GameObject(menuName).transform;
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
            CreateBackground(menuCanvas);
            title.SetParent(menuCanvas);
            
            menuCanvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = menuCanvas.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            
            TMP_Text titleTextComp = title.GetComponent<TMP_Text>();
            titleTextComp.text = $"<b><size={titleFontSize}>{titleText}</size></b>";
            titleTextComp.richText = true;
            titleTextComp.color = titleColor;
            if (titleHasOutline)
            {
                Material newTitleTextMat = new Material(titleTextComp.fontMaterial);
                newTitleTextMat.SetColor(outlineColorId, titleOutlineColor);
                newTitleTextMat.SetFloat(outlineThicknessId, titleOutlineThickness);
                titleTextComp.fontMaterial = newTitleTextMat;
            }
            RectTransform titleRect = title.GetComponent<RectTransform>();
            titleRect.sizeDelta = new Vector2(1800, 50);
            titleRect.anchoredPosition = new Vector2(0, 475);
        }

        protected void CreatePrefab(string menuName)
        {
            if (!TryGetSelectedFolderPath(out string path)) return;
            path += $"/{menuName}.prefab";
            
            PrefabUtility.SaveAsPrefabAsset(menuParent.gameObject, path);
            if (!keepOnScene) DestroyImmediate(menuParent.gameObject);
        }

        protected void DisplayTitleSettings(string expandName)
        {
            expandTitleSettings = EditorGUILayout.Foldout(expandTitleSettings, expandName, true);
            if (expandTitleSettings)
            {
                titleFontSize = EditorGUILayout.IntField("Font Size", titleFontSize);
                titleColor = EditorGUILayout.ColorField("Font Color", titleColor);
                titleHasOutline = EditorGUILayout.Toggle("Has Outline", titleHasOutline);
                if (titleHasOutline)
                {
                    titleOutlineColor = EditorGUILayout.ColorField("Outline Color", titleOutlineColor);
                    titleOutlineThickness = EditorGUILayout.Slider("Outline Thickness", titleOutlineThickness, 0, 1);
                }
            }
        }

        private void CreateBackground(Transform parent)
        {
            Transform background = new GameObject("Background", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image)).transform;
            background.SetParent(parent);
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