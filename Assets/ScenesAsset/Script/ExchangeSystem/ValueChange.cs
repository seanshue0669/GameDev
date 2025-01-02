using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.ReorderableList.Internal;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.StandaloneInputModule;

namespace UnityEngine
{
    public class ValueChange : MonoBehaviour
    {
        private static ValueChange _instance;
        public static ValueChange Instance
        {
            get
            {
                if (_instance == null)
                {
                    // 嘗試去找場景中的實例
                    _instance = Object.FindAnyObjectByType<ValueChange>();

                    // 如果找不到就自己想辦法生成，或是直接報錯，都可
                    if (_instance == null)
                    {
                        Debug.LogError("There is no ValueChange in the scene!");
                    }
                }
                return _instance;
            }
        }

        private void Awake()
        {
            // 保證場上只有一個實例
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
        }

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

        public Button exit;

        private Dictionary<string, int> itemPrices; // 字典存儲選項與價格對應關係

        public void Init()
        {
            GameObject FindObject = GameObject.FindWithTag("ExchangeUI");
            canvasObject = FindObject;
            if (canvasObject == null)
            {
                Debug.LogError("cant find exchange ui!!!????");
            }
            EventSystem.Instance.RegisterEvent<int>("Exchange", "callUI", Initialize);

            canvasObject.GetComponent<Canvas>().enabled = false;
        }

        public void Initialize(int tmp)
        {
            GetComponent<AudioSource>().Play(0); // play dave sound
            Debug.Log("Enter Initialize");

            if (canvasObject == null)
            {
                Debug.LogError("cant find exchange ui!!!!!!!!");
            }
            canvasObject.GetComponent<Canvas>().enabled = true;

            // 初始化字典
            itemPrices = new Dictionary<string, int>
            {
                { "house", 1000 },
                { "kidney", 2000 },
                { "dignity", 50 }
            };

            selling.onClick.AddListener(SellingOnClick);
            exchangeChips.onClick.AddListener(exchangeChipsOnClick);
            exchangeMoney.onClick.AddListener(exchangeMoneyOnClick);
            exit.onClick.AddListener(exitOnClick);

            // 初始化 UI
            if (dropdown.options.Count > 0)
            {
                uiText.text = $"{itemPrices[dropdown.options[0].text]}$";
            }

            if (dropdown != null)
            {
                dropdown.onValueChanged.AddListener(UpdateText);
            }

            DataManager.Instance.playerData.SetValue("canMoving", false);

            inputChips.text = "";
            inputMoney.text = "";

            UnityEngine.Cursor.lockState = CursorLockMode.None;
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
                    uiText.text = ""; 
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
            if (IsInteger(inputMoney.text))
            {
                int chips = int.Parse(inputMoney.text);

                if (chips <= DataManager.Instance.playerData.GetValue<int>("money"))
                {
                    DataManager.Instance.playerData.SubValue("money", chips);
                    DataManager.Instance.playerData.AddValue("chips", chips);
                }
            }

            inputMoney.text = "";
        }

        void exchangeMoneyOnClick()
        {
            if (IsInteger(inputChips.text))
            {
                int money = int.Parse(inputChips.text);

                if (money <= DataManager.Instance.playerData.GetValue<int>("chips"))
                {
                    DataManager.Instance.playerData.SubValue("chips", money);
                    DataManager.Instance.playerData.AddValue("money", money);
                }
            }

            inputChips.text = "";
        }

        public bool IsInteger(string input)
        {
            // 移除輸入字串前後的空白字符
            string trimmedInput = input.Trim();

            // 嘗試將字串轉換為整數
            int result;
            bool isInteger = int.TryParse(trimmedInput, out result);

            return isInteger;
        }
        #endregion

        private void exitOnClick()
        {
            DataManager.Instance.playerData.SetValue("canMoving", true);

            GameObject FindObject = GameObject.FindWithTag("ExchangeUI");
            canvasObject = FindObject;

            if (canvasObject == null)
            {
                Debug.LogError("cant find exchange ui!!!");
            }

            canvasObject.GetComponent<Canvas>().enabled = false;

            selling.onClick.RemoveListener(SellingOnClick);
            exchangeChips.onClick.RemoveListener(exchangeChipsOnClick);
            exchangeMoney.onClick.RemoveListener(exchangeMoneyOnClick);
            exit.onClick.RemoveListener(exitOnClick);

            dropdown.onValueChanged.RemoveListener(UpdateText);

            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        }
    }

}