using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace UnityEngine
{
    public class ValueChange : MonoBehaviour
    {
        private static ValueChange _instance;
        public static ValueChange Instance => _instance ??= new ValueChange();

        public GameObject canvasObject;

        public TMP_Dropdown dropdown;
        public TMP_Text uiText;

        public Button selling;

        //兌換籌碼
        public Button exchangeChips;
        public TMP_InputField inputMoney;

        //換回錢
        public Button exchangeMoney;
        public TMP_InputField inputChips;

        private Dictionary<string, int> itemPrices; // 字典存儲選項與價格對應關係

        public void Init()
        {
            GameObject FindObject = GameObject.FindWithTag("ExchangeUI");
            canvasObject = FindObject;
            if (canvasObject != null)
            {
                Debug.LogError("cant find exchange ui!!!");
            }
            EventSystem.Instance.RegisterEvent<int>("Exchange", "callUI", Initialize);

            canvasObject.GetComponent<Canvas>().enabled = false;
        }

        private void Initialize(int tmp)
        {
            /*canvasObject = GameObject.FindWithTag("ExchangeUI");
            if (canvasObject != null)
            {
                Debug.LogError("cant find exchange ui!!!");
            }*/

            Debug.Log("Enter Initialize");
            GameObject FindObject = GameObject.FindWithTag("ExchangeUI");
            canvasObject = FindObject;
            if (canvasObject != null)
            {
                Debug.LogError("cant find exchange ui!!!");
            }
            canvasObject.GetComponent<Canvas>().enabled = true;
            //canvasObject.gameObject.GetComponent<Renderer>().enabled = true;


            // 初始化字典
            itemPrices = new Dictionary<string, int>
            {
                { "house", 1000 },
                { "kidney", 2000 },
                { "dignity", 50 }
            };

            // 初始化 UI
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
                    uiText.text = ""; // 若選項不在字典中，設置為空
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
                    // 根據選項添加金額
                    DataManager.Instance.playerData.AddValue("money", itemPrices[selectedText]);

                    // 刪除選項
                    dropdown.options.RemoveAt(dropdown.value);

                    // 確保顯示更新
                    if (dropdown.options.Count > 0)
                    {
                        dropdown.value = 0; // 選擇第一個選項
                        UpdateText(dropdown.value); // 更新文字
                    }
                    else
                    {
                        dropdown.value = -1; // 設置為無效值
                        uiText.text = "nothing"; // 顯示提示文字
                    }

                    // 刷新顯示
                    dropdown.RefreshShownValue();
                }
                else
                {
                    Debug.LogWarning("未處理的選項文字內容: " + selectedText);
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
}
