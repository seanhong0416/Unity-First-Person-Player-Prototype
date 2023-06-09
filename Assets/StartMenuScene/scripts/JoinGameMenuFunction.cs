using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine.UI;
using TMPro;
using System;

public class JoinGameMenuFunction : NetworkBehaviour
{
    [SerializeField] GameObject inputField;
    
    //text
    [SerializeField] GameObject connectingSitutation;
    [SerializeField] GameObject timeoutCountdown;

    //menu or page
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject connectingPage;
    [SerializeField] GameObject notConnectingPage;

    float time_stamp = 0f;
    bool connection_timeout_counting_flag = false;

    private void Update()
    {
        if (connection_timeout_counting_flag)
        {
            //Debug.Log(Time.time - time_stamp);
            timeoutCountdown.GetComponent<TMP_Text>().SetText("Connecting...\nTimeout:" + ( 10 - (int)(Time.time-time_stamp) ) );

            if (Time.time - time_stamp > 10)
            {
                ConnectionFailed();
            }
        }
    }

    public void ConnectByInputIp()
    {
        Debug.Log("ip address input = " + inputField.GetComponent<TMP_InputField>().text);
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(inputField.GetComponent<TMP_InputField>().text, (ushort)12345, "0.0.0.0");
        if (!NetworkManager.Singleton.StartClient())
        {
            connectingSitutation.GetComponent<TMP_Text>().SetText("Connection Failed");
        }
        else
        {
            Debug.Log("Set Timestamp");
            time_stamp = Time.time;
            connection_timeout_counting_flag = true;
            connectingPage.SetActive(true);
            notConnectingPage.SetActive(false);
        }
    }

    public void BackToMainMenu()
    {
        mainMenu.SetActive(true);
        gameObject.SetActive(false);
    }

    public void ConnectionFailed()
    {
        NetworkManager.Singleton.Shutdown();
        notConnectingPage.SetActive(true);
        connectingSitutation.GetComponent<TMP_Text>().SetText("Server Connection Timeout");
        connectingPage.SetActive(false);
        connection_timeout_counting_flag = false;
    }


}
