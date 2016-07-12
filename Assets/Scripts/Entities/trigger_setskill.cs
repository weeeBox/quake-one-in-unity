using System;

using UnityEngine;

public class trigger_setskill : trigger
{
    [SerializeField]
    string m_message;

    #region Collision

    protected override void OnCharacterEnter(CharacterController character)
    {
        Debug.Log(m_message);
    }

    #endregion

    #region Properties

    public string message
    {
        get { return m_message; }
        set { m_message = value; }
    }


    #endregion
}
