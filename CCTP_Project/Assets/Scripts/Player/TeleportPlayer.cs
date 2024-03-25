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

    public LayerMask objectLayer;

    private GameObject currentObject;

    private int teleportRange = 30;

    public void CapturePositions()
    {
        playerPos = transform.position;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        {
            if(Physics.Raycast(ray, out hit, teleportRange, objectLayer))
            {
                currentObject = hit.collider.gameObject;
                objectPos = currentObject.transform.position;
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
}
