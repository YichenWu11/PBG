using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blur : MonoBehaviour
{
    public Material BlurMat;
    [Range(0f, 10f)] public float BlurSize = 1f;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        BlurMat.SetFloat("_BlurSize", BlurSize);
        Graphics.Blit(source, destination, BlurMat);
    }
}