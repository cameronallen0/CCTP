using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private PlayerControls inputActions;

    private CharacterController controller;

    [SerializeField] private Camera cam;
    [SerializeField] private float movementSpeed = 10f;
    [SerializeField] public float lookSensitivity = 30f;
    [SerializeField] public float drag = 0.9f;
    private float minVelocityMagnitude = 0.1f;
    private bool isMoving;

    public LayerMask groundLayer;

    private float xRotation = 0f;

    private Vector3 velocity;
    private bool grounded;

    private void Awake()
    {
        inputActions = new PlayerControls();
    }
    private void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void OnEnable()
    {
        inputActions.Enable();
    }
    private void Update()
    {
        if(!isMoving)
        {
            DoMovement();
        }
        DoLooking();
    }
    private void DoLooking()
    {
        Vector2 looking = GetPlayerLook();
        float lookX = looking.x * lookSensitivity * Time.deltaTime;
        float lookY = looking.y * lookSensitivity * Time.deltaTime;

        xRotation -= lookY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Clamp to prevent over-rotation

        cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * lookX);
    }

    private void DoMovement()
    {
        grounded = controller.isGrounded;
        isMoving = true;

        Vector2 movement = GetPlayerMovement();
        Vector3 camForward = cam.transform.forward;
        Vector3 camRight = cam.transform.right;

        camForward.Normalize();
        camRight.Normalize();

        Vector3 move = camForward * movement.y + camRight * movement.x;
        //move.y = 0; // Ignore the y-component of the movement
        velocity *= drag;

        controller.Move(move * movementSpeed * Time.deltaTime);
        if(velocity.magnitude < minVelocityMagnitude)
        {
            isMoving = false;
            velocity = Vector3.zero;
        }
    }

    public Vector2 GetPlayerLook()
    {
        return inputActions.PlayerController.Look.ReadValue<Vector2>();
    }
    public Vector2 GetPlayerMovement()
    {
        return inputActions.PlayerController.Move.ReadValue<Vector2>();
    }
    private void OnDisable()
    {
        inputActions.Disable();
    }
}

