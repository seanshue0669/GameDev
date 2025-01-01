using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DiceGame_EndStage : IStage
{
    #region Fields and Properties
    private readonly string instructionMessage = "Welcome To Dice Game";


    //UI
    private TMP_Text statusText;

    private Button quitButton;
    private Button continueButton;

    private Action currentValidationAction;
    private TaskCompletionSource<bool> phaseCompletionSource;
    public delegate Task InputHandler();
    public InputHandler InputDelegate;
    #endregion

    #region Constructor
    public DiceGame_EndStage()
    {
        //Init the variable here
    }
    #endregion

    #region Execute Phase Logic
    public async Task ExecuteAsync(SharedDataSO sharedData, UIComponentCollectionSO uiComponents)
    {
        if (!InitializeUI(uiComponents)) return;

        RegisterButtonListeners();

        await ShowDialogAsync("Playing Again?");
        await WaitForPhaseCompletionAsync();
        await ShowDialogAsync("Preepare Next Round!");
        CleanupUI();
    }
    #endregion

    #region UI Management
    private bool InitializeUI(UIComponentCollectionSO uiComponents)
    {
        string UITag = "DiceGameUI";
        var canvas = uiComponents.FindCanvasByTag(UITag);
        statusText = uiComponents.CreateUIComponent<TMP_Text>("GameStatusText", canvas.transform);
        quitButton = uiComponents.CreateUIComponent<Button>("Quit", canvas.transform);
        continueButton = uiComponents.CreateUIComponent<Button>("PlayingNextRound", canvas.transform);
        return true;
    }

    private void CleanupUI()
    {
        GameObject.Destroy(statusText.gameObject);
        GameObject.Destroy(quitButton.gameObject);
        GameObject.Destroy(continueButton.gameObject);
    }
    #endregion

    #region Button Logic
    private void RegisterButtonListeners()
    {
        continueButton.onClick.RemoveAllListeners();
        continueButton.onClick.AddListener(()=> phaseCompletionSource?.SetResult(true));
    }
    #endregion

    #region Validation Logic
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
        statusText.text = text;
        await Task.Delay(1000);
    }
    #endregion

    #region Custom InputAsync

    #endregion
}
