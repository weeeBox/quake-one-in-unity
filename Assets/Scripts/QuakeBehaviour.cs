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

    protected void StopCoroutine(Delegate del)
    {
        var name = del.Method.Name;
        StopCoroutine(name);
    }

    #endregion
}
