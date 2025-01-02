using UnityEngine;
using UnityEngine.UI;

public class MenuProbe : MonoBehaviour
{
    [SerializeField]
    public Toggle toggle;
    public GameObject m_gameObject;
    bool cur = false;
    void Update()
    {
        if(toggle.isOn != cur)
        {
            cur = toggle.isOn;
            m_gameObject.SetActive(toggle.isOn);
        }
    }
}
