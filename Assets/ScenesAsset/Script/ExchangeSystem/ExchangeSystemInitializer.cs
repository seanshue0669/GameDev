using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ExchangeSystemInitializer : MonoBehaviour
{
    private void Awake()
    {
        ValueChange.Instance.Init();
    }
}
