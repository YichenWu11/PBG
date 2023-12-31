using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteAlways]
[ImageEffectAllowedInSceneView]
public class Bloom : MonoBehaviour
{
    public enum BloomDebugFlag
    {
        None = 0,
        DownSample = 1,
        UpSample = 2
    }

    [Space(20)] public int downSampleStep = 7;

    [Range(3, 15)] public int downSampleBlurSize = 5;
    [Range(0.01f, 10.0f)] public float downSampleBlurSigma = 1.0f;

    [Range(3, 15)] public int upSampleBlurSize = 5;
    [Range(0.01f, 10.0f)] public float upSampleBlurSigma = 1.0f;

    [Space(20)] public bool useKarisAverage = false;
    [Range(0.001f, 10.0f)] public float luminanceThreshole = 1.0f;
    [Range(0.001f, 10.0f)] public float bloomIntensity = 1.0f;

    [Space(20)] public BloomDebugFlag debugFlag;
    [Range(0, 6)] public int mipDebugIndex = 0;

    // Start is called before the first frame update
    private void Start()
    {
        GetComponent<Camera>().allowHDR = true;
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void OnPreRender()
    {
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Shader.SetGlobalInt("_downSampleBlurSize", downSampleBlurSize);
        Shader.SetGlobalFloat("_downSampleBlurSigma", downSampleBlurSigma);
        Shader.SetGlobalInt("_upSampleBlurSize", upSampleBlurSize);
        Shader.SetGlobalFloat("_upSampleBlurSigma", upSampleBlurSigma);

        Shader.SetGlobalFloat("_luminanceThreshole", luminanceThreshole);
        Shader.SetGlobalFloat("_bloomIntensity", bloomIntensity);


        var RT_threshold = RenderTexture.GetTemporary(
            Screen.width, Screen.height, 0, RenderTextureFormat.ARGBFloat,
            RenderTextureReadWrite.Linear);
        RT_threshold.filterMode = FilterMode.Bilinear;
        Graphics.Blit(source, RT_threshold, new Material(Shader.Find("Shaders/threshold")));

        var N = downSampleStep;
        var downSize = 2;
        var RT_BloomDown = new RenderTexture[N];

        for (var i = 0; i < N; i++)
        {
            var w = Screen.width / downSize;
            var h = Screen.height / downSize;
            RT_BloomDown[i] =
                RenderTexture.GetTemporary(w, h, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            RT_BloomDown[i].filterMode = FilterMode.Bilinear;
            downSize *= 2;
        }

        // down sample
        Graphics.Blit(RT_threshold, RT_BloomDown[0],
            new Material(Shader.Find(useKarisAverage ? "Shaders/firstDownSample" : "Shaders/downSample")));
        for (var i = 1; i < N; i++)
            Graphics.Blit(RT_BloomDown[i - 1], RT_BloomDown[i], new Material(Shader.Find("Shaders/downSample")));


        var RT_BloomUp = new RenderTexture[N];
        for (var i = 0; i < N - 1; i++)
        {
            var w = RT_BloomDown[N - 2 - i].width;
            var h = RT_BloomDown[N - 2 - i].height;
            RT_BloomUp[i] =
                RenderTexture.GetTemporary(w, h, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            RT_BloomUp[i].filterMode = FilterMode.Bilinear; // 启用双线性滤波
        }

        // up sample : RT_BloomUp[i] = Blur(RT_BloomDown[N-2-i]) + RT_BloomUp[i-1]
        Shader.SetGlobalTexture("_PrevMip", RT_BloomDown[N - 1]);
        Graphics.Blit(RT_BloomDown[N - 2], RT_BloomUp[0], new Material(Shader.Find("Shaders/upSample")));
        for (var i = 1; i < N - 1; i++)
        {
            var prev_mip = RT_BloomUp[i - 1];
            var curr_mip = RT_BloomDown[N - 2 - i];
            Shader.SetGlobalTexture("_PrevMip", prev_mip);
            Graphics.Blit(curr_mip, RT_BloomUp[i], new Material(Shader.Find("Shaders/upSample")));
        }


        // pass to shader
        Shader.SetGlobalTexture("_BloomTex", RT_BloomUp[N - 2]);

        // output
        if (debugFlag == BloomDebugFlag.None)
            Graphics.Blit(source, destination, new Material(Shader.Find("Shaders/post")));
        else if (debugFlag == BloomDebugFlag.DownSample)
            Graphics.Blit(RT_BloomDown[mipDebugIndex], destination, new Material(Shader.Find("Shaders/postDebug")));
        else if (debugFlag == BloomDebugFlag.UpSample)
            Graphics.Blit(RT_BloomUp[mipDebugIndex], destination, new Material(Shader.Find("Shaders/postDebug")));


        for (var i = 0; i < N; i++)
        {
            RenderTexture.ReleaseTemporary(RT_BloomDown[i]);
            RenderTexture.ReleaseTemporary(RT_BloomUp[i]);
        }

        RenderTexture.ReleaseTemporary(RT_threshold);
    }
}