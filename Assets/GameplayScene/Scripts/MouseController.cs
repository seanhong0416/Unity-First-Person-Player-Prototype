using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Unity.Netcode;

public class MouseController : MonoBehaviour//NetworkBehaviour
{
    public static float mouse_sensitivity = 500f;
    [SerializeField] private Transform playerBody;
    [SerializeField] private Transform cameraView;
    //public GameObject player_camera;

    float x_rotation = 0f;

    // Start is called before the first frame update
    void Start()
    {
        //for multiplayer
        /*
        if (!IsOwner)
        {
            player_camera.SetActive(false);
        }
        */
        x_rotation = cameraView.localEulerAngles.x;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        //if (!IsOwner) return;

        //if (!PauseTransition.isPaused)
        //{
            float mouse_x = Input.GetAxis("Mouse X") * mouse_sensitivity * Time.deltaTime;
            float mouse_y = Input.GetAxis("Mouse Y") * mouse_sensitivity * Time.deltaTime;

            playerBody.Rotate(Vector3.up * mouse_x);

            x_rotation -= mouse_y;
            x_rotation = Mathf.Clamp(x_rotation, -90, 90);
            cameraView.localEulerAngles = Vector3.right * x_rotation;
        //}
    }
}
