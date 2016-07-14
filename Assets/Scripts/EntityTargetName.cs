using UnityEngine;
using System.Collections;

public class EntityTargetName : MonoBehaviour
{
    public entity GetEntity<T>() where T : entity
    {
        return gameObject.GetComponent<T>();
    }
}
