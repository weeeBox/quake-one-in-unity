using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour
{
    [SerializeField]
    float m_lifeTime = 1.5f;

	void Start ()
    {
        Destroy(gameObject, m_lifeTime);
	}
}
