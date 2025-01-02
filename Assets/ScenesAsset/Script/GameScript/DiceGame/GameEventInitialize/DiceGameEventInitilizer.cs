using UnityEngine;

public class DiceGameEventInitilizer : MonoBehaviour
{
    void Awake()
    {
        DiceGameEvent.Instance.InitEvent();
    }
}
