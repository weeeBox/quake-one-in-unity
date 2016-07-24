using System;

using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class trigger_teleport : trigger
{
    [SerializeField]
    AudioClip[] m_teleportClips;

    EntityTargetName m_targetName;

    protected override void OnStart()
    {
        base.OnStart();

        m_targetName = GetTargetName();
        if (m_targetName == null)
        {
            Debug.LogError("Can't resolve teleport target");
        }
    }

    protected override void OnCharacterEnter(CharacterController character)
    {
        Rigidbody rigidbody = character.GetComponent<Rigidbody>();
        rigidbody.Sleep();
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        character.transform.position = m_targetName.transform.position + 0.5f * character.height * Vector3.up;
        rigidbody.WakeUp();

        PlayRandomAudioClip(m_teleportClips);
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
