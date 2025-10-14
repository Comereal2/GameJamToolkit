using Attributes;
using UnityEditor;
using UnityEngine;

namespace Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(ReadonlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight( SerializedProperty property, GUIContent label ) => EditorGUI.GetPropertyHeight( property, label, true );

        public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
        {
            using var scope = new EditorGUI.DisabledGroupScope(true);
            EditorGUI.PropertyField( position, property, label, true );
        }
    }
}