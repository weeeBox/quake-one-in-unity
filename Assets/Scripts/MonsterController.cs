using UnityEngine;
using System.Collections;
using System;

using Random = UnityEngine.Random;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(MDLAnimator))]
[RequireComponent(typeof(AudioSource))]
public class MonsterController : QuakeBehaviour
{
    enum MonsterState
    {
        Sleeping,
        Standing,
        Patrolling,
        Sighting,
        Chasing,
        Attacking,
        Loading,
        Hurt,
        Dead
    }

    [SerializeField]
    MonsterData m_data;

    [SerializeField]
    float m_closeRangeDistance = 1.0f;

    NavMeshAgent m_navMeshAgent;
    AudioSource m_audioSource;
    MDLAnimator m_animator;

    TimerManager m_timerManager;
    CharacterController m_character;
    MonsterVision m_vision;

    MonsterState m_state;

    void Awake()
    {
        CheckAssigned(m_data, "data");

        m_state = MonsterState.Sleeping;
        m_timerManager = new TimerManager();

        m_navMeshAgent = GetRequiredComponent<NavMeshAgent>();
        m_animator = GetRequiredComponent<MDLAnimator>();
        m_audioSource = GetRequiredComponent<AudioSource>();
    }

    void Start()
    {
        m_state = MonsterState.Standing;

        m_character = FindObjectOfType<CharacterController>();
        if (m_character == null)
        {
            Debug.LogError("Can't find character controller");
        }

        m_vision = GetComponentInChildren<MonsterVision>();
        m_vision.onPlayerBecomeVisible = onPlayerBecomeVisible;
        m_vision.onPlayerBecomeInvisible = onPlayerBecomeInvisible;
    }
    
    void Update()
    {
        m_timerManager.Update(Time.deltaTime);
    }

    #region States

    void SetState(MonsterState state)
    {
        switch (state)
        {
            case MonsterState.Standing:

                break;
        }
    }

    #endregion

    #region Vision

    private void onPlayerBecomeVisible()
    {
        if (m_state == MonsterState.Standing ||
            m_state == MonsterState.Patrolling ||
            m_state == MonsterState.Sleeping)
        {
            Sight();
        }
    }

    private void onPlayerBecomeInvisible()
    {
    }

    #endregion

    #region Sight

    private void Sight()
    {
        PlayRandomAudioClip(m_data.audio.sight);
        StartChasing();
    }

    #endregion

    #region Chase

    void StartChasing()
    {
        m_state = MonsterState.Chasing;
        PlayRandomAnimation(m_data.animations.chase);

        ScheduleTimer(TryAttack, Random.Range(1.0f, 2.0f));
        ChaseTick();
    }

    void ChaseTick()
    {
        if (m_state == MonsterState.Chasing) // still chasing?
        {
            if (IsPlayerInCloseRange())
            {
                m_navMeshAgent.Stop();
            }
            else
            {
                m_navMeshAgent.destination = m_character.transform.position;
                m_navMeshAgent.stoppingDistance = m_closeRangeDistance;
                m_navMeshAgent.Resume();
            }

            ScheduleTimer(ChaseTick, 0.5f); // keep chasing
        }
    }

    bool IsPlayerInCloseRange()
    {
        var distance = m_character.transform.position - transform.position;
        return distance.sqrMagnitude < m_closeRangeDistance * m_closeRangeDistance;
    }

    #endregion

    #region Attack

    void TryAttack()
    {
        if (m_vision.playerVisible)
        {
            // stop moving towards the player
            m_navMeshAgent.Stop();

            m_state = MonsterState.Attacking;
            PlayRandomAnimation(m_data.animations.attack, AttackAnimationFinished);
            PlayRandomAudioClip(m_data.audio.attack);
        }
    }

    void AttackAnimationFinished(MDLAnimator animator, MDLAnimation animation, bool cancelled)
    {
        StartChasing();
    }

    #endregion

    #region Animation

    void PlayRandomAnimation(MDLAnimation[] animations, MDLAnimatorDelegate del = null)
    {
        if (animations != null && animations.Length > 0)
        {
            var index = Random.Range(0, animations.Length);
            m_animator.PlayAnimation(animations[index], del);
        }
    }

    #endregion

    #region Sounds

    void PlayRandomAudioClip(AudioClip[] clips)
    {
        if (clips != null && clips.Length > 0)
        {
            var index = Random.Range(0, clips.Length);
            m_audioSource.clip = clips[index];
            m_audioSource.Play();
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
