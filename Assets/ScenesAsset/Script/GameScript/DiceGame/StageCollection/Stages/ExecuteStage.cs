using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class ExecuteStage : IStage
{
    #region Fields and Properties
    //Properties 
    private string Properties;

    //UI
    string UITag = "DiceGameUI";
    private TMP_Text statusText;

    //Core
    private TaskCompletionSource<bool> phaseCompletionSource;
    private Action currentValidationAction;
    private bool isWaiting = false;
    public delegate Task InputHandler();
    public InputHandler InputDelegate;
    #endregion

    #region Constructor
    public ExecuteStage()
    {
        //Init the variable here
        Properties = "ExecuteStage Construct";
    }
    #endregion

    #region Execute Phase Logic
    public async Task ExecuteAsync(SharedDataSO sharedData, UIComponentCollectionSO uiComponents)
    {
        if (!InitializeUI(uiComponents)) return;

        RegisterButtonListeners();
        //await ShowDialogAsync(instructionMessage);

        // Phase 1: Input Bet Amount
        await ShowDialogAsync("Please Enter your Bet Amount:");
        currentValidationAction = () => ValidateInput();
        InputDelegate = Option;
        //InputDelegate = null;
        await WaitForPhaseCompletionAsync();

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
        //declare the UI varable & Create UIElement
        statusText = uiComponents.CreateUIComponent<TMP_Text>("GameStatusText", canvas.transform);
        return true;
    }

    private void CleanupUI()
    {
        GameObject.Destroy(statusText.gameObject);
    }
    #endregion

    #region Button Logic
    private void RegisterButtonListeners()
    {
        //confirmButton.onClick.RemoveAllListeners();
        //confirmButton.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        currentValidationAction?.Invoke();
    }
    #endregion

    #region Validation Logic
    private void ValidateInput()
    {
        phaseCompletionSource?.SetResult(true);
    }
    #endregion

    #region Support Functions
    private async Task WaitForPhaseCompletionAsync()
    {
        phaseCompletionSource = new TaskCompletionSource<bool>();
        await phaseCompletionSource.Task;
    }

    public async Task InputAsync()
    {
        if (InputDelegate != null)
            await InputDelegate();
    }

    private async Task ShowDialogAsync(string text)
    {
        //statusText.text = text;
        //your TextUI Element
        await Task.Delay(1000);
    }
    #endregion

    #region Custom InputAsync
    private Task Option()
    {
        if (isWaiting)
            return Task.CompletedTask;
        isWaiting = true;
        /*var tcs = EventSystem.Instance.WaitForCallBack("DiceGame", "Options");
        betOptions = (string)await tcs;*/
        isWaiting = false;
        return Task.CompletedTask;
    }
    #endregion
}
