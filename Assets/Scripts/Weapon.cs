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

    [SerializeField]
    Camera m_weaponCamera;

    [SerializeField]
    Transform m_shootingOrigin;

    [SerializeField]
    GameObject m_hit;
    
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }   
    }

    public void Shoot()
    {
        Ray ray = m_weaponCamera.ViewportPointToRay(new Vector2(0.5f, 0.5f));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100.0f))
        {
            Instantiate(m_hit, hit.point, Quaternion.identity);
        }
    }

    public WeaponType weaponType
    {
        get { return m_weaponType; }
    }
}
