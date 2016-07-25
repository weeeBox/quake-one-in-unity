using UnityEngine;
using System.Collections;
using System;

public class MonsterController : QuakeBehaviour
{
    enum EnemyState
    {
        Sleeping,
        Standing,
        Patrolling,
        Attacking,
        Hurt,
        Dead
    }

    NavMeshAgent m_navMeshAgent;
    CharacterController m_character;
    EnemyState m_state;

    void Awake()
    {
        m_state = EnemyState.Sleeping;
        m_navMeshAgent = GetRequiredComponent<NavMeshAgent>();
    }

    void Start()
    {
        m_state = EnemyState.Standing;

        m_character = FindObjectOfType<CharacterController>();
        if (m_character == null)
        {
            Debug.LogError("Can't find character controller");
        }

        var vision = GetComponentInChildren<MonsterVision>();
        vision.onPlayerBecomeVisible = onPlayerBecomeVisible;
        vision.onPlayerBecomeInvisible = onPlayerBecomeInvisible;
    }
    
    void Update()
    {   
    }

    #region Vision

    private void onPlayerBecomeInvisible()
    {
        SetColor(Color.white);
    }

    private void onPlayerBecomeVisible()
    {
        SetColor(Color.red);
    }

    private void SetColor(Color color)
    {
        var meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material.color = color;
    }

    #endregion
}
