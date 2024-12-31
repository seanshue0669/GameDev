using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PokerGameInitalizer : MonoBehaviour
{
    [SerializeField]
    GameObject[] Objects;

    private void Awake()
    {
        PokerGameEvent.Instance.Init(Objects);
    }
    /*private List<GameObject> LoadObjectByFilePath()
    {
        List <GameObject> passingData=new List<GameObject>();
        for (int i=0;i<objPath.Length;i++)
        {
            Object[] prefabs = Resources.LoadAll<GameObject>(objPath[i]);
            foreach (GameObject prefab in prefabs)
            {
                passingData.Add(prefab);
            }
        }
        return passingData;
    }*/
}
