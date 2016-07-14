using System;
using System.Collections;

using UnityEngine;

public enum door_items
{
    IT_KEY1 = 131072,
    IT_KEY2 = 262144
}

public class func_door : entity
{   
    [HideInInspector]
    public Vector3 pos1;

    [HideInInspector]
    public Vector3 pos2;

    [HideInInspector]
    public door_items items;

    [HideInInspector]
    public float speed;

    void OnTriggerEnter(Collider other)
    {
        Open();
    }

    public void Open()
    {
        StartCoroutine(MoveToTarget(pos2));
    }

    public void Close()
    {
        StartCoroutine(MoveToTarget(pos1));
    }

    IEnumerator MoveToTarget(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        Vector3 offset;
        Vector3 remains;
        do
        {
            remains = target - transform.position;
            offset = direction * speed * Time.deltaTime;

            if (remains.sqrMagnitude < offset.sqrMagnitude)
            {
                transform.Translate(remains);
                break;
            }
            else
            {
                transform.Translate(offset);
            }

            yield return null;
        }
        while (true);

        transform.position = target;
    }
}
