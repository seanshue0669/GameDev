using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.InputSystem.DefaultInputActions;

public class DiceGame_EndStage : IStage
{
    #region Fields and Properties
    private readonly string instructionMessage = "Welcome To Dice Game";
    private string playerOptions;
    private int playerBetAmount;
    private int diceResult;
    private int isEqual;
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
        //Calulate result
        playerOptions = sharedData.GetString("BetOption");
        playerBetAmount = sharedData.GetInt("BetAmount");
        diceResult = sharedData.GetInt("Result");
        isEqual = sharedData.GetInt("IsEqual");

        if (playerOptions == "smaller" && diceResult < 7)
            DataManager.Instance.playerData.AddValue("money", playerBetAmount * 2);
        else if(playerOptions == "smaller" && diceResult >= 7)
            DataManager.Instance.playerData.SubValue("money", playerBetAmount * 2);
        else if (playerOptions == "bigger" && diceResult >= 7)
            DataManager.Instance.playerData.AddValue("money", playerBetAmount * 2);
        else if (playerOptions == "bigger" && diceResult < 7)
            DataManager.Instance.playerData.SubValue("money", playerBetAmount * 2);
        else if (playerOptions == "equal" && isEqual == 1)
            DataManager.Instance.playerData.AddValue("money", playerBetAmount * 7);
        else if (playerOptions == "equal" && isEqual == 0)
            DataManager.Instance.playerData.SubValue("money", playerBetAmount * 7);

        await ShowDialogAsync("The Dice Result is "+ TranslateResult(diceResult) + "\nPlaying Again?");

        await WaitForPhaseCompletionAsync();
        await ShowDialogAsync("Preepare Next Round!");
        EventSystem.Instance.TriggerEvent("DiceGameEvent", "StopSpawn", 0);
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
    private string TranslateResult(int p_Diceresult)
    {
        if (p_Diceresult == -1)
        {
            return "integer overflow!";
        }
        else
            return p_Diceresult.ToString();
    }
    #endregion

    #region Custom InputAsync

    #endregion
}
