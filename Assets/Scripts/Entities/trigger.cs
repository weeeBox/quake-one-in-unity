using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public abstract class trigger : entity
{
    BoxCollider m_collider;

    #region Collision

    void OnTriggerEnter(Collider other)
    {   
    }

    void OnTriggerExit(Collider other)
    {
    }

    #endregion

    #region Gizmos

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

        Gizmos.color = selected ? Color.yellow : Color.green;
        Gizmos.DrawCube(transform.position, m_collider.size);
    }

    #endregion
}
