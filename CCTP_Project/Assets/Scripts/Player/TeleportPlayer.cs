using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TeleportPlayer : MonoBehaviour
{
    private Vector3 playerPos;
    private Vector3 playerCurrentPos;
    private Vector3 objectPos;
    private Vector3 objectCurrentPos;

    private List<ObjectPosition> capturedObjects = new List<ObjectPosition>(); // List to store captured objects and their initial positions

    public LayerMask objectLayer;

    private GameObject currentObject;

    private int teleportRange = 30;

    private KillZone[] allKillZones;

    private void Start()
    {
        allKillZones = FindObjectsOfType<KillZone>();
    }

    public void CapturePositions()
    {
        playerPos = transform.position;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        {
            if (Physics.Raycast(ray, out hit, teleportRange, objectLayer))
            {
                currentObject = hit.collider.gameObject;
                objectPos = currentObject.transform.position;

                // Store the captured object and its initial position in the list
                capturedObjects.Add(new ObjectPosition(currentObject, objectPos));
            }
            else
            {
                return;
            }
        }
        SwapPositions();
    }

    private void SwapPositions()
    {
        playerCurrentPos = objectPos;
        objectCurrentPos = playerPos;

        transform.position = playerCurrentPos;
        currentObject.transform.position = objectCurrentPos;
    }

    public void PlayerFail()
    {
        // Reset all captured objects to their initial positions
        foreach (ObjectPosition objPos in capturedObjects)
        {
            objPos.ResetPosition();
        }

        capturedObjects.Clear(); // Clear the list after resetting positions
    }
}

public class ObjectPosition
{
    public GameObject gameObject;
    public Vector3 initialPosition;

    public ObjectPosition(GameObject obj, Vector3 pos)
    {
        gameObject = obj;
        initialPosition = pos;
    }

    public void ResetPosition()
    {
        gameObject.transform.position = initialPosition;
    }
}

