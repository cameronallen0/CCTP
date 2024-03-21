using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class PointCompass : MonoBehaviour
{
    public GameObject player;

    void Update()
    {
        Vector3 target = player.transform.position + Vector3.forward;

        Vector3 relativeTarget = transform.parent.InverseTransformPoint(target);

        float needleRotation = Mathf.Atan2(relativeTarget.x, relativeTarget.z) * Mathf.Rad2Deg;

        transform.localRotation = Quaternion.Euler(0, needleRotation, 0);
    }
}
