using UnityEngine;
using System.Threading.Tasks;

public class RotationListener : MonoBehaviour
{
    private static RotationListener _instance;
    public static RotationListener Instance => _instance ??= new RotationListener();

    [SerializeField]
    public GameObject WheelObject1;
    public GameObject WheelObject2;
    public GameObject WheelObject3;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Init()
    {
        WheelObject1 = GameObject.Find("wheel1");
        WheelObject2 = GameObject.Find("wheel2");
        WheelObject3 = GameObject.Find("wheel3");
        EventSystem.Instance.RegisterCallBack<int, int>("Execute", "ReelRotation1", Rotate1);
        EventSystem.Instance.RegisterCallBack<int, int>("Execute", "ReelRotation2", Rotate2);
        EventSystem.Instance.RegisterCallBack<int, int>("Execute", "ReelRotation3", Rotate3);
    }

    private async Task<int> Rotate1(int angle)
    {
        float duration = 1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            WheelObject1.transform.Rotate(0, 0, -600 * Time.deltaTime);

            elapsed += Time.deltaTime;

            await Task.Yield();
        }

        WheelObject1.transform.rotation = Quaternion.Euler(0, -90, (angle - 1) * -60);
        return angle;
    }
    private async Task<int> Rotate2(int angle)
    {
        float duration = 1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            WheelObject2.transform.Rotate(0, 0, -600 * Time.deltaTime);

            elapsed += Time.deltaTime;

            await Task.Yield();
        }

        WheelObject2.transform.rotation = Quaternion.Euler(0, -90, (angle - 1) * -60);
        return angle;
    }
    private async Task<int> Rotate3(int angle)
    {
        float duration = 1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            WheelObject3.transform.Rotate(0, 0, -600 * Time.deltaTime);

            elapsed += Time.deltaTime;

            await Task.Yield();
        }

        WheelObject3.transform.rotation = Quaternion.Euler(0, -90, (angle - 1) * -60);
        return angle;
    }

}
