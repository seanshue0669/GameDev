using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class RenderSettings : MonoBehaviour
{
    private static RenderSettings instance;

    public static RenderSettings Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new RenderSettings();
            }
            return instance;
        }
    }

    // Settings Properties
    [SerializeField]
    public bool GI = false;
    public bool SSR = false;
    public bool SSAO = false;
    public bool PostProcess = false;

    public UniversalRenderPipelineAsset urpAsset;
    public UniversalRendererData rendererData;
    public PostProcessData postProcessData;

    void Awake()
    {
        Debug.Log($"GI: {GI}");
        Debug.Log($"SSR: {SSR}");
        Debug.Log($"SSAO: {SSAO}");
        Debug.Log($"PPC: {PostProcess}");
    }

    #region Call Methods

    public void ChangeGI(bool p_Option)
    {
        GI = p_Option;
        Debug.Log($"Global Illumination changed to: {GI}");
    }

    public void ChangeSSR(bool p_Option)
    {
        SSR = p_Option;
        ToggleRenderFeature("LimSSR", SSR);
        ToggleRenderFeature("DepthPyramid", SSR);
        RefreshRendererData();
    }

    public void ChangeSSAO(bool p_Option)
    {
        SSAO = p_Option;
        ToggleRenderFeature("ScreenSpaceAmbientOcclusion", SSAO);
        RefreshRendererData();
    }

    public void ChangePPC(bool p_Option)
    {
        PostProcess = p_Option;
        rendererData.postProcessData = PostProcess ? postProcessData : null;
        Debug.Log($"Post-processing changed to: {PostProcess}");
        RefreshRendererData();
    }

    private void ToggleRenderFeature(string featureName, bool enable)
    {
        if (rendererData == null || rendererData.rendererFeatures == null)
        {
            Debug.LogError("Renderer Data or Renderer Features is Null!");
            return;
        }

        foreach (var feature in rendererData.rendererFeatures)
        {
            if (feature != null && feature.name == featureName)
            {
                feature.SetActive(enable);
                Debug.Log($"{featureName} {(enable ? "Enabled" : "Disabled")}");
                return;
            }
        }
        Debug.LogWarning($"Renderer Feature {featureName} not found!");
    }

    private void RefreshRendererData()
    {
        if (rendererData != null)
        {
            rendererData.SetDirty();
        }
    }


    #endregion
}
