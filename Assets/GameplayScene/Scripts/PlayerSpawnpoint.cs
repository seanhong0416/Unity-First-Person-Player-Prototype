using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnpoint : MonoBehaviour
{
    GameObject spawnpoint;

    bool spawnpoint_teleported = false;

    private void Update()
    {
        if (!spawnpoint_teleported)
        {
            Debug.Log(spawnpoint = GameObject.Find("Spawnpoint"));
            if (spawnpoint = GameObject.Find("Spawnpoint"))
            {
                gameObject.GetComponent<CharacterController>().enabled = false;
                transform.position = spawnpoint.transform.position;
                transform.rotation = spawnpoint.transform.rotation;
                gameObject.GetComponent<CharacterController>().enabled = true;
                spawnpoint_teleported = true;
                Debug.Log("Player is teleported to spawnpoint at Update() function");
            }
        }
    }
}
