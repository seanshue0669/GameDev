using UnityEngine;

public class LightController : MonoBehaviour
{
    [SerializeField] private GameObject lightParent;
    [SerializeField] private GameObject lightObject;
    [SerializeField] private float rotationSpeed = 1.0f;   
    [SerializeField] private float maxAngle = 30.0f;
    [SerializeField] private Color startColor = Color.red;
    [SerializeField] private Color endColor = Color.blue;
    [SerializeField] private float colorChangeSpeed = 1.0f;

    private Light lightComponent;

    private float timeElapsed = 0f;
    private Quaternion initRotation; 

    void Start()
    {

        if (lightParent == null)
        {
            lightParent = this.gameObject;
        }
        lightComponent = lightObject.GetComponent<Light>();
        initRotation = lightParent.transform.rotation;
    }
    void Update()
    {
        timeElapsed += Time.deltaTime * rotationSpeed;
        float rotationZ = Mathf.Sin(timeElapsed) * maxAngle;
        Quaternion newRotation = initRotation * Quaternion.Euler(0, 0, rotationZ);
        lightParent.transform.rotation = newRotation;

        float t = Mathf.PingPong(timeElapsed, 1f);
        Color currentColor = Color.Lerp(startColor, endColor, t);
        lightComponent.color = currentColor;
    }
}
