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
        var oldAnimation = animator.animation;
        var oldSkin = animator.skin;

        base.OnInspectorGUI();

        var model = animator.model;
        if (model != null && model.animationCount > 0)
        {
            string[] names = new string[model.animationCount];
            for (int i = 0; i < names.Length; ++i)
            {
                names[i] = model.animations[i].name;
            }

            int currentIndex = 0;
            if (animator.animationName != null)
            {
                int index = 0;
                foreach (var animation in model.animations)
                {
                    if (animation.name == animator.animationName)
                    {
                        currentIndex = index;
                        break;
                    }

                    ++index;
                }
            }

            int newIndex = EditorGUILayout.Popup("Animation", currentIndex, names);
            if (currentIndex != newIndex)
            {
                var animation = model.animations[newIndex];
                if (Application.isPlaying)
                {
                    animator.SetAnimation(animation.name);
                }
                else
                {
                    animator.animation = animation;
                }
            }
        }

        if (animator.model != oldModel)
        {
            animator.RefreshModel();
        }
    }
}
