using System;

[AttributeUsage(AttributeTargets.Class)]
public class EntityGroupAttribute : Attribute
{
    public EntityGroupAttribute(string group)
    {
        this.group = group;
    }

    public string group
    {
        get; private set;
    }
}

