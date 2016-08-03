using UnityEngine;
using UnityEditor;

using System;
using System.Collections.Generic;

[CustomEditor(typeof(Weapon))]
public class WeaponEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var weapon = target as Weapon;
        var oldType = weapon.weaponType;

        base.OnInspectorGUI();

        if (oldType != weapon.weaponType)
        {
            ChangeWeapon(weapon, weapon.weaponType);
        }
    }

    private void ChangeWeapon(Weapon weapon, WeaponType weaponType)
    {
        var animator = weapon.GetComponent<MDLAnimator>();
        var weaponInfo = weapon.weapons[(int)weaponType];
        animator.sharedModel = weaponInfo.model;
        animator.animation = weaponInfo.shotAnimation;
    }
}
