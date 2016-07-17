using UnityEngine;
using System.Collections;

public class MDLAnimationPlayer : MonoBehaviour
{
    [SerializeField]
    MDLAnimation m_animation;

    [SerializeField]
    float m_frameTime = 0.01f;

    Mesh m_mesh;

    Vector3[] m_frameVertices;
    float m_elaspedTime;

    int m_frameIndex;
    int m_nextFrameIndex;
    
    void Start()
    {
        var meshFilter = GetComponent<MeshFilter>();
        m_mesh = meshFilter.mesh;
        m_mesh.MarkDynamic();
        
        SetFrameIndex(0);
    }

    void Update()
    {
        if (m_animation != null)
        {
            m_elaspedTime += Time.deltaTime;
            if (m_elaspedTime < m_frameTime)
            {
                float alpha = m_elaspedTime / m_frameTime;
                Blend(m_frameIndex, m_nextFrameIndex, alpha);
            }
            else
            {
                SetFrameIndex((m_frameIndex + 1) % m_animation.frameCount);
                m_elaspedTime = 0.0f;
            }
        }
    }

    void Blend(int frameIndex1, int frameIndex2, float alpha)
    {
        var v1 = m_animation.frames[frameIndex1].vertices;
        var v2 = m_animation.frames[frameIndex2].vertices;

        for (int i = 0; i < m_frameVertices.Length; ++i)
        {
            m_frameVertices[i] = Vector3.Lerp(v1[i], v2[i], alpha);
        }

        m_mesh.vertices = m_frameVertices;
    }

    void SetFrameIndex(int index)
    {
        m_frameIndex = index;
        m_nextFrameIndex = (index + 1) % m_animation.frameCount;

        var frame = m_animation.frames[index];
        m_mesh.vertices = frame.vertices;
        m_frameVertices = m_mesh.vertices;
    }
}
