using System;

using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class func_button : entity, IShootTarget
{
    public void Trigger()
    {
        SignalTarget();
    }

    protected override void OnCharacterEnter(CharacterController character)
    {
        Trigger();
    }

    #region IShootTarget

    public void TakeDamage(int damage)
    {
        Trigger();   
    }

    #endregion
}
