using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(air_bubbles))]
class air_bubbles_editor : EntityEditor<air_bubbles>
{
}

[CustomEditor(typeof(ambient_comp_hum))]
class ambient_comp_hum_editor : EntityEditor<ambient_comp_hum>
{
}

[CustomEditor(typeof(ambient_drip))]
class ambient_drip_editor : EntityEditor<ambient_drip>
{
}

[CustomEditor(typeof(ambient_drone))]
class ambient_drone_editor : EntityEditor<ambient_drone>
{
}

[CustomEditor(typeof(ambient_flouro_buzz))]
class ambient_flouro_buzz_editor : EntityEditor<ambient_flouro_buzz>
{
}

[CustomEditor(typeof(ambient_light_buzz))]
class ambient_light_buzz_editor : EntityEditor<ambient_light_buzz>
{
}

[CustomEditor(typeof(ambient_suck_wind))]
class ambient_suck_wind_editor : EntityEditor<ambient_suck_wind>
{
}

[CustomEditor(typeof(ambient_swamp1))]
class ambient_swamp1_editor : EntityEditor<ambient_swamp1>
{
}

[CustomEditor(typeof(ambient_swamp2))]
class ambient_swamp2_editor : EntityEditor<ambient_swamp2>
{
}

[CustomEditor(typeof(ambient_thunder))]
class ambient_thunder_editor : EntityEditor<ambient_thunder>
{
}

[CustomEditor(typeof(event_lightning))]
class event_lightning_editor : EntityEditor<event_lightning>
{
}

[CustomEditor(typeof(func_bossgate))]
class func_bossgate_editor : EntityEditor<func_bossgate>
{
}

[CustomEditor(typeof(func_button))]
class func_button_editor : EntityEditor<func_button>
{
}

[CustomEditor(typeof(func_door))]
class func_door_editor : EntityEditor<func_door>
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Open"))
        {
            func_door door = target as func_door;
            door.Open();
        }
        if (GUILayout.Button("Close"))
        {
            func_door door = target as func_door;
            door.Close();
        }
    }
}

[CustomEditor(typeof(func_door_secret))]
class func_door_secret_editor : EntityEditor<func_door_secret>
{
}

[CustomEditor(typeof(func_episodegate))]
class func_episodegate_editor : EntityEditor<func_episodegate>
{
}

[CustomEditor(typeof(func_illusionary))]
class func_illusionary_editor : EntityEditor<func_illusionary>
{
}

[CustomEditor(typeof(func_plat))]
class func_plat_editor : EntityEditor<func_plat>
{
}

[CustomEditor(typeof(func_train))]
class func_train_editor : EntityEditor<func_train>
{
}

[CustomEditor(typeof(func_wall))]
class func_wall_editor : EntityEditor<func_wall>
{
}

[CustomEditor(typeof(info_intermission))]
class info_intermission_editor : EntityEditor<info_intermission>
{
}

[CustomEditor(typeof(info_notnull))]
class info_notnull_editor : EntityEditor<info_notnull>
{
}

[CustomEditor(typeof(info_null))]
class info_null_editor : EntityEditor<info_null>
{
}

[CustomEditor(typeof(info_player_coop))]
class info_player_coop_editor : EntityEditor<info_player_coop>
{
}

[CustomEditor(typeof(info_player_deathmatch))]
class info_player_deathmatch_editor : EntityEditor<info_player_deathmatch>
{
}

[CustomEditor(typeof(info_player_start))]
class info_player_start_editor : EntityEditor<info_player_start>
{
}

[CustomEditor(typeof(info_player_start2))]
class info_player_start2_editor : EntityEditor<info_player_start2>
{
}

[CustomEditor(typeof(info_teleport_destination))]
class info_teleport_destination_editor : EntityEditor<info_teleport_destination>
{
}

[CustomEditor(typeof(item_armor1))]
class item_armor1_editor : EntityEditor<item_armor1>
{
}

[CustomEditor(typeof(item_armor2))]
class item_armor2_editor : EntityEditor<item_armor2>
{
}

[CustomEditor(typeof(item_armorInv))]
class item_armorInv_editor : EntityEditor<item_armorInv>
{
}

[CustomEditor(typeof(item_artifact_envirosuit))]
class item_artifact_envirosuit_editor : EntityEditor<item_artifact_envirosuit>
{
}

[CustomEditor(typeof(item_artifact_invisibility))]
class item_artifact_invisibility_editor : EntityEditor<item_artifact_invisibility>
{
}

[CustomEditor(typeof(item_artifact_invulnerability))]
class item_artifact_invulnerability_editor : EntityEditor<item_artifact_invulnerability>
{
}

[CustomEditor(typeof(item_artifact_super_damage))]
class item_artifact_super_damage_editor : EntityEditor<item_artifact_super_damage>
{
}

[CustomEditor(typeof(item_cells))]
class item_cells_editor : EntityEditor<item_cells>
{
}

[CustomEditor(typeof(item_health))]
class item_health_editor : EntityEditor<item_health>
{
}

[CustomEditor(typeof(item_key1))]
class item_key1_editor : EntityEditor<item_key1>
{
}

[CustomEditor(typeof(item_key2))]
class item_key2_editor : EntityEditor<item_key2>
{
}

[CustomEditor(typeof(item_rockets))]
class item_rockets_editor : EntityEditor<item_rockets>
{
}

[CustomEditor(typeof(item_shells))]
class item_shells_editor : EntityEditor<item_shells>
{
}

[CustomEditor(typeof(item_sigil))]
class item_sigil_editor : EntityEditor<item_sigil>
{
}

[CustomEditor(typeof(item_spikes))]
class item_spikes_editor : EntityEditor<item_spikes>
{
}

[CustomEditor(typeof(item_weapon))]
class item_weapon_editor : EntityEditor<item_weapon>
{
}

[CustomEditor(typeof(light))]
class light_editor : EntityEditor<light>
{
}

[CustomEditor(typeof(light_flame_large_yellow))]
class light_flame_large_yellow_editor : EntityEditor<light_flame_large_yellow>
{
}

[CustomEditor(typeof(light_flame_small_white))]
class light_flame_small_white_editor : EntityEditor<light_flame_small_white>
{
}

[CustomEditor(typeof(light_flame_small_yellow))]
class light_flame_small_yellow_editor : EntityEditor<light_flame_small_yellow>
{
}

[CustomEditor(typeof(light_fluoro))]
class light_fluoro_editor : EntityEditor<light_fluoro>
{
}

[CustomEditor(typeof(light_fluorospark))]
class light_fluorospark_editor : EntityEditor<light_fluorospark>
{
}

[CustomEditor(typeof(light_globe))]
class light_globe_editor : EntityEditor<light_globe>
{
}

[CustomEditor(typeof(light_torch_small_walltorch))]
class light_torch_small_walltorch_editor : EntityEditor<light_torch_small_walltorch>
{
}

[CustomEditor(typeof(misc_explobox))]
class misc_explobox_editor : EntityEditor<misc_explobox>
{
}

[CustomEditor(typeof(misc_explobox2))]
class misc_explobox2_editor : EntityEditor<misc_explobox2>
{
}

[CustomEditor(typeof(misc_fireball))]
class misc_fireball_editor : EntityEditor<misc_fireball>
{
}

[CustomEditor(typeof(misc_noisemaker))]
class misc_noisemaker_editor : EntityEditor<misc_noisemaker>
{
}

[CustomEditor(typeof(misc_teleporttrain))]
class misc_teleporttrain_editor : EntityEditor<misc_teleporttrain>
{
}

[CustomEditor(typeof(monster_army))]
class monster_army_editor : EntityEditor<monster_army>
{
}

[CustomEditor(typeof(monster_boss))]
class monster_boss_editor : EntityEditor<monster_boss>
{
}

[CustomEditor(typeof(monster_demon1))]
class monster_demon1_editor : EntityEditor<monster_demon1>
{
}

[CustomEditor(typeof(monster_dog))]
class monster_dog_editor : EntityEditor<monster_dog>
{
}

[CustomEditor(typeof(monster_enforcer))]
class monster_enforcer_editor : EntityEditor<monster_enforcer>
{
}

[CustomEditor(typeof(monster_fish))]
class monster_fish_editor : EntityEditor<monster_fish>
{
}

[CustomEditor(typeof(monster_hell_knight))]
class monster_hell_knight_editor : EntityEditor<monster_hell_knight>
{
}

[CustomEditor(typeof(monster_knight))]
class monster_knight_editor : EntityEditor<monster_knight>
{
}

[CustomEditor(typeof(monster_ogre))]
class monster_ogre_editor : EntityEditor<monster_ogre>
{
}

[CustomEditor(typeof(monster_oldone))]
class monster_oldone_editor : EntityEditor<monster_oldone>
{
}

[CustomEditor(typeof(monster_shalrath))]
class monster_shalrath_editor : EntityEditor<monster_shalrath>
{
}

[CustomEditor(typeof(monster_shambler))]
class monster_shambler_editor : EntityEditor<monster_shambler>
{
}

[CustomEditor(typeof(monster_tarbaby))]
class monster_tarbaby_editor : EntityEditor<monster_tarbaby>
{
}

[CustomEditor(typeof(monster_wizard))]
class monster_wizard_editor : EntityEditor<monster_wizard>
{
}

[CustomEditor(typeof(monster_zombie))]
class monster_zombie_editor : EntityEditor<monster_zombie>
{
}

[CustomEditor(typeof(noclass))]
class noclass_editor : EntityEditor<noclass>
{
}

[CustomEditor(typeof(path_corner))]
class path_corner_editor : EntityEditor<path_corner>
{
}

[CustomEditor(typeof(trap_shooter))]
class trap_shooter_editor : EntityEditor<trap_shooter>
{
}

[CustomEditor(typeof(trap_spikeshooter))]
class trap_spikeshooter_editor : EntityEditor<trap_spikeshooter>
{
}

[CustomEditor(typeof(trigger_changelevel))]
class trigger_changelevel_editor : EntityEditor<trigger_changelevel>
{
}

[CustomEditor(typeof(trigger_counter))]
class trigger_counter_editor : EntityEditor<trigger_counter>
{
}

[CustomEditor(typeof(trigger_hurt))]
class trigger_hurt_editor : EntityEditor<trigger_hurt>
{
}

[CustomEditor(typeof(trigger_monsterjump))]
class trigger_monsterjump_editor : EntityEditor<trigger_monsterjump>
{
}

[CustomEditor(typeof(trigger_multiple))]
class trigger_multiple_editor : EntityEditor<trigger_multiple>
{
}

[CustomEditor(typeof(trigger_once))]
class trigger_once_editor : EntityEditor<trigger_once>
{
}

[CustomEditor(typeof(trigger_onlyregistered))]
class trigger_onlyregistered_editor : EntityEditor<trigger_onlyregistered>
{
}

[CustomEditor(typeof(trigger_push))]
class trigger_push_editor : EntityEditor<trigger_push>
{
}

[CustomEditor(typeof(trigger_relay))]
class trigger_relay_editor : EntityEditor<trigger_relay>
{
}

[CustomEditor(typeof(trigger_secret))]
class trigger_secret_editor : EntityEditor<trigger_secret>
{
}

[CustomEditor(typeof(trigger_setskill))]
class trigger_setskill_editor : EntityEditor<trigger_setskill>
{
}

[CustomEditor(typeof(trigger_teleport))]
class trigger_teleport_editor : EntityEditor<trigger_teleport>
{
}

[CustomEditor(typeof(viewthing))]
class viewthing_editor : EntityEditor<viewthing>
{
}

[CustomEditor(typeof(weapon_grenadelauncher))]
class weapon_grenadelauncher_editor : EntityEditor<weapon_grenadelauncher>
{
}

[CustomEditor(typeof(weapon_lightning))]
class weapon_lightning_editor : EntityEditor<weapon_lightning>
{
}

[CustomEditor(typeof(weapon_nailgun))]
class weapon_nailgun_editor : EntityEditor<weapon_nailgun>
{
}

[CustomEditor(typeof(weapon_rocketlauncher))]
class weapon_rocketlauncher_editor : EntityEditor<weapon_rocketlauncher>
{
}

[CustomEditor(typeof(weapon_supernailgun))]
class weapon_supernailgun_editor : EntityEditor<weapon_supernailgun>
{
}

[CustomEditor(typeof(weapon_supershotgun))]
class weapon_supershotgun_editor : EntityEditor<weapon_supershotgun>
{
}

[CustomEditor(typeof(worldspawn))]
class worldspawn_editor : EntityEditor<worldspawn>
{
}
