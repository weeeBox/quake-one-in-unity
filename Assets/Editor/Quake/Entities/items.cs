using System;

public abstract class ItemEntity : entity_t
{
    protected int m_health;
}

#region Health box

public class item_health : ItemEntity {}
public class item_megahealth_rot : ItemEntity {}

#endregion

#region Armor

public class item_armor1 : entity_t { }
public class item_armor2 : entity_t { }
public class item_armorInv : entity_t { }

#endregion

#region Weapons

public class weapon_supershotgun : entity_t { }
public class weapon_nailgun : entity_t { }
public class weapon_supernailgun : entity_t { }
public class weapon_grenadelauncher : entity_t { }
public class weapon_rocketlauncher : entity_t { }
public class weapon_lightning : entity_t { }

#endregion

#region Ammo

public class item_shells : entity_t {}
public class item_spikes : entity_t {}
public class item_rockets : entity_t {}
public class item_cells : entity_t {}

#endregion

#region Keys

/*
 * SILVER key
 * In order for keys to work
 * you MUST set your maps
 * worldtype to one of the
 * following:
 * 0: medieval
 * 1: metal
 * 2: base
 */ 
public class item_key1 : entity_t {}

/*
 * GOLD key
 * In order for keys to work
 * you MUST set your maps
 * worldtype to one of the
 * following:
 * 0: medieval
 * 1: metal
 * 2: base
 */ 
public class item_key2 : entity_t {}

#endregion

#region END OF LEVEL RUNES

/* End of level sigil, pick up to end episode and return to jrstart. */ 
public class item_sigil : entity_t { }

#endregion

#region Powerups

/* Player is invulnerable for 30 seconds */ 
public class item_artifact_invulnerability : entity_t {}

/* Player takes no damage from water or slime for 30 seconds */
public class item_artifact_envirosuit : entity_t {}

/* Player is invisible for 30 seconds */
public class item_artifact_invisibility : entity_t {}

/* The next attack from the player will do 4x damage */
public class item_artifact_super_damage : entity_t {}

#endregion