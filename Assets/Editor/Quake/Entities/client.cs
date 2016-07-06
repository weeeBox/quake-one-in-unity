using System;

using UnityEngine;

/*
 * (1 0.5 0.5) (-16 -16 -16) (16 16 16)
 * This is the camera point for the intermission.
 * Use mangle instead of angle, so you can set pitch or roll as well as yaw.  'pitch roll yaw'
 */
public class info_intermission : entity_t
{
    protected Vector3 m_mangle;
}

/*
 * (0.5 0.5 0.5) ? NO_INTERMISSION
 * When the player touches this, he gets sent to the map listed in the "map" variable.  Unless the NO_INTERMISSION flag is set, the view will go to the info_intermission spot and display stats.
 */
public class trigger_changelevel : entity_t
{
    protected string m_map;
}

/*
 * (1 0 0) (-16 -16 -24) (16 16 24)
 * The normal starting point for a level.
 */
public class info_player_start : entity_t
{
}

/*
 * (1 0 0) (-16 -16 -24) (16 16 24)
 * Only used on start map for the return point from an episode.
 */
public class info_player_start2 : entity_t
{
}

/*
 * (1 0 1) (-16 -16 -24) (16 16 24)
 * potential spawning position for deathmatch games
 */
public class info_player_deathmatch : entity_t
{
}

/*
 * (1 0 1) (-16 -16 -24) (16 16 24)
 * potential spawning position for coop games
 */
public class info_player_coop : entity_t
{
}
