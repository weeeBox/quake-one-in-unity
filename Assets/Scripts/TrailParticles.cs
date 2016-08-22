using UnityEngine;
using System.Collections;

public class TrailParticles : Particles
{
    [SerializeField]
    Transform m_emitter;
    
    [SerializeField]
    float m_emissionRate = 0.1f;

    [SerializeField]
    float m_emissionRadius = 0.01f;

    [SerializeField]
    float m_floatSpeed = 0.5f;

    float m_emissionDelay;
    float m_emissionElapsed;

    protected override void Awake()
    {
        base.Awake();
        m_emissionDelay = 1.0f / m_emissionRate;
    }

    protected override void Update()
    {
        if (m_particleCount < m_maxParticles)
        {
            m_emissionElapsed += Time.deltaTime;
            if (m_emissionElapsed > m_emissionDelay)
            {
                Vector3 pos = m_emitter.transform.position + Random.insideUnitSphere * m_emissionRadius;
                AddParticle(pos, Color.white);
                m_emissionElapsed = 0.0f;
            }
        }

        Vector3 offset = Vector3.up * m_floatSpeed * Time.deltaTime;
        for (int i = 0; i < m_particleCount; ++i)
        {
            MoveParticle(i, offset);
        }

        base.Update();
    }
}
