using System;

/*
 * (0 0 0) (-8 -8 -8) (8 8 8)
 * prints a warning message when spawned
 */
public class noclass : entity_t
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
public class item_health : entity_t
{
}

/*
 * (0 .5 .8) (-16 -16 0) (16 16 32)
 */
public class item_armor1 : entity_t
{
}

/*
 * (0 .5 .8) (-16 -16 0) (16 16 32)
 */
public class item_armor2 : entity_t
{
}

/*
 * (0 .5 .8) (-16 -16 0) (16 16 32)
 */
public class item_armorInv : entity_t
{
}

/*
 * (0 .5 .8) (-16 -16 0) (16 16 32)
 */
public class weapon_supershotgun : entity_t
{
}

/*
 * (0 .5 .8) (-16 -16 0) (16 16 32)
 */
public class weapon_nailgun : entity_t
{
}

/*
 * (0 .5 .8) (-16 -16 0) (16 16 32)
 */
public class weapon_supernailgun : entity_t
{
}

/*
 * (0 .5 .8) (-16 -16 0) (16 16 32)
 */
public class weapon_grenadelauncher : entity_t
{
}

/*
 * (0 .5 .8) (-16 -16 0) (16 16 32)
 */
public class weapon_rocketlauncher : entity_t
{
}

/*
 * (0 .5 .8) (-16 -16 0) (16 16 32)
 */
public class weapon_lightning : entity_t
{
}

/*
 * (0 .5 .8) (0 0 0) (32 32 32) big
 */
public class item_shells : entity_t
{
}

/*
 * (0 .5 .8) (0 0 0) (32 32 32) big
 */
public class item_spikes : entity_t
{
}

/*
 * (0 .5 .8) (0 0 0) (32 32 32) big
 */
public class item_rockets : entity_t
{
}

/*
 * (0 .5 .8) (0 0 0) (32 32 32) big
 */
public class item_cells : entity_t
{
}

/*
 * (0 .5 .8) (0 0 0) (32 32 32) shotgun rocket spikes big
 * DO NOT USE THIS!!!! IT WILL BE REMOVED!
 */
public class item_weapon : entity_t
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
public class item_key1 : entity_t
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
public class item_key2 : entity_t
{
}

/*
 * (0 .5 .8) (-16 -16 -24) (16 16 32) E1 E2 E3 E4
 * End of level sigil, pick up to end episode and return to jrstart.
 */
public class item_sigil : entity_t
{
}

/*
 * (0 .5 .8) (-16 -16 -24) (16 16 32)
 * Player is invulnerable for 30 seconds
 */
public class item_artifact_invulnerability : entity_t
{
}

/*
 * (0 .5 .8) (-16 -16 -24) (16 16 32)
 * Player takes no damage from water or slime for 30 seconds
 */
public class item_artifact_envirosuit : entity_t
{
}

/*
 * (0 .5 .8) (-16 -16 -24) (16 16 32)
 * Player is invisible for 30 seconds
 */
public class item_artifact_invisibility : entity_t
{
}

/*
 * (0 .5 .8) (-16 -16 -24) (16 16 32)
 * The next attack from the player will do 4x damage
 */
public class item_artifact_super_damage : entity_t
{
}
