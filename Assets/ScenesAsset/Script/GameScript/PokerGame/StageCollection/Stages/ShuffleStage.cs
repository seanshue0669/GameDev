using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShuffleStage : IStage
{
    #region Fields and Properties
    private readonly string instructionMessage = "Shuffling";

    //UI
    private TMP_Text statusText;
    public TMP_Text playerScoreText;
    public TMP_Text hostScoreText;
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
    public ShuffleStage()
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

        // Phase 1: Shuffle deck

        ShuffleDeck(sharedData);
        InputDelegate = null;

        // Finalize

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
        playerScoreText = uiComponents.CreateUIComponent<TMP_Text>("PlayerScoreDisplay", canvas.transform);
        hostScoreText = uiComponents.CreateUIComponent<TMP_Text>("HostScoreDisplay", canvas.transform);
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
        GameObject.Destroy(playerScoreText.gameObject);
        GameObject.Destroy(hostScoreText.gameObject);
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

    private void ShuffleDeck(SharedDataSO sharedData)
    {
        string[] deck = sharedData.GetString("deck").Split(' ');
        string shuffledDeck = "";
        for (int i = 0; i < deck.Length; i++)
        {

            string tmp = deck[i];
            int r = UnityEngine.Random.Range(i, deck.Length);
            deck[i] = deck[r];
            deck[r] = tmp;
        }

        for (int i = 0; i < deck.Length; i++)
        {
            shuffledDeck += deck[i];
            if (i + 1 != deck.Length)
                shuffledDeck += " ";
        }

        sharedData.SetString("deck", shuffledDeck);

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
