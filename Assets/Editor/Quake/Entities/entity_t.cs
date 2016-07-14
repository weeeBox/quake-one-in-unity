using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

using UnityEngine;

public abstract class entity_t
{
    [EntityFieldPrefix("*")]
    int m_model = -1;

    [EntityFieldPrefix("t")]
    int m_target = -1;

    [EntityFieldPrefix("t")]
    int m_targetname = -1;

    [BSPTransform]
    Vector3 m_origin;

    [BSPTransform]
    float m_speed;

    [BSPTransform]
    float m_lip;

    public entity_t()
    {
        this.classname = GetType().Name;
    }

    #region Game Object

    public virtual void SetupInstance(BSP bsp, entity entity, SceneEntities entities)
    {
        if (this.target != -1)
        {
            var targetName = entities.FindTargetName(this.target);
            if (targetName == null)
            {
                Debug.LogError("Can't find entity with target name: " + this.target);
            }

            var target = entity.gameObject.AddComponent<EntityTarget>();
            target.targetName = targetName;
        }

        var instanceFields = ReflectionUtils.ListFields(entity);
        var dataFields = ReflectionUtils.ListFields(this);

        foreach (var name in instanceFields.Keys)
        {
            FieldInfo dataField;
            if (dataFields.TryGetValue(name, out dataField))
            {
                object value = dataField.GetValue(this);
                if (dataField.GetCustomAttribute<BSPTransformAttribute>() != null)
                {
                    Type fieldType = dataField.FieldType;
                    if (fieldType == typeof(int))
                    {
                        value = BSP.Scale((int) value);
                    }
                    else if (fieldType == typeof(float))
                    {
                        value = BSP.Scale((float) value);
                    }
                    else if (fieldType == typeof(Vector3))
                    {
                        value = BSP.TransformVector((Vector3) value);
                    }
                    else
                    {
                        throw new NotImplementedException("Unexpected field type: " + fieldType);
                    }
                }

                FieldInfo instanceField = instanceFields[name];
                instanceField.SetValue(entity, value);
            }
        }
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
        get { return m_origin; }
        protected set { m_origin = value; }
    }

    public int model
    {
        get { return m_model; }
        protected set { m_model = value; }
    }

    public BSPModel modelRef
    {
        get; set;
    }

    public Vector3 size
    {
        get { return modelRef != null ? modelRef.boundbox.size : Vector3.zero; }
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

    public float speed
    {
        get { return m_speed; }
        protected set { m_speed = value; }
    }

    public int wait
    {
        get; protected set;
    }

    public float lip
    {
        get { return m_lip; }
        protected set { m_lip = value; }
    }

    public int dmg
    {
        get; protected set;
    }

    public int health
    {
        get; protected set;
    }

    public bool solid
    {
        get; protected set;
    }

    public bool movable
    {
        get; protected set;
    }

    public string data
    {
        get; set;
    }

#endregion
}

public abstract class solid_entity_t : entity_t
{
    public solid_entity_t()
    {
        this.solid = true;
    }
}
