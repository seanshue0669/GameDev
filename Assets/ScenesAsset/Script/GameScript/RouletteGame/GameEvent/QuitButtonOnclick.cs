using UnityEngine;
using UnityEngine.UI;

public class QuitButtonclick : MonoBehaviour
{
    void Start()
    {
        // 確保 Button 組件存在並自動綁定事件
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
        }
    }

    public void OnButtonClick()
    {
        if(!DataManager.Instance.playerData.GetValue<bool>("isRouletteSpinning"))
            EventSystem.Instance.TriggerEvent("Roulette", "quit", 0);
    }
}