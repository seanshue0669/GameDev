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

        // �����C�@�V���欰
        while (elapsed < duration)
        {
            this.transform.Rotate(0, 0, -600 * Time.deltaTime);

            elapsed += Time.deltaTime;

            // ���ݤU�@�V
            await Task.Yield();
        }

        // �̲׹���L��m�i��վ�
        this.transform.rotation = Quaternion.Euler(0, -90, (angle - 1) * -60);
        return angle;
    }
}
