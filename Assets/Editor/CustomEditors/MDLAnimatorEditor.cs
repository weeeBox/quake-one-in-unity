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

        var model = animator.model;
        if (model != null && model.animationCount > 0)
        {
            string[] names = new string[1 + model.animationCount];
            names[0] = "None";
            for (int i = 0; i < model.animationCount; ++i)
            {
                names[i + 1] = model.animations[i].name;
            }

            int currentIndex = 0;
            if (animator.animationName != null)
            {
                int index = 0;
                foreach (var animation in model.animations)
                {
                    if (animation.name == animator.animationName)
                    {
                        currentIndex = index + 1;
                        break;
                    }

                    ++index;
                }
            }

            int newIndex = EditorGUILayout.Popup("Animation", currentIndex, names);
            if (currentIndex != newIndex)
            {
                var animation = newIndex > 0 ? model.animations[newIndex - 1] : null;
                if (Application.isPlaying)
                {
                    if (animation != null)
                    {
                        animator.PlayAnimation(animation.name);
                    }
                    else
                    {
                        animator.StopAnimation();
                    }
                }
                else
                {
                    animator.sharedAnimation = animation;
                }
            }
        }

        if (model != null && model.materials.Length > 1)
        {
            string[] names = new string[model.materials.Length];
            for (int i = 0; i < names.Length; ++i)
            {
                names[i] = model.materials[i].name;
            }

            int currentIndex = 0;
            var currentSkin = animator.sharedSkin;
            if (currentSkin != null)
            {
                int index = 0;
                var name = currentSkin.name;
                if (Application.isPlaying && name.EndsWith(" (Instance)"))
                {
                    name = name.Substring(0, name.Length - " (Instance)".Length);
                }
                foreach (var skin in model.materials)
                {
                    if (skin.name == name)
                    {
                        currentIndex = index;
                        break;
                    }

                    ++index;
                }
            }

            int newIndex = EditorGUILayout.Popup("Skin", currentIndex, names);
            if (currentIndex != newIndex)
            {
                animator.sharedSkin = model.materials[newIndex];
            }
        }

        if (animator.model != oldModel)
        {
            animator.RefreshModel();
        }
    }
}
