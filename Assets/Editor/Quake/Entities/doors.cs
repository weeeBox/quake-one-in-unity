using System;
using UnityEngine;

[EntityGroup("Doors")]
public abstract class door_entity_t : solid_entity_t
{
    protected const int DOOR_START_OPEN = 1;
    protected const int DOOR_DONT_LINK = 4;
    protected const int DOOR_GOLD_KEY = 8;
    protected const int DOOR_SILVER_KEY = 16;
    protected const int DOOR_TOGGLE = 32;

    public door_entity_t()
    {
        this.movable = true;
    }
}

/*
 * (0 .5 .8) ? START_OPEN x DOOR_DONT_LINK GOLD_KEY SILVER_KEY TOGGLE
 * if two doors touch, they are assumed to be connected and operate as a unit.
 * 
 * TOGGLE causes the door to wait in both the start and end states for a trigger event.
 * 
 * START_OPEN causes the door to move to its destination when spawned, and operate in reverse.  It is used to temporarily or permanently close off an area when triggered (not usefull for touch or takedamage doors).
 * 
 * Key doors are allways wait -1.
 * 
 * "message"       is printed when the door is touched if it is a trigger door and it hasn't been fired yet
 * "angle"         determines the opening direction
 * "targetname" if set, no touch field will be spawned and a remote button or trigger field activates the door.
 * "health"        if set, door must be shot open
 * "speed"         movement speed (100 default)
 * "wait"          wait before returning (3 default, -1 = never return)
 * "lip"           lip remaining at end of move (8 default)
 * "dmg"           damage to inflict when blocked (2 default)
 * "sounds"
 * 0)      no sound
 * 1)      stone
 * 2)      base
 * 3)      stone chain
 * 4)      screechy metal
 */
public class func_door_t : door_entity_t
{
    public func_door_t()
    {
        this.speed = 100;
        this.wait = 3;
        this.lip = 8;
        this.dmg = 2;
    }

    public override void SetupInstance(BSP bsp, entity entity, SceneEntities entities)
    {
        base.SetupInstance(bsp, entity, entities);

        var door = entity as func_door;

        SetupTrigger(door);
        SetupItems(door);
        SetupMovement(door);
    }

    void SetupTrigger(func_door door)
    {
        var collider = door.GetComponent<BoxCollider>();

        var colliderSize = this.size;
        colliderSize.x += 0.4f;
        colliderSize.z += 0.4f;

        collider.size = colliderSize;
    }

    void SetupItems(func_door door)
    {
        if ((spawnflags & DOOR_SILVER_KEY) != 0)
            door.items = door_items.IT_KEY1;
        if ((spawnflags & DOOR_GOLD_KEY) != 0)
            door.items = door_items.IT_KEY2;
    }

    void SetupMovement(func_door door)
    {
        Vector3 movedir;
        float amount;
        if (angle == -1) // moving up
        {
            movedir = Vector3.up;
            amount = this.size.y;
        }
        else if (angle == -2) // moving down
        {
            movedir = Vector2.down;
            amount = this.size.y;
        }
        else if (angle >= 0 && angle < 360)
        {
            movedir = Quaternion.AngleAxis(angle, Vector3.up) * Vector3.left;
            amount = this.size.z;
        }
        else
        {
            movedir = Vector3.zero;
            amount = 0.0f;
            Debug.LogError("Unexpected angle: " + angle);
        }
        door.pos1 = door.transform.position;
        door.pos2 = door.pos1 + movedir * (amount - BSP.Scale(lip));
    }

    public Vector3 movedir
    {
        get; protected set;
    }
}

/*
 * (0 .5 .8) ? open_once 1st_left 1st_down no_shoot always_shoot
 * Basic secret door. Slides back, then to the side. Angle determines direction.
 * wait  = # of seconds before coming back
 * 1st_left = 1st move is left of arrow
 * 1st_down = 1st move is down from arrow
 * always_shoot = even if targeted, keep shootable
 * t_width = override WIDTH to move back (or height if going down)
 * t_length = override LENGTH to move sideways
 * "dmg"           damage to inflict when blocked (2 default)
 * 
 * If a secret door has a targetname, it will only be opened by it's botton or trigger, not by damage.
 * "sounds"
 * 1) medieval
 * 2) metal
 * 3) base
 */
public class func_door_secret_t : door_entity_t
{
}
