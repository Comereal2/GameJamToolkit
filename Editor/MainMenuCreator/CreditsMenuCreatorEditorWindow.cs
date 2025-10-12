using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace Editor.MainMenuCreator
{
    public class CreditsMenuCreatorEditorWindow : MenuWindowTemplate
    {
        public struct CreditsEntry
        {
            public string entry;
            public List<string> names;

            public CreditsEntry(string entryName)
            {
                entry = entryName;
                names = new(){"Giles Corey"};
            }
        }

        private ReorderableList entryList;
        private List<CreditsEntry> entries = new();

        private void OnEnable()
        {
            if (entries.Count == 0)
            {
                entries.Add(new CreditsEntry("Category Name"));
            }
            
            entryList = new ReorderableList(entries, typeof(CreditsEntry), true, true, true, true);

            entryList.drawElementCallback = (rect, index, active, focused) =>
            {
                if (entries.Count == 0)
                {
                    entryList.elementHeight = EditorGUIUtility.singleLineHeight;
                    return;
                }
                
                CreditsEntry entry = entries[index];
                entry.entry = EditorGUI.TextField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Category Name", entry.entry);

                float yOffset = EditorGUIUtility.singleLineHeight + 5;
                int entryAmount = EditorGUI.IntField(new Rect(rect.x, rect.y + yOffset, rect.width, EditorGUIUtility.singleLineHeight), "Names Amount", entry.names.Count);

                if (entryAmount > entry.names.Count)
                {
                    for (int i = entry.names.Count; i < entryAmount; i++)
                    {
                        entry.names.Add("");
                    }
                }
                else if (entryAmount < entry.names.Count && entryAmount != 0)
                {
                    for (int i = entryAmount; i < entry.names.Count;)
                    {
                        entry.names.RemoveAt(i);
                    }
                }
                
                for (int i = 0; i < entry.names.Count; i++)
                {
                    entry.names[i] = EditorGUI.TextField(new Rect(rect.x, rect.y + (yOffset) * (i + 2), rect.width, EditorGUIUtility.singleLineHeight), "Dev Name", entry.names[i]);
                }
                
                float height = (entry.names.Count + 2) * (EditorGUIUtility.singleLineHeight + 5);
                entryList.elementHeight = height;
                
                entries[index] = entry;
            };
            
            entryList.drawHeaderCallback = rect =>
            {
                EditorGUI.LabelField(rect, "Credits entries");
            };

            entryList.onAddCallback = list =>
            {
                entries.Add(new CreditsEntry("Category Name"));
            };

            entryList.onRemoveCallback = list =>
            {
                entries.RemoveAt(list.index);
            };
        }

        private void OnGUI()
        {
            DisplayTitleSettings("Credits Title Settings");
            
            entryList.DoLayoutList();
            
            base.OnGUI();

            if (saveMenuPressed)
            {
                CreatePrefab();
            }
        }

        private void CreatePrefab()
        {
            string menuName = "CreditsMenuPrefab";
            CreateObjectBase(menuName, "Credits");

            title.GetComponent<TMP_Text>().alignment = TextAlignmentOptions.Center;

            float startingY = 360f;
            float entryTitleHeight = 60f;
            float entryTitleWidth = 600f;
            Vector2 entryTitleDimensions = new Vector2(entryTitleWidth, entryTitleHeight);
            float nameSize = 40f;
            float offset = 0f;
            
            for (int i = 0; i < entries.Count; i++)
            {
                CreditsEntry entry = entries[i];
                RectTransform transform = new GameObject(entry.entry, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI)).GetComponent<RectTransform>();
                transform.SetParent(menuCanvas);
                transform.anchoredPosition = new Vector2(0, startingY - offset);
                transform.sizeDelta = entryTitleDimensions;
                offset += entryTitleHeight + spacing;
                TMP_Text text = transform.GetComponent<TMP_Text>();
                text.text = $"<size=60>{entry.entry}</size>";
                text.richText = true;
                text.alignment = TextAlignmentOptions.Center;
                for (int j = 0; j < entry.names.Count; j++)
                {
                    RectTransform nameTransform = new GameObject(entry.entry + " " + j, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI)).GetComponent<RectTransform>();
                    nameTransform.SetParent(transform);
                    nameTransform.anchoredPosition = new Vector2(0, -(nameSize + spacing) * (j + 1));
                    nameTransform.sizeDelta = entryTitleDimensions;
                    offset += nameSize + spacing;
                    TMP_Text nameText = nameTransform.GetComponent<TMP_Text>();
                    nameText.text = $"<size=40>{entry.names[j]}</size>";
                    nameText.alignment = TextAlignmentOptions.Center;
                }
            }
            
            CreatePrefab(menuName);
        }
    }
}