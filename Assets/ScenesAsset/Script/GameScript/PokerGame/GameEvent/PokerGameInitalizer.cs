using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;

public class PokerGameInitalizer : MonoBehaviour
{
    [SerializeField]
    string[] objPath;
    bool des = true;
    private void Awake()
    {
        if (des)
            PokerGameEvent.Instance.Init(LoadObjectByFilePath());
        des = false;
    }
    private List<GameObject> LoadObjectByFilePath()
    {
        List <GameObject> passingData=new List<GameObject>();
        for (int i=0;i<objPath.Length;i++)
        {
            Object[] prefabs = Resources.LoadAll(objPath[i]);

            foreach (var prefab in prefabs)
            {
                passingData.Add(prefab as GameObject);
            }
            
        }
        return passingData;
    }
}
