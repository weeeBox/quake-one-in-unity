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
    protected int m_speed; // How fast the model is moved
    protected int m_wait;  // How long a pause between completion of movement and return to the original position (in seconds I think)
    protected int m_lip;   // Seems to be a means of adjusting the starting position of model
    protected int m_dmg;   // How much damage the model causes when it shuts on you?
    protected int m_health;

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

public class worldspawn : entity_t
{
    protected int m_worldtype;

    public int worldtype
    {
        get { return m_worldtype; }
    }
}