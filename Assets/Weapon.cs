using UnityEngine;
using System.Collections;

public enum WeaponType
{
    Axe,
    Shotgun,
    SuperShotgun,
    Nailgun,
    SuperNailgun,
    GrenadeLauncher,
    RockerLauncher,
    Lightning
}

public class Weapon : MonoBehaviour
{
    [SerializeField]
    WeaponType m_weaponType;

    public WeaponType weaponType
    {
        get { return m_weaponType; }
    }
}
