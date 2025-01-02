using System.Collections.Generic;
using UnityEngine;

public class PersistenceObject : MonoBehaviour
{
    private static HashSet<string> persistedObjects = new HashSet<string>();

    void Awake()
    {
        string objectID = gameObject.name;

        if (persistedObjects.Contains(objectID))
        {
            Destroy(gameObject);
        }
        else
        {
            persistedObjects.Add(objectID);
            DontDestroyOnLoad(gameObject);
        }
    }
}