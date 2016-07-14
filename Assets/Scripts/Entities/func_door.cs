using System;
using System.Collections;

using UnityEngine;

public enum door_items
{
    IT_KEY1 = 131072,
    IT_KEY2 = 262144
}

public class func_door : entity
{
    enum State
    {
        Closed,
        Opening,
        Opened,
        Closing
    }

    [HideInInspector]
    public Vector3 pos1;

    [HideInInspector]
    public Vector3 pos2;

    [HideInInspector]
    public door_items items;

    [HideInInspector]
    public float speed;

    State m_state;
    
    Vector3 m_targetPos;
    Vector3 m_targetDir;

    void Start()
    {
        m_state = State.Closed;
    }

    void Update()
    {
        if (m_state == State.Opening || m_state == State.Closing)
        {
            Vector3 remains = m_targetPos - transform.position;
            Vector3 offset = m_targetDir * speed * Time.deltaTime;

            if (remains.sqrMagnitude < offset.sqrMagnitude)
            {
                transform.position = m_targetPos;
                OnTargetPosReach();
            }
            else
            {
                transform.Translate(offset);
            }
        }
    }

    void MoveToTargetPos(Vector3 targetPos)
    {
        m_targetPos = targetPos;
        m_targetDir = (targetPos - transform.position).normalized;
    }

    void OnTargetPosReach()
    {
        if (m_state == State.Closing)
        {
            m_state = State.Closed;
        }
        else if (m_state == State.Opening)
        {
            m_state = State.Opened;
        }
    }

    protected override void OnSignal()
    {
        Toggle();
    }

    protected override void OnCharacterEnter(CharacterController character)
    {   
        if (!HasTargetName())
        {
            Open();
        }
    }

    public void Toggle()
    {
        if (m_state == State.Closed || m_state == State.Closing)
        {
            Open();
        }
        else
        {
            Close();
        }
    }

    public void Open()
    {
        if (m_state != State.Opened && m_state != State.Opening)
        {
            m_state = State.Opening;
            MoveToTargetPos(pos2);
        }
    }

    public void Close()
    {
        if (m_state != State.Closed && m_state != State.Closing)
        {
            m_state = State.Closing;
            MoveToTargetPos(pos1);
        }
    }
}
