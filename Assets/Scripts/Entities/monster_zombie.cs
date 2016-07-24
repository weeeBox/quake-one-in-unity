using System;

using UnityEngine;

// [RequireComponent(typeof())]
public class monster_zombie : monster_entity
{
    static readonly string[] kPainAnimations =
    {
        "paina_animation", "painb_animation", "painc_animation", "paind_animation", "paine_animation"
    };

    static readonly string[] kDeathAnimations = { };

    [HideInInspector]
    [SerializeField]
    public bool crucified;

    protected override void Start()
    {
        base.Start();

        if (crucified)
        {
            PlayAnimation("cruc__animation"); // TODO: play from a random frame
        }
    }

    #region Damage

    protected override string[] DeathAnimations
    {
        get { return kDeathAnimations; }
    }

    protected override string[] PainAnimations
    {
        get { return kPainAnimations; }
    }

    #endregion
}
