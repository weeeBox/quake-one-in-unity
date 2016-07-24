using System;

using UnityEngine;

public class item_health : item_entity
{
    protected override bool Pickup()
    {
        Debug.LogWarning("Implement me!");
        return true;
    }
}
