using UnityEngine;
using UnityEditor;
 
[CustomPropertyDrawer(typeof(Quaternion))]
public class QuaternionEditor : PropertyDrawer
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
        Vector3 euler = property.quaternionValue.eulerAngles;
        euler = EditorGUI.Vector3Field(position, label.text + " (Euler)", euler);
        property.quaternionValue = Quaternion.Euler(euler);
    }
 
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return Screen.width < 333 ? (16f + 18f) : 16f;
    }
}
