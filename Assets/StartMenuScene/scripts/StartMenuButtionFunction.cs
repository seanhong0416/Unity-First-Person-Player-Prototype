using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class StartMenuButtionFunction : MonoBehaviour
{
    [SerializeField] GameObject joinGameMenu;
    public void StartGameButtonFunction()
    {
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SceneManager.LoadScene("GameplayScene", LoadSceneMode.Single);
    }

    public void JoinGameButtonFunction()
    {
        joinGameMenu.SetActive(true);
        gameObject.SetActive(false);
    }
}
