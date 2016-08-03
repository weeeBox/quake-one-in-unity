using UnityEngine;

using System;
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
    WeaponInfo[] m_weapons;

    [SerializeField]
    WeaponType m_weaponType;

    [SerializeField]
    Camera m_weaponCamera;

    [SerializeField]
    Transform m_shootingOrigin;

    [SerializeField]
    GameObject m_hit;

    [SerializeField]
    LayerMask m_shootingMask;

    AudioSource m_audioSource;
    MDLAnimator m_animator;

    void Awake()
    {
        m_animator = GetComponent<MDLAnimator>();
        m_audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }   
    }

    public void Shoot()
    {
        // animation
        m_animator.PlayAnimation(weaponInfo.shotAnimation);

        // sound
        m_audioSource.Play();

        // physics
        Ray ray = m_weaponCamera.ViewportPointToRay(new Vector2(0.5f, 0.5f));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100.0f, m_shootingMask))
        {
            var target = hit.collider.GetComponent<QuakeBehaviour>();
            if (target != null)
            {
                target.TakeDamage(20);
            }

            Instantiate(m_hit, hit.point, Quaternion.LookRotation(-ray.direction));
        }
    }

    public WeaponInfo weaponInfo
    {
        get { return m_weapons[(int)m_weaponType]; }
    }

    public WeaponType weaponType
    {
        get { return m_weaponType; }
    }
}
