using UnityEngine;
using UnityEditor;

using System;
using System.Collections.Generic;

[CustomEditor(typeof(Weapon))]
public class WeaponEditor : Editor
{
    static Dictionary<WeaponType, string> lookup;
    
    public override void OnInspectorGUI()
    {
        var weapon = target as Weapon;
        var oldType = weapon.weaponType;

        base.OnInspectorGUI();

        if (oldType != weapon.weaponType)
        {
            ChangeWeapon(weapon.weaponType);
        }
    }

    private void ChangeWeapon(WeaponType weaponType)
    {
        if (lookup == null)
        {
            lookup = new Dictionary<WeaponType, string>();
            lookup[WeaponType.Axe] = "v_axe";
            lookup[WeaponType.Lightning] = "v_light";
            lookup[WeaponType.Nailgun] = "v_nail";
            lookup[WeaponType.SuperNailgun] = "v_nail2";
            lookup[WeaponType.GrenadeLauncher] = "v_rock";
            lookup[WeaponType.RockerLauncher] = "v_rock2";
            lookup[WeaponType.Shotgun] = "v_shot";
            lookup[WeaponType.SuperShotgun] = "v_shot2";
        }

        string name = lookup[weaponType];
        ChangeWeapon(name);
    }

    private void ChangeWeapon(string name)
    {
        var weapon = target as Weapon;

        var modelPath = string.Format("Assets/Models/{0}/{0}.asset", name);
        var model = AssetDatabase.LoadAssetAtPath<MDL>(modelPath);
        if (model == null)
        {
            Debug.LogError("Can't load model: " + modelPath);
            return;
        }

        var meshRenderer = weapon.GetComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = model.material;

        var meshFilter = weapon.GetComponent<MeshFilter>();
        meshFilter.sharedMesh = model.mesh;
    }
}
