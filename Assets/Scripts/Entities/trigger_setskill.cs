using System;

using UnityEngine;

public class trigger_setskill : trigger
{
    [SerializeField]
    GameSkill m_skill = GameSkill.Normal;

    #region Collision

    protected override void OnCharacterEnter(CharacterController character)
    {
        GameMode.skill = m_skill;
        Debug.Log("Trigger set skill: " + m_skill);
    }

    #endregion

    #region Properties

    public GameSkill skill
    {
        get { return m_skill; }
        set { m_skill = value; }
    }

    #endregion
}
