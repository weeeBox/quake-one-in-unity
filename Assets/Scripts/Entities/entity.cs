using UnityEngine;
using System.Collections;

/// <summary>
/// A base class for every 'entity' game object
/// </summary>
public abstract class entity : MonoBehaviour
{
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
}
