using UnityEngine;

using System;
using System.Collections;

using Random = UnityEngine.Random;

public abstract class monster_entity : entity
{
    [SerializeField]
    int m_health = 100;

    [SerializeField]
    AudioClip[] m_hurtSounds;

    [SerializeField]
    AudioClip[] m_deathSounds;

    MDLAnimator m_animator;

    protected override void Awake()
    {
        base.Awake();
        m_animator = GetRequiredComponent<MDLAnimator>();
    }

    #region Damage

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

        m_health -= damage;
        if (m_health < 0)
        {
            Kill();
        }
        else
        {
            Hurt();
        }
    }

    protected virtual void Kill()
    {   
        PlayRandomAnimation(DeathAnimations);
        PlayRandomAudioClip(m_deathSounds);
    }
    
    protected virtual void Hurt()
    {
        PlayRandomAnimation(PainAnimations);
        PlayRandomAudioClip(m_hurtSounds);
    }
    
    protected abstract string[] PainAnimations { get; }
    protected abstract string[] DeathAnimations { get; }

    #endregion

    #region Animations

    void PlayRandomAnimation(string[] names)
    {
        int animationIndex = Random.Range(0, names.Length);
        PlayAnimation(names[animationIndex]);
    }

    protected void PlayAnimation(string name)
    {
        m_animator.PlayAnimation(name);
    }

    #endregion
}
