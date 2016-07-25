using UnityEngine;
using System;
using System.Collections;

public class QuakeBehaviour : MonoBehaviour
{
    #region Damage

    public virtual void TakeDamage(int damage)
    {   
    }

    #endregion

    #region Helpers

    protected void CheckAssigned(object field, string name)
    {
        if (field == null)
        {
            Debug.LogError("Field '" + name + "' not assigned");
        }
    }

    protected T GetRequiredComponent<T>()
    {
        var component = GetComponent<T>();
        if (component == null)
        {
            Debug.LogError("Can't get required component: " + typeof(T)); // TODO: throw exception?
        }

        return component;
    }

    protected void StopCoroutine(Delegate del)
    {
        var name = del.Method.Name;
        StopCoroutine(name);
    }

    #endregion
}
