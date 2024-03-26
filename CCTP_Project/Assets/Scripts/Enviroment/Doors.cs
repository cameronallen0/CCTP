using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doors : MonoBehaviour
{
    public GameObject door;
    public Transform closePos;
    public string playerTag = "Player";

    private bool isOpen = false;

    private Vector3 startPos;
    private float moveSpeed = 2.0f;

    private void Start()
    {
        startPos = door.transform.position;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            if (!isOpen)
            {
                StartCoroutine(OpenDoor());
            }
            else
            {
                return;
            }
        }
    }

    IEnumerator OpenDoor()
    {
        Vector3 targetPos = startPos - new Vector3(0f, 7f, 0f);

        while (door.transform.position != targetPos)
        {
            door.transform.position = Vector3.MoveTowards(door.transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        isOpen = true;
    }
}
