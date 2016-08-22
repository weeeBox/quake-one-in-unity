using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour {

    [SerializeField]
    float m_rotationSpeed;

	// Update is called once per frame
	void Update () {
        transform.Rotate(Vector3.up, m_rotationSpeed * Time.deltaTime);
	}
}
