using UnityEngine;
using System.Collections;

public class DelayedDestroy : MonoBehaviour {

    [SerializeField]
    float m_delay = 1.0f;

	void Start ()
    {
        Destroy(gameObject, m_delay);
	}
}
