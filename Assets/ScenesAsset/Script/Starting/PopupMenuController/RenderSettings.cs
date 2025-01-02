using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class RenderSettings : MonoBehaviour
{
    private static RenderSettings _instance;
    public static RenderSettings Instance;

    // Settings Properties
    [SerializeField]
    public bool GI = false;
    public bool SSR = false;
    public bool SSAO = false;
    public bool PostProcess = false;

    public UniversalRenderPipelineAsset urpAsset;
    public UniversalRendererData rendererData;
    public PostProcessData postProcessData;
    // Tracking
    private List<ScriptableRendererFeature> m_DisabledRenderFeatures = new List<ScriptableRendererFeature>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        Debug.Log($"GI:{GI}");
        Debug.Log($"SSR :{SSR}");
        Debug.Log($"SSAO:{SSAO}");
        Debug.Log($"PPC :{PostProcess}");
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
        ToggleRenderFeature("Lim SSR", SSR);
        ForceRenderPipelineUpdate();
    }

    public void ChangeSSAO(bool p_Option)
    {
        SSAO = p_Option;
        ToggleRenderFeature("Screen Space Ambient Occlusion", SSAO);
        ForceRenderPipelineUpdate();
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
            Debug.LogError("Renderer Data or Renderer Features is Null！");
            return;
        }

        foreach (var feature in rendererData.rendererFeatures)
        {
            if (feature != null && feature.name == featureName)
            {
                feature.SetActive(enable);
                Debug.Log($"{featureName} {(enable ? "Enable" : "Disable")}");
                return;
            }
        }
        Debug.LogWarning($"Renderer Feature {featureName} unFind！");
    }

    private void DisableRenderFeature<T>() where T : ScriptableRendererFeature
    {
        if (rendererData == null || rendererData.rendererFeatures == null)
        {
            Debug.LogError("Renderer Data or Renderer Features is Null！");
            return;
        }

        foreach (var feature in rendererData.rendererFeatures)
        {
            if (feature is T)
            {
                feature.SetActive(false);
                m_DisabledRenderFeatures.Add(feature);
                Debug.Log($"Renderer Feature {typeof(T).Name} Disable");
                return;
            }
        }
        Debug.LogWarning($"Renderer Feature {typeof(T).Name} unFind！");
    }

    [System.Obsolete]
    private void ForceRenderPipelineUpdate()
    {       
        var currentPipeline = GraphicsSettings.renderPipelineAsset;
        GraphicsSettings.renderPipelineAsset = null;
        GraphicsSettings.renderPipelineAsset = currentPipeline;

        Debug.Log("渲染管線已重新初始化");
    }
    private void RefreshRendererData()
    {
        if (rendererData != null)
        {
            rendererData.SetDirty();
            Debug.Log("Renderer Data 已刷新");
        }
    }
    #endregion
}
