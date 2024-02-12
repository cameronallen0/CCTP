using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class GravityChanger : MonoBehaviour
{
    private PlayerControls inputActions;

    private CharacterController controller;

    [SerializeField] private Camera cam;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float crouchSpeed = 5f;
    [SerializeField] private float walkSpeed = 10f;
    [SerializeField] private float runSpeed = 15f;
    [SerializeField] public float lookSensitivity = 30f;

    public LayerMask groundLayer;

    private float xRotation = 0f;

    // Movement Variables
    private Vector3 velocity;
    public float gravity = -19.62f;
    private bool grounded;
    private bool isRunning;

    private bool change = false;

    //Jump Variables
    [SerializeField] private float jumpHeight = 3.0f;
    private bool isJumping;

    // Crouch Variables
    private float initHeight;
    [SerializeField] private float crouchHeight;
    private bool isCrouching;

    private void Awake()
    {
        inputActions = new PlayerControls();
    }
    private void Start()
    {
        controller = GetComponent<CharacterController>();
        initHeight = controller.height;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void Update()
    {
        DoMovement();
        DoLooking();
        DoChange();
        DoCrouch();
        DoJump();
        DoRun();
    }

    private void DoLooking()
    {
        Vector2 looking = GetPlayerLook();
        float lookX = looking.x * lookSensitivity * Time.deltaTime;
        float lookY = looking.y * lookSensitivity * Time.deltaTime;

        xRotation -= lookY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * lookX);
    }

    private void DoMovement()
    {
        grounded = controller.isGrounded;
        if (grounded && velocity.y < 0)
        {
            velocity.y = -2f;
            isJumping = false;
            isRunning = false;
        }
        if (isCrouching && grounded)
        {
            movementSpeed = crouchSpeed;
        }
        else
        {
            movementSpeed = walkSpeed;
        }

        Vector2 movement = GetPlayerMovement();
        Vector3 move = transform.right * movement.x + transform.forward * movement.y;
        controller.Move(move * movementSpeed * Time.deltaTime);
        if(!change)
        {
            velocity.y += gravity * Time.deltaTime;
        }
        controller.Move(velocity * Time.deltaTime);
    }

    private void DoRun()
    {
        if (isCrouching && grounded)
        {
            isRunning = false;
        }
        else
        {
            if (inputActions.PlayerController.Run.ReadValue<float>() > 0)
            {
                isRunning = !isRunning;
                movementSpeed = runSpeed;
            }
            else
            {
                movementSpeed = walkSpeed;
            }
        }
    }

    private void DoChange()
    {
        //Implement code changes, this is going to be a lot more difficult than i thought hehe
    }

    private void DoCrouch()
    {
        if (inputActions.PlayerController.Crouch.ReadValue<float>() > 0)
        {
            isCrouching = true;
            controller.height = crouchHeight;
            movementSpeed = crouchSpeed;
        }
        else
        {
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.up), 2.0f, -1))
            {
                controller.height = crouchHeight;
                movementSpeed = crouchSpeed;
            }
            else
            {
                controller.height = initHeight;
                movementSpeed = walkSpeed;
                isCrouching = false;

            }
        }
    }

    private Vector2 GetMousePositionInWorld()
    {
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
        {
            return hit.point;
        }

        return Vector2.zero;
    }

    private void DoJump()
    {
        if (grounded)
        {
            if (inputActions.PlayerController.Jump.triggered)
            {
                isJumping = !isJumping;
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    public Vector2 GetPlayerMovement()
    {
        return inputActions.PlayerController.Move.ReadValue<Vector2>();
    }

    public Vector2 GetPlayerLook()
    {
        return inputActions.PlayerController.Look.ReadValue<Vector2>();
    }
}