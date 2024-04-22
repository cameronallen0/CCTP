using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compass : MonoBehaviour
{
    public Transform player;
    Vector3 dir;

    private void Update()
    {
        dir.z = player.eulerAngles.y;
        transform.localEulerAngles = dir;
    }
}
