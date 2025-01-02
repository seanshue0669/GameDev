using UnityEngine;

public class AssetDisplayInitializer : MonoBehaviour
{
    void Start()
    {
        EventSystem.Instance.TriggerEvent("Assetdisplay", "display", 0);
    }
}
