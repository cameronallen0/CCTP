using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Swapper : MonoBehaviour
{
    public PlayerRiftMovement1 playerScript;
    public SwapPosCorrect swapPos;
    public LayerMask swapLayers;

    public void GetObjectPosition()
    {
        Ray swapRay = playerScript.cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit swapHit;

        if(Physics.Raycast(swapRay, out swapHit, Mathf.Infinity, swapLayers))
        {
            if (swapHit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                Debug.Log("No Swap");
            }
            if(swapHit.collider.gameObject.layer == LayerMask.NameToLayer("Riftable"))
            {
                Debug.Log("No Swap");
            }
            else
            {
                Vector3 hitObjectPos = swapHit.transform.position;
                Vector3 playerPos = playerScript.transform.position;
                Debug.Log(playerPos);
                Debug.Log(hitObjectPos);
            }
        }
    }
}
