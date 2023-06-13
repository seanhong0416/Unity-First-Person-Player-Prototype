using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GamePausedFunctions : NetworkBehaviour
{
    bool is_paused = false;

    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject settingMenu;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P) && !is_paused)
        {
            pauseMenu.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
        }
        else if(Input.GetKeyDown(KeyCode.P) && is_paused)
        {
            pauseMenu.SetActive(false);
            settingMenu.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void ResumeButtonFunction()
    {
        pauseMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void TeleportTeamChisatoButtonFunction()
    {
        var clientId = NetworkManager.Singleton.LocalClientId;
        NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<PlayerSpawnpoint>().TeleportTeamChisato();
    }

    public void TeleportTeamTakinaButtonFunction()
    {
        var clientId = NetworkManager.Singleton.LocalClientId;
        NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<PlayerSpawnpoint>().TeleportTeamTakina();
    }
}
