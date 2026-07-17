using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(FieldNameAttribute))]
public class FieldNameDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // 获取我们的自定义特性，拿到预设的名称
        FieldNameAttribute attr = (FieldNameAttribute)attribute;
        
        // 使用自定义名称替换默认显示名
        label.text = attr.Name;
        
        // 调用默认的字段绘制（Slider、InputField 等都会正常显示）
        EditorGUI.PropertyField(position, property, label, true);
    }
}
