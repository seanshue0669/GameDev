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
                    // ���եh������������
                    _instance = Object.FindAnyObjectByType<ValueChange>();

                    // �p�G�䤣��N�ۤv�Q��k�ͦ��A�άO���������A���i
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
            // �O�ҳ��W�u���@�ӹ��
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

        //�I���w�X
        public Button exchangeChips;
        public TMP_InputField inputMoney;

        //���^��
        public Button exchangeMoney;
        public TMP_InputField inputChips;

        public Button exit;

        private Dictionary<string, int> itemPrices; // �r��s�x�ﶵ�P����������Y

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

            // ��l�Ʀr��
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

            // ��l�� UI
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
            // ������J�r��e�᪺�ťզr��
            string trimmedInput = input.Trim();

            // ���ձN�r���ഫ�����
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