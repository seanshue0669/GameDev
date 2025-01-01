using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ValueChange : MonoBehaviour
{
    private static ValueChange _instance;
    public static ValueChange Instance => _instance ??= new ValueChange();

    private GameObject canvasObject;

    public TMP_Dropdown dropdown;
    public TMP_Text uiText;

    public Button selling;

    //�I���w�X
    public Button exchangeChips;
    public TMP_InputField inputMoney;

    //���^��
    public Button exchangeMoney;
    public TMP_InputField inputChips;

    private Dictionary<string, int> itemPrices; // �r��s�x�ﶵ�P����������Y

    public void Init()
    {
        canvasObject = GameObject.FindWithTag("ExchangeUI");
        if (canvasObject != null)
        {
            Debug.LogError("cant find exchange ui!!!");
        }
        EventSystem.Instance.RegisterEvent<int>("Exchange", "callUI", Initialize);

        canvasObject.transform.gameObject.SetActive(false);
    }

    private void Initialize(int tmp)
    {
        canvasObject = GameObject.FindWithTag("ExchangeUI");
        if (canvasObject != null)
        {
            Debug.LogError("cant find exchange ui!!!");
        }

        Debug.Log("hihi");
        canvasObject.transform.gameObject.SetActive(true);


        // ��l�Ʀr��
        itemPrices = new Dictionary<string, int>
        {
            { "house", 1000 },
            { "kidney", 2000 },
            { "dignity", 50 }
        };

        // ��l�� UI
        if (dropdown.options.Count > 0)
        {
            uiText.text = $"{itemPrices[dropdown.options[0].text]}$";
        }

        if (dropdown != null)
        {
            dropdown.onValueChanged.AddListener(UpdateText);
        }

        selling.onClick.AddListener(SellingOnClick);
        exchangeChips.onClick.AddListener(exchangeChipsOnClick);
        exchangeMoney.onClick.AddListener(exchangeMoneyOnClick);

        Cursor.lockState = CursorLockMode.None;
    }

    #region selling
    public void UpdateText(int index)
    {
        if (uiText != null && dropdown.options.Count > 0)
        {
            string selectedText = dropdown.options[index].text;

            if (itemPrices.ContainsKey(selectedText))
            {
                uiText.text = $"{itemPrices[selectedText]}$";
            }
            else
            {
                uiText.text = ""; // �Y�ﶵ���b�r�夤�A�]�m����
            }
        }
    }

    public void SellingOnClick()
    {
        if (dropdown.options.Count > 0)
        {
            string selectedText = dropdown.options[dropdown.value].text;

            if (itemPrices.ContainsKey(selectedText))
            {
                // �ھڿﶵ�K�[���B
                DataManager.Instance.playerData.AddValue("money", itemPrices[selectedText]);

                // �R���ﶵ
                dropdown.options.RemoveAt(dropdown.value);

                // �T�O��ܧ�s
                if (dropdown.options.Count > 0)
                {
                    dropdown.value = 0; // ��ܲĤ@�ӿﶵ
                    UpdateText(dropdown.value); // ��s��r
                }
                else
                {
                    dropdown.value = -1; // �]�m���L�ĭ�
                    uiText.text = "nothing"; // ��ܴ��ܤ�r
                }

                // ��s���
                dropdown.RefreshShownValue();
            }
            else
            {
                Debug.LogWarning("���B�z���ﶵ��r���e: " + selectedText);
            }
        }
    }
    #endregion

    #region Exchange
    void exchangeChipsOnClick()
    {
        int chips = int.Parse(inputMoney.text);

        if (chips <= DataManager.Instance.playerData.GetValue<int>("money"))
        {
            DataManager.Instance.playerData.SubValue("money", chips);
            DataManager.Instance.playerData.AddValue("chips", chips);
        }
    }

    void exchangeMoneyOnClick()
    {
        int money = int.Parse(inputMoney.text);

        if (money <= DataManager.Instance.playerData.GetValue<int>("chips"))
        {
            DataManager.Instance.playerData.SubValue("chips", money);
            DataManager.Instance.playerData.AddValue("money", money);
        }
    }
    #endregion
}
