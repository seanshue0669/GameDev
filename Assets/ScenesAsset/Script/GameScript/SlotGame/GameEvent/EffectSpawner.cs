using UnityEngine;
using System.Threading.Tasks;
using System.Collections;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public GameObject effectPrefab;  // ��J�S�Ī� Prefab
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
        yield return new WaitForSeconds(1f); // ��������

        // �T�{�S��Prefab�O�_�w���w
        if (effectPrefab != null)
        {
            // �p�G spawnPoint �Q���w�A�h�ϥΥ�����m�H��
            Vector3 spawnPosition = new Vector3(0.122230791f, 0.909935892f, -9.22999954f);

            // �q�{�L����
            Quaternion spawnRotation = Quaternion.identity;

            // �ͦ��S��
            GameObject spawnedEffect = Instantiate(effectPrefab, spawnPosition, spawnRotation);
            Debug.Log("Generated effect: " + spawnedEffect.name);

            // �i��G�]�m�S�Ħ۰ʾP��
            Destroy(spawnedEffect, 2f); // 2���۰ʾP���S�Ī���
        }
        else
        {
            Debug.LogError("Effect prefab is not assigned!");
        }

        // ������{
        yield break;
    }
}
