using Types;
using UnityEditor;
using UnityEngine;

namespace Editor.CustomInspectors
{
    [CustomPropertyDrawer(typeof(Structs.PlayerPrefData))]
    public class PlayerPrefDataEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            var typeProp = property.FindPropertyRelative(nameof(Structs.PlayerPrefData.DataType));
        
            var rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            property.isExpanded = EditorGUI.Foldout(rect, property.isExpanded, label);

            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;

                rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(rect, property.FindPropertyRelative(nameof(Structs.PlayerPrefData.Key)));

                rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(rect, typeProp);

                rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                switch ((Enums.PlayerPrefsDataTypes)typeProp.enumValueIndex)
                {
                    case Enums.PlayerPrefsDataTypes.Float:
                        var fProp = property.FindPropertyRelative(Structs.PlayerPrefData.FloatValueSubfieldName);
                        fProp.floatValue = EditorGUI.FloatField(rect, "Default Value", fProp.floatValue);
                        break;
                    case Enums.PlayerPrefsDataTypes.Int:
                        var iProp = property.FindPropertyRelative(Structs.PlayerPrefData.IntValueSubfieldName);
                        iProp.intValue = EditorGUI.IntField(rect, "Default Value", iProp.intValue);
                        break;
                    case Enums.PlayerPrefsDataTypes.String:
                        var sProp = property.FindPropertyRelative(Structs.PlayerPrefData.StringValueSubfieldName);
                        sProp.stringValue = EditorGUI.TextField(rect, "Default Value", sProp.stringValue);
                        break;
                }

                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => property.isExpanded ? EditorGUIUtility.singleLineHeight * 4 + EditorGUIUtility.standardVerticalSpacing * 3 : EditorGUIUtility.singleLineHeight;
    }
}