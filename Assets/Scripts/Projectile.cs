using UnityEngine;
using System.Collections;
using System;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    bool m_useGravity;

    [SerializeField]
    float m_velocity;

    [SerializeField]
    Transform m_trailParticlesOrigin;

    [SerializeField]
    ParticleSystem m_trailParticles;

    [SerializeField]
    Explosion m_explosion;

	void Start()
    {
        var rigidbody = GetComponent<Rigidbody>();
        rigidbody.useGravity = m_useGravity;
        rigidbody.velocity = transform.rotation * Vector3.forward * m_velocity;

        var trailParticles = Instantiate(m_trailParticles, m_trailParticlesOrigin.position, Quaternion.identity) as ParticleSystem;
        trailParticles.transform.parent = transform;
    }

    void OnCollisionEnter(Collision collision)
    {
        Explode();
    }

    private void Explode()
    {
        Instantiate(m_explosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
