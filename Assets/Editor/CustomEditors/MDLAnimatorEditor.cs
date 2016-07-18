using UnityEngine;
using UnityEditor;

using System.Collections;

[CustomEditor(typeof(MDLAnimator))]
public class MDLAnimatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var animator = target as MDLAnimator;
        var oldModel = animator.model;

        base.OnInspectorGUI();

        if (animator.model != oldModel)
        {
            animator.RefreshModel();
        }
    }
}
