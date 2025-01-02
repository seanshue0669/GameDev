using UnityEngine;
using UnityEngine.UI;

public class QuitButtonclick : MonoBehaviour
{
    void Start()
    {
        // �T�O Button �ե�s�b�æ۰ʸj�w�ƥ�
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