using UnityEngine;

using System;
using System.Collections.Generic;

public class SceneEntities
{
    readonly List<GameObject> m_objects;
    readonly Dictionary<int, GameObject> m_targets;

    public SceneEntities()
    {
        m_objects = new List<GameObject>();
        m_targets = new Dictionary<int, GameObject>();
    }

    public GameObject this[int id]
    {
        get { return m_objects[id]; }
    }

    public void Add(entity_t entity, GameObject entityInstance)
    {
        m_objects.Add(entityInstance);

        var targetname = entity.targetname;
        if (targetname != -1)
        {
            if (m_targets.ContainsKey(targetname))
            {
                Debug.LogWarning("Duplicate target name: " + targetname + " " + m_targets[targetname]);
            }
            m_targets[targetname] = entityInstance;
        }
    }

    public EntityTargetName FindTargetName(int id)
    {
        GameObject target;
        if (m_targets.TryGetValue(id, out target))
        {
            return target.GetComponent<EntityTargetName>();
        }

        return null;
    }
}
