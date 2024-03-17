using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShootingMovement : MonoBehaviour
{
    private PlayerControls inputActions;

    //Movement Variables
    public float forceMagnitude;
    private float shotgunForce = 20f;
    private float pistolForce = 5f;
    private float rifleForce = 10f;

    List<object> gunForcesList = new List<object>();
    private int currentIndex = 0;

    List<GameObject> gunList = new List<GameObject>();
    private int currentGunIndex = 0;

    public GameObject shotgun;
    public GameObject pistol;
    public GameObject rifle;
    public GameObject gun;

    private float reloadTime;

    private bool scrollUp;
    
    //Looking Variables
    public float lookSensitivity = 50f;
    private float xRotation = 0f;

    public Camera cam;
    private Rigidbody rb;


    private void Awake()
    {
        inputActions = new PlayerControls();
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

        gunList.Add(shotgun);
        gunList.Add(pistol);
        gunList.Add(rifle);

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

    private void MovePlayer()
    {
        if (inputActions.PlayerController.Shoot.triggered)
        {
            Vector3 forceDirection = -transform.forward;
            rb.AddForce(forceDirection * forceMagnitude, ForceMode.Impulse);
            Debug.Log("Shoot");
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
        

        forceMagnitude = (float)gunForcesList[currentIndex];
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


    private void OnDisable()
    {
        inputActions.Disable();
    }
}
