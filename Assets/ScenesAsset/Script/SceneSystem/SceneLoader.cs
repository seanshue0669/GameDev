using System;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System.Collections;

namespace Scene
{
    public class SceneLoader : MonoBehaviour
    {
        private Dictionary<string, string> SceneTypeToGameName = new Dictionary<string, string>
        {
            { "DiceScene", "DiceGame" },
            { "PokerScene", "PokerGame" },
            { "SlotScene", "RouletteGame" },
            { "WheelScene", "WheelGame" }
        };
        
        public async void LoadScene(string p_sceneTypeString)
        {
            SceneType sceneEnum= PhasingString2Enum(p_sceneTypeString);
            if (sceneEnum == SceneType.Deflaut)
            {
                Debug.LogWarning("SceneType Error or SceneType No Exist!");
                return;
            }
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync((int)sceneEnum);
            Debug.Log($"LoadScene: {p_sceneTypeString}");
            loadOperation.allowSceneActivation = false;
            await loadOperation;
            loadOperation.allowSceneActivation = true;
            Debug.Log("Scene fully loaded");
            await Task.Delay(500);
            if (SceneTypeToGameName.TryGetValue(p_sceneTypeString, out string gameName))
            {
                Debug.Log($"StartGame:{gameName}");
                EventSystem.Instance.TriggerEvent("SwitchScene", "StartGame", gameName);
            }           
        }

        #region Tool Function
        private SceneType PhasingString2Enum(string sceneName)
        {
            return (Enum.TryParse(sceneName, out SceneType sceneTypeEnum)) ? sceneTypeEnum : SceneType.Deflaut;
        }
        #endregion
    }
}
