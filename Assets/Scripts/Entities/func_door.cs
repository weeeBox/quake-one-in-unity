using System;

using UnityEngine;

public enum door_items
{
    IT_KEY1 = 131072,
    IT_KEY2 = 262144
}

// [RequireComponent(typeof())]
public class func_door : entity
{   
    public Vector3 pos1;
    
    public Vector3 pos2;

    public door_items items;
}
