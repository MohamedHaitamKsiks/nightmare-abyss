using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    // consts
    private const float MOUSE_SENSITIVITY = 100.0f;
    private const float PLAYER_ACC = 12.0f;
    private const float PLAYER_SPEED = 10.0f;
    private const float PLAYER_JUMP_FORCE = 14.0f;
    private const float PLAYER_JUMP_BUFFER_TIME = 0.2f;
    private const float GRAVITY = -21.81f;
    private const float GROUND_CHECKER_SIZE = 0.1f;
    private const float HEAD_BOB_AMP = 0.2f;
    private const float HEAD_BOB_ROT = 1.0f;
    private const float HEAD_BOB_FREQ = 10.0f;
    private const float HEAD_TILT_SPEED = 9.0f;
    private const float HEAD_TILT_ANGLE = 4.0f;

    // open fields
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform headTransform;
    [SerializeField] private GunHolder gunHolder;
    [SerializeField] private Transform groundCheckerTransform;
    [SerializeField] private LayerMask groundMask;
    //sfx
    [SerializeField] private SFXPlayer sfxGround;
    
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
    private float headTilt = 0.0f;

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
        // skip when pause
        if (PauseManager.IsPaused()) return;

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

        var groundCollided = Physics.CheckBox(
            groundCheckerTransform.position, 
            new Vector3(0.2f, GROUND_CHECKER_SIZE, 0.2f),
            Quaternion.identity, 
            groundMask
        );

        if (groundCollided && velocity.y < 0.0f)
        {
            velocity.y = -0.0f;

            // on land
            if (!isGrounded)
            {
                isGrounded = true;

                // land feedback
                headBobAmp = -HEAD_BOB_AMP;
                headBobFreq = HEAD_BOB_FREQ;
                headTime = 0.0f;
                sfxGround.Play();
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
        
        velocity.x += (movementVelocity.x - velocity.x) * (PLAYER_ACC * Time.deltaTime);
        velocity.z += (movementVelocity.z - velocity.z) * (PLAYER_ACC * Time.deltaTime);

        // gun animation when moving
        gunHolder.MovementSpeed = isGrounded? velocity.magnitude / PLAYER_SPEED: 0.0f;
        gunHolder.SwayVelocity.x = -direction.x * 2.0f;
        gunHolder.SwayVelocity.y = velocity.y * 3.0f / PLAYER_SPEED;

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

        // control jump hight
        if (Input.GetKeyUp("space") && velocity.y > 0.0f)
        {
            velocity.y *= 0.5f;
        }

        controller.Move(velocity * Time.deltaTime);

        // head bobbing
        bool isMoving = direction.magnitude > 0.2f;
        headBobAmp = Mathf.Lerp(headBobAmp, isMoving? HEAD_BOB_AMP: 0.0f, Time.deltaTime * 2.0f);
        headBobFreq = Mathf.Lerp(headBobFreq, isMoving? HEAD_BOB_FREQ: 0.0f, Time.deltaTime * 2.0f);
        headOffset = Mathf.Lerp(headOffset, 0.0f, Time.deltaTime * 5.0f);

        headTime += Time.deltaTime * headBobFreq;
        var bob = (Mathf.Sin(headTime) + 1.0f) * headBobAmp + headOffset;
        headTransform.SetLocalPositionAndRotation(
            headPositionOrigin + Vector3.up * bob, 
            Quaternion.Euler(Mathf.Sin(headTime) * HEAD_BOB_ROT, 0.0f, headTilt)
        );
        
        // head tilt
        headTilt = Mathf.Lerp(headTilt, HEAD_TILT_ANGLE * (Vector3.Dot(direction, transform.right)), Time.deltaTime * HEAD_TILT_SPEED);
    }

    // push player in a certain direction
    public void Push(Vector3 direction, float Force)
    {
        velocity += direction.normalized * Force;
    }
}
