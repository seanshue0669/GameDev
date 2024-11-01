using UnityEngine;

public class ApplyReflectedForceOnCollision : MonoBehaviour
{
    public float minForceStrength = 5f;
    public float maxForceStrength = 15f;

    void OnCollisionEnter(Collision collision)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            ContactPoint contact = collision.contacts[0];
            Vector3 incomingDirection = collision.relativeVelocity;
            Vector3 reflectedDirection = Vector3.Reflect(incomingDirection, contact.normal).normalized;
            float randomForceStrength = Random.Range(minForceStrength, maxForceStrength);
            rb.AddForce(reflectedDirection * randomForceStrength, ForceMode.Impulse);
        }
    }
}
