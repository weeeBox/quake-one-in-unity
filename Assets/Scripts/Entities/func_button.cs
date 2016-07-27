using System;

using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class func_button : entity
{
    public void Trigger()
    {
        SignalTarget();
    }

    protected override void OnCharacterEnter(CharacterController character)
    {
        Trigger();
    }

    #region Damage

    protected override void OnKill(int damage)
    {
        Trigger();
    }

    #endregion
}
