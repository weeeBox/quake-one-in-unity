using UnityEngine;
using System.Collections;

public enum GameSkill
{
    Easy,
    Normal,
    Hard,
    Nightmare,
    Undefined
}

public class GameMode : SingletonBehaviour<GameMode>
{
    GameSkill m_skill = GameSkill.Normal;

    #region Properties

    public static string skillName
    {
        get
        {
            switch (skill)
            {
                case GameSkill.Easy:
                    return "EASY";
                case GameSkill.Normal:
                    return "NORMAL";
                case GameSkill.Hard:
                    return "HARD";
                case GameSkill.Nightmare:
                    return "NIGHTMARE";
            }

            return "undefined";
        }
    }

    public static GameSkill skill
    {
        get { return instance != null ? instance.m_skill : GameSkill.Undefined; }
        set { instance.m_skill = value; }
    }

    #endregion
}
