using UnityEngine;

using System;
using System.Collections.Generic;

public class MDLAnimator : MonoBehaviour
{
    [SerializeField]
    MDL m_model;

    [SerializeField]
    [HideInInspector]
    MDLAnimation m_animation;

    [SerializeField]
    [HideInInspector]
    Material m_skin;

    [SerializeField]
    float m_frameTime = 0.01f;

    Dictionary<string, MDLAnimation> m_animationLookup;

    Mesh m_mesh;

    Vector3[] m_frameBlendVertices;
    float m_elaspedTime;

    int m_frameIndex;
    int m_nextFrameIndex;

    void Start()
    {
        var meshFilter = GetComponent<MeshFilter>();
        m_mesh = meshFilter.mesh;
        m_mesh.MarkDynamic();

        if (m_animation == null)
        {
            SetAnimation(m_model.animation); // if not sure - set the first animation
        }

        if (m_skin == null)
        {
            SetSkin(m_model.material);
        }
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
            }
        }
    }
    
    void Blend(int frameIndex1, int frameIndex2, float alpha)
    {
        var v1 = m_animation.frames[frameIndex1].vertices;
        var v2 = m_animation.frames[frameIndex2].vertices;

        for (int i = 0; i < m_frameBlendVertices.Length; ++i)
        {
            m_frameBlendVertices[i] = Vector3.Lerp(v1[i], v2[i], alpha);
        }

        m_mesh.vertices = m_frameBlendVertices;
    }

    void SetFrameIndex(int index)
    {
        m_elaspedTime = 0.0f;

        // indices
        m_frameIndex = index;
        m_nextFrameIndex = (index + 1) % m_animation.frameCount;

        // initialize blend vertices
        var vertices = m_animation.frames[index].vertices;
        m_mesh.vertices = vertices;
        if (m_frameBlendVertices == null || m_frameBlendVertices.Length != vertices.Length)
        {
            m_frameBlendVertices = new Vector3[vertices.Length];
        }
        Array.Copy(vertices, m_frameBlendVertices, vertices.Length);
    }

    #region Skins

    void SetSkin(Material skin)
    {
        var meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = skin;
    }

    #endregion

    #region Animations

    public void SetAnimation(string name)
    {
        if (this.animationName != name)
        {
            var animation = FindAnimation(name);
            if (animation == null)
            {
                Debug.LogError("Can't find animation: " + name);
                return;
            }
        }
    }

    void SetAnimation(MDLAnimation animation)
    {
        m_animation = animation;
        SetFrameIndex(0);
    }

    MDLAnimation FindAnimation(string name)
    {
        if (m_animationLookup == null)
        {
            m_animationLookup = new Dictionary<string, MDLAnimation>(m_model.animationCount);
            foreach (var anim in m_model.animations)
            {
                m_animationLookup[anim.name] = anim;
            }
        }

        MDLAnimation animation;
        if (m_animationLookup.TryGetValue(name, out animation))
        {
            return animation;
        }

        return null;
    }

    #endregion

    #region Editor helpers

    #if UNITY_EDITOR

    public void RefreshModel()
    {
        if (m_model != null)
        {
            SetMesh(m_model.mesh);
            SetAnimation(m_model.animation);
            SetSkin(m_model.material);
        }
    }

    void SetMesh(Mesh mesh)
    {
        var meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;
    }

#endif

    #endregion

    #region Properties

    public MDL model
    {
        get { return m_model; }
    }

    public new MDLAnimation animation
    {
        get { return m_animation; }
    }

    public string animationName
    {
        get { return m_animation != null ? m_animation.name : null; }
    }

    public Material skin
    {
        get { return m_skin; }
    }

    #endregion
}
