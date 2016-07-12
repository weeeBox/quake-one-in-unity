using System;

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
public class func_door_t : solid_entity_t
{
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
public class func_door_secret_t : solid_entity_t
{
}
