using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public abstract class trigger : entity
{
    BoxCollider m_collider;

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

    protected override void DrawGizmos(bool selected)
    {
        if (m_collider == null)
        {
            m_collider = GetComponent<BoxCollider>();
        }

        Gizmos.color = selected ? Color.yellow : Color.green;
        Gizmos.DrawCube(transform.position, m_collider.size);
    }

    #endregion
}
