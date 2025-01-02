using UnityEngine;

public class ProbeController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]
    public GameObject probes;
    void Start()
    {
        probes = GameObject.Find("probes");
        if (!RenderSettings.Instance.GI)
        {
            Debug.Log($"Disable{probes.name} probe");
            probes.gameObject.SetActive(false);
        }
    }
}
