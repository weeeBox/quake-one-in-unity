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

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(MDLAnimator))]
public class Weapon : MonoBehaviour
{
    [SerializeField]
    WeaponInfo[] m_weapons;

    [SerializeField]
    WeaponType m_weaponType;

    WeaponInfo m_weaponInfo;

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

    float m_shootElapsed;
    float m_shootDelayed;

    void Awake()
    {
        m_animator = GetComponent<MDLAnimator>();
        m_audioSource = GetComponent<AudioSource>();

        ChangeWeapon(m_weaponType);
    }

    void Update()
    {
        if (Input.GetButton("Fire1"))
        {
            m_shootElapsed += Time.deltaTime;
            if (m_shootElapsed > m_shootDelayed)
            {
                m_shootElapsed = 0.0f;
                Shoot();
            }
        }   
    }

    public void Shoot()
    {
        // animation
        m_animator.PlayAnimation(m_weaponInfo.shotAnimation);

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

    public void ChangeWeapon(WeaponType weaponType)
    {
        m_weaponType = weaponType;
        m_weaponInfo = m_weapons[(int) weaponType];

        #if UNITY_EDITOR
        m_animator = GetComponent<MDLAnimator>();
        #endif

        m_animator.model = m_weaponInfo.model;
        m_shootDelayed = 60.0f / m_weaponInfo.shotsPerMinute;
    }

    public WeaponType weaponType
    {
        get { return m_weaponType; }
    }

    public WeaponInfo[] weapons
    {
        get { return m_weapons; }
    }
}
