using UnityEngine;
using System.Collections;

/// <summary>
/// A base class for every 'entity' game object
/// </summary>
public abstract class entity : MonoBehaviour
{
    [HideInInspector]
    public string data;

    #region Gizmos

    void OnDrawGizmos()
    {
        DrawGizmos(false);
    }

    void OnDrawGizmosSelected()
    {
        DrawGizmos(true);
    }

    protected virtual void DrawGizmos(bool selected)
    {   
    }

    #endregion

    #region Target

    public T GetTarget<T>() where T : entity
    {
        var target = GetComponent<EntityTarget>();
        return target != null ? target.GetComponent<T>() : null;
    }

    #endregion
}
