using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;

public class SlotGameInitializer : MonoBehaviour
{
    private void Awake()
    {
        Debug.Log("Trigger SlotGame EventInitializer");
        EffectSpawner.Instance.Init();
        ObjectSpawner.Instance.Init();
        RotationListener.Instance.Init();
        VideoPopupWindow.Instance.Init();
    }
    
}