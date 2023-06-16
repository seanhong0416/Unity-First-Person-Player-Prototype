using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using VolumetricLines;

public class Shoot : NetworkBehaviour
{
    [SerializeField] float impact_force = 15f;
    [SerializeField] float fire_rate = 15f;
    [SerializeField] float max_distance = 15f;
    [SerializeField] float line_width = 1f;
    public bool stop_shooting = false;
    float next_fire_time;
    bool scene_object_initiated = false;

    GameObject scoreboard;
    [SerializeField] GameObject razerPrefab;
    GameObject razerInstance;
    Transform cameraTransform;
    //RazerSynchronization razerSynchronization;

    // Start is called before the first frame update
    void Start()
    {
        if (!IsOwner) return;
        //Instantiate and spawn razer on network
        //SpawnRazerServerRpc();
        cameraTransform = transform.GetChild(0);
        /*
        razerSynchronization = NetworkManager.Singleton.GetComponent<RazerSynchronization>();
        if(razerSynchronization)
        razerSynchronization.clientRazer.Add(NetworkManager.Singleton.LocalClientId,
            new RazerSynchronization.ClientRazer()
            {
                
            }
            );
        */
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;

        if (!scene_object_initiated)
        {
            if(SceneManager.GetActiveScene().name == "GameplayScene")
            {
                Debug.Log("spawning razer line");
                SpawnRazerServerRpc();
                Debug.Log("after razer spawn server rpc : " + razerInstance);
                scene_object_initiated = true;

                Debug.Log("setting scoreboard");
                if(!(scoreboard = GameObject.Find("Scoreboard")))
                {
                    Debug.Log("can't find scoreboard object");
                }
                else
                {
                    Debug.Log("scoreboard set successfully");
                }
            }
        }

        //Debug.Log("at the start of update function" + razerInstance);

        if (Input.GetButton("Fire1") && Time.time >= next_fire_time && !stop_shooting)
        {
            fire();
            next_fire_time = Time.time + 1 / fire_rate;
        }
        else if(Input.GetButtonUp("Fire1")){
            //reset razer
            razerInstance.GetComponent<VolumetricLineBehavior>().LineWidth = 0f;
            SyncRazerServerRpc(razerInstance.GetComponent<NetworkObject>().NetworkObjectId, false);
            //razerInstance.GetComponent<LineRenderer>().positionCount = 0;
        }
        //Debug.Log("at the end of update function" + razerInstance);
    }

    void fire()
    {
        RaycastHit hit;

        //Activate razer
        //razerInstance.GetComponent<LineRenderer>().positionCount = 2;
        razerInstance.GetComponent<VolumetricLineBehavior>().LineWidth = 1f;
        Vector3 razer_start;
        Vector3 razer_end;

        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, max_distance))
        {
            Debug.Log(hit.transform.name);
            //SetScoreServerRpc(scoreboard.GetComponent<NetworkObject>().NetworkObjectId);
            //Debug.Log(scoreboard.GetComponent<TestNetworkVariable>().test_number.Value);

            if (hit.rigidbody != null)
            {
                HitBallServerRpc(hit.collider.gameObject.name, hit.normal);
            }
            razer_start = transform.position;
            razer_end = hit.point;
            razerInstance.GetComponent<VolumetricLineBehavior>().StartPos = razer_start;
            razerInstance.GetComponent<VolumetricLineBehavior>().EndPos = razer_end;
            //razerInstance.GetComponent<LineRenderer>().SetPosition(0, razer_start);
            //razerInstance.GetComponent<LineRenderer>().SetPosition(1, razer_end);
        }
        else
        {
            razer_start = transform.position;
            razer_end = razer_start + cameraTransform.forward * max_distance;
            razerInstance.GetComponent<VolumetricLineBehavior>().StartPos = razer_start;
            razerInstance.GetComponent<VolumetricLineBehavior>().EndPos = razer_end;
            //razerInstance.GetComponent<LineRenderer>().SetPosition(0, razer_start);
            //razerInstance.GetComponent<LineRenderer>().SetPosition(1, razer_end);
        }

        SyncRazerServerRpc(razerInstance.GetComponent<NetworkObject>().NetworkObjectId, true, razer_start, razer_end);

    }

    [ServerRpc(RequireOwnership = false)]
    void SpawnRazerServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;
        razerInstance = Instantiate(razerPrefab);
        razerInstance.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
        AssignSpawnedRazerClientRpc(clientId, razerInstance.GetComponent<NetworkObject>().NetworkObjectId);
        Debug.Log("In razer spawn server rpc : " + razerInstance);
    }

    [ClientRpc]
    void AssignSpawnedRazerClientRpc(ulong clientId, ulong networkObjectId)
    {
        if (NetworkManager.Singleton.LocalClientId != clientId) return;
        razerInstance = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId].gameObject;
    }

    [ServerRpc(RequireOwnership = false)]
    void HitBallServerRpc(string ObjectName, Vector3 normal)
    {
        GameObject.Find(ObjectName).GetComponent<Rigidbody>().AddForce(-normal * impact_force);
    }

    [ServerRpc(RequireOwnership = false)]
    void SyncRazerServerRpc(ulong networkObjectId, bool state, Vector3 start_pos = default(Vector3), Vector3 end_pos = default(Vector3), ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;
        if(state)
            SetRazerClientRpc(start_pos, end_pos, networkObjectId, clientId);
        else
            ResetRazerClientRpc(networkObjectId, clientId);
    }

    [ClientRpc]
    void SetRazerClientRpc(Vector3 start_pos, Vector3 end_pos, ulong networkObjectId, ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId) return;

        var razer = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId].gameObject.GetComponent<VolumetricLineBehavior>();
        razer.LineWidth = 1f;
        razer.StartPos = start_pos;
        razer.EndPos = end_pos;
    }

    [ClientRpc]
    void ResetRazerClientRpc(ulong networkObjectId, ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId) return;

        var razer = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId].gameObject.GetComponent<VolumetricLineBehavior>();
        razer.LineWidth = 0f;
    }

    /*
    //for testing network variable
    [ServerRpc(RequireOwnership =false)]
    void SetScoreServerRpc(ulong networkObjectId)
    {
        NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId].gameObject.GetComponent<TestNetworkVariable>().test_number.Value += 1;
    }
    */
}
