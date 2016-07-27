using System;

using UnityEngine;

// [RequireComponent(typeof())]
public class monster_zombie : monster_entity
{
    [HideInInspector]
    [SerializeField]
    public bool crucified;

    protected override void OnStart()
    {
        base.OnStart();

        if (crucified)
        {
            // PlayAnimation("cruc__animation"); // TODO: play from a random frame
            Debug.LogWarning("Play crucified animation");
        }
    }
}
