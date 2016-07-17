using UnityEngine;
using System.Collections;

public class MDLAnimator : MonoBehaviour
{
    [SerializeField]
    MDL m_model;

    [SerializeField]
    float m_frameTime = 0.01f;

    [SerializeField]
    [HideInInspector]
    int m_skinIndex;

    [SerializeField]
    [HideInInspector]
    int m_animationIndex;
    
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
        if (currentAnimation != null)
        {
            m_elaspedTime += Time.deltaTime;
            if (m_elaspedTime < m_frameTime)
            {
                float alpha = m_elaspedTime / m_frameTime;
                Blend(m_frameIndex, m_nextFrameIndex, alpha);
            }
            else
            {
                SetFrameIndex((m_frameIndex + 1) % currentAnimation.frameCount);
                m_elaspedTime = 0.0f;
            }
        }
    }

    void Blend(int frameIndex1, int frameIndex2, float alpha)
    {
        var v1 = currentAnimation.frames[frameIndex1].vertices;
        var v2 = currentAnimation.frames[frameIndex2].vertices;

        for (int i = 0; i < m_frameVertices.Length; ++i)
        {
            m_frameVertices[i] = Vector3.Lerp(v1[i], v2[i], alpha);
        }

        m_mesh.vertices = m_frameVertices;
    }

    void SetFrameIndex(int index)
    {
        m_frameIndex = index;
        m_nextFrameIndex = (index + 1) % currentAnimation.frameCount;

        var frame = currentAnimation.frames[index];
        m_mesh.vertices = frame.vertices;
        m_frameVertices = m_mesh.vertices;
    }

    #region Properties

    MDLAnimation currentAnimation
    {
        get { return m_model.animations[m_animationIndex]; }
    }

    #endregion
}
