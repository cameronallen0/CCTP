using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapPosCorrect : MonoBehaviour
{
    public LayerMask groundLayer;

    public void CorrectPosition()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;

        if(Physics.Raycast(ray , out hit, Mathf.Infinity, groundLayer))
        {
            transform.position = hit.point;
        }
    }
}
