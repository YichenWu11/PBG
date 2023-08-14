using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InvisibleObject : MonoBehaviour
{
    public bool Visibility = false;

    private Renderer m_Renderer;

    private void Start()
    {
        m_Renderer = GetComponent<Renderer>();
    }

    private void Update()
    {
        m_Renderer.enabled = Visibility;
    }
}