using UnityEngine;
using System.Collections;

[CreateAssetMenu]
public class WeaponInfo : ScriptableObject
{
    [SerializeField]
    public MDL model;

    [SerializeField]
    public MDLAnimation shotAnimation;

    [SerializeField]
    public AudioClip shotStartSound;

    [SerializeField]
    public AudioClip shotSound;

    [SerializeField]
    public int damage;

    [SerializeField]
    public int shotsPerMinute;

    [SerializeField]
    public int maxAmmo;
}
