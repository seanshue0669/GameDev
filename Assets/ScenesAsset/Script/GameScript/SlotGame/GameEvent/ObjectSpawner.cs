using UnityEngine;
using System.Threading.Tasks;
using System.Collections;

public class ObjectSpawner : MonoBehaviour
{
    [SerializeField]
    public GameObject temp;
    
    public GameObject objectPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EventSystem.Instance.RegisterEvent<int>("ObjectSpawn", "Goat", GenerateObject);
    }

    private void GenerateObject(int x)
    {
        StartCoroutine(GenerateObjectCoroutine());
    }

    private IEnumerator GenerateObjectCoroutine()
    {
        yield return new WaitForSeconds(1f); // 模擬延遲
        // 檢查Prefab是否存在
        if (objectPrefab != null)
        {
            // 指定生成位置和旋轉
            Vector3 spawnPosition = new Vector3(0.122230791f, 0.909935892f, -9.22999954f);
            Quaternion spawnRotation = Quaternion.identity;

            // 生成物體
            GameObject spawnedObject = Instantiate(objectPrefab, spawnPosition, spawnRotation);
            Debug.Log("Generated object: " + spawnedObject.name);
        }
        else
        {
            Debug.LogError("Object prefab is not assigned!");
        }
        // 結束協程
        yield break;
    }
}