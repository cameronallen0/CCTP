using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

public class ShootingMovement : MonoBehaviour
{
    private PlayerControls inputActions;
    TeleportPlayer teleportPlayer;

    //Movement Variables
    public float forceMagnitude;
    private float shotgunForce = 20f;
    private float pistolForce = 5f;
    private float rifleForce = 10f;
    private float teleporterForce = 0f;
    private float maxVelcoity;

    List<object> gunForcesList = new List<object>();
    private int currentIndex = 0;

    List<GameObject> gunList = new List<GameObject>();
    private int currentGunIndex = 0;

    public GameObject shotgun;
    public GameObject pistol;
    public GameObject rifle;
    public GameObject teleporter;
    public GameObject gun;

    private float teleportCooldown = 3f;
    private bool canShoot = true;

    private bool scrollUp;

    //Looking Variables
    public float lookSensitivity = 50f;
    private float xRotation = 0f;

    public Camera cam;
    public Rigidbody rb;


    private void Awake()
    {
        inputActions = new PlayerControls();
        teleportPlayer = GetComponent<TeleportPlayer>();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;

        gunForcesList.Clear();
        gunList.Clear();

        gunForcesList.Add(shotgunForce);
        gunForcesList.Add(pistolForce);
        gunForcesList.Add(rifleForce);
        gunForcesList.Add(teleporterForce);

        gunList.Add(shotgun);
        gunList.Add(pistol);
        gunList.Add(rifle);
        gunList.Add(teleporter);

        foreach(GameObject gunPrefab in gunList)
        {
            gunPrefab.SetActive(false);
        }

        gunList[0].SetActive(true);
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void Update()
    {
        MovePlayer();
        DoLooking();
        SwitchGun();
    }

    private void TeleportPlayer()
    {
        teleportPlayer.CapturePositions();
    }

    private void MovePlayer()
    {
        maxVelcoity = forceMagnitude * 2;

        if (inputActions.PlayerController.Shoot.triggered)
        {
            forceMagnitude = (float)gunForcesList[currentIndex];
            Vector3 forceDirection = -transform.forward;
            rb.AddForce(forceDirection * forceMagnitude, ForceMode.Impulse);
            Debug.Log("Shoot");
        }

        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxVelcoity);

        if(forceMagnitude == teleporterForce && canShoot)
        {
            if(inputActions.PlayerController.Shoot.triggered)
            {
                StartCoroutine(TeleportCooldown());
            }
        }
    }

    private void SwitchGun()
    {
        float z = inputActions.PlayerController.SwitchWeapon.ReadValue<float>();
        if(z > 0)
        {
            //Scroll Up
            scrollUp = true;
            currentIndex = (currentIndex + 1) % gunForcesList.Count;
            currentGunIndex = (currentGunIndex + 1) % gunList.Count;
            UpdateCurrentValue();
        }
        else if(z < 0)
        {
            //Scroll Down
            scrollUp = false;
            currentIndex = (currentIndex - 1 + gunForcesList.Count) % gunForcesList.Count;
            currentGunIndex = (currentGunIndex - 1 + gunList.Count) % gunList.Count;
            UpdateCurrentValue();
        }
    }

    private void UpdateCurrentValue()
    {
        if (scrollUp == true)
        {
            GameObject previousGun = gunList[(currentGunIndex - 1 + gunList.Count) % gunList.Count];
            previousGun.SetActive(false);
        }
        if (scrollUp == false) 
        {
            GameObject previousGun = gunList[(currentGunIndex + 1 + gunList.Count) % gunList.Count];
            previousGun.SetActive(false);
        }
        gun = gunList[currentGunIndex];
        gun.SetActive(true);
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

    public Vector2 GetPlayerLook()
    {
        return inputActions.PlayerController.Look.ReadValue<Vector2>();
    }

    IEnumerator TeleportCooldown()
    {
        canShoot = false;

        TeleportPlayer();

        yield return new WaitForSeconds(teleportCooldown);

        canShoot = true;

        yield return null;
    }


    private void OnDisable()
    {
        inputActions.Disable();
    }
}
