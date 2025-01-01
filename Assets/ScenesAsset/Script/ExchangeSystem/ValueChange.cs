using UnityEngine;
using UnityEngine.UI;

public class ValueChange : MonoBehaviour
{
    public Dropdown dropdown;  // 拖入你的 Dropdown 元件
    public Text uiText;        // 拖入需要更新的 Text 元件

    private void Start()
    {
        // 確保 Dropdown 的值變化時觸發更新方法
        if (dropdown != null)
        {
            dropdown.onValueChanged.AddListener(UpdateText);
        }
    }

    // 當 Dropdown 值改變時更新文字
    public void UpdateText(int index)
    {
        if (uiText != null)
        {
            uiText.text = $"{dropdown.options[index].text}: {index}"; // 自定義顯示格式
        }
    }
}
