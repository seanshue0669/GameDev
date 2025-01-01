using UnityEngine;

public class PersistenceObject : MonoBehaviour
{
    [System.Obsolete]
    private void Awake()
    {
        if (FindObjectsOfType<PersistenceObject>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }
}