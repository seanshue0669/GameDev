using UnityEngine;
using System.Collections;

public class EffectSpawner : MonoBehaviour
{
    private static EffectSpawner _instance;
    public static EffectSpawner Instance => _instance ??= new EffectSpawner();

    public GameObject effectPrefab;

    public float moveSpeed = 0.0001f;
    public float moveDuration = 2f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Init()
    {
        EventSystem.Instance.RegisterEvent<int>("EffectSpawn", "Boom", GenerateEffect);
    }

    private void GenerateEffect(int triggerId)
    {
        StartCoroutine(GenerateEffectCoroutine());
    }

    private IEnumerator GenerateEffectCoroutine()
    {
        yield return new WaitForSeconds(1f);

        if (effectPrefab != null)
        {
            for (int i = 0; i < 50; i++)
            {
                Vector3 spawnPosition = new Vector3(0.0149999997f, 0.458365202f, -6.34700012f); //Vector3(0.0f, 1.0f, -9.5f);
                Vector3 spawnRotation = new Vector3(90.0f, 90.0f, -90.0f);
                GameObject instance = Instantiate(effectPrefab, spawnPosition, Quaternion.Euler(spawnRotation));
                StartCoroutine(MoveObject(instance));
            }
            yield return new WaitForSeconds(0.5f);

            //Quaternion spawnRotation = Quaternion.identity;
            //GameObject spawnedEffect = Instantiate(effectPrefab, spawnPosition, spawnRotation);
            //Debug.Log("Generated effect: " + spawnedEffect.name);
            //Destroy(spawnedEffect, 2f);
        }
        else
        {
            Debug.LogError("Effect prefab is not assigned!");
        }

        yield break;
    }

    private IEnumerator MoveObject(GameObject obj)
    {
        float minValue = -1.0f, maxValue = 1.0f;
        Vector3 randomDirection = new Vector3(UnityEngine.Random.Range(minValue, maxValue),
                                              UnityEngine.Random.Range(minValue, maxValue),
                                              UnityEngine.Random.Range(minValue, maxValue));
        float elapsedTime = 0;
        while (elapsedTime < moveDuration)
        {
            if (obj == null) yield break;
            obj.transform.position += randomDirection * moveSpeed * Time.deltaTime;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        yield return null;
    }
}
