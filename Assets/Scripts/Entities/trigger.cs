using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public abstract class trigger : entity
{
    BoxCollider m_collider;

    protected new BoxCollider collider
    {
        get
        {
            if (m_collider == null)
            {
                m_collider = GetComponent<BoxCollider>();
            }
            return m_collider;
        }
    }
}
