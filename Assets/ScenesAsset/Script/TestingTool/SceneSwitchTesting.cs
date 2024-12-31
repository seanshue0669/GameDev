using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Scene;
using System.Collections.Generic;
using System.Diagnostics;
namespace UnityEngine
{
    public class SceneSwitchTesting : MonoBehaviour
    {
        public TMP_Dropdown dropdown;
        private int currentIndex;
        private SceneType m_Scene;
        private Dictionary<string, string> SceneTypeToGameName = new Dictionary<string, string>
            {
                { "DiceScene", "DiceGame" },
                { "PokerScene", "PokerGame" },
                { "SlotScene", "RouletteGame" },
                { "WheelScene", "WheelGame" }
            };

        void Start()
        {
            string currentScene = SceneManager.GetActiveScene().name;
            currentIndex = SceneManager.GetActiveScene().buildIndex;
            dropdown.value = currentIndex;
            dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
        }

        async void OnDropdownValueChanged(int index)
        {
            currentIndex = index;
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(currentIndex);
            await loadOperation;
            string sceneName = ((SceneType)currentIndex).ToString();
            Debug.Log($"Switch To {sceneName}");
            if (SceneTypeToGameName.TryGetValue(sceneName, out string gameName))
            {
                Debug.Log($"Perpare {gameName}");
                EventSystem.Instance.TriggerEvent("SwitchScene", "StartGame", gameName);
            }
        }
    }
}
