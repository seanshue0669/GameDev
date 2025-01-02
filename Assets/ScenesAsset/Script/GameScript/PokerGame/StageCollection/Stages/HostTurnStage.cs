using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HostTurnStage : IStage
{
    #region Fields and Properties
    private readonly string instructionMessage = "Host turn start";
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
    public HostTurnStage()
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

        if (!InitializeUI(uiComponents)) return;

        deck = sharedData.GetString("deck").Split(' ');

        drawnCard = sharedData.GetInt("drawnCard");

        playerHasA = sharedData.GetInt("playerHasA");
        playerScore = sharedData.GetInt("playerScore");

        hostHasA = sharedData.GetInt("hostHasA");
        hostScore = sharedData.GetInt("hostScore");

        for (int i=0; i < drawnCard; i++)
        {
            if (i != 3 && i != 2)
            {
                if (deck[i] == "FM")
                {
                    otherpointLimit += 2;
                }
            }
        }
        playerScoreText.text = playerScore + "/" + otherpointLimit;

        pointLimit = 21;

        if (deck[2] == "FM")
        {
            pointLimit += 2;
        }
        if (deck[3] == "FM")
        {
            pointLimit += 2;
        }
        hostScoreText.text = hostScore + "/" + pointLimit;

        EventSystem.Instance.TriggerEvent("BJgame", "CheckPlayer", false);

        RegisterButtonListeners();
        await ShowDialogAsync(instructionMessage);

        // Phase 1: player choose to hit or stand

        while (hostScore < 17)
        {
            
            HitPressed(sharedData);
            await Task.Delay(1000);
            //InputDelegate = null;
            //await WaitForPhaseCompletionAsync();
        }

        await ShowDialogAsync("Host have " + hostScore + " points");

        if (hostScore == pointLimit)
        {
            //statusText.text = "BLACK JACK!!!";
            await ShowDialogAsync("BLACK JACK!!!");
            //phaseCompletionSource?.SetResult(true);
        }

        else if (hostScore > pointLimit && hostHasA == 0)
        {
            sharedData.SetInt("hostBust", 1);
            //statusText.text = "You Busted...";
            await ShowDialogAsync("Host Busted...");
            //phaseCompletionSource?.SetResult(true);
        }

        if (playerScore <= otherpointLimit)
        {
            sharedData.SetInt("playerBust", 0);
        }
        else sharedData.SetInt("playerBust", 1);




        await ShowDialogAsync("Host turn end");

        int playerBust = sharedData.GetInt("playerBust");
        int hostBust = sharedData.GetInt("hostBust");

        if (playerBust == 1 || (playerScore < hostScore && hostBust == 0) && sharedData.GetInt("playerHasFive") == 0)
        {

            await ShowDialogAsync("You lose...");
        }
        else if (sharedData.GetInt("playerHasFive") == 1)
        {
            DataManager.Instance.playerData.AddValue("chips", sharedData.GetInt("BetAmount") * 5);
            await ShowDialogAsync("You got a five card win!!!");
        }

        else if ((playerScore > hostScore && playerScore == otherpointLimit) || (playerScore == otherpointLimit && hostBust == 1))
        {
            DataManager.Instance.playerData.AddValue("chips", sharedData.GetInt("BetAmount") * 3);
            await ShowDialogAsync("You got a black jack win!!!");
        }

        else if (playerScore > hostScore || (playerBust == 0 && hostBust == 1))
        {
            DataManager.Instance.playerData.AddValue("chips", sharedData.GetInt("BetAmount") * 2);
            await ShowDialogAsync("You win!!");
        }
        else
        {
            DataManager.Instance.playerData.AddValue("chips", sharedData.GetInt("BetAmount"));
            await ShowDialogAsync("It's a tie");
        }


        sharedData.SetInt("drawnCard", drawnCard);

        sharedData.SetInt("playerHasA", playerHasA);
        sharedData.SetInt("playerScore", playerScore);

        sharedData.SetInt("hostHasA", hostHasA);
        sharedData.SetInt("hostScore", hostScore);

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
        //standButton = uiComponents.CreateUIComponent<Button>("StandButton", canvas.transform);

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
        //hitButton.onClick.AddListener(HitClick);
        //standButton.onClick.AddListener(StandClick);
    }

    private void OnClick()
    {
        currentValidationAction?.Invoke();
        stand?.Invoke();
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

        string hostCard = deck[drawnCard];
        string cardName = "Card_";
        if (hostCard[1] == 'A') hostHasA++;

        if (hostCard[1] == 'R')
        {
            (playerScore, hostScore) = (hostScore, playerScore);
            (playerHasA, hostHasA) = (hostHasA, playerHasA);
        }
        else
            hostScore += scoreTransfer(hostCard);
        cardName += nameTransfer(hostCard);



        if (hostScore > pointLimit && hostHasA != 0)                //check if player has A
        {
            hostScore -= 10;
            hostHasA--;
        }

        Debug.Log(cardName);
        Debug.Log(hostScore);

        EventSystem.Instance.TriggerEvent("BJgame", "CardSpawn", cardName);

        drawnCard++;
        fiveCard++;

        hostScoreText.text = hostScore + "/" + pointLimit;
        playerScoreText.text = playerScore + "/" + otherpointLimit;

        if (hostScore == pointLimit)
        {
            //statusText.text = "BLACK JACK!!!";
            phaseCompletionSource?.SetResult(true);
        }

        else if (hostScore > pointLimit && hostHasA == 0)
        {
            sharedData.SetInt("hostBust", 1);
            //statusText.text = "You Busted...";
            //phaseCompletionSource?.SetResult(true);
        }

        
    }
    private void StandPressed(SharedDataSO sharedData)
    {
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

    private void ShuffleDeck(SharedDataSO sharedData)
    {
        string[] deck = sharedData.GetString("deck").Split(' ');
        string shuffledDeck = "";
        for (int i = 0; i < deck.Length; i++)
        {
            Debug.Log(deck[i]);
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
