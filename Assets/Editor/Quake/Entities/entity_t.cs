using System;
using System.Collections.Generic;
using System.Text;

using UnityEngine;

public abstract class entity_t
{
    [EntityFieldPrefix("*")]
    private int m_model = -1;

    [EntityFieldPrefix("t")]
    private int m_target = -1;

    [EntityFieldPrefix("t")]
    private int m_targetname = -1;

    public entity_t()
    {
        this.classname = GetType().Name;
    }

    #region Game Object

    public virtual entity CreateGameObject(BSP bsp)
    {
        Debug.LogWarning("Can't create entity: " + this.classname);
        return null;
    }

    #endregion

    #region String representation

    public override string ToString()
    {
        return string.Format("[entity_t \"classname\"=\"{0}\" \"origin\"={1} {2}]", this.classname);
    }

    #endregion

    #region Properties

    public string classname
    {
        get; private set;
    }

    public Vector3 origin
    {
        get; protected set;
    }

    public int model
    {
        get { return m_model; }
        protected set { m_model = value; }
    }

    public int target
    {
        get { return m_target; }
        protected set { m_target = value; }
    }

    public int targetname
    {
        get { return m_targetname; }
        protected set { m_targetname = value; }
    }

    public int angle
    {
        get; protected set;
    }

    public string wad
    {
        get; protected set;
    }

    public int spawnflags
    {
        get; protected set;
    }

    public int style
    {
        get; protected set;
    }

    public int sounds
    {
        get; protected set;
    }

    public string message
    {
        get; protected set;
    }

    public int speed
    {
        get; protected set;
    }

    public int wait
    {
        get; protected set;
    }

    public int lip
    {
        get; protected set;
    }

    public int dmg
    {
        get; protected set;
    }

    public int health
    {
        get; protected set;
    }

    #endregion
}

public abstract class entity_t<T> : entity_t where T : entity
{
    public override entity CreateGameObject(BSP bsp)
    {
        GameObject obj = new GameObject(this.classname);
        T entity = obj.AddComponent<T>();
        SetupEntity(bsp, entity);

        Vector3 position;
        if (this.model != -1)
        {
            var model = bsp.FindModel(this.model);
            position = model.origin;
        }
        else
        {
            position = this.origin;
        }
        entity.transform.position = BSP.TransformVertex(position);

        return entity;
    }

    protected virtual void SetupEntity(BSP bsp, T entity)
    {
    }
}
