using UnityEngine;
using System.Collections;
using System;

public class MonsterVision : QuakeBehaviour
{
    [SerializeField]
    float m_sightDistance = 20;

    [SerializeField]
    float m_fov = 90;

    [SerializeField]
    float m_thinkRate = 5;

    [SerializeField]
    LayerMask m_raycastMask;

    CharacterController m_character;
    float m_thinkDelay;
    float m_lastThinkElasped;
    bool m_playerInRange;
    bool m_playerVisible;

    void Start()
    {
        m_thinkDelay = 1.0f / m_thinkRate;

        m_character = FindObjectOfType<CharacterController>();
        if (m_character == null)
        {
            Debug.LogError("Can't find character");
        }
    }

    void Update()
    {
        m_lastThinkElasped += Time.deltaTime;
        if (m_lastThinkElasped > m_thinkDelay)
        {
            m_lastThinkElasped = 0.0f;
            Think();
        }
    }

    #region Gizmos

    private void OnDrawGizmosSelected()
    {
        Color oldColor = Gizmos.color;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, m_sightDistance);
        Gizmos.color = oldColor;
    }

    #endregion

    #region Thinking

    private void Think()
    {
        bool playerWasInRange = m_playerInRange;

        Vector3 distance = m_character.transform.position - transform.position;
        m_playerInRange = distance.sqrMagnitude < m_sightDistance * m_sightDistance;
        if (m_playerInRange)
        {
            Debug.DrawRay(transform.position, distance, m_playerVisible ? Color.red : Color.green, m_thinkDelay);

            bool playerWasVisible = m_playerVisible;

            RaycastHit hit;
            if (Physics.Raycast(transform.position, distance, out hit, m_sightDistance, m_raycastMask))
            {
                m_playerVisible = hit.collider.tag == "Player";
            }
            else
            {
                m_playerVisible = false;
            }

            if (m_playerVisible && !playerWasVisible)
            {
                NotifyPlayerBecomeVisible();
            }
            else if (!m_playerVisible && playerWasVisible)
            {
                NotifyPlayerBecomeInvisible();
            }
        }
        else if (playerWasInRange && m_playerVisible)
        {
            m_playerVisible = false;
            NotifyPlayerBecomeInvisible();
        }
    }

    #endregion

    #region Triggers

    void OnTriggerEnter(Collider other)
    {
        m_playerInRange = true;
        Debug.Log("Character entered trigger");
    }

    void OnTriggerExit(Collider other)
    {
        m_playerInRange = false;
        Debug.Log("Character exited trigger");
    }

    #endregion

    #region Notifications

    void NotifyPlayerBecomeVisible()
    {
        if (onPlayerBecomeVisible != null)
        {
            onPlayerBecomeVisible();
        }
    }

    void NotifyPlayerBecomeInvisible()
    {
        if (onPlayerBecomeInvisible != null)
        {
            onPlayerBecomeInvisible();
        }
    }

    #endregion

    #region Properties

    public Action onPlayerBecomeVisible { get; set; }
    public Action onPlayerBecomeInvisible { get; set; }

    public bool playerInRange
    {
        get { return m_playerInRange; }
    }

    public bool playerVisible
    {
        get { return m_playerVisible; }
    }

    #endregion
}
