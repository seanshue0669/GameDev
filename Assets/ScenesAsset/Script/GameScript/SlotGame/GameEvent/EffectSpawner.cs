using UnityEngine;
using System.Threading.Tasks;
using System.Collections;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public GameObject effectPrefab;  // 拖入特效的 Prefab
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EventSystem.Instance.RegisterEvent<int>("EffectSpawn", "Boom", GenerateEffect);
    }

    private void GenerateEffect(int triggerId)
    {
        StartCoroutine(GenerateEffectCoroutine());
    }

    private IEnumerator GenerateEffectCoroutine()
    {
        yield return new WaitForSeconds(1f); // 模擬延遲

        // 確認特效Prefab是否已指定
        if (effectPrefab != null)
        {
            // 如果 spawnPoint 被指定，則使用它的位置信息
            Vector3 spawnPosition = new Vector3(0.122230791f, 0.909935892f, -9.22999954f);

            // 默認無旋轉
            Quaternion spawnRotation = Quaternion.identity;

            // 生成特效
            GameObject spawnedEffect = Instantiate(effectPrefab, spawnPosition, spawnRotation);
            Debug.Log("Generated effect: " + spawnedEffect.name);

            // 可選：設置特效自動銷毀
            Destroy(spawnedEffect, 2f); // 2秒後自動銷毀特效物件
        }
        else
        {
            Debug.LogError("Effect prefab is not assigned!");
        }

        // 結束協程
        yield break;
    }
}
