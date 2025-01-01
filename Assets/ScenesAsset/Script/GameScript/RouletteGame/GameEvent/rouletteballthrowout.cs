using UnityEngine;

public class rouletteballthrowout : MonoBehaviour
{
    private static rouletteballthrowout _instance;
    public static rouletteballthrowout Instance => _instance ??= new rouletteballthrowout();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    Rigidbody rb;
    public void Init()
    {
        EventSystem.Instance.RegisterEvent<float>("Roulette", "throwout", ApplyForce);
    }

    public void ApplyForce(float mag)
    {
        rb = GameObject.Find("ball").GetComponent<Rigidbody>();
        GameObject.Find("ball").transform.localPosition = new Vector3(0.128900006f, -0.068599999f, -0.2861f);
        rb.isKinematic = false;
        rb.useGravity = true;

        float forceAmount = mag;

        if (rb != null)
        {
            Vector3 force = new Vector3(0, 0, forceAmount);
            rb.AddForce(force, ForceMode.Impulse);
        }
        else
        {
            Debug.LogError("未找到Rigidbody組件！");
        }
    }
}

