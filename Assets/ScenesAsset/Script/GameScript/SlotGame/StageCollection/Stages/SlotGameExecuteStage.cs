using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class SlotGameExecuteStage : IStage
{
    #region Fields and Properties
    //Properties 
    private string Properties;
    private int num1, num2, num3;
    private int[] symbols = { 1, 2, 3, 4, 5, 6 };

    //UI
    string UITag;
    private TMP_Text statusText;
    //Core
    private TaskCompletionSource<bool> phaseCompletionSource;
    private Action currentValidationAction;
    private bool isWaiting = false;
    public delegate Task InputHandler();
    public InputHandler InputDelegate;
    #endregion

    #region Constructor
    public SlotGameExecuteStage()
    {
        //Init the variable here
        Properties = "TemplateStage Construct";
        UITag = "SlotGameUI";
    }
    #endregion

    #region Execute Phase Logic
    public async Task ExecuteAsync(SharedDataSO sharedData, UIComponentCollectionSO uiComponents)
    {
        if (!InitializeUI(uiComponents)) return;

        await ShowDialogAsync("Start Gambling!");

        // Phase 1: Start gambling
        num1 = symbols[UnityEngine.Random.Range(0, 6)];
        num2 = symbols[UnityEngine.Random.Range(0, 6)];
        num3 = symbols[UnityEngine.Random.Range(0, 6)];
        sharedData.SetInt("Reel1", num1);
        sharedData.SetInt("Reel2", num2);
        sharedData.SetInt("Reel3", num3);
        await EventSystem.Instance.TriggerCallBack<int, int>("Execute", "ReelRotation1", num1);
        await EventSystem.Instance.TriggerCallBack<int, int>("Execute", "ReelRotation2", num2);
        await EventSystem.Instance.TriggerCallBack<int, int>("Execute", "ReelRotation3", num3);

        await ShowDialogAsync("All executions are completed!");
        CleanupUI();
    }
    #endregion

    #region UI Management
    private bool InitializeUI(UIComponentCollectionSO uiComponents)
    {
        var canvas = uiComponents.FindCanvasByTag(UITag);
        if (canvas == null)
        {
            Debug.LogError("Canvas not found in the scene.");
            return false;
        }
        statusText = uiComponents.CreateUIComponent<TMP_Text>("GameStatusText", canvas.transform);
        //declare the UI varable & Create UIElement
        return true;
    }

    private void CleanupUI()
    {
        //GameObject.Destroy(UIElement.GameObject);
        GameObject.Destroy(statusText.gameObject);
    }
    #endregion

    #region Support Functions
    public async Task InputAsync()
    {
    }

    private async Task ShowDialogAsync(string text)
    {
        statusText.text = text;
        //your TextUI Element
        await Task.Delay(1000);
    }

    #endregion
}
