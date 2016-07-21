using UnityEngine;

using System;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MDLAnimator : MonoBehaviour
{
    [SerializeField]
    MDL m_model;

    [SerializeField]
    [HideInInspector]
    MDLAnimation m_animation;

    float m_frameTime = 1.0f / 10;

    Dictionary<string, MDLAnimation> m_animationLookup;

    MeshRenderer m_meshRenderer;
    Mesh m_mesh;

    Vector3[] m_initialVertices;
    Vector3[] m_frameBlendVertices;
    float m_elaspedTime;

    int m_frameIndex;
    int m_nextFrameIndex;
    bool m_animationFinished;

    void Awake()
    {
        var meshFilter = GetComponent<MeshFilter>();
        m_mesh = meshFilter.mesh;
        m_mesh.MarkDynamic();
        m_initialVertices = m_mesh.vertices;

        m_meshRenderer = GetComponent<MeshRenderer>();
    }

    void Start()
    {
        if (m_animation != null)
        {
            SetFrameIndex(0); // rewind to the first frame
        }
    }

    void Update()
    {
        if (m_animation != null && !m_animationFinished)
        {
            m_elaspedTime += Time.deltaTime;
            if (m_elaspedTime > m_frameTime)
            {
                int nextFrame = m_frameIndex + 1;
                if (nextFrame < m_animation.frameCount)
                {
                    SetFrameIndex(nextFrame);
                }
                else
                {
                    if (m_animation.type == MDLAnimationType.Looped)
                    {
                        SetFrameIndex(0);
                    }
                    else 
                    {
                        if (m_animation.type == MDLAnimationType.Rewind)
                        {
                            SetFrameIndex(0);
                        }

                        m_animationFinished = true;
                    }
                }
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

    public void PlayAnimation(string name)
    {
        var animation = FindAnimation(name);
        if (animation == null)
        {
            Debug.LogError("Can't find animation: " + name);
            return;
        }

        PlayAnimation(animation);
    }

    public void StopAnimation()
    {
        m_animation = null;
        m_mesh.vertices = m_initialVertices;
    }

    void PlayAnimation(MDLAnimation animation)
    {
        m_animation = animation;
        m_animationFinished = false;
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
            m_mesh = m_model.mesh;
            m_animation = m_model.animation;

            var meshFilter = GetComponent<MeshFilter>();
            meshFilter.sharedMesh = m_model.mesh;

            var meshRenderer = GetComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = m_model.material;
        }
        else
        {
            m_mesh = null;
            m_animation = null;

            var meshFilter = GetComponent<MeshFilter>();
            meshFilter.sharedMesh = null;

            var meshRenderer = GetComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = null;
        }
    }

    public MDLAnimation sharedAnimation
    {
        get { return this.animation; }
        set { m_animation = value; }
    }

    public Material sharedSkin
    {
        get
        {
            if (Application.isPlaying)
            {
                return this.skin;
            }

            var meshRenderer = GetComponent<MeshRenderer>();
            return meshRenderer.sharedMaterial;
        }
        set
        {
            if (Application.isPlaying)
            {
                this.skin = value;
            }
            else
            {
                var meshRenderer = GetComponent<MeshRenderer>();
                meshRenderer.sharedMaterial = value;
            }
        }
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
        get { return m_meshRenderer.material; }
        set { m_meshRenderer.material = value; }
    }

    #endregion
}
