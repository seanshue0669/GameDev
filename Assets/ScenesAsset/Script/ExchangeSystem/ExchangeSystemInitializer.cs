using UnityEngine;

public class ExchangeSystemInitializer : MonoBehaviour
{
    private void Awake()
    {

        ValueChange.Instance.Init();
    }
}
