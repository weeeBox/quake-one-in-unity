using System;

using UnityEngine;

public class trigger_teleport : trigger
{
    EntityTargetName m_targetName;

    void Start()
    {
        m_targetName = GetTargetName();
    }

    protected override void OnCharacterEnter(CharacterController character)
    {
        Rigidbody rigidbody = character.GetComponent<Rigidbody>();
        rigidbody.Sleep();
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        character.transform.position = m_targetName.transform.position + 0.5f * character.height * Vector3.up;
        rigidbody.WakeUp();
    }

    protected override void DrawGizmos(bool selected)
    {
        base.DrawGizmos(selected);

        if (selected && m_targetName != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, m_targetName.transform.position);
        }
    }
}
