using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using VolumetricLines;

public class BlastShoot : NetworkBehaviour
{
    [SerializeField] float max_distance = 15f;
    [SerializeField] float fire_range = 15f;
    [SerializeField] float impact_force = 15f;
    [SerializeField] float fire_rate = 1f;
    [SerializeField] float shotgun_razer_delay = 1f;
    [SerializeField] float spread_range = 1;
    [SerializeField] int pellets_per_shot = 6;
    float next_fire_time = 0f;
    bool razer_Instantiated = false;

    [SerializeField] GameObject razerPrefab;
    GameObject[] razerInstance;
    Transform cameraTransform;

    // Start is called before the first frame update
    void Start()
    {
        if (!IsOwner) return;
        cameraTransform = transform.GetChild(0);
        razerInstance = new GameObject[pellets_per_shot];
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;

        if (!razer_Instantiated)
        {
            if (SceneManager.GetActiveScene().name == "GameplayScene")
            {
                Debug.Log("spawning razer line");
                SpawnRazerServerRpc();
                razer_Instantiated = true;
            }
        }

        //Debug.Log("at the start of update function" + razerInstance);

        if (Input.GetButton("Fire2") && Time.time >= next_fire_time)
        {
            fire();
            next_fire_time = Time.time + 1 / fire_rate;
        }
        else if (Input.GetButtonUp("Fire2"))
        {
            SetRazer(false);
            //razerInstance.GetComponent<LineRenderer>().positionCount = 0;
        }
        //Debug.Log("at the end of update function" + razerInstance);
    }

    void SetRazer(bool state)
    {
        if (state)
        {
            for (int i = 0; i < pellets_per_shot; i++)
            {
                razerInstance[i].GetComponent<VolumetricLineBehavior>().LineWidth = 1f;
            }
        }
        else
        {
            for (int i = 0; i < pellets_per_shot; i++)
            {
                razerInstance[i].GetComponent<VolumetricLineBehavior>().LineWidth = 0f;
            }
        }
    }

    void fire()
    {
        RaycastHit hit;

        //Activate razer
        //razerInstance.GetComponent<LineRenderer>().positionCount = 2;
        SetRazer(true);
        for (int i = 0; i < pellets_per_shot; i++)
        {
            Vector3 offset = Random.Range(-spread_range, spread_range) * cameraTransform.right + Random.Range(-spread_range, spread_range) * cameraTransform.up;
            Vector3 final_direction = Vector3.Normalize(fire_range * cameraTransform.forward + offset);

            if (Physics.Raycast(cameraTransform.position, final_direction, out hit, max_distance))
            {
                Debug.Log(hit.transform.name);
                if (hit.rigidbody != null)
                {
                    HitBallServerRpc(hit.collider.gameObject.name, hit.normal);
                }
                Vector3 razer_start = transform.position;
                Vector3 razer_end = hit.point;
                razerInstance[i].GetComponent<VolumetricLineBehavior>().StartPos = razer_start;
                razerInstance[i].GetComponent<VolumetricLineBehavior>().EndPos = razer_end;
                //razerInstance.GetComponent<LineRenderer>().SetPosition(0, razer_start);
                //razerInstance.GetComponent<LineRenderer>().SetPosition(1, razer_end);
            }
            else
            {
                Vector3 razer_start = transform.position;
                Vector3 razer_end = razer_start + final_direction * max_distance;
                razerInstance[i].GetComponent<VolumetricLineBehavior>().StartPos = razer_start;
                razerInstance[i].GetComponent<VolumetricLineBehavior>().EndPos = razer_end;
                //razerInstance.GetComponent<LineRenderer>().SetPosition(0, razer_start);
                //razerInstance.GetComponent<LineRenderer>().SetPosition(1, razer_end);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void SpawnRazerServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;
        razerInstance = new GameObject[pellets_per_shot];
        for(int i = 0; i < pellets_per_shot; i++)
        {
            razerInstance[i] = Instantiate(razerPrefab);
            razerInstance[i].GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
            AssignSpawnedRazerClientRpc(clientId, razerInstance[i].GetComponent<NetworkObject>().NetworkObjectId, i);
        }
    }

    [ClientRpc]
    void AssignSpawnedRazerClientRpc(ulong clientId, ulong networkObjectId, int pelletNumber)
    {
        if (NetworkManager.Singleton.LocalClientId != clientId) return;
        razerInstance[pelletNumber] = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId].gameObject;
    }

    [ServerRpc(RequireOwnership =false)]
    void HitBallServerRpc(string ObjectName, Vector3 normal)
    {
        GameObject.Find(ObjectName).GetComponent<Rigidbody>().AddForce(-normal * impact_force);
    }
}
