using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    #region Variables
    private CharacterController m_characterController;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;

    private Vector3 m_velocity;
    private bool m_isGrounded;

    [Header("Mouse Settings")]
    public float mouseSensitivity = 2f; 
    private float xRotation = 0f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    public Transform cameraTransform;
    #endregion

    private void Start()
    {
        m_characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        HandleMouseLook();
        HandleMovement();
        HandleGravityAndJump();
    }

    #region Movement Methods
    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        m_characterController.Move(move * moveSpeed * Time.deltaTime);
    }

    private void HandleGravityAndJump()
    {
        m_isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (m_isGrounded && m_velocity.y < 0)
        {
            m_velocity.y = -2f; 
        }
        if (Input.GetButtonDown("Jump") && m_isGrounded)
        {
            m_velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        m_velocity.y += gravity * Time.deltaTime;
        m_characterController.Move(m_velocity * Time.deltaTime);
    }
    #endregion
}
