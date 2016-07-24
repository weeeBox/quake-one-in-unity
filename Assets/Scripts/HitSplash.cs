using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class HitSplash : MonoBehaviour
{
    [SerializeField]
    int m_particleCount = 50;

    [SerializeField]
    float m_particleSize = 0.01f;

    [SerializeField]
    float m_spreadRadius = 1.0f;

    [SerializeField]
    float m_acc = -9.8f;

    [SerializeField]
    float m_velocity;

    [SerializeField]
    float m_lifetime = 0.5f;

    [SerializeField]
    Color[] m_colors = { Color.white };

    Mesh m_mesh;

    Vector3[] m_vertices;

    void Awake()
    {
        var meshFilter = GetComponent<MeshFilter>();

        m_mesh = meshFilter.mesh;
        m_mesh.MarkDynamic();

        m_vertices = new Vector3[m_particleCount * 4]; // each particle is a quad
        var triangles = new int[m_particleCount * 6];  // 2 trigs per particle (6 indices)
        var colors = new Color[m_vertices.Length];     // same as vertices

        float halfSize = 0.5f * m_particleSize;

        int vertexIndex = 0, trigIndex = 0;
        for (int particleIndex = 0; particleIndex < m_particleCount; ++particleIndex)
        {
            int v0 = vertexIndex++;
            int v1 = vertexIndex++;
            int v2 = vertexIndex++;
            int v3 = vertexIndex++;

            Vector3 pos = Random.insideUnitCircle * m_spreadRadius;
            m_vertices[v0] = new Vector3(pos.x - halfSize, pos.y + halfSize, 0.1f);
            m_vertices[v1] = new Vector3(pos.x - halfSize, pos.y - halfSize, 0.1f);
            m_vertices[v2] = new Vector3(pos.x + halfSize, pos.y - halfSize, 0.1f);
            m_vertices[v3] = new Vector3(pos.x + halfSize, pos.y + halfSize, 0.1f);

            triangles[trigIndex++] = v0;
            triangles[trigIndex++] = v1;
            triangles[trigIndex++] = v3;
            triangles[trigIndex++] = v1;
            triangles[trigIndex++] = v2;
            triangles[trigIndex++] = v3;

            int colorIndex = Random.Range(0, m_colors.Length);
            colors[v0] = colors[v1] = colors[v2] = colors[v3] = m_colors[colorIndex];
        }

        m_mesh.vertices = m_vertices;
        m_mesh.triangles = triangles;
        m_mesh.colors = colors;
        m_mesh.RecalculateNormals();
    }

    void Start()
    {
        Destroy(gameObject, m_lifetime);
    }

    void Update()
    {
        m_velocity += m_acc * Time.deltaTime;
        Vector3 offset = m_velocity * Time.deltaTime * Vector3.up;

        for (int i = 0; i < m_vertices.Length; ++i)
        {
            m_vertices[i] += offset;
        }
        m_mesh.vertices = m_vertices;
    }
}
