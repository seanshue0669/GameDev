using UnityEngine;

public class rouletteballthrowout : MonoBehaviour
{
    private static rouletteballthrowout _instance;
    public static rouletteballthrowout Instance => _instance ??= new rouletteballthrowout();
    
    public Transform ballSelf;

    public Rigidbody rb;
    void Awake()
    {
        EventSystem.Instance.RegisterEvent<float>("Roulette", "throwout", ApplyForce);
    }

    public void ApplyForce(float mag)
    {
        //rb = GameObject.Find("ball").GetComponent<Rigidbody>();
        //GameObject.Find("ball").transform.localPosition = new Vector3(0.128900006f, -0.068599999f, -0.2861f);
        GameObject FindObject = GameObject.FindWithTag("RouletteObject");
        if (FindObject == null)
        {
            Debug.LogError("cant find RouletteObject!!!");
        }

        ballSelf = TransformExtensions.FindChildComponent<Transform>(FindObject.transform, "ballcenter/ball");
        rb = TransformExtensions.FindChildComponent<Rigidbody>(FindObject.transform, "ballcenter/ball");

        ballSelf.localPosition = new Vector3(0.128900006f, -0.068599999f, -0.2861f);
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

