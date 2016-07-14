using UnityEngine;
using System;
using System.Collections;

public class MonoBehaviourEx : MonoBehaviour
{
    protected void StopCoroutine(Delegate del)
    {
        var name = del.Method.Name;
        StopCoroutine(name);
    }
}
