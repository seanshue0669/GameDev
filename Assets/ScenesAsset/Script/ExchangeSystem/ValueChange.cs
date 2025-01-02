using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

namespace UnityEngine
{
    public class ValueChange : MonoBehaviour
    {
        #region Define Variable
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
        #endregion

        #region init
        public void Init()
        {
            GameObject FindObject = GameObject.FindWithTag("ExchangeUI");
            canvasObject = FindObject;
            if (canvasObject == null)
            {
                Debug.LogError("cant find exchange ui!!!????");
            }

            EventSystem.Instance.RegisterEvent<int>("Exchange", "callUI", Initialize);

            selling = TransformExtensions.FindChildComponent<Button>(canvasObject.transform, "Selling");
            exchangeChips = TransformExtensions.FindChildComponent<Button>(canvasObject.transform, "ExchangeChips");
            exchangeMoney = TransformExtensions.FindChildComponent<Button>(canvasObject.transform, "ExchangeMoney");
            exit = TransformExtensions.FindChildComponent<Button>(canvasObject.transform, "exit");

            dropdown = TransformExtensions.FindChildComponent<TMP_Dropdown>(canvasObject.transform, "Question/Dropdown"); // 替換 "DropdownName" 為實際名稱
            uiText = TransformExtensions.FindChildComponent<TMP_Text>(canvasObject.transform, "Question/value"); // 替換 "UITextName" 為實際名稱
            inputMoney = TransformExtensions.FindChildComponent<TMP_InputField>(canvasObject.transform, "InputMoney"); // 替換 "InputMoneyName" 為實際名稱
            inputChips = TransformExtensions.FindChildComponent<TMP_InputField>(canvasObject.transform, "InputChips"); // 替換 "InputChipsName" 為實際名稱

            canvasObject.GetComponent<Canvas>().enabled = false;
        }

        #endregion

        #region buttonListener
        private void AddButtonListeners()
        {
            // 確保不重複添加監聽器
            selling.onClick.RemoveListener(SellingOnClick);
            selling.onClick.AddListener(SellingOnClick);

            exchangeChips.onClick.RemoveListener(exchangeChipsOnClick);
            exchangeChips.onClick.AddListener(exchangeChipsOnClick);

            exchangeMoney.onClick.RemoveListener(exchangeMoneyOnClick);
            exchangeMoney.onClick.AddListener(exchangeMoneyOnClick);

            exit.onClick.RemoveListener(exitOnClick);
            exit.onClick.AddListener(exitOnClick);
        }

        private void RemoveButtonListeners()
        {
            selling.onClick.RemoveListener(SellingOnClick);
            exchangeChips.onClick.RemoveListener(exchangeChipsOnClick);
            exchangeMoney.onClick.RemoveListener(exchangeMoneyOnClick);
            exit.onClick.RemoveListener(exitOnClick);
        }
        #endregion

        #region ExchangeRegisteredEvent
        public void Initialize(int tmp)
        {
            GetComponent<AudioSource>().Play(0); // play dave sound
            Debug.Log("Enter Initialize");

            if (canvasObject == null)
            {
                Debug.LogError("ExchangeUI 未找到！");
                return;
            }

            canvasObject.GetComponent<Canvas>().enabled = true;

            // 初始化字典
            itemPrices = new Dictionary<string, int>
            {
                { "house", 1000 },
                { "kidney", 2000 },
                { "dignity", 50 }
            };

            // 初始化 Dropdown
            dropdown.onValueChanged.RemoveAllListeners();
            dropdown.onValueChanged.AddListener(UpdateText);

            RebuildDropdownOptions();

            if (dropdown.options.Count > 0)
            {
                dropdown.value = 0;
                UpdateText(dropdown.value);
            }
            else
            {
                uiText.text = "0";
            }

            // 添加按鈕監聽
            AddButtonListeners();

            DataManager.Instance.playerData.SetValue("canMoving", false);

            inputChips.text = "";
            inputMoney.text = "";

            Cursor.lockState = CursorLockMode.None;
        }
        #endregion

        #region selling

        //change the value of each goods
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
                    uiText.text = "0";
                }
            }
            else
            {
                uiText.text = "0";
            }
        }

        // add unsold goods onto dropdown
        private void RebuildDropdownOptions()
        {
            if (dropdown == null)
            {
                Debug.LogError("Dropdown null！");
                return;
            }

            dropdown.ClearOptions();
            List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

            foreach (var item in itemPrices)
            {
                if (DataManager.Instance.playerData.GetValue<bool>(item.Key))
                {
                    options.Add(new TMP_Dropdown.OptionData(item.Key));
                }
            }

            if (options.Count == 0)
            {
                options.Add(new TMP_Dropdown.OptionData("nothing"));
            }

            dropdown.AddOptions(options);
            dropdown.RefreshShownValue();
        }

        private void SellingOnClick()
        {
            if (dropdown.options.Count > 0 && dropdown.value >= 0)
            {
                string selectedText = dropdown.options[dropdown.value].text;

                if (itemPrices.TryGetValue(selectedText, out int price))
                {
                    if (selectedText == "house") AchievementManager.instance.Unlock("Moving here");
                    else if (selectedText == "kidney") AchievementManager.instance.Unlock("Home alone");
                    else if (selectedText == "dignity") AchievementManager.instance.Unlock("That's it...");
                    DataManager.Instance.playerData.AddValue("money", price);
                    DataManager.Instance.playerData.SetValue(selectedText, false);

                    RebuildDropdownOptions();

                    if (dropdown.options.Count > 0 && !dropdown.options[0].text.Equals("暂无可售物品"))
                    {
                        dropdown.value = 0;
                        UpdateText(dropdown.value);
                    }
                    else
                    {
                        uiText.text = "Nothing";
                    }

                    dropdown.RefreshShownValue();
                }
                else
                {
                    Debug.LogWarning($"未處理的選項: {selectedText}");
                }
            }
            else
            {
                Debug.LogWarning("沒有可售物品或選項索引無效！");
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
                    DataManager.Instance.SubAndDisplayValue("money", chips);
                    DataManager.Instance.AddAndDisplayValue("chips", chips);
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
                    DataManager.Instance.SubAndDisplayValue("chips", money);
                    DataManager.Instance.AddAndDisplayValue("money", money);
                    
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

        #region exit
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

            RemoveButtonListeners();
            
            dropdown.onValueChanged.RemoveListener(UpdateText);

            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        }
        #endregion
    }
}