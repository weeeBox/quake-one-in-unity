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

    [SerializeField]
    public func_door linkedDoor;

    [HideInInspector]
    public Vector3 pos1;

    [HideInInspector]
    public Vector3 pos2;

    [SerializeField]
    public door_items items;

    [SerializeField]
    public float speed;

    [SerializeField]
    public float wait = -1;
    
    State m_state;
    
    Vector3 m_targetPos;
    Vector3 m_targetDir;

    float m_openedTime;

    protected override void OnStart()
    {
        base.OnStart();
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
        else if (m_state == State.Opened && this.wait != -1)
        {
            m_openedTime += Time.deltaTime;
            if (m_openedTime > this.wait)
            {
                m_openedTime = 0.0f;
                Close();
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
        if (this.CanBeOpenedOnTouch)
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

    #region Damage

    protected override void OnKill(int damage)
    {
        Open();
    }

    #endregion

    public bool CanBeOpenedOnTouch
    {
        get { return !HasTargetName() && this.health == 0; }
    }
}
