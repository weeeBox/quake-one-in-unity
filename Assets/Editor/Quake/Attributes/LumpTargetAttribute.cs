using System;

[AttributeUsage(AttributeTargets.Field)]
public class LumpTargetAttribute : Attribute
{
    public LumpTargetAttribute(Type target)
    {
        this.target = target;
    }

    public Type target
    {
        get; private set;
    }
}

