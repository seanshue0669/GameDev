using UnityEngine;
using System.Threading.Tasks;
using System.Collections;

public class RotationListener : MonoBehaviour
{
    [SerializeField]
    public string identifier;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EventSystem.Instance.RegisterCallBack<int, int>("Execute", identifier, Rotate);
    }

    private async Task<int> Rotate(int angle)
    {
        float duration = 1f;
        float elapsed = 0f;

        // 模擬每一幀的行為
        while (elapsed < duration)
        {
            this.transform.Rotate(0, 0, -600 * Time.deltaTime);

            elapsed += Time.deltaTime;

            // 等待下一幀
            await Task.Yield();
        }

        // 最終對轉盤位置進行調整
        this.transform.rotation = Quaternion.Euler(0, -90, (angle - 1) * -60);
        return angle;
    }
}
