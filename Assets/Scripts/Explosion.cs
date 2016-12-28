using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ParticleSystem))]
public class Explosion : MonoBehaviour
{
    void Start ()
    {
        var particleSystem = GetComponent<ParticleSystem>();
        Destroy(gameObject, particleSystem.duration);
	}
}
