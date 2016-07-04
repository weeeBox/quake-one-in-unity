using UnityEngine;
using System.Collections;

public class TempTrig : MonoBehaviour
{
    public Vector3 v1;
    public Vector3 v2;
    public Vector3 v3;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawLine(v1, v2);
        Gizmos.DrawLine(v2, v3);
        Gizmos.DrawLine(v3, v1);
    }
}
