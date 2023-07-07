using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Vector4))]
public class Vector4Editor : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.hasMultipleDifferentValues)
        {
            EditorGUI.showMixedValue = true;
            EditorGUI.PropertyField(position, property, label);
            EditorGUI.showMixedValue = false;
            return;
        }
        Vector4 value = property.vector4Value;
        value = EditorGUI.Vector4Field(position, label.text + "", value);
        property.vector4Value = value;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return Screen.width < 333 ? (16f + 18f) : 16f;
    }
}
