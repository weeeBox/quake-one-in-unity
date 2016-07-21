using UnityEngine;

using System;
using System.Collections;

using Random = UnityEngine.Random;

public abstract class monster_entity : entity
{
    [SerializeField]
    int m_health = 100;

    MDLAnimator m_animator;

    void Awake()
    {
        m_animator = GetRequiredComponent<MDLAnimator>();
    }

    // Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
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
    }
    
    protected virtual void Hurt()
    {
        PlayRandomAnimation(PainAnimations);
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
