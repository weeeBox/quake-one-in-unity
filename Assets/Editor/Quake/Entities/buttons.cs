using System;

using UnityEngine;

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
public class func_button_t : solid_entity_t
{
    public func_button_t()
    {
        this.movable = true;
        this.speed = 40;
        this.wait = 1;
        this.lip = 4;
    }

    public override void SetupInstance(BSP bsp, entity entity, SceneEntities entities)
    {
        base.SetupInstance(bsp, entity, entities);

        var collider = entity.GetComponent<BoxCollider>();

        var colliderSize = this.size;
        colliderSize.x += 0.4f;
        colliderSize.y += 0.4f;
        colliderSize.z += 0.4f;

        collider.size = colliderSize;
    }
}
