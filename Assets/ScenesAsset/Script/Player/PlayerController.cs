using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.XR;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class move : MonoBehaviour
{
    private float rotate_x;
    private float rotate_y;

    Vector3 Player_Move;

    CharacterController controller;

    [SerializeField]
    float walkSpeed = 3.0f;
    [SerializeField]
    float runSpeed = 5.0f;
    [SerializeField]
    float gravity = 10f;
    [SerializeField]
    float mouseSensitivity = 3.4f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        character_movement();
        character_rotate();
    }

    void character_rotate()
    {
        //抓滑鼠移動
        rotate_x += Input.GetAxis("Mouse X") * mouseSensitivity;
        rotate_y -= Input.GetAxis("Mouse Y") * mouseSensitivity;

        rotate_y = Mathf.Clamp(rotate_y, -90f, 90f);

        var rotate = Quaternion.Euler(rotate_y, rotate_x, 0);   //(以x軸轉動rotate_y,以y軸轉動rotate_x,0)

        transform.rotation = rotate;
    }

    void character_movement()
    {
        if (controller.isGrounded)
        {
            float horizon = Input.GetAxis("Horizontal");    
            float vertical = Input.GetAxis("Vertical");     
            Player_Move = new Vector3(horizon, 0, vertical);

            Player_Move = transform.TransformDirection(Player_Move);

            if (Input.GetKey(KeyCode.LeftShift))
                Player_Move *= runSpeed;
            else
                Player_Move *= walkSpeed;

        }

        Player_Move.y -= gravity * Time.deltaTime;
        controller.Move(Player_Move * Time.deltaTime);

    }

}

