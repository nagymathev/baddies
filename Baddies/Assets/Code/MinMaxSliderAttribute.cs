using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class MinMaxSliderAttribute : PropertyAttribute
{
    public readonly float max;
    public readonly float min;

	public MinMaxSliderAttribute( float min_, float max_)
    {
        min = min_;
        max = max_;
    }
}

#if UNITY_EDITOR

[CustomPropertyDrawer( typeof( MinMaxSliderAttribute ) )]
internal class MinMaxSliderPropertyDrawer : PropertyDrawer
{
    private const int _control_height = 16;
    public override float GetPropertyHeight( SerializedProperty property_, GUIContent label_ )
    {
		return base.GetPropertyHeight(property_, label_) + _control_height;// * 2f;
    }
 
    public override void OnGUI( Rect position_, SerializedProperty property_, GUIContent label_ )
    {
        label_ = EditorGUI.BeginProperty( position_, label_, property_ );
 
        if( property_.propertyType == SerializedPropertyType.Vector2 )
        {
            Vector2 range = property_.vector2Value;
            MinMaxSliderAttribute attr = attribute as MinMaxSliderAttribute;
 
            range.x = Mathf.Max( range.x, attr.min );
            range.y = Mathf.Min( range.y, attr.max );
 
            //range = EditorGUI.Vector2Field( position_, label_, range );
 
            //Rect position = EditorGUI.IndentedRect( position_ );
            //position.y += _control_height * 1.5f;
            //position.height = _control_height + 5;
            EditorGUI.MinMaxSlider(label_, position_, ref range.x, ref range.y, attr.min, attr.max );

			Rect position = EditorGUI.IndentedRect(position_);
			position.y += _control_height;// * 1.5f;
			position.x += EditorGUIUtility.labelWidth;
			position.width -= EditorGUIUtility.labelWidth;
			range = EditorGUI.Vector2Field(position, "", range);

			property_.vector2Value = range;         
        }
        else
        {
            EditorGUI.LabelField( position_, label_, "Use only with Vector2" );
        }
 
        EditorGUI.EndProperty( );
    }
}
#endif
