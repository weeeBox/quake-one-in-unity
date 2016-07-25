using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public abstract class trigger : entity
{
    BoxCollider m_collider;

    #region Damage

    public override void TakeDamage(int damage)
    {
        if (this.health > 0)
        {
            SignalTarget();
        }
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
