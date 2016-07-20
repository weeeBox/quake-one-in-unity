using UnityEngine;
using System;
using System.Collections;

public class QuakeBehaviour : MonoBehaviour
{
    protected void StopCoroutine(Delegate del)
    {
        var name = del.Method.Name;
        StopCoroutine(name);
    }
}
