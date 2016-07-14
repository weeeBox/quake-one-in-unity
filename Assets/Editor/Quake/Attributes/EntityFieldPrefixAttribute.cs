using System;

[AttributeUsage(AttributeTargets.Field)]
public class EntityFieldPrefixAttribute : Attribute
{
    public EntityFieldPrefixAttribute(string prefix)
    {
        this.prefix = prefix;
    }

    public string prefix
    {
        get; private set;
    }
}

