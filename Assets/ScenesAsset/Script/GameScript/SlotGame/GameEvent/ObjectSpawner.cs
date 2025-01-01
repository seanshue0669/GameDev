using UnityEngine;
using System.Threading.Tasks;
using System.Collections;

public class ObjectSpawner : MonoBehaviour
{
    private static ObjectSpawner _instance;
    public static ObjectSpawner Instance => _instance ??= new ObjectSpawner();

    [SerializeField]
    public GameObject[] objectPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Init()
    {
        EventSystem.Instance.RegisterEvent<int>("ObjectSpawn", "Goat", GenerateObject);
    }

    private void GenerateObject(int x)
    {
        StartCoroutine(GenerateObjectCoroutine());
    }

    private IEnumerator GenerateObjectCoroutine()
    {
        yield return new WaitForSeconds(1f);
        if (objectPrefab[0] != null)
        {
            Vector3 spawnPosition = new Vector3(0.00800000038f, 0.741999984f, -9.22999954f);
            Quaternion spawnRotation = Quaternion.identity;

            GameObject spawnedObject = Instantiate(objectPrefab[0], spawnPosition, spawnRotation);
            Debug.Log("Generated object: " + spawnedObject.name);
        }
        else
        {
            Debug.LogError("Object prefab is not assigned!");
        }
        yield break;
    }
}