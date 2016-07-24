using UnityEngine;
using System.Collections;

public class item_entity : entity
{
    #region Collision
    
    protected override void OnCharacterEnter(CharacterController character)
    {
        if (Pickup())
        {
            Destroy(gameObject);
        }   
    }

    #endregion

    #region Pickup

    protected virtual bool Pickup()
    {
        return true;
    }

    #endregion
}
