using UnityEngine;

public class FieldNameAttribute : PropertyAttribute
{
    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; private set; }
    /// <summary>
    /// 字段名称
    /// </summary>
    public FieldNameAttribute(string name)
    {
        Name = name;
    }
}
