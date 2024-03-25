using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OneUseObject : MonoBehaviour
{
    private Vector3 startPos;

    public void Awake()
    {
        startPos = transform.position;
    }

    public void Update()
    {
        if (transform.position != startPos)
        {
            Destroy(gameObject);
        }
    }
}
