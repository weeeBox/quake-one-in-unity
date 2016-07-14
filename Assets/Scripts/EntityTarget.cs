using UnityEngine;
using System.Collections;

public class EntityTarget : MonoBehaviour
{
    [SerializeField]
    EntityTargetName m_targetName;

    public EntityTargetName targetName
    {
        get { return m_targetName; }
        set { m_targetName = value; }
    }
}
