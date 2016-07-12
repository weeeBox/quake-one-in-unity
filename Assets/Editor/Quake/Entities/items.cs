using System;

[EntityGroup("Items")]
public abstract class item_entity_t : entity_t
{
}

[EntityGroup("Weapons")]
public abstract class weapon_entity_t : entity_t
{
}

/*
 * (0 0 0) (-8 -8 -8) (8 8 8)
 * prints a warning message when spawned
 */
public class noclass_t : entity_t
{
}

/*
 * (.3 .3 1) (0 0 0) (32 32 32) rotten megahealth
 * Health box. Normally gives 25 points.
 * Rotten box heals 5-10 points,
 * megahealth will add 100 health, then 
 * rot you down to your maximum health limit, 
 * one point per second.
 */
public class item_health_t : item_entity_t
{
}

/*
 * (0 .5 .8) (-16 -16 0) (16 16 32)
 */
public class item_armor1_t : item_entity_t
{
}

/*
 * (0 .5 .8) (-16 -16 0) (16 16 32)
 */
public class item_armor2_t : item_entity_t
{
}

/*
 * (0 .5 .8) (-16 -16 0) (16 16 32)
 */
public class item_armorInv_t : item_entity_t
{
}

/*
 * (0 .5 .8) (-16 -16 0) (16 16 32)
 */
public class weapon_supershotgun_t : weapon_entity_t
{
}

/*
 * (0 .5 .8) (-16 -16 0) (16 16 32)
 */
public class weapon_nailgun_t : weapon_entity_t
{
}

/*
 * (0 .5 .8) (-16 -16 0) (16 16 32)
 */
public class weapon_supernailgun_t : weapon_entity_t
{
}

/*
 * (0 .5 .8) (-16 -16 0) (16 16 32)
 */
public class weapon_grenadelauncher_t : weapon_entity_t
{
}

/*
 * (0 .5 .8) (-16 -16 0) (16 16 32)
 */
public class weapon_rocketlauncher_t : weapon_entity_t
{
}

/*
 * (0 .5 .8) (-16 -16 0) (16 16 32)
 */
public class weapon_lightning_t : weapon_entity_t
{
}

/*
 * (0 .5 .8) (0 0 0) (32 32 32) big
 */
public class item_shells_t : entity_t
{
}

/*
 * (0 .5 .8) (0 0 0) (32 32 32) big
 */
public class item_spikes_t : entity_t
{
}

/*
 * (0 .5 .8) (0 0 0) (32 32 32) big
 */
public class item_rockets_t : entity_t
{
}

/*
 * (0 .5 .8) (0 0 0) (32 32 32) big
 */
public class item_cells_t : entity_t
{
}

/*
 * (0 .5 .8) (0 0 0) (32 32 32) shotgun rocket spikes big
 * DO NOT USE THIS!!!! IT WILL BE REMOVED!
 */
public class item_weapon_t : entity_t
{
}

/*
 * (0 .5 .8) (-16 -16 -24) (16 16 32)
 * SILVER key
 * In order for keys to work
 * you MUST set your maps
 * worldtype to one of the
 * following:
 * 0: medieval
 * 1: metal
 * 2: base
 */
public class item_key1_t : entity_t
{
}

/*
 * (0 .5 .8) (-16 -16 -24) (16 16 32)
 * GOLD key
 * In order for keys to work
 * you MUST set your maps
 * worldtype to one of the
 * following:
 * 0: medieval
 * 1: metal
 * 2: base
 */
public class item_key2_t : entity_t
{
}

/*
 * (0 .5 .8) (-16 -16 -24) (16 16 32) E1 E2 E3 E4
 * End of level sigil, pick up to end episode and return to jrstart.
 */
public class item_sigil_t : entity_t
{
}

/*
 * (0 .5 .8) (-16 -16 -24) (16 16 32)
 * Player is invulnerable for 30 seconds
 */
public class item_artifact_invulnerability_t : entity_t
{
}

/*
 * (0 .5 .8) (-16 -16 -24) (16 16 32)
 * Player takes no damage from water or slime for 30 seconds
 */
public class item_artifact_envirosuit_t : entity_t
{
}

/*
 * (0 .5 .8) (-16 -16 -24) (16 16 32)
 * Player is invisible for 30 seconds
 */
public class item_artifact_invisibility_t : entity_t
{
}

/*
 * (0 .5 .8) (-16 -16 -24) (16 16 32)
 * The next attack from the player will do 4x damage
 */
public class item_artifact_super_damage_t : entity_t
{
}
