using UnityEngine;

public class ConditionalHideAttribute : PropertyAttribute
{
    public string ConditionalSourceField;
    public bool HideInInspector;

    public ConditionalHideAttribute(string conditionalSourceField, bool hideInInspector = true)
    {
        ConditionalSourceField = conditionalSourceField;
        HideInInspector = hideInInspector;
    }
}