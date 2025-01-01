using UnityEngine;
using System.Threading.Tasks;
using System.Collections;

public class RotationListener : MonoBehaviour
{
    private static RotationListener _instance;
    public static RotationListener Instance => _instance ??= new RotationListener();

    [SerializeField]
    public string identifier;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Init()
    {
        EventSystem.Instance.RegisterCallBack<int, int>("Execute", identifier, Rotate);
    }

    private async Task<int> Rotate(int angle)
    {
        float duration = 1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            this.transform.Rotate(0, 0, -600 * Time.deltaTime);

            elapsed += Time.deltaTime;

            await Task.Yield();
        }

        this.transform.rotation = Quaternion.Euler(0, -90, (angle - 1) * -60);
        return angle;
    }
}
