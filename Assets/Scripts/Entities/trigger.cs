using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public abstract class trigger : entity
{
    BoxCollider m_collider;

    void OnDrawGizmos()
    {
        OnDrawGizmos(false);
    }

    void OnDrawGizmosSelected()
    {
        OnDrawGizmos(true);
    }

    void OnDrawGizmos(bool selected)
    {
        if (m_collider == null)
        {
            m_collider = GetComponent<BoxCollider>();
        }

        Gizmos.color = selected ? Color.white : Color.green;
        Gizmos.DrawCube(transform.position, m_collider.size);
    }
}
