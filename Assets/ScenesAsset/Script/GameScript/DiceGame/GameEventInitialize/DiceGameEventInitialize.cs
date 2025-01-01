using System.Diagnostics;
using UnityEngine;
namespace UnityEngine
{
    public class DiceGameEventInitiler : MonoBehaviour
    {
        void Awake()
        {
            EventSystem.Instance.RegisterEvent<string>("DiceGameEventTest", "Sendmsg", Sendmsg);
            Debug.Log("Reg Sendmsg Event");
            EventSystem.Instance.TriggerEvent<string>("DiceGameEventTest", "Sendmsg", "Testing1");
        }
        void Sendmsg(string p_msg)
        {
            Debug.Log(p_msg);
        }
    }
}
