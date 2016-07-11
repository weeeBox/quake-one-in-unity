using System;

/*
 * (0 0 0) ?
 * Only used for the world entity.
 * Set message to the level name.
 * Set sounds to the cd track to play.
 * 
 * World Types:
 * 0: medieval
 * 1: metal
 * 2: base
 */
public class worldspawn_t : entity_t
{
    protected int m_worldtype;
}
