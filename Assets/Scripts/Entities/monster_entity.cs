using UnityEngine;

using System;
using System.Collections;

using Random = UnityEngine.Random;
using StateUpdateCallback = System.Action<float>;

[RequireComponent(typeof(MDLAnimator))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AudioSource))]
public abstract class monster_entity : entity
{
    enum MonsterState
    {
        Sleep,              // not activated
        Stand,              // standing still
        Patrol,             // patrolling the area
        Sight,              // spotted the player
        Chase,              // chasing the player
        Leap,               // jumping attack
        LongRangeAttack,    // attack from long range (shooting, etc)
        CloseRangeAttack,   // attack from close range (melee, smash, etc)
        Reload,             // reloading
        Hurt,               // taking damage
        Dead                // dead
    }

    [SerializeField]
    MonsterData m_data;

    [SerializeField]
    LayerMask m_visionRaycastMask;

    NavMeshAgent m_navMeshAgent;
    MDLAnimator m_animator;

    TimerManager m_timerManager;
    CharacterController m_character;

    MonsterState m_state;
    Action<float> m_stateUpdateCallback;
    float m_stateUpdateElapsed;
    float m_stateUpdateDelay;

    bool m_playerVisible;
    float m_lastAttackElapsed;

    #region Life cycle

    protected override void OnAwake()
    {
        base.OnAwake();

        CheckAssigned(m_data, "data");
        
        m_timerManager = new TimerManager();

        m_navMeshAgent = GetRequiredComponent<NavMeshAgent>();
        m_animator = GetRequiredComponent<MDLAnimator>();
    }

    protected override void OnStart()
    {
        base.OnStart();

        SetState(MonsterState.Stand);

        m_character = FindObjectOfType<CharacterController>();
        if (m_character == null)
        {
            Debug.LogError("Can't find character controller");
        }

        StartUpdateVision();
    }
    
    protected override void OnUpdate(float deltaTime)
    {
        if (m_stateUpdateCallback != null)
        {
            m_stateUpdateElapsed += deltaTime;
            if (m_stateUpdateElapsed > m_stateUpdateDelay)
            {   
                m_stateUpdateCallback(m_stateUpdateElapsed);
                m_stateUpdateElapsed = 0.0f;
            }
        }

        m_timerManager.Update(deltaTime);
    }

    #endregion

    #region Player

    private bool IsPlayerInCloseRange()
    {
        var distance = m_character.transform.position - transform.position;
        return distance.sqrMagnitude < m_data.closeCombatRangeSqr;
    }

    #endregion

    #region Damage

    protected override void OnHurt(int damage)
    {
        base.OnHurt(damage);

        SetState(MonsterState.Hurt);

        m_navMeshAgent.Stop();
        PlayRandomAudioClip(m_data.audio.pain);
        PlayRandomAnimation(m_data.animations.pain, HurtAnimationFinished);
    }

    protected override void OnKill(int damage)
    {
        base.OnKill(damage);

        SetState(MonsterState.Dead);
        m_navMeshAgent.Stop();
        PlayRandomAudioClip(m_data.audio.death);
        PlayRandomAnimation(m_data.animations.death);
    }

    private void HurtAnimationFinished()
    {
        StartChasing();
    }

    #endregion

    #region States

    private void SetState(MonsterState state)
    {
        m_state = state;
        m_stateUpdateElapsed = 0.0f;

        switch (state)
        {
            case MonsterState.Chase:
                m_stateUpdateCallback = UpdateChasing;
                m_stateUpdateDelay = 0.5f;
                break;
            default:
                m_stateUpdateCallback = null;
                m_stateUpdateDelay = 0.0f;
                break;
        }
    }

    #endregion

    #region Vision

    private void StartUpdateVision()
    {
        ScheduleTimer(UpdateVision, 1.0f / m_data.visionRate, 0);
    }

    private void UpdateVision()
    {
        Vector3 distance = m_character.transform.position - transform.position;
        bool playerInRange = distance.sqrMagnitude < m_data.sightDistanceSqr;
        if (playerInRange)
        {
            bool playerWasVisible = m_playerVisible;

            RaycastHit hit;
            if (Physics.Raycast(transform.position, distance, out hit, m_data.sightDistance, m_visionRaycastMask))
            {
                m_playerVisible = hit.collider.tag == "Player";
            }
            else
            {
                m_playerVisible = false;
            }

            if (m_playerVisible && !playerWasVisible)
            {
                PlayerBecomeVisible();
            }
            else if (!m_playerVisible && playerWasVisible)
            {
                PlayerBecomeInvisible();
            }
        }
    }

    private void PlayerBecomeVisible()
    {
        if (m_state == MonsterState.Stand ||
            m_state == MonsterState.Patrol ||
            m_state == MonsterState.Sleep)
        {
            Sight();
        }
    }

    private void PlayerBecomeInvisible()
    {
    }

    #endregion

    #region Sight

    private void Sight()
    {
        m_state = MonsterState.Sight;
        PlayRandomAudioClip(m_data.audio.sight);

        StartChasing();
    }

    #endregion

    #region Chase

    void StartChasing()
    {
        SetState(MonsterState.Chase);
        PlayRandomAnimation(m_data.animations.chase);

        MoveToPlayer();
    }

    void UpdateChasing(float deltaTime)
    {
        if (IsPlayerInCloseRange())
        {
            m_navMeshAgent.Stop();
        }
        else
        {
            MoveToPlayer();
        }
    }

    void MoveToPlayer()
    {
        m_navMeshAgent.destination = m_character.transform.position;
        m_navMeshAgent.stoppingDistance = m_data.closeCombatRange;
        m_navMeshAgent.Resume();
    }
    
    #endregion

    #region Attack

    //void TryAttack()
    //{
    //    if (m_vision.playerVisible)
    //    {
    //        // stop moving towards the player
    //        m_navMeshAgent.Stop();

    //        m_state = MonsterState.Attacking;
    //        PlayRandomAnimation(m_data.animations.attack, AttackAnimationFinished);
    //        PlayRandomAudioClip(m_data.audio.attack);
    //    }
    //}

    #endregion

    #region Animation

    void PlayRandomAnimation(MDLAnimation[] animations, Action finishCallback = null)
    {
        if (animations != null && animations.Length > 0)
        {
            var index = Random.Range(0, animations.Length);
            m_animator.PlayAnimation(animations[index], finishCallback);
        }
    }

    #endregion

    #region Timers

    Timer ScheduleTimer(Action callback, float delay = 0.0f, int numRepeats = 1)
    {
        return m_timerManager.Schedule(callback, delay, numRepeats);
    }

    Timer RescheduleTimer(Action callback, float delay = 0.0f, int numRepeats = 1)
    {
        CancelTimer(callback);
        return ScheduleTimer(callback, delay, numRepeats);
    }

    void CancelTimer(Action callback)
    {
        m_timerManager.Cancel(callback);
    }

    void CancelAllTimers()
    {
        m_timerManager.CancelAll();
    }

    #endregion
}
