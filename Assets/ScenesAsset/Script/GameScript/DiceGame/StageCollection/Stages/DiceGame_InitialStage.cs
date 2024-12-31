using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DiceGame_InitialStage : IStage
{
    #region Fields and Properties
    private readonly string instructionMessage = "Welcome To Dice Game";
    private readonly int maxBetAmount = 1000;
    private readonly int minBetAmount = 1;

    //UI
    private TMP_Text statusText;
    private TMP_InputField inputField;

    private Button confirmButton;
    private Button largerButton;
    private Button equalButton;
    private Button smallerButton;
    private TaskCompletionSource<bool> phaseCompletionSource;
    private Action currentValidationAction;
    private string betOptions;
    private bool isWaiting = false;
    public delegate Task InputHandler();
    public InputHandler InputDelegate;
    #endregion

    #region Constructor
    public DiceGame_InitialStage()
    {
        //Init the variable here
    }
    #endregion

    #region Execute Phase Logic
    public async Task ExecuteAsync(SharedDataSO sharedData, UIComponentCollectionSO uiComponents)
    {
        if (!InitializeUI(uiComponents)) return;

        RegisterButtonListeners();
        await ShowDialogAsync(instructionMessage);

        // Phase 1: Input Bet Amount
        await DisableOptionButton();
        await ShowDialogAsync("Please Enter your Bet Amount:");
        currentValidationAction = () => ValidateBetAmountInput(sharedData);
        InputDelegate = null;
        await WaitForPhaseCompletionAsync();

        // Phase 2: Select Bet Option
        await EnableOptionButton();
        await ShowDialogAsync("Please Choose your Bet Option:");
        currentValidationAction = () => ValidateOptionInput();
        InputDelegate = null;
        await WaitForPhaseCompletionAsync();

        // Finalize
        sharedData.SetString("BetOption", betOptions);
        await ShowDialogAsync($"You Chose'{betOptions}'");
        await ShowDialogAsync("All inputs are completed!");
        CleanupUI();
    }
    #endregion

    #region UI Management
    private bool InitializeUI(UIComponentCollectionSO uiComponents)
    {
        string UITag = "DiceGameUI";
        var canvas = uiComponents.FindCanvasByTag(UITag);
        if (canvas == null)
        {
            Debug.LogError("Canvas not found in the scene.");
            return false;
        }

        statusText = uiComponents.CreateUIComponent<TMP_Text>("GameStatusText", canvas.transform);
        inputField = uiComponents.CreateUIComponent<TMP_InputField>("BetInputField", canvas.transform);
        confirmButton = uiComponents.CreateUIComponent<Button>("ConfirmButton", canvas.transform);

        largerButton = uiComponents.CreateUIComponent<Button>("BetOptionLarger", canvas.transform);
        equalButton = uiComponents.CreateUIComponent<Button>("BetOptionEqual", canvas.transform);
        smallerButton = uiComponents.CreateUIComponent<Button>("BetOptionSmaller", canvas.transform);
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
        GameObject.Destroy(smallerButton.gameObject);
        GameObject.Destroy(largerButton.gameObject);
        GameObject.Destroy(equalButton.gameObject);
    }
    #endregion

    #region Button Logic
    private void RegisterButtonListeners()
    {
        confirmButton.onClick.RemoveAllListeners();
        smallerButton.onClick.RemoveAllListeners();
        largerButton.onClick.RemoveAllListeners();
        equalButton.onClick.RemoveAllListeners();

        confirmButton.onClick.AddListener(OnClick);
        smallerButton.onClick.AddListener(() => OptionsOnClick("smaller"));
        largerButton.onClick.AddListener(() => OptionsOnClick("bigger"));
        equalButton.onClick.AddListener(() => OptionsOnClick("equal"));
    }

    private void OnClick()
    {
        currentValidationAction?.Invoke();
    }
    private void OptionsOnClick(string p_option)
    {
        betOptions = p_option;
        currentValidationAction?.Invoke();
    }
    #endregion

    #region Validation Logic
    private void ValidateBetAmountInput(SharedDataSO sharedData)
    {
        string input = inputField.text;
        if (int.TryParse(input, out int betAmount) && betAmount >= minBetAmount && betAmount <= maxBetAmount)
        {
            sharedData.SetInt("BetAmount", betAmount);
            statusText.text = "Bet amount accepted!";
            phaseCompletionSource?.SetResult(true);
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
        if (InputDelegate != null)
            await InputDelegate();
    }
    private async Task ShowDialogAsync(string text)
    {
        statusText.text = text;
        await Task.Delay(1000);
    }
    private async Task DisableOptionButton()
    {
        largerButton.enabled = false;
        equalButton.enabled = false;
        smallerButton.enabled = false;
        await Task.Delay(10);
    }
    private async Task EnableOptionButton()
    {
        largerButton.enabled = true;
        equalButton.enabled = true;
        smallerButton.enabled = true;
        await Task.Delay(10);
    }
    #endregion

    #region Custom InputAsync
    private async Task Option()
    {

    }
    #endregion
}
