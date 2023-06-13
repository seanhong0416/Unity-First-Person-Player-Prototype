using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnpoint : MonoBehaviour
{
    GameObject spawnpoint;
    GameObject teamChisatoFloor;
    GameObject teamTakinaFloor;

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

    public void TeleportTeamChisato()
    {
        teamChisatoFloor = GameObject.Find("TeamChisatoFloor");
        gameObject.GetComponent<CharacterController>().enabled = false;
        transform.position = teamChisatoFloor.transform.position + new Vector3(0f, 6f, 0f);
        gameObject.GetComponent<CharacterController>().enabled = true;
    }

    public void TeleportTeamTakina()
    {
        teamTakinaFloor = GameObject.Find("TeamTakinaFloor");
        gameObject.GetComponent<CharacterController>().enabled = false;
        transform.position = teamTakinaFloor.transform.position + new Vector3(0f, 6f, 0f);
        gameObject.GetComponent<CharacterController>().enabled = true;

    }
}
