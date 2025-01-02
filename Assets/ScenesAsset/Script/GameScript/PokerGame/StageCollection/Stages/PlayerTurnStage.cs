using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTurnStage : IStage
{
    #region Fields and Properties
    private readonly string instructionMessage = "Player turn start";
    private readonly int maxBetAmount = 1000;
    private readonly int minBetAmount = 1;

    //UI
    private TMP_Text statusText;
    private TMP_Text playerScoreText;
    private TMP_Text hostScoreText;
    private TMP_InputField inputField;
    private Button confirmButton;
    private Button hitButton;
    private Button standButton;

    private TaskCompletionSource<bool> phaseCompletionSource;
    private Action currentValidationAction;
    private Action stand;
    private string betOptions;

    // variables used
    private string[] deck;
    private int drawnCard;
    private int playerHasA;
    private int playerScore;
    private int hostHasA;
    private int hostScore;
    private int fiveCard;
    private int pointLimit;
    private int otherpointLimit;

    private bool isWaiting = false;
    public delegate Task InputHandler();
    public InputHandler InputDelegate;
    #endregion

    #region Constructor
    public PlayerTurnStage()
    {
        //Init the variable here

        pointLimit = 21;
        fiveCard = 2;
    }
    #endregion

    #region Execute Phase Logic
    public async Task ExecuteAsync(SharedDataSO sharedData, UIComponentCollectionSO uiComponents)
    {
        pointLimit = 21;
        otherpointLimit = 21;
        fiveCard = 2;
        if (!InitializeUI(uiComponents)) return;



        deck = sharedData.GetString("deck").Split(' ');

        drawnCard = sharedData.GetInt("drawnCard");

        playerHasA = sharedData.GetInt("playerHasA");
        playerScore = sharedData.GetInt("playerScore");

        hostHasA = sharedData.GetInt("hostHasA");
        hostScore = sharedData.GetInt("hostScore");

        if (deck[2] == "FM")
        {
            otherpointLimit += 2;
        }
        if (deck[3] == "FM")
        {
            otherpointLimit += 2;
        }
        hostScoreText.text = hostScore + "/" + otherpointLimit;

        pointLimit = 21;

        if (deck[0] =="FM")
        {
            pointLimit += 2;
        }
        if (deck[1] == "FM")
        {
            pointLimit += 2;
        }

        playerScoreText.text = playerScore + "/" + pointLimit;

        EventSystem.Instance.TriggerEvent("BJgame", "CheckPlayer", true);

        await ShowDialogAsync(instructionMessage);

        RegisterButtonListeners();
        if (playerScore >= pointLimit)
        {
            GameObject.Destroy(hitButton.gameObject);
            GameObject.Destroy(standButton.gameObject);
        }

        

        // Phase 1: player choose to hit or stand

        if (playerScore < pointLimit)
        {
            statusText.text = "Hit or stand";
            currentValidationAction = () => HitPressed(sharedData);
            stand = () => StandPressed(sharedData);
            InputDelegate = null;
            await WaitForPhaseCompletionAsync();
        }

        await ShowDialogAsync("You have " + playerScore + " points");

        if (playerScore == pointLimit)
        {
            statusText.text = "BLACK JACK!!!";
            await ShowDialogAsync("BLACK JACK!!!");
            //phaseCompletionSource?.SetResult(true);
        }

        else if (playerScore > pointLimit && playerHasA == 0)
        {
            //sharedData.SetInt("playerBust", 1);
            statusText.text = "You Busted...";
            await ShowDialogAsync("You Busted...");
            //phaseCompletionSource?.SetResult(true);
        }

        else if (playerScore <= pointLimit && fiveCard == 5)
        {
            //sharedData.SetInt("playerHasFive", 1);
            statusText.text = "You Have Five Cards!!!";
            await ShowDialogAsync("You Have Five Cards!!!");
            //phaseCompletionSource?.SetResult(true);
        }


        await ShowDialogAsync("Player turn end");



        sharedData.SetInt("drawnCard", drawnCard);

        sharedData.SetInt("playerHasA", playerHasA);
        sharedData.SetInt("playerScore", playerScore);

        sharedData.SetInt("hostHasA", hostHasA);
        sharedData.SetInt("hostScore", hostScore);

        InputDelegate = null;

        // Finalize

        await ShowDialogAsync("All inputs are completed!");
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
        hitButton = uiComponents.CreateUIComponent<Button>("HitButton", canvas.transform);
        standButton = uiComponents.CreateUIComponent<Button>("StandButton", canvas.transform);

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
        //GameObject.Destroy(standButton.gameObject);
    }
    #endregion

    #region Button Logic
    private void RegisterButtonListeners()
    {
        confirmButton.onClick.RemoveAllListeners();
        hitButton.onClick.AddListener(HitClick);
        standButton.onClick.AddListener(StandClick);
    }
    private void HitClick()
    {
        currentValidationAction?.Invoke();
    }
    private void StandClick()
    {
        stand?.Invoke();
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

    private void HitPressed(SharedDataSO sharedData)
    {
        
        string playerCard = deck[drawnCard];
        string cardName = "Card_";
        if (playerCard[1] == 'A') playerHasA++;

        if (playerCard[1] == 'R')
        {
            (playerScore, hostScore) = (hostScore, playerScore);
            (playerHasA, hostHasA) = (hostHasA, playerHasA);
        }
        else
            playerScore += scoreTransfer(playerCard);
        cardName += nameTransfer(playerCard);

        

        if (playerScore > pointLimit && playerHasA != 0)                //check if player has A
        {
            playerScore -= 10;
            playerHasA--;
        }

        Debug.Log(cardName);
        Debug.Log(playerScore);

        EventSystem.Instance.TriggerEvent("BJgame", "CardSpawn", cardName);

        drawnCard++;
        fiveCard++;

        playerScoreText.text = playerScore + "/" + pointLimit;
        hostScoreText.text = hostScore + "/" + otherpointLimit;

        if (playerScore == pointLimit && fiveCard != 5)
        {
            //statusText.text = "BLACK JACK!!!";
            phaseCompletionSource?.SetResult(true);
            GameObject.Destroy(hitButton.gameObject);
            GameObject.Destroy(standButton.gameObject);
        }

        else if (playerScore > pointLimit && playerHasA == 0)
        {
            sharedData.SetInt("playerBust", 1);
            //statusText.text = "You Busted...";
            phaseCompletionSource?.SetResult(true);
            GameObject.Destroy(hitButton.gameObject);
            GameObject.Destroy(standButton.gameObject);
        }

        else if (playerScore <= pointLimit && fiveCard == 5)
        {
            sharedData.SetInt("playerHasFive", 1);
            //statusText.text = "You Have Five Cards!!!";
            phaseCompletionSource?.SetResult(true);
            GameObject.Destroy(hitButton.gameObject);
            GameObject.Destroy(standButton.gameObject);
        }
    }
    private void StandPressed(SharedDataSO sharedData)
    {
        GameObject.Destroy(hitButton.gameObject);
        GameObject.Destroy(standButton.gameObject);
        phaseCompletionSource?.SetResult(true);
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

    

    public int scoreTransfer(string card)                                   //read card string
    {
        Debug.Log(card);

        switch (card[1])
        {
            case 'A':

                return 11;
            case 'T':

                return 10;

            case 'J':

                return 10;

            case 'Q':

                return 10;

            case 'K':

                return 10;

            case 'R':
                
                return 1 - 1;

            case 'N':
 
                return 3090;

            case 'M':
                pointLimit += 2;
                return 1 - 1;


            default:

                return card[1] - '0';
        }


    }

    public string nameTransfer(string card)                                   //read card string
    {

        string name = "";

        switch (card[0])
        {
            case 'C':
                name += "Club";
                break;
            case 'D':
                name += "Diamond";
                break;

            case 'H':
                name += "Heart";
                break;

            case 'S':
                name += "Spade";
                break;

            case 'F':
                name += "Funny";
                break;


        }

        switch (card[1])
        {
            case 'A':
                name += "Ace";
                return name;
            case 'T':
                name += "10";
                return name;

            case 'J':
                name += "Jack";
                return name;

            case 'Q':
                name += "Queen";
                return name;

            case 'K':
                name += "King";
                return name;

            case 'R':
                name += "Reverse";
                return name;

            case 'N':
                name += "Nvidia";
                return name;

            case 'M':
                name += "Memory";
                return name;

            default:
                name += card[1];
                return name;
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
        await Task.Delay(2000);
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

