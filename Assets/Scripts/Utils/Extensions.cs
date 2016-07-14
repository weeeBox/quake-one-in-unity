using UnityEngine;
using System;
using System.Collections;

public static class Extensions
{
    #region GameObject

    public static T GetSubclassComponent<T>(this GameObject obj) where T : Component
    {
        Type type = typeof(T);

        foreach (var component in obj.GetComponents<Component>())
        {
            if (component.GetType().IsSubclassOf(type))
            {
                return (T)component;
            }
        }

        return null;
    }

    #endregion

}
