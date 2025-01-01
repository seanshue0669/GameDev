using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class RouletteGameExecuteStage : IStage
{
    #region Fields and Properties
    //Properties 
    private int ball;

    //UI
    string UITag = "RouletteGameUI";
    private TMP_Text statusText;
    //Core
    private TaskCompletionSource<bool> phaseCompletionSource;

    //private bool isWaiting = false;
    public delegate Task InputHandler();
    public InputHandler InputDelegate;
    #endregion

    #region Constructor
    public RouletteGameExecuteStage()
    {
        //Init the variable here
        
    }
    #endregion

    #region Execute Phase Logic
    public async Task ExecuteAsync(SharedDataSO sharedData, UIComponentCollectionSO uiComponents)
    {
        if (!InitializeUI(uiComponents)) return;

        int event_type = UnityEngine.Random.Range(1, 101);

        if (event_type >= 1 && event_type <= 90)
        {
            ball = UnityEngine.Random.Range(1, 37);

            sharedData.SetInt("ball", ball);

            EventSystem.Instance.TriggerEvent("Roulette", "spin", ball);

            await ShowDialogAsync("The ball stops on......");


            await ShowDialogAsync("The ball stops on the " + ball.ToString() + "!!");
        }
        else if (event_type >= 91 && event_type <= 95)
        {
            EventSystem.Instance.TriggerEvent("Roulette", "throwout", 5f);

            await ShowDialogAsync("oops! it seems like the ball flew away");

            await ShowDialogAsync("just like your bet");

            await ShowDialogAsync("dun be upset, i bewieve chu can eawn dis back in da next wound~ >w<");

            sharedData.SetInt("ball", -1);
        }
        else if (event_type >= 96 && event_type <= 100)
        {
            ball = 0;

            sharedData.SetInt("ball", ball);

            EventSystem.Instance.TriggerEvent("Roulette", "spin", ball);

            await ShowDialogAsync("The ball stops on......");

            await ShowDialogAsync("The ball stops on the " + ball.ToString() + "!!");
        }

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

        //declare the UI varable & Create UIElement
        statusText = uiComponents.CreateUIComponent<TMP_Text>("GameStatusText", canvas.transform);
        return true;
    }

    private void CleanupUI()
    {
        GameObject.Destroy(statusText.gameObject);
    }
    #endregion

    #region Validation Logic
    private void ValidateInput()
    {
        phaseCompletionSource?.SetResult(true);
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
        //your TextUI Element
        await Task.Delay(2000);
    }
    #endregion


}
