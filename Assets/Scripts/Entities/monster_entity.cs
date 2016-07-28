using UnityEngine;

using System;
using System.Collections;

using Random = UnityEngine.Random;

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

    static readonly string kStateTimerTag = "State";

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
    
    bool m_playerVisible;

    const float kUpdateChasePathDelay = 1.0f / 2; // 2 times/second

    #region Life cycle

    protected override void OnAwake()
    {
        base.OnAwake();

        CheckAssigned(m_data, "data");
        
        m_timerManager = new TimerManager();

        m_navMeshAgent = GetRequiredComponent<NavMeshAgent>();
        m_navMeshAgent.stoppingDistance = m_data.closeCombatRange;
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
            m_stateUpdateCallback(deltaTime);
        }

        m_timerManager.Update(deltaTime);
    }

    #endregion

    #region Player

    private bool IsPlayerInCloseRange()
    {
        return !m_navMeshAgent.pathPending &&
            m_navMeshAgent.remainingDistance <= m_navMeshAgent.stoppingDistance &&
            (!m_navMeshAgent.hasPath || m_navMeshAgent.velocity.sqrMagnitude < 0.0001f);
    }

    #endregion

    #region Damage

    protected override void OnHurt(int damage)
    {
        base.OnHurt(damage);

        SetState(MonsterState.Hurt);

        PlayRandomAudioClip(m_data.audio.pain);
        PlayRandomAnimation(m_data.animations.pain, HurtAnimationFinished);
    }

    protected override void OnKill(int damage)
    {
        base.OnKill(damage);

        SetState(MonsterState.Dead);

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
        CancelAllTimers(kStateTimerTag);

        m_state = state;
        m_navMeshAgent.Stop();
    }

    #endregion

    #region Vision

    private void StartUpdateVision()
    {
        ScheduleTimer(UpdateVision, 1.0f / m_data.visionRate, true);
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
        
        UpdateChasePath();

        ScheduleTimer(UpdateChase, 0.0f, true, kStateTimerTag);
        ScheduleTimer(UpdateChasePath, kUpdateChasePathDelay, true, kStateTimerTag);

        if (m_data.canAttackLongRange)
        {
            ScheduleTimer(AttackLongRange, Random.Range(1.0f, 1.5f), false, kStateTimerTag); // TODO: configure attack range
        }
    }

    void UpdateChase()
    {
        if (IsPlayerInCloseRange())
        {
            m_navMeshAgent.Stop(); 

            if (m_data.canAttackCloseRange)
            {
                AttackCloseRange();
            }
        }
    }
    
    void UpdateChasePath()
    {
        m_navMeshAgent.destination = m_character.transform.position;
        m_navMeshAgent.Resume();
    }

    #endregion

    #region Attack

    private void AttackCloseRange()
    {
        SetState(MonsterState.CloseRangeAttack);
        PlayRandomAnimation(m_data.animations.attackShortRangeLight, AttackAnimationFinished);
    }

    private void AttackLongRange()
    {
        SetState(MonsterState.LongRangeAttack);
        PlayRandomAnimation(m_data.animations.attackLongRange, AttackAnimationFinished);
    }

    private void AttackAnimationFinished()
    {
        StartChasing();
    }

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

    Timer ScheduleTimer(Action callback, float delay = 0.0f, bool repeated = false, string tag = null)
    {
        return m_timerManager.Schedule(callback, delay, repeated ? 0 : 1, tag);
    }

    void CancelTimer(Action callback)
    {
        m_timerManager.Cancel(callback);
    }

    void CancelAllTimers(string tag)
    {
        m_timerManager.Cancel(tag);
    }

    void CancelAllTimers()
    {
        m_timerManager.CancelAll();
    }

    #endregion
}
