using System;
using System.Threading.Tasks;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class RouletteGameInitialStage : IStage
{
    #region Fields and Properties
    private readonly string instructionMessage = "Welcome To Roulette Game";
    private readonly int maxBetAmount = 1000;
    private readonly int minBetAmount = 1;

    //UI
    private TMP_Text statusText;
    private TMP_InputField inputField;
    private Button confirmButton;
    private Button redButton;
    private Button blackButton;
    private Button lowButton;
    private Button highButton;

    private TaskCompletionSource<bool> phaseCompletionSource;
    private Action currentValidationAction;
    private Action[] currentInput;

    private bool isValid;
    private int actionIdx;
    private string betOptions;
    private bool isWaiting = false;
    public delegate Task InputHandler();
    public InputHandler InputDelegate;
    #endregion

    #region Constructor
    public RouletteGameInitialStage() {
        currentInput = new Action[10];
        isValid = false;
        actionIdx = 0;
    }
    #endregion

    #region Execute Phase Logic
    public async Task ExecuteAsync(SharedDataSO sharedData, UIComponentCollectionSO uiComponents)
    {
        if (!InitializeUI(uiComponents)) return;

        inputField.gameObject.SetActive(false);
        confirmButton.gameObject.SetActive(false);

        await ShowDialogAsync(instructionMessage);

        // Phase 1: Input Bet Amount
        await ShowDialogAsync("What type of bet would you like to place?");

        RegisterButtonListeners();
        currentValidationAction = () => NoneValidation();

        currentInput[0] = () => RedOnClick(sharedData);
        currentInput[1] = () => BlackOnClick(sharedData);
        currentInput[2] = () => LowOnClick(sharedData);
        currentInput[3] = () => HighOnClick(sharedData);

        InputDelegate = null;
        await WaitForPhaseCompletionAsync();

        redButton.gameObject.SetActive(false);
        blackButton.gameObject.SetActive(false);
        highButton.gameObject.SetActive(false);
        lowButton.gameObject.SetActive(false);

        await ShowDialogAsync("You place a bet on " + BetTypeTransfer(sharedData.GetInt("type")) + ".");

        await ShowDialogAsync("How much would you like to bet?");

        inputField.gameObject.SetActive(true);
        confirmButton.gameObject.SetActive(true);

        currentValidationAction = () => ValidateBetAmountInput(sharedData);
        currentInput[0] = () => SetBetAccount(sharedData);

        InputDelegate = null;
        await WaitForPhaseCompletionAsync();

        inputField.gameObject.SetActive(false);
        confirmButton.gameObject.SetActive(false);

        await ShowDialogAsync($"You bet {sharedData.GetInt("BetAmount")}$");

        RemoveButtonListener();
        CleanupUI();
    }

    string BetTypeTransfer(int betType)
    {
        string[] transfer = { "red", "black", "1-18", "19-36" };

        if (betType < 0 || betType >= transfer.Length)
        {
            return "Wrong";
        }

        return transfer[betType];
    }

    #endregion

    #region UI Management
    private bool InitializeUI(UIComponentCollectionSO uiComponents)
    {
        string UITag = "RouletteGameUI";
        var canvas = uiComponents.FindCanvasByTag(UITag);
        if (canvas == null)
        {
            Debug.LogError("Canvas not found in the scene.");
            return false;
        }

        statusText = uiComponents.CreateUIComponent<TMP_Text>("GameStatusText", canvas.transform);
        inputField = uiComponents.CreateUIComponent<TMP_InputField>("BetInputField", canvas.transform);
        confirmButton = uiComponents.CreateUIComponent<Button>("ConfirmButton", canvas.transform);

        redButton = uiComponents.CreateUIComponent<Button>("red", canvas.transform);
        blackButton = uiComponents.CreateUIComponent<Button>("black", canvas.transform);
        lowButton = uiComponents.CreateUIComponent<Button>("low", canvas.transform);
        highButton = uiComponents.CreateUIComponent<Button>("high", canvas.transform);

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

        GameObject.Destroy(redButton.gameObject);
        GameObject.Destroy(blackButton.gameObject);
        GameObject.Destroy(lowButton.gameObject);
        GameObject.Destroy(highButton.gameObject);
    }
    #endregion

    #region Button Logic
    private void RegisterButtonListeners()
    {
        RemoveButtonListener();

        redButton.onClick.AddListener(() => OnClick(0));
        blackButton.onClick.AddListener(() => OnClick(1));
        lowButton.onClick.AddListener(() => OnClick(2));
        highButton.onClick.AddListener(() => OnClick(3));

        confirmButton.onClick.AddListener(() => OnClick(0));
    }

    private void RemoveButtonListener()
    {
        confirmButton.onClick.RemoveAllListeners();
        redButton.onClick.RemoveAllListeners();
        blackButton.onClick.RemoveAllListeners();
        highButton.onClick.RemoveAllListeners();
        lowButton.onClick.RemoveAllListeners();
    }

    private void OnClick(int i)
    {
        if (phaseCompletionSource != null && !phaseCompletionSource.Task.IsCompleted)
        {
            currentValidationAction?.Invoke();

            if (isValid)
            {
                currentInput[i]?.Invoke();
            }
        }
    }

    private void RedOnClick(SharedDataSO sharedData)
    {
        sharedData.SetInt("type",0);
        phaseCompletionSource?.SetResult(true);
    }

    private void BlackOnClick(SharedDataSO sharedData)
    {
        sharedData.SetInt("type", 1);
        phaseCompletionSource?.SetResult(true);
    }

    private void LowOnClick(SharedDataSO sharedData)
    {
        sharedData.SetInt("type", 2);
        phaseCompletionSource?.SetResult(true);
    }

    private void HighOnClick(SharedDataSO sharedData)
    {
        sharedData.SetInt("type", 3);
        phaseCompletionSource?.SetResult(true);
    }

    private void SetBetAccount(SharedDataSO sharedData)
    {
        string input = inputField.text;
        int betAmount = int.Parse(input);

        sharedData.SetInt("BetAmount", betAmount);

        phaseCompletionSource?.SetResult(true);
    }
#endregion

    #region Validation Logic
    private void ValidateBetAmountInput(SharedDataSO sharedData)
    {
        isValid = false;
        string input = inputField.text;
        if (int.TryParse(input, out int betAmount) && betAmount >= minBetAmount && betAmount <= maxBetAmount)
        {
            statusText.text = "Bet amount accepted!";
            isValid = true;
        }
        else
        {
            statusText.text = $"Invalid bet! Enter a value between {minBetAmount} and {maxBetAmount}.";
        }
    }

    void NoneValidation()
    {
        isValid = true;
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
