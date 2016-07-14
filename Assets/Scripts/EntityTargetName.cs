using UnityEngine;
using System.Collections;

public class EntityTargetName : MonoBehaviour
{
    public void Signal()
    {
        gameObject.SendMessage("OnSignal");
    }
}
