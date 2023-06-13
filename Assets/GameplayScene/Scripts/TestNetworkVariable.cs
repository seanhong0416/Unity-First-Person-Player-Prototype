using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
public class TestNetworkVariable : NetworkBehaviour
{
    public NetworkVariable<int> test_number = new NetworkVariable<int>(0);
    [SerializeField] TMP_Text scoreboard; 

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        //initialize text
        scoreboard.SetText(test_number.Value.ToString());

        test_number.OnValueChanged += (int previous_value, int new_value) =>
        {
            scoreboard.SetText(test_number.Value.ToString());
        };
    }
}
