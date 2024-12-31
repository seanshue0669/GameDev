using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Video;

public class SlotGameEndStage : IStage
{
    #region Fields and Properties
    //UI
    private TMP_Text statusText;
    private TMP_Text resultText;
    private Button confirmButton;
    string UITag;
    //private VideoClip videoClip1;
    //private VideoClip videoClip2;
    private string videoPath1;
    private string videoPath2;

    //private Action continueAction;
    private TaskCompletionSource<bool> buttonClickedTcs;
    #endregion

    #region Constructor
    public SlotGameEndStage()
    {
        //Init the variable here
        UITag = "SlotGameUI";
        //videoClip1 = Resources.Load<VideoClip>("video5s");
        //videoClip2 = Resources.Load<VideoClip>("video10s");
        videoPath1 = "C:\\Users\\aaa93\\Desktop\\vs\\軟體工程unity\\DiceGame-main\\Assets\\Script\\EventSystem\\Testing\\video5s.mp4";
        videoPath2 = "C:\\Users\\aaa93\\Desktop\\vs\\軟體工程unity\\DiceGame-main\\Assets\\Script\\EventSystem\\Testing\\video10s.mp4";
    }
    #endregion

    #region Execute Phase Logic
    public async Task ExecuteAsync(SharedDataSO sharedData, UIComponentCollectionSO uiComponents)
    {
        if (!InitializeUI(uiComponents)) return;
        RegisterButtonListeners();

        await ShowDialogAsync("Showing Result!");

        string reels1 = sharedData.GetInt("Reel1").ToString();
        string reels2 = sharedData.GetInt("Reel2").ToString();
        string reels3 = sharedData.GetInt("Reel3").ToString();
        string result = SlotResult(reels1, reels2, reels3);
        await ShowResultAsync(result);
        //await EventSystem.Instance.TriggerCallBack<string, string>("PlayVideo", "PlayWindow", videoPath2);
        //EventSystem.Instance.TriggerEvent<int>("EffectSpawn", "Boom", 1);
        //EventSystem.Instance.TriggerEvent<int>("ObjectSpawn", "Goat", 1);

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
        await buttonClickedTcs.Task; // 等待按鈕點擊
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
        if (text == "QuitGambling")
        {
            await EventSystem.Instance.TriggerCallBack<string, string>("PlayVideo", "PlayWindow", videoPath2);
        }
        if (text == "QuitGamblingFor5s")
        {
            await EventSystem.Instance.TriggerCallBack<string, string>("PlayVideo", "PlayWindow", videoPath1);
        }
        if (text == "Goat")
        {
            EventSystem.Instance.TriggerEvent<int>("ObjectSpawn", "Goat", 1);
        }
        if (text == "TemporaryGoat")
        {
            EventSystem.Instance.TriggerEvent<int>("ObjectSpawn", "Goat", 1);
        }
        if (text == "Toilet")
        {
            EventSystem.Instance.TriggerEvent<int>("ObjectSpawn", "Goat", 1);
        }
        if (text == "ToiletSeat")
        {
            EventSystem.Instance.TriggerEvent<int>("ObjectSpawn", "Goat", 1);
        }
        if (text == "Boom")
        {
            EventSystem.Instance.TriggerEvent<int>("EffectSpawn", "Boom", 1);
        }
        await Task.Delay(1000);
    }

    public string SlotResult(string reels1, string reels2, string reels3)
    {
        // 定義可能的結果與條件的對應關係
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

        // 將拉桿的值組成集合，方便檢查條件
        var reels = new[] { reels1, reels2, reels3 };

        // 檢查條件，依照匹配結果回傳對應的 Result
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
