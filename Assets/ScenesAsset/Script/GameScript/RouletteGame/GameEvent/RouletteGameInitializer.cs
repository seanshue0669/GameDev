using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RouletteGameInitializer : MonoBehaviour
{
    private void Awake()
    {
        rouletteballthrowout.Instance.Init();
        RouletteSpinListener.Instance.Init();
    }
}
