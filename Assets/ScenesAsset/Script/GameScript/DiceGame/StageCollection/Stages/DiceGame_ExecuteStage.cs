using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class DiceGame_ExecuteStage : IStage
{
    #region Fields and Properties
    //Properties 

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
    public DiceGame_ExecuteStage()
    {
        //Init the variable here
    }
    #endregion

    #region Execute Phase Logic
    public async Task ExecuteAsync(SharedDataSO sharedData, UIComponentCollectionSO uiComponents)
    {
        if (!InitializeUI(uiComponents)) return;

        RegisterButtonListeners();
        //await ShowDialogAsync(instructionMessage);

        // Phase 1: Input Bet Amount
        await ShowDialogAsync("Rolliing!!");
        currentValidationAction = () => ValidateInput();
        InputDelegate = null;

        //Generate Dice Reault
        int diceOneResult = UnityEngine.Random.Range(0,8);
        int diceTwoResult = UnityEngine.Random.Range(0,8);
        string passingOperation = diceOneResult.ToString()+" "+diceTwoResult.ToString();
        //>! should be remove
        Debug.Log($"Dice:{passingOperation}");
        //Rolling Event
        EventSystem.Instance.TriggerEvent<int>("DiceGameEvent", "MoveCup",0);
        await Task.Delay(1000);
        EventSystem.Instance.TriggerEvent<string>("DiceGameEvent", "MoveCup", passingOperation);
        await Task.Delay(1000);
        EventSystem.Instance.TriggerEvent<int>("DiceGameEvent", "MoveCup", 1);

        //Passing the Result to Next Stage
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
