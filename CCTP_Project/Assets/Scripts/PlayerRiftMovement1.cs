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

    //Movemnt Type Variables
    public int movemmentType = 0;

    //Rift Variables
    [SerializeField] private GameObject riftObject;
    private List<GameObject> riftObjects = new List<GameObject>();
    private int nextIndex;
    private bool isInsideRift = false;

    //Camera Variables
    [SerializeField] private Camera cam;
    [SerializeField] public float lookSensitivity = 30f;
    private float xRotation = 0f;

    //Layer Variables
    public LayerMask groundLayer;
    public LayerMask riftLayers;

    // Movement Variables
    private Vector3 velocity;
    private float gravity = -20f;
    private bool grounded;
    private bool isRunning;
    [SerializeField] private float movementSpeed;
    private float crouchSpeed = 5f;
    private float walkSpeed = 10f;
    private float runSpeed = 15f;

    //Jump Variables
    private float jumpHeight = 3.0f;
    private bool isJumping;

    // Crouch Variables
    private float initHeight;
    private float crouchHeight;
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
        DoChangeMovement();
        DoMovement();
        DoLooking();
        DoCrouch();
        DoJump();
        DoRun();
        DoFire();
        DoRemove();
    }

    private void DoChangeMovement()
    {
        if(inputActions.PlayerController.Movement0.triggered)
        {
            movemmentType = 0;
        }
        if (inputActions.PlayerController.Movement1.triggered)
        {
            movemmentType = 1;
        }
        if (inputActions.PlayerController.Movement2.triggered)
        {
            movemmentType = 2;
        }
        if (inputActions.PlayerController.Movement3.triggered)
        {
            movemmentType = 3;
        }
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
        if(movemmentType == 1)
        {
            if (inputActions.PlayerController.Fire.triggered)
            {
                Ray riftRay = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
                RaycastHit riftHit;

                if (Physics.Raycast(riftRay, out riftHit, Mathf.Infinity, riftLayers))
                {
                    SpawnObject(riftHit.point, riftHit.normal);
                    Debug.Log("Click");
                    RemoveRift();
                }
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
        if (riftObjects.Count > 2)
        {
            Destroy(riftObjects[0]);
            riftObjects.RemoveAt(0);
        }
    }

    public void DoRemove()
    {
        if(movemmentType == 1)
        {
            if (inputActions.PlayerController.Remove.triggered)
            {
                foreach (GameObject rift in riftObjects)
                {
                    Destroy(rift);
                }
                riftObjects.Clear();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Rift") && !isInsideRift)
        {
            GameObject collidedObject = other.gameObject;
            isInsideRift = true;
            int riftIndex = riftObjects.IndexOf(collidedObject);

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

    private void TeleportPlayer(int riftIndex)
    {
        if (riftObjects.Count >= 2)
        {
            if (riftIndex == 0 || riftIndex == 1)
            {
                nextIndex = (riftIndex + 1) % 2;
                GameObject originRift = riftObjects[riftIndex];
                GameObject destinationRift = riftObjects[nextIndex];

                controller.enabled = false;
                transform.position = destinationRift.transform.position;
                controller.enabled = true;
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
