using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // consts
    private const float MOUSE_SENSITIVITY = 200.0f;
    private const float PLAYER_ACC = 100.0f;
    private const float PLAYER_SPEED = 5.0f;
    private const float PLAYER_JUMP_FORCE = 7.0f;
    private const float PLAYER_JUMP_BUFFER_TIME = 0.2f;
    private const float GRAVITY = -15.81f;
    private const float GROUND_CHECKER_SIZE = 0.1f;
    private const float HEAD_BOB_AMP = 0.1f;
    private const float HEAD_BOB_FREQ = 10.0f;

    // open fields
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform headTransform;

    [SerializeField] private Transform groundCheckerTransform;
    [SerializeField] private LayerMask groundMask;
    
    // components
    private CharacterController controller;

    // data
    private float jumpBuffer = 0.0f;

    private Vector3 velocity;
    private bool isGrounded = false;

    private float cameraRotationVertical = 0.0f;

    private Vector3 headPositionOrigin;
    private float headTime = 0.0f;
    private float headBobAmp = 0.0f;
    private float headBobFreq = 0.0f;
    private float headOffset = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        // lock cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // get controller
        controller = GetComponent<CharacterController>();

        // get origin head position
        headPositionOrigin = headTransform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        // mouse movement
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = -Input.GetAxis("Mouse Y");

        // horizontal rotation
        float horizantalRotation = mouseX * MOUSE_SENSITIVITY * Time.deltaTime;
        transform.Rotate(Vector3.up, horizantalRotation);

        // vertical rotation
        cameraRotationVertical += mouseY * MOUSE_SENSITIVITY * Time.deltaTime;
        cameraRotationVertical = Mathf.Clamp(cameraRotationVertical, -90.0f, 90.0f);
        cameraTransform.localRotation = Quaternion.Euler(cameraRotationVertical, 0.0f, 0.0f);

        // gravity 
        velocity.y += GRAVITY * Time.deltaTime;

        var groundCollided = Physics.CheckSphere(groundCheckerTransform.position, GROUND_CHECKER_SIZE, groundMask);
        if (groundCollided && velocity.y < 0.0f)
        {
            velocity.y = -0.0f;

            // on land
            if (!isGrounded)
            {
                isGrounded = true;

                // land feedback
                headOffset = -0.2f;
            }
        }
        if (!groundCollided && velocity.y < 0.0f)
        {
            isGrounded = false;
        }

        // character movement
        float moveForward = Input.GetAxis("Vertical");
        float moveSide= Input.GetAxis("Horizontal");

        Vector3 direction = (transform.right * moveSide + transform.forward * moveForward).normalized;
        Vector3 movementVelocity = direction * PLAYER_SPEED;
        
        velocity.x = movementVelocity.x;
        velocity.z = movementVelocity.z;

        // jump 
        jumpBuffer -= Time.deltaTime;
        if (Input.GetKeyDown("space"))
        {
            jumpBuffer = PLAYER_JUMP_BUFFER_TIME;
        }

        if (isGrounded && jumpBuffer > 0.0001f)
        {
            velocity.y = PLAYER_JUMP_FORCE;
            isGrounded = false;
            jumpBuffer = 0.0f;
        }

        controller.Move(velocity * Time.deltaTime);

        // head bobbing
        bool isMoving = direction.magnitude > 0.2f;
        headBobAmp = Mathf.Lerp(headBobAmp, isMoving? HEAD_BOB_AMP: 0.0f, Time.deltaTime * 2.0f);
        headBobFreq = Mathf.Lerp(headBobFreq, isMoving? HEAD_BOB_FREQ: 0.0f, Time.deltaTime * 2.0f);
        headOffset = Mathf.Lerp(headOffset, 0.0f, Time.deltaTime * 5.0f);

        headTime += Time.deltaTime * headBobFreq;
        var bob = (Mathf.Sin(headTime) + 1.0f) * headBobAmp + headOffset;
        headTransform.localPosition = headPositionOrigin + Vector3.up * bob;
        
    }
}
