using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerRiftMovement1 : MonoBehaviour
{
    private PlayerControls inputActions;

    private CharacterController controller;

    //Rift
    public GameObject riftObject;
    private List<GameObject> riftObjects = new List<GameObject>();
    private int nextIndex = 0;
    private bool  isInsideRift = false;

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
        DoCrouch();
        DoJump();
        DoRun();
        DoFire();
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

        velocity.y += gravity * Time.deltaTime;

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

    public void DoFire()
    {
        if(inputActions.PlayerController.Fire.triggered)
        {
            Ray riftRay = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit riftHit;

            if(Physics.Raycast(riftRay, out riftHit))
            {
                SpawnObject(riftHit.point, riftHit.normal);
                Debug.Log("Click");
                RemoveRift();
            }
        }
    }

    void SpawnObject(Vector3 spawnPosition, Vector3 surfaceNormal)
    {
        GameObject newRift = Instantiate(riftObject, spawnPosition, Quaternion.identity);
        newRift.transform.up = surfaceNormal;
        riftObjects.Add(newRift);
    }

    public void RemoveRift()
    {
        if(riftObjects.Count > 2)
        {
            Destroy(riftObjects[0]);
            riftObjects.RemoveAt(0);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Rift") && !isInsideRift)
        {
            isInsideRift = true;

            string riftIdentifier = other.gameObject.name;
            int riftIndex = GetRiftIndex(riftIdentifier);

            if (riftIndex != -1)
            {
                TeleportPlayer(riftIndex);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Rift"))
        {
            isInsideRift = false;
        }
    }


    int GetRiftIndex(string riftIdentifier)
    {
        for(int i = 0; i < riftObjects.Count; i++)
        {
            if(riftObjects[i].name == riftIdentifier)
            {
                return i;
            }
        }
        return -1;
    }

    private void TeleportPlayer(int riftIndex)
    {
        if (riftObjects.Count >= 2)
        {
            nextIndex = (riftIndex + 1);
            GameObject destination = riftObjects[nextIndex];
            Transform destinationTransform = destination.transform;

            controller.enabled = false;
            transform.position = destinationTransform.position;
            controller.enabled = true;

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
