using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PokerInitialStage : IStage
{
    #region Fields and Properties
    private readonly string instructionMessage = "Welcome To Black Jack";
    private readonly int maxBetAmount = 1000;
    private readonly int minBetAmount = 1;

    //UI
    private TMP_Text statusText;
    private TMP_InputField inputField;
    private Button confirmButton;
    private Button hitButton;

    private TaskCompletionSource<bool> phaseCompletionSource;
    private Action currentValidationAction;
    private string betOptions;
    private bool isWaiting = false;
    public delegate Task InputHandler();
    public InputHandler InputDelegate;
    #endregion

    #region Constructor
    
    #endregion

    #region Execute Phase Logic
    public async Task ExecuteAsync(SharedDataSO sharedData, UIComponentCollectionSO uiComponents)
    {
        if (!InitializeUI(uiComponents)) return;

        EventSystem.Instance.TriggerEvent("Assetdisplay", "display", 0);

        for (int i = 0; i < sharedData.GetInt("drawnCard"); i++)
        {
            await Task.Delay(1);
            EventSystem.Instance.TriggerEvent("BJgame", "DestroyCards", sharedData.GetInt("drawnCard"));
        }
        sharedData.SetInt("playerScore", 0);
        sharedData.SetInt("hostScore", 0);
        sharedData.SetInt("drawnCard", 0);
        sharedData.SetInt("playerHasA", 0);
        sharedData.SetInt("hostHasA", 0);
        sharedData.SetInt("playerBust", 0);
        sharedData.SetInt("hostBust", 0);
        sharedData.SetInt("playerHasFive", 0);

        RegisterButtonListeners();
        await ShowDialogAsync(instructionMessage);

        // Phase 1: Input Bet Amount
        statusText.text = "Please Enter your Bet Amount:";
        currentValidationAction = () => ValidateBetAmountInput(sharedData);
        InputDelegate = null;
        await WaitForPhaseCompletionAsync();

        // Phase 2: Select Bet Option
        //await ShowDialogAsync("Please Choose your Bet Option:");
        //currentValidationAction = () => ValidateOptionInput();
        //InputDelegate = Option;
        //await WaitForPhaseCompletionAsync();

        // Finalize
        //sharedData.SetString("BetOption", betOptions);
        //await ShowDialogAsync($"You Chose'{betOptions}'");
        //await ShowDialogAsync("All inputs are completed!");
        CleanupUI();
    }
    #endregion

    #region UI Management
    private bool InitializeUI(UIComponentCollectionSO uiComponents)
    {
        string UITag = "PokerGameUI";
        var canvas = uiComponents.FindCanvasByTag(UITag);
        if (canvas == null)
        {
            Debug.LogError("Canvas not found in the scene.");
            return false;
        }

        statusText = uiComponents.CreateUIComponent<TMP_Text>("GameStatusText", canvas.transform);
        inputField = uiComponents.CreateUIComponent<TMP_InputField>("BetInputField", canvas.transform);
        confirmButton = uiComponents.CreateUIComponent<Button>("ConfirmButton", canvas.transform);
        //hitButton = uiComponents.CreateUIComponent<Button>("HitButton", canvas.transform);

        if (statusText == null || inputField == null || confirmButton == null)
        {
            Debug.LogError("Required UI components are not correctly created.");
            return false;
        }

        inputField.interactable = true;
        return true;
    }

    private void CleanupUI()
    {
        inputField.interactable = false;
        GameObject.Destroy(statusText.gameObject);
        GameObject.Destroy(inputField.gameObject);
        GameObject.Destroy(confirmButton.gameObject);
        //GameObject.Destroy(hitButton.gameObject);
    }
    #endregion

    #region Button Logic
    private void RegisterButtonListeners()
    {
        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        currentValidationAction?.Invoke();
    }
    #endregion

    #region Validation Logic
    private void ValidateBetAmountInput(SharedDataSO sharedData)
    {
        string input = inputField.text;
        if (int.TryParse(input, out int betAmount) && betAmount >= minBetAmount && betAmount <= maxBetAmount && DataManager.Instance.playerData.GetValue<int>("chips") > 0)
        {
            sharedData.SetInt("BetAmount", betAmount);
            DataManager.Instance.SubAndDisplayValue("chips", betAmount);
            statusText.text = "Bet amount accepted!";
            phaseCompletionSource?.SetResult(true);
        }
        else if(DataManager.Instance.playerData.GetValue<int>("chips") <= 0)
        {
            statusText.text = "You're too broke, sorry";
        }
        else
        {
            statusText.text = $"Invalid bet! Enter a value between {minBetAmount} and {maxBetAmount}.";
        }
    }

    private void ValidateOptionInput()
    {
        if (!string.IsNullOrEmpty(betOptions))
        {
            phaseCompletionSource?.SetResult(true);
        }
        else
        {
            statusText.text = "Please select a valid option.";
        }
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
        if(InputDelegate!=null)
            await InputDelegate();
    }

    private async Task ShowDialogAsync(string text)
    {
        statusText.text = text;
        await Task.Delay(1000);
    }
    #endregion

    #region Custom InputAsync
    private async Task Option()
    {
        if (isWaiting)
            return;
        isWaiting = true;
        var tcs = EventSystem.Instance.WaitForCallBack("DiceGame", "Options");
        betOptions = (string)await tcs;
        isWaiting = false;
    }
    #endregion
}
