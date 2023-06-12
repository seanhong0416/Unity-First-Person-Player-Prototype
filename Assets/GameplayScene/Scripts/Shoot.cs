using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Shoot : NetworkBehaviour
{


    [SerializeField] float fire_range = 15f;
    [SerializeField] float impact_force = 15f;
    [SerializeField] float fire_rate = 15f;

    [SerializeField] GameObject razerPrefab;
    GameObject razerInstance;

    // Start is called before the first frame update
    void Start()
    {
        if (!IsOwner) return;

        //Instantiate and spawn on network
        SpawnRazerServerRpc();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ServerRpc(RequireOwnership = false)]
    void SpawnRazerServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;
        razerInstance = Instantiate(razerPrefab);
        razerInstance.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
    }
}
