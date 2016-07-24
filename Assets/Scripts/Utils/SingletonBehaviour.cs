using UnityEngine;
using System.Collections;

public class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    static SingletonBehaviour<T> s_instance;

    #region Lifecycle

    void Awake()
    {
        if (s_instance == null)
        {
            s_instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (s_instance != this)
        {
            Destroy(gameObject);
        }
    }

    #endregion

    #region Properties

    protected static T instance
    {
        get { return s_instance as T; }
    }

    #endregion
}
