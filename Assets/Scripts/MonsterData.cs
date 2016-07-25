using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class MonsterDataAnimations
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
    public MDLAnimation[] attack;

    [SerializeField]
    public MDLAnimation[] pain;

    [SerializeField]
    public MDLAnimation[] death;
}

[Serializable]
public class MonsterDataAudio
{
    [SerializeField]
    public AudioClip[] idle;

    [SerializeField]
    public AudioClip[] sight;

    [SerializeField]
    public AudioClip[] attack;

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
    public MonsterDataAnimations animations;

    [SerializeField]
    public MonsterDataAudio audio;
}
