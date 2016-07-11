using System;

/*
 * (0 .5 .8) ? PLAT_LOW_TRIGGER
 * speed	default 150
 * 
 * Plats are always drawn in the extended position, so they will light correctly.
 * 
 * If the plat is the target of another trigger or button, it will start out disabled in the extended position until it is trigger, when it will lower and become a normal plat.
 * 
 * If the "height" key is set, that will determine the amount the plat moves, instead of being implicitly determined by the model's height.
 * Set "sounds" to one of the following:
 * 1) base fast
 * 2) chain slow
 */
public class func_plat_t : entity_t
{
    protected int m_height;
}

/*
 * (0 .5 .8) ?
 * Trains are moving platforms that players can ride.
 * The targets origin specifies the min point of the train at each corner.
 * The train spawns at the first target it is pointing at.
 * If the train is the target of a button or trigger, it will not begin moving until activated.
 * speed	default 100
 * dmg		default	2
 * sounds
 * 1) ratchet metal
 * 
 */
public class func_train_t : entity_t
{
}

/*
 * (0 .5 .8) (-8 -8 -8) (8 8 8)
 * This is used for the final bos
 */
public class misc_teleporttrain_t : entity_t
{
}
