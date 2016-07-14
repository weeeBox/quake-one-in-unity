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

    protected virtual void OnSignal()
    {
    }

    public void SignalTarget()
    {
        var targetName = GetTargetName();
        if (targetName != null)
        {
            targetName.Signal();
        }
        else
        {
            Debug.LogError("No target to signal");
        }
    }

    public EntityTargetName GetTargetName()
    {
        var target = GetComponent<EntityTarget>();
        return target != null ? target.targetName : null;
    }

    #endregion
}
