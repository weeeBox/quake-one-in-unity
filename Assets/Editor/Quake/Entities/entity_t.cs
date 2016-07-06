using System;
using System.Collections.Generic;
using System.Text;

using UnityEngine;

public abstract class entity_t
{
    readonly string m_classname; // type of entity to be defined (mandatory)
    protected Vector3 m_origin; // coordinates of where it starts in space
    protected int m_angle; // direction it faces or moves (sometimes in degrees)
    [EntityFieldPrefix("t")] protected int m_target; // matches a targetname & would appear to work
    [EntityFieldPrefix("t")] protected int m_targetname; //  like a linedef tag
    protected string m_wad;
    protected int m_spawnflags; //Used to flag the definition of an object that is to be different than the default behavior or type for a classname. See item_health for a good example of this
    [EntityFieldPrefix("*")] protected int m_model;
    protected int m_style;
    protected int m_sounds;
    protected string m_message;

    public entity_t()
    {
        m_classname = GetType().Name;
    }

    public override string ToString()
    {
        return string.Format("[entity_t \"classname\"=\"{0}\" \"origin\"={1} {2}]", m_classname);
    }

    public string classname
    {
        get { return m_classname; }
    }

    public Vector3 origin
    {
        get { return m_origin; }
    }
}

public abstract class model_entity_t : entity_t
{
    protected int m_speed; // How fast the model is moved
    protected int m_wait;  // How long a pause between completion of movement and return to the original position (in seconds I think)
    protected int m_lip;   // Seems to be a means of adjusting the starting position of model
    protected int m_style; // I am guessing that it is like the spawnflag arg in that it may determine a different than default type.
    protected int m_dmg;   // How much damage the model causes when it shuts on you?
}

public class worldspawn : entity_t
{
    protected int m_worldtype;

    public int worldtype
    {
        get { return m_worldtype; }
    }
}

// player starting coordinates
public class info_player_start : entity_t { }

#region Lights

/*
 * Non-displayed light.
 * Default light value is 300
 * Default style is 0
 * If targeted, it will toggle between on or off.
 */
public class light : entity_t
{
    int m_light;

    public light()
    {
        m_light = 300;
        m_style = 0;
    }

    public int lightValue
    {
        get { return m_light; }
    }
}

/*
 * Non-displayed light.
 * Default light value is 300
 * Default style is 0
 * If targeted, it will toggle between on or off.
 * Makes steady fluorescent humming sound
 */ 
public class light_fluoro : light {}

/*
 * Non-displayed light.
 * Default light value is 300
 * Default style is 10
 * Makes sparking, broken fluorescent sound
 */ 
public class light_fluorospark : light
{
    public light_fluorospark()
    {
        m_style = 10;
    }
}

/*
 * Sphere globe light.
 * Default light value is 300
 * Default style is 0
 */ 
public class light_globe : light { }

#endregion

#region Monsters

public class monster_ogre : entity_t {}
public class monster_demon1 : entity_t {}
public class monster_shambler : entity_t {}
public class monster_knight : entity_t {}
public class monster_army : entity_t {}
public class monster_wizard : entity_t {}
public class monster_dog : entity_t {}
public class monster_zombie : entity_t {}
public class monster_boss : entity_t {}
public class monster_tarbaby : entity_t {}
public class monster_hell_knight : entity_t {}
public class monster_fish : entity_t {}
public class monster_shalrath : entity_t {}
public class monster_enforcer : entity_t {}
public class monster_oldone : entity_t {}
public class event_lightning : entity_t {}

#endregion

#region Triggers

/*
 * Variable sized trigger. Triggers once, then removes itself.  You must set the key "target" to the name of another object in the level that has a matching
 * "targetname".  If "health" is set, the trigger must be killed to activate.
 * If notouch is set, the trigger is only fired by other entities, not by touching.
 * if "killtarget" is set, any objects that have a matching "target" will be removed when the trigger is fired.
 * if "angle" is set, the trigger will only fire when someone is facing the direction of the angle.  Use "360" for an angle of 0.
 *     sounds
 *     1)  secret
 *     2)  beep beep
 *     3)  large switch
 *     4)
 *     set "message" to text string
 */ 
public class trigger_once : model_entity_t
{
    public trigger_once()
    {
        m_wait = -1;
    }
}

/*
 * secret counter trigger
 * sounds
 * 1)  secret
 * 2)  beep beep
 * 3)
 * 4)
 * set "message" to text string
*/
public class trigger_secret : model_entity_t
{
    public trigger_secret()
    {
        m_wait = -1;
        m_message = "You found a secret area!";
        m_sounds = 1;
    }
}

#endregion

// door
public class func_door : model_entity_t {}

// secret door
public class func_door_secret : model_entity_t {}

// button
public class func_button : model_entity_t {}

// lifts
public class func_plat : model_entity_t {}

// sliding platforms
public class func_train : model_entity_t {}

// teleporters that only show up in deathmatch (may work with other things but, no examples here)
public class func_dm_only : model_entity_t {}

// teleporter entrance (walk-over)
public class trigger_teleport : entity_t {}

// aultiple actions that are activated by a walk over rather than button type of switch.
public class trigger_multiple : entity_t
{
    int m_health;
}

// Illusion: visible, but cannot be touched.
public class func_illusionary : entity_t {}