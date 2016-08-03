using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class MonsterAI
{
    [SerializeField]
    public float sightDistance = 20;

    [SerializeField]
    public float fov = 90;

    [SerializeField]
    public float closeCombatRange = 1.2f;

    [SerializeField]
    public float leapRange;

    [SerializeField]
    public int visionRate = 2;
}

[Serializable]
public class MonsterAnimations
{
    [SerializeField]
    public MDLAnimation[] stand;

    [SerializeField]
    public MDLAnimation[] patrol;

    [SerializeField]
    public MDLAnimation[] chase;

    [SerializeField]
    public MDLAnimation[] leap;

    [SerializeField]
    public MDLAnimation[] attackLongRange;

    [SerializeField]
    public MDLAnimation[] attackShortRangeLight;

    [SerializeField]
    public MDLAnimation[] attackShortRangeHard;

    [SerializeField]
    public MDLAnimation[] pain;

    [SerializeField]
    public MDLAnimation[] death;
}

[Serializable]
public class MonsterAudio
{
    [SerializeField]
    public AudioClip[] idle;

    [SerializeField]
    public AudioClip[] sight;

    [SerializeField]
    public AudioClip[] attack;

    [SerializeField]
    public AudioClip[] smash;

    [SerializeField]
    public AudioClip[] swing;

    [SerializeField]
    public AudioClip[] pain;

    [SerializeField]
    public AudioClip[] death;
}

[Serializable]
[CreateAssetMenu]
public class MonsterData : ScriptableObject
{
    [SerializeField]
    public MDL model;

    [SerializeField]
    public MonsterAI ai;

    [SerializeField]
    public MonsterAnimations animations;

    [SerializeField]
    public MonsterAudio audio;

    public bool canAttackLongRange
    {
        get { return HasAnimations(animations.attackLongRange); }
    }

    public bool canAttackCloseRange
    {
        get { return HasAnimations(animations.attackShortRangeLight) || HasAnimations(animations.attackShortRangeHard); }
    }

    public float sightDistanceSqr
    {
        get { return ai.sightDistance * ai.sightDistance; }
    }

    public float closeCombatRangeSqr
    {
        get { return ai.closeCombatRange * ai.closeCombatRange; }
    }

    bool HasAnimations(MDLAnimation[] animations)
    {
        return animations != null && animations.Length > 0;
    }
}
