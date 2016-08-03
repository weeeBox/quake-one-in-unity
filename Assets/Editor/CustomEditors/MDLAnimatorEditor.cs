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
