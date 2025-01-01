using UnityEngine;
using UnityEngine.UI;

public class ValueChange : MonoBehaviour
{
    public Dropdown dropdown;  // ��J�A�� Dropdown ����
    public Text uiText;        // ��J�ݭn��s�� Text ����

    private void Start()
    {
        // �T�O Dropdown �����ܤƮ�Ĳ�o��s��k
        if (dropdown != null)
        {
            dropdown.onValueChanged.AddListener(UpdateText);
        }
    }

    // �� Dropdown �ȧ��ܮɧ�s��r
    public void UpdateText(int index)
    {
        if (uiText != null)
        {
            uiText.text = $"{dropdown.options[index].text}: {index}"; // �۩w�q��ܮ榡
        }
    }
}
