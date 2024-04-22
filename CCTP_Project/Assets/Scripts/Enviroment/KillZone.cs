using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillZone : MonoBehaviour
{
    public Transform respawnPoint;
    public TeleportPlayer playerT;
    public ShootingMovement playerScript;

    public string objectTag = "Player";
    public bool isKill;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(objectTag))
        {
            KillPlayer(other.gameObject);
        }
    }

    private void KillPlayer(GameObject player)
    {
        player.transform.position = respawnPoint.position;
        playerScript.rb.velocity = Vector3.zero;
        isKill = true;
        playerT.PlayerFail();
    }
}
