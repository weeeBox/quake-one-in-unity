using UnityEngine;
using System.Collections;

/// <summary>
/// A base class for every 'entity' game object
/// </summary>
public abstract class entity : QuakeBehaviour
{
    [HideInInspector]
    public string data;

    [SerializeField]
    public float health;

    #region Life cycle

    protected virtual void Awake()
    {

    }

    protected virtual void Start()
    {
    }

    #endregion

    #region Collision

    void OnTriggerEnter(Collider other)
    {
        CharacterController character = other.GetComponent<CharacterController>();
        if (character != null)
        {
            OnCharacterEnter(character);
        }
    }

    void OnTriggerExit(Collider other)
    {
        CharacterController character = other.GetComponent<CharacterController>();
        if (character != null)
        {
            OnCharacterExit(character);
        }
    }

    protected virtual void OnCharacterEnter(CharacterController character)
    {
    }

    protected virtual void OnCharacterExit(CharacterController character)
    {
    }

    #endregion

    #region Gizmos

    void OnDrawGizmos()
    {
        DrawGizmos(false);
    }

    void OnDrawGizmosSelected()
    {
        DrawGizmos(true);
    }

    protected virtual void DrawGizmos(bool selected)
    {   
    }

    #endregion

    #region Damage

    public override void TakeDamage(int damage)
    {
        if (this.health > 0)
        {
            this.health -= damage;
            if (health <= 0)
            {
                OnKill();
            }
        }
    }

    protected virtual void OnKill()
    {
    }

    #endregion

    #region Target

    protected virtual void OnSignal()
    {
    }

    public void SignalTarget()
    {
        var targetName = GetTargetName();
        if (targetName != null)
        {
            targetName.Signal();
        }
        else
        {
            Debug.LogError("No target to signal");
        }
    }

    public EntityTargetName GetTargetName()
    {
        var target = GetComponent<EntityTarget>();
        return target != null ? target.targetName : null;
    }
    
    public bool HasTargetName()
    {
        return GetComponent<EntityTargetName>();
    }

    #endregion
}
