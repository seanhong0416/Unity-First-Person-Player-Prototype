using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VolumetricLines;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class WindWallAbility : NetworkBehaviour
{
    bool wind_wall_pressed = false;
    bool wind_wall_placed = false;
    bool scene_object_initiated = false;
    float next_wind_wall_time = 0f;
    int layer_of_floor;
    int layer_of_wall;
    int layer_of_barrier;
    int layer_of_net;
    Vector3 size_of_wind_wall;
    float floor_height;
    [SerializeField] float wind_wall_duration = 3f;
    float wind_wall_placed_time = 0f;

    [SerializeField] GameObject windWallPrefab;
    GameObject windWallInstance;
    [SerializeField] GameObject lineStripPrefab;
    GameObject lineStripInstance;
    [SerializeField] Transform cameraTransform;
    Transform teamChisatoFloor;

    // Start is called before the first frame update
    void Start()
    {
        if (!IsOwner) return;

        layer_of_wall = LayerMask.NameToLayer("Wall");
        layer_of_floor = LayerMask.NameToLayer("Floor");
        layer_of_barrier = LayerMask.NameToLayer("Barrier");
        layer_of_net = LayerMask.NameToLayer("Net");
    }

    #region Network Spawn for Strip
    [ServerRpc(RequireOwnership = false)]
    void SpawnStripServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;
        lineStripInstance = Instantiate(lineStripPrefab);
        lineStripInstance.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
        AssignSpawnedStripClientRpc(clientId, lineStripInstance.GetComponent<NetworkObject>().NetworkObjectId);
        Debug.Log("In razer spawn server rpc : " + lineStripInstance);
    }

    [ClientRpc]
    void AssignSpawnedStripClientRpc(ulong clientId, ulong networkObjectId)
    {
        if (NetworkManager.Singleton.LocalClientId != clientId) return;
        lineStripInstance = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId].gameObject;
    }
    #endregion

    #region Network Spawn for Wind Wall
    [ServerRpc(RequireOwnership = false)]
    void SpawnWindWallServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;
        windWallInstance = Instantiate(windWallPrefab);
        windWallInstance.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
        AssignSpawnedWindWallClientRpc(clientId, windWallInstance.GetComponent<NetworkObject>().NetworkObjectId);
        Debug.Log("In razer spawn server rpc : " + windWallInstance);
    }

    [ClientRpc]
    void AssignSpawnedWindWallClientRpc(ulong clientId, ulong networkObjectId)
    {
        if (NetworkManager.Singleton.LocalClientId != clientId) return;
        windWallInstance = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId].gameObject;
    }
    #endregion

    [ServerRpc(RequireOwnership = false)]
    void MakeWindWallDisappearServerRpc(ulong networkObjectId,ServerRpcParams serverRpcParams = default)
    {
        MakeWindWallDisappearClientRpc(networkObjectId, serverRpcParams.Receive.SenderClientId);
    }

    [ClientRpc]
    void MakeWindWallDisappearClientRpc(ulong networkObjectId, ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId) return;

        NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId].gameObject.SetActive(false);
    }

    [ServerRpc(RequireOwnership = false)]
    void MakeWindWallServerRpc(ulong networkObjectId, Vector3 windWallTeleportPosition, Quaternion windWallTeleportRotation, ServerRpcParams serverRpcParams = default)
    {
        GameObject windWall = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId].gameObject;

        MakeWindWallClientRpc(networkObjectId, serverRpcParams.Receive.SenderClientId);
        windWall.transform.position = windWallTeleportPosition;
        windWall.transform.rotation = windWallTeleportRotation;
    }

    [ClientRpc]
    void MakeWindWallClientRpc(ulong networkObjectId, ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId) return;

        NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId].gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;


        if (!scene_object_initiated)
        {
            if (SceneManager.GetActiveScene().name == "GameplayScene")
            {
                Debug.Log("spawning razer line");
                SpawnStripServerRpc();
                SpawnWindWallServerRpc();
                Debug.Log("after razer spawn server rpc : " + lineStripInstance);

                floor_height = GameObject.Find("TeamChisatoFloor").transform.position.y;
                scene_object_initiated = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            if(!wind_wall_pressed && Time.time > next_wind_wall_time)
            {

                size_of_wind_wall = windWallInstance.GetComponent<MeshRenderer>().bounds.size;
                Vector3[] line_vertices = new Vector3[]{
                    new Vector3(size_of_wind_wall.x/2, 0, size_of_wind_wall.z/2),
                    new Vector3(-size_of_wind_wall.x/2, 0, size_of_wind_wall.z/2),
                    new Vector3(-size_of_wind_wall.x/2, 0, -size_of_wind_wall.z/2),
                    new Vector3(size_of_wind_wall.x/2, 0, -size_of_wind_wall.z/2),
                    new Vector3(size_of_wind_wall.x/2, 0, size_of_wind_wall.z/2)
                };
                lineStripInstance.GetComponent<VolumetricLineStripBehavior>().UpdateLineVertices(line_vertices);

                Debug.Log("skill activated");
                SetWindWallMode(true);
            }
            else if (wind_wall_pressed)
            {
                Debug.Log("skill deactivate");
                SetWindWallMode(false);
            }
            else
            {
                Debug.Log("wind wall still in cd");
            }
        }

        if (wind_wall_pressed)
        {
            RaycastHit hit;

            if(Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit))
            {
                if(hit.collider.gameObject.layer == layer_of_floor)
                {
                    Debug.Log("set effect position on floor " + lineStripInstance.GetComponent<VolumetricLineStripBehavior>().LineWidth);
                    lineStripInstance.GetComponent<VolumetricLineStripBehavior>().LineWidth = 2;
                    lineStripInstance.transform.position = new Vector3(hit.point.x, floor_height, hit.point.z);
                    lineStripInstance.transform.rotation = Quaternion.Euler(lineStripInstance.transform.rotation.eulerAngles.x, cameraTransform.rotation.eulerAngles.y + 90, lineStripInstance.transform.rotation.eulerAngles.z);
                }
                else if (hit.collider.gameObject.layer == layer_of_wall || hit.collider.gameObject.layer == layer_of_barrier || hit.collider.gameObject.layer == layer_of_net)
                {
                    Debug.Log("set effect position on wall");
                    lineStripInstance.GetComponent<VolumetricLineStripBehavior>().LineWidth = 2;
                    lineStripInstance.transform.position = hit.point + (hit.normal * (size_of_wind_wall.x / 2)) + new Vector3(0f,floor_height - hit.point.y ,0f);
                    lineStripInstance.transform.right = hit.normal;
                }
                else
                {
                    Debug.Log("deactivate effect");
                    lineStripInstance.GetComponent<VolumetricLineStripBehavior>().LineWidth = 0;
                }
            }
            else
            {
                lineStripInstance.GetComponent<VolumetricLineStripBehavior>().LineWidth = 0;
            }

        }

        if (Input.GetButtonDown("Fire1"))
        {
            if(lineStripInstance.GetComponent<VolumetricLineStripBehavior>().LineWidth != 0)
            {
                windWallInstance.SetActive(true);
                windWallInstance.transform.position = lineStripInstance.transform.position + new Vector3(0f, size_of_wind_wall.y / 2, 0f);
                windWallInstance.transform.rotation = lineStripInstance.transform.rotation;

                MakeWindWallServerRpc(windWallInstance.GetComponent<NetworkObject>().NetworkObjectId, windWallInstance.transform.position, windWallInstance.transform.rotation);
                
                wind_wall_placed = true;
                wind_wall_placed_time = Time.time;

                lineStripInstance.GetComponent<VolumetricLineStripBehavior>().LineWidth = 0;
                SetWindWallMode(false);
                return;
            }
            else
            {
                Debug.Log("not point to a place that can spawn wind wall");
            }
        }

        if (wind_wall_placed)
        {
            if(Time.time - wind_wall_placed_time > wind_wall_duration)
            {
                wind_wall_placed = false;
                windWallInstance.SetActive(false);
                MakeWindWallDisappearServerRpc(windWallInstance.GetComponent<NetworkObject>().NetworkObjectId);
            }
        }
    }

    void SetWindWallMode(bool state)
    {
        gameObject.GetComponent<Shoot>().stop_shooting = state;
        wind_wall_pressed = state;
    }
}
