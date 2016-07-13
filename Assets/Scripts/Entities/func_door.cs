using System;

using UnityEngine;

// [RequireComponent(typeof())]
public class func_door : entity
{
    [SerializeField]
    Vector3 m_pos1;

    [SerializeField]
    Vector3 m_pos2;

    #region Properties

    public Vector3 pos1
    {
        get { return m_pos1; }
        set { m_pos1 = value; }
    }

    public Vector3 pos2
    {
        get { return m_pos2; }
        set { m_pos2 = value; }
    }

    #endregion
}
