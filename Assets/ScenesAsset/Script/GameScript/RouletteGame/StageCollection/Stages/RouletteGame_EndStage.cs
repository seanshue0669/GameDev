using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RouletteGameEndStage : IStage
{
    #region Fields and Properties
    private static readonly HashSet<int> RedNumbers = new HashSet<int>
    {
        1, 3, 5, 7, 9, 12, 14, 16, 18,
        19, 21, 23, 25, 27, 30, 32, 34, 36
    };

    private static readonly HashSet<int> BlackNumbers = new HashSet<int>
    {
        2, 4, 6, 8, 10, 11, 13, 15, 17,
        20, 22, 24, 26, 28, 29, 31, 33, 35
    };

    // UI Components
    private TMP_Text statusText;

    // Core
    private TaskCompletionSource<bool> phaseCompletionSource;
    //private bool isWaiting = false;
    public delegate Task InputHandler();
    public InputHandler InputDelegate;

    // UI Tags
    private string UITag = "RouletteGameUI";
    #endregion

    #region Execute Phase Logic
    public async Task ExecuteAsync(SharedDataSO sharedData, UIComponentCollectionSO uiComponents)
    {
        if (!InitializeUI(uiComponents)) return;

        int betType = sharedData.GetInt("type");
        int betAmount = sharedData.GetInt("BetAmount");
        int ball = sharedData.GetInt("ball");

        if (ball != -1 && ball!=0)
        {
            string betTypeStr = BetTypeTransfer(betType);
            bool isWin = DetermineWin(betType, ball);

            // ��s�Τ�l�B
            if (isWin)
            {
                int winnings = betAmount * 2;

                DataManager.Instance.playerData.AddValue("chips", winnings);

                await ShowDialogAsync($"Congratulation!You won {winnings}$");
            }
            else
            {
                await ShowDialogAsync($"Sorry, You loss {betAmount}$");
            }
        }
        else if(ball==0)
        {
            float payoutMultiplier = 1.5f;
            float tmp = betAmount;
            int winnings = (int)(tmp * payoutMultiplier+0.5);

            DataManager.Instance.playerData.AddValue("chips", winnings);

            await ShowDialogAsync($"Yay! You��ve just won 1.5 times your money�Xno strings attached!");

            await ShowDialogAsync($"Congratulation!You won {winnings}$");
        }

        CleanupUI();
    }

    string BetTypeTransfer(int betType)
    {
        string[] transfer = { "����", "�¦�", "1-18", "19-36" };

        if (betType < 0 || betType >= transfer.Length)
        {
            return "�L�Ī���`����";
        }

        return transfer[betType];
    }

    private bool DetermineWin(int betType, int ball)
    {
        switch (betType)
        {
            case 0: // ����
                return IsRed(ball);
            case 1: // �¦�
                return IsBlack(ball);
            case 2: // �p (1-18)
                return IsSmall(ball);
            case 3: // �j (19-36)
                return IsBig(ball);
            default:
                return false;
        }
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

        // �Ыة����UI�ե�
        statusText = uiComponents.CreateUIComponent<TMP_Text>("GameStatusText", canvas.transform);


        if (statusText == null)
        {
            Debug.LogError("Required UI components are not correctly created.");
            return false;
        }

        return true;
    }

    private void CleanupUI()
    {
        GameObject.Destroy(statusText.gameObject);
    }
    #endregion

    #region Support Functions

    public async Task InputAsync()
    {
        if (InputDelegate != null)
            await InputDelegate();
    }

    private async Task ShowDialogAsync(string text)
    {
        statusText.text = text;
        await Task.Delay(2000); // ����2��H�K�Τ�\Ū
    }
    #endregion

    #region EndStage Specific Functions
    /// <summary>
    /// �P�_�O�_���u�j�v
    /// </summary>
    public bool IsBig(int number)
    {
        return number >= 19 && number <= 36;
    }

    /// <summary>
    /// �P�_�O�_���u�p�v
    /// </summary>
    public bool IsSmall(int number)
    {
        return number >= 1 && number <= 18;
    }

    /// <summary>
    /// �P�_�O�_���u���v
    /// </summary>
    public bool IsRed(int number)
    {
        return RedNumbers.Contains(number);
    }

    /// <summary>
    /// �P�_�O�_���u�¡v
    /// </summary>
    public bool IsBlack(int number)
    {
        return BlackNumbers.Contains(number);
    }
    #endregion
}
