using UnityEngine;
using UnityEngine.UI;

public class PopUpHint : MonoBehaviour
{
    [SerializeField]
    public Toggle controllToggle;
    public void Awake()
    {
        gameObject.SetActive(false);
    }
    public void TriggerPokerPopUp()
    {
        gameObject.SetActive(controllToggle.isOn);
    }
}
