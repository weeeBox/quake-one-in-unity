using System;

using UnityEngine;

// [RequireComponent(typeof())]
public class monster_army : monster_entity
{
    static readonly string[] kPainAnimations =
    {
        "pain_animation", "painb_animation", "painc_animation"
    };

    static readonly string[] kDeathAnimations =
    {
        "death_animation", "deathc_animation"
    };

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
