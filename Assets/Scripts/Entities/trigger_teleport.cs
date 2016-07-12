using System;

using UnityEngine;

public class trigger_teleport : trigger
{
    [SerializeField]
    info_teleport_destination m_destination;

    protected override void OnCharacterEnter(CharacterController character)
    {
        Rigidbody rigidbody = character.GetComponent<Rigidbody>();
        rigidbody.Sleep();
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        character.transform.position = m_destination.transform.position + 0.5f * character.height * Vector3.up;
        rigidbody.WakeUp();
    }
}
