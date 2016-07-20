using UnityEngine;
using System.Collections;

[CreateAssetMenu]
public class WeaponInfo : ScriptableObject
{
    [SerializeField]
    MDL m_model;

    [SerializeField]
    int m_damage;

    [SerializeField]
    int m_shotsPerMinute;

    public MDL model
    {
        get { return m_model; }
    }

    public int damage
    {
        get { return m_damage; }
    }

    public int shotsPerMinute
    {
        get { return m_shotsPerMinute; }
    }
}
