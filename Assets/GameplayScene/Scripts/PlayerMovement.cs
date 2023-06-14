using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Transform groundCheck;
    [SerializeField] Animator animator;

    public float movement_speed = 12f;
    public float gravity = 9.8f;
    public float jumping_velocity = 6f;

    Vector3 vertical_velocity = new Vector3(0f, -2f, 0f);
    bool on_ground = false;
    [SerializeField] private float ground_check_radius = 0.4f;
    [SerializeField] private LayerMask ground_mask;

    // Start is called before the first frame update
    void Start()
    {
        //ClientPlayerSpawnServerRpc();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;

        //For in-game pause design
        //if (!PauseTransition.isPaused)
        //{
        //for wasd movement
        float movement_x = Input.GetAxis("Vertical");
        float movement_y = Input.GetAxis("Horizontal");
        Vector3 final_movement = transform.right * movement_y + transform.forward * movement_x;
        characterController.Move(movement_speed * final_movement * Time.deltaTime);
        if(final_movement != Vector3.zero)
        {
            if(Mathf.Abs(movement_x) > Mathf.Abs(movement_y))
            {
                if(movement_x > 0)
                {
                    animator.SetBool("is_moving_forward", true);
                    animator.SetBool("is_moving_back", false);
                    animator.SetBool("is_moving_right", false);
                    animator.SetBool("is_moving_left", false);
                }
                else
                {
                    animator.SetBool("is_moving_forward", false);
                    animator.SetBool("is_moving_back", true);
                    animator.SetBool("is_moving_right", false);
                    animator.SetBool("is_moving_left", false);
                }
            }
            else
            {
                if(movement_y > 0)
                {
                    animator.SetBool("is_moving_forward", false);
                    animator.SetBool("is_moving_back", false);
                    animator.SetBool("is_moving_right", true);
                    animator.SetBool("is_moving_left", false);
                }
                else
                {
                    animator.SetBool("is_moving_forward", false);
                    animator.SetBool("is_moving_back", false);
                    animator.SetBool("is_moving_right", false);
                    animator.SetBool("is_moving_left", true);
                }
            }
        }
        else
        {
            animator.SetBool("is_moving_forward", false);
            animator.SetBool("is_moving_back", false);
            animator.SetBool("is_moving_right", false);
            animator.SetBool("is_moving_left", false);
        }
        //Debug.Log(movement_speed * final_movement * Time.deltaTime);

        //for jumping
        if (on_ground && Input.GetButtonDown("Jump"))
            vertical_velocity.y = jumping_velocity;
        //}

        //for falling
        vertical_velocity.y -= gravity * Time.deltaTime;
        on_ground = Physics.CheckSphere(groundCheck.position, ground_check_radius, ground_mask);
        if (on_ground && vertical_velocity.y < 0)
            vertical_velocity.y = -2;
        //mid-air movement
        characterController.Move(vertical_velocity * Time.deltaTime);
    }

    //for client player to be spawned at specific position
    /*
    [ServerRpc(RequireOwnership =false)]
    void ClientPlayerSpawnServerRpc(ServerRpcParams serverRpcParams = default) {
        var clientId = serverRpcParams.Receive.SenderClientId;
        if (NetworkManager.ConnectedClients.ContainsKey(clientId))
        {
            var client = NetworkManager.ConnectedClients[clientId];
            client.PlayerObject.GetComponent<CharacterController>().enabled = false;
            client.PlayerObject.transform.position = GameObject.Find("FloorLeft").transform.position + new Vector3(0, 6, 0);
            client.PlayerObject.GetComponent<CharacterController>().enabled = true;
        }
    }
    */

}
