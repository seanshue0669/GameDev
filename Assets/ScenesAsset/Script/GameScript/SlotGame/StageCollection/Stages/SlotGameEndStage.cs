using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class SlotGameEndStage : IStage
{
    #region Fields and Properties
    //UI
    private TMP_Text statusText;
    private TMP_Text resultText;
    private Button confirmButton;
    string UITag;
    string videoPath1;
    string videoPath2;
    //public PlayerData playerData;
    public int currentMoney;

    //private Action continueAction;
    private TaskCompletionSource<bool> buttonClickedTcs;
    #endregion

    #region Constructor
    public SlotGameEndStage()
    {
        //Init the variable here
        UITag = "SlotGameUI";
    }
    #endregion

    #region Execute Phase Logic
    public async Task ExecuteAsync(SharedDataSO sharedData, UIComponentCollectionSO uiComponents)
    {
        if (!InitializeUI(uiComponents)) return;
        RegisterButtonListeners();

        currentMoney = DataManager.Instance.playerData.GetValue<int>("money");

        await ShowDialogAsync("Showing Result!");

        string reels1 = sharedData.GetInt("Reel1").ToString();
        string reels2 = sharedData.GetInt("Reel2").ToString();
        string reels3 = sharedData.GetInt("Reel3").ToString();
        string result = SlotResult(reels1, reels2, reels3);
        //result = "Boom";
        result = "QuitGambling";
        //result = "QuitGamblingFor5s";
        await ShowResultAsync(result);

        await ShowDialogAsync("Game finished!");
        await ShowDialogAsync("Would you like to restart?");
        await WaitForButtonClick();

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

        statusText = uiComponents.CreateUIComponent<TMP_Text>("GameStatusText", canvas.transform);
        resultText = uiComponents.CreateUIComponent<TMP_Text>("ResultText", canvas.transform);
        confirmButton = uiComponents.CreateUIComponent<Button>("ConfirmButton", canvas.transform);
        //declare the UI varable & Create UIElement
        return true;
    }

    private void CleanupUI()
    {
        GameObject.Destroy(statusText.gameObject);
        GameObject.Destroy(resultText.gameObject);
        GameObject.Destroy(confirmButton.gameObject);
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
        //continueAction?.Invoke();
        buttonClickedTcs?.SetResult(true);
    }

    private async Task WaitForButtonClick()
    {
        buttonClickedTcs = new TaskCompletionSource<bool>();
        await buttonClickedTcs.Task;
    }
    #endregion

    #region Support Functions

    public async Task InputAsync()
    {
    }

    private async Task ShowDialogAsync(string text)
    {
        statusText.text = text;
        //your TextUI Element
        await Task.Delay(1000);
    }

    private async Task ShowResultAsync(string text)
    {
        resultText.text = text;
        //your TextUI Element

        switch (text)
        {
            case "Jackpot":
                DataManager.Instance.playerData.SetValue("money", currentMoney * 7);
                break;
            case "Penniless":
                DataManager.Instance.playerData.SetValue("money", 0);
                break;
            case "Goat":
                EventSystem.Instance.TriggerEvent<int>("ObjectSpawn", "Goat", 1);
                break;
            case "NoSlotGamesAnymore":
                break;
            case "Toilet":
                EventSystem.Instance.TriggerEvent<int>("ObjectSpawn", "Goat", 1);
                break;
            case "QuitGambling":
                EventSystem.Instance.TriggerEvent<int>("PlayVideo", "PlayWindow", 2);
                await Task.Delay(10000);
                break;
            case "Triple":
                DataManager.Instance.playerData.SetValue("money", currentMoney * 3);
                break;
            case "HalfMoney":
                DataManager.Instance.playerData.SetValue("money", currentMoney / 2);
                break;
            case "TemporaryGoat":
                EventSystem.Instance.TriggerEvent<int>("ObjectSpawn", "Goat", 1);
                break;
            case "Boom":
                EventSystem.Instance.TriggerEvent<int>("EffectSpawn", "Boom", 1);
                break;
            case "ToiletSeat":
                EventSystem.Instance.TriggerEvent<int>("ObjectSpawn", "Goat", 1);
                break;
            case "QuitGamblingFor5s":
                EventSystem.Instance.TriggerEvent<int>("PlayVideo", "PlayWindow", 1);
                await Task.Delay(5000);
                break;
            case "Double":
                DataManager.Instance.playerData.SetValue("money", currentMoney * 2);
                break;
        }

        await Task.Delay(1000);
    }

    public string SlotResult(string reels1, string reels2, string reels3)
    {
        var conditions = new[]
        {
            new { Result = "Jackpot", Condition = new[] { "1", "1", "1" } },
            new { Result = "Penniless", Condition = new[] { "2", "2", "2" } },
            new { Result = "Goat", Condition = new[] { "3", "3", "3" } },
            new { Result = "NoSlotGamesAnymore", Condition = new[] { "4", "4", "4" } },
            new { Result = "Toilet", Condition = new[] { "5", "5", "5" } },
            new { Result = "QuitGambling", Condition = new[] { "6", "6", "6" } },
            new { Result = "Triple", Condition = new[] { "1", "1" } },
            new { Result = "HalfMoney", Condition = new[] { "2", "2" } },
            new { Result = "TemporaryGoat", Condition = new[] { "3", "3" } },
            new { Result = "Boom", Condition = new[] { "4", "4" } },
            new { Result = "ToiletSeat", Condition = new[] { "5", "5" } },
            new { Result = "QuitGamblingFor5s", Condition = new[] { "6", "6" } },
            new { Result = "Double", Condition = new[] { "1" } }
        };

        var reels = new[] { reels1, reels2, reels3 };

        foreach (var condition in conditions)
        {
            if (condition.Condition.All(c => reels.Count(r => r == c) >= condition.Condition.Count(c => c == c)))
            {
                return condition.Result;
            }
        }

        return "None";
    }
    #endregion
}
