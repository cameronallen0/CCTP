using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TeleportPlayer : MonoBehaviour
{
    public Vector3 playerPos;
    public Vector3 playerCurrentPos;
    public Vector3 objectPos;
    public Vector3 objectCurrentPos;

    public LayerMask objectLayer;

    public GameObject currentObject;

    public void CapturePositions()
    {
        playerPos = transform.position;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        {
            if(Physics.Raycast(ray, out hit, Mathf.Infinity, objectLayer))
            {
                objectPos = hit.point;
                currentObject = hit.collider.gameObject;
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
