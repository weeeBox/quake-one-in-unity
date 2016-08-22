using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Particles : MonoBehaviour
{
    [SerializeField]
    protected int m_maxParticles = 100;

    [SerializeField]
    float m_particleRadius = 0.005f;

    Mesh m_mesh;
    
    protected Vector3[] m_vertices;
    protected int[] m_triangles;
    protected Color[] m_colors;
    protected int m_particleCount;
    bool m_dirty;

    protected virtual void Awake()
    {
        int vertexCount = 4 * m_maxParticles;
        m_vertices = new Vector3[vertexCount];
        m_colors = new Color[vertexCount];
        m_triangles = new int[6 * m_maxParticles];
        for (int ti = 0, vi = 0; ti < m_triangles.Length; ti += 6, vi += 4)
        {
            m_triangles[ti] = vi;
            m_triangles[ti + 1] = vi + 1;
            m_triangles[ti + 2] = vi + 3;
            m_triangles[ti + 3] = vi + 1;
            m_triangles[ti + 4] = vi + 2;
            m_triangles[ti + 5] = vi + 3;
        }

        m_mesh = new Mesh();
        m_mesh.vertices = m_vertices;
        m_mesh.triangles = m_triangles;
        m_mesh.MarkDynamic();

        var meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = m_mesh;
    }

    protected virtual void Update()
    {
        if (m_dirty)
        {
            m_dirty = false;
            m_mesh.vertices = m_vertices;
            m_mesh.colors = m_colors;
        }
    }

    protected int AddParticle(Vector3 pos, Color color)
    {
        int v1 = 4 * m_particleCount;
        int v2 = v1 + 1;
        int v3 = v1 + 2;
        int v4 = v1 + 3;
        m_vertices[v1] = pos + new Vector3(-m_particleRadius, -m_particleRadius);
        m_vertices[v2] = pos + new Vector3(-m_particleRadius, m_particleRadius);
        m_vertices[v3] = pos + new Vector3(m_particleRadius, m_particleRadius);
        m_vertices[v4] = pos + new Vector3(m_particleRadius, -m_particleRadius);
        m_colors[v1] = m_colors[v2] = m_colors[v3] = m_colors[v4] = color;
        
        m_dirty = true;
        ++m_particleCount;

        return m_particleCount;
    }
    
    protected void MoveParticle(int particle, Vector3 pos)
    {
        int index = 4 * particle;
        m_vertices[index] += pos;
        m_vertices[index + 1] += pos;
        m_vertices[index + 2] += pos;
        m_vertices[index + 3] += pos;
        m_dirty = true;
    }

    protected void SetColor(int particle, Color color)
    {
        int index = 4 * particle;
        m_colors[index] = color;
        m_dirty = true;
    }
}
