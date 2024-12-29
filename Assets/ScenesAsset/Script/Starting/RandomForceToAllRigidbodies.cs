using UnityEngine;
using System.Collections;

public class RandomForceToAllRigidbodies : MonoBehaviour
{
    public float forceStrength = 10f;
    public float minInterval = 2f;
    public float maxInterval = 10f;

    [System.Obsolete]
    void Start()
    {
        Rigidbody[] allRigidbodies = FindObjectsOfType<Rigidbody>();

        foreach (Rigidbody rb in allRigidbodies)
        {
            StartCoroutine(ApplyRandomForceToRigidbody(rb));
        }
    }

    IEnumerator ApplyRandomForceToRigidbody(Rigidbody rb)
    {
        while (true)
        {
            float randomInterval = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(randomInterval);

            Vector3 randomDirection = new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(-0.2f, 0.2f),
                Random.Range(-1f, 1f)
            ).normalized;

            rb.AddForce(randomDirection * forceStrength, ForceMode.Impulse);
        }
    }
}
