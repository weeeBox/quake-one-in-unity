using System;

/*
 * (0 .5 .8) ?
 * When a button is touched, it moves some distance in the direction of it's angle, triggers all of it's targets, waits some time, then returns to it's original position where it can be triggered again.
 * 
 * "angle"		determines the opening direction
 * "target"	all entities with a matching targetname will be used
 * "speed"		override the default 40 speed
 * "wait"		override the default 1 second wait (-1 = never return)
 * "lip"		override the default 4 pixel lip remaining at end of move
 * "health"	if set, the button must be killed instead of touched
 * "sounds"
 * 0) steam metal
 * 1) wooden clunk
 * 2) metallic click
 * 3) in-out
 */
public class func_button_t : solid_entity_t {}
