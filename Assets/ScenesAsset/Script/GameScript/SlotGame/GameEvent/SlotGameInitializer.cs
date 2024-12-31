using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SlotGameInitalizer : MonoBehaviour
{
    private void Awake()
    {
        SlotGameListener.Instance.Init();
    }

}