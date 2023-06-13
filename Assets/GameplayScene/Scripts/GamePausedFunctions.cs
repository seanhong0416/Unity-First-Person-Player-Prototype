using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using System;

public class GamePausedFunctions : NetworkBehaviour
{
    bool is_paused = false;

    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject settingMenu;
    [SerializeField] TMP_InputField mouseSensitivityInput;
    [SerializeField] TMP_Text applyResult;
    GameObject scoreboard;

    // Start is called before the first frame update
    void Start()
    {
        scoreboard = GameObject.Find("Scoreboard");
        Debug.Log("scoreboard = " + scoreboard);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P) && !is_paused)
        {
            pauseMenu.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            is_paused = true;
        }
        else if(Input.GetKeyDown(KeyCode.P) && is_paused)
        {
            pauseMenu.SetActive(false);
            settingMenu.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            is_paused = false;
        }
    }

    #region Pause Menu Button Functions
    public void ResumeButtonFunction()
    {
        pauseMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        is_paused = false;
    }

    public void TeleportTeamChisatoButtonFunction()
    {
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerSpawnpoint>().TeleportTeamChisato();
    }

    public void TeleportTeamTakinaButtonFunction()
    {
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerSpawnpoint>().TeleportTeamTakina();
    }

    public void ResetScoreButtonFunction()
    {
        Debug.Log("at start of ResetScoreButtonFunction");
        ResetScoreServerRpc(scoreboard.GetComponent<NetworkObject>().NetworkObjectId);
    }

    [ServerRpc(RequireOwnership = false)]
    void ResetScoreServerRpc(ulong networkObjectId)
    {
        Debug.Log("at start of reset score server rpc");
        var teamScoreManager = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId].gameObject.GetComponent<TeamScoreManagement>();
        teamScoreManager.team_chisato_score.Value = 0;
        teamScoreManager.team_takina_score.Value = 0;
    }

    public void MoreSettingsButtonFunction()
    {
        pauseMenu.SetActive(false);
        settingMenu.SetActive(true);
    }

    #endregion

    #region More Settings Menu Button Functions

    public void BackButtonFunction()
    {
        settingMenu.SetActive(false);
        pauseMenu.SetActive(true);
    }

    public void ApplyButtonFunction()
    {
        try
        {
            NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<MouseController>().mouse_sensitivity = float.Parse(mouseSensitivityInput.text);
            applyResult.SetText("Success");
            applyResult.color = Color.green;
        }
        catch(Exception e)
        {
            applyResult.SetText("Failed");
            applyResult.color = Color.red;
        }
    }

    #endregion

}
