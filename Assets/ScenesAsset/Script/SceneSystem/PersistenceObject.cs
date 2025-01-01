using UnityEngine;

public class PersistenceObject : MonoBehaviour
{
    [System.Obsolete]
    private void Awake()
    {

        DontDestroyOnLoad(gameObject);
        //Debug.Log(gameObject.name);
    }
}