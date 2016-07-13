using System;
using UnityEngine;

/*
 * (0 0.5 0) (-4 -4 -4) (4 4 4)
 * Used as a positional target for spotlights, etc.
 */
public class info_null_t : entity_t
{
}

/*
 * (0 0.5 0) (-4 -4 -4) (4 4 4)
 * Used as a positional target for lightning.
 */
public class info_notnull_t : entity_t
{
}

[EntityGroup("Lights")]
public abstract class light_entity_t : entity_t
{
    protected int m_light;
}

/*
 * (0 1 0) (-8 -8 -8) (8 8 8) START_OFF
 * Non-displayed light.
 * Default light value is 300
 * Default style is 0
 * If targeted, it will toggle between on or off.
 */
public class light_t : light_entity_t
{   
}

/*
 * (0 1 0) (-8 -8 -8) (8 8 8) START_OFF
 * Non-displayed light.
 * Default light value is 300
 * Default style is 0
 * If targeted, it will toggle between on or off.
 * Makes steady fluorescent humming sound
 */
public class light_fluoro_t : light_entity_t
{
}

/*
 * (0 1 0) (-8 -8 -8) (8 8 8)
 * Non-displayed light.
 * Default light value is 300
 * Default style is 10
 * Makes sparking, broken fluorescent sound
 */
public class light_fluorospark_t : light_entity_t
{
}

/*
 * (0 1 0) (-8 -8 -8) (8 8 8)
 * Sphere globe light.
 * Default light value is 300
 * Default style is 0
 */
public class light_globe_t : light_entity_t
{
}

/*
 * (0 .5 0) (-10 -10 -20) (10 10 20)
 * Short wall torch
 * Default light value is 200
 * Default style is 0
 */
public class light_torch_small_walltorch_t : light_entity_t
{
}

/*
 * (0 1 0) (-10 -10 -12) (12 12 18)
 * Large yellow flame ball
 */
public class light_flame_large_yellow_t : light_entity_t
{
}

/*
 * (0 1 0) (-8 -8 -8) (8 8 8) START_OFF
 * Small yellow flame ball
 */
public class light_flame_small_yellow_t : light_entity_t
{
}

/*
 * (0 1 0) (-10 -10 -40) (10 10 40) START_OFF
 * Small white flame ball
 */
public class light_flame_small_white_t : light_entity_t
{
}

/*
 * (0 .5 .8) (-8 -8 -8) (8 8 8)
 * Lava Balls
 */
public class misc_fireball_t : entity_t
{
}

/*
 * (0 .5 .8) (0 0 0) (32 32 64)
 * TESTING THING
 */
public class misc_explobox_t : entity_t
{
}

/*
 * (0 .5 .8) (0 0 0) (32 32 64)
 * Smaller exploding box, REGISTERED ONLY
 */
public class misc_explobox2_t : entity_t
{
}

/*
 * (0 .5 .8) (-8 -8 -8) (8 8 8) superspike laser
 * When triggered, fires a spike in the direction set in QuakeEd.
 * Laser is only for REGISTERED.
 */
public class trap_spikeshooter_t : entity_t
{
}

/*
 * (0 .5 .8) (-8 -8 -8) (8 8 8) superspike laser
 * Continuously fires spikes.
 * "wait" time between spike (1.0 default)
 * "nextthink" delay before firing first spike, so multiple shooters can be stagered.
 */
public class trap_shooter_t : entity_t
{
}

/*
 * (0 .5 .8) (-8 -8 -8) (8 8 8)
 * 
 * testing air bubbles
 */
public class air_bubbles_t : entity_t
{
}

/*
 * (0 .5 .8) (-8 -8 -8) (8 8 8)
 * 
 * Just for the debugging level.  Don't use
 */
public class viewthing_t : entity_t
{
}

/*
 * (0 .5 .8) ?
 * This is just a solid wall if not inhibitted
 */
public class func_wall_t : solid_entity_t
{
}

/*
 * (0 .5 .8) ?
 * A simple entity that looks solid but lets you walk through it.
 */
public class func_illusionary_t : entity_t
{
}

/*
 * (0 .5 .8) ? E1 E2 E3 E4
 * This bmodel will appear if the episode has allready been completed, so players can't reenter it.
 */
public class func_episodegate_t : entity_t
{
}

/*
 * (0 .5 .8) ?
 * This bmodel appears unless players have all of the episode sigils.
 */
public class func_bossgate_t : solid_entity_t
{
}

/*
 * (0.3 0.1 0.6) (-10 -10 -8) (10 10 8)
 */
public class ambient_suck_wind_t : entity_t
{
}

/*
 * (0.3 0.1 0.6) (-10 -10 -8) (10 10 8)
 */
public class ambient_drone_t : entity_t
{
}

/*
 * (0.3 0.1 0.6) (-10 -10 -8) (10 10 8)
 */
public class ambient_flouro_buzz_t : entity_t
{
}

/*
 * (0.3 0.1 0.6) (-10 -10 -8) (10 10 8)
 */
public class ambient_drip_t : entity_t
{
}

/*
 * (0.3 0.1 0.6) (-10 -10 -8) (10 10 8)
 */
public class ambient_comp_hum_t : entity_t
{
}

/*
 * (0.3 0.1 0.6) (-10 -10 -8) (10 10 8)
 */
public class ambient_thunder_t : entity_t
{
}

/*
 * (0.3 0.1 0.6) (-10 -10 -8) (10 10 8)
 */
public class ambient_light_buzz_t : entity_t
{
}

/*
 * (0.3 0.1 0.6) (-10 -10 -8) (10 10 8)
 */
public class ambient_swamp1_t : entity_t
{
}

/*
 * (0.3 0.1 0.6) (-10 -10 -8) (10 10 8)
 */
public class ambient_swamp2_t : entity_t
{
}

/*
 * (1 0.5 0) (-10 -10 -10) (10 10 10)
 * 
 * For optimzation testing, starts a lot of sounds.
 */
public class misc_noisemaker_t : entity_t
{
}
