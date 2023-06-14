using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerInvisibleToOwner : NetworkBehaviour
{
    [SerializeField] bool player_invisible = true;
    Transform wizardBody;
    // Start is called before the first frame update
    void Start()
    {
        if (!IsOwner) return;

        if (player_invisible)
        {
            if (wizardBody = transform.Find("PolyArtWizardStandardMat/WizardBody"))
            {
                Debug.Log("set wizard body to not active");
                wizardBody.gameObject.SetActive(false);
            }
            else
            {
                Debug.Log("can't find wizard body");
            }
        }
    }

    /*
    private void Update()
    {
        if (!IsOwner) return;

        if (player_invisible)
        {
            if (wizardBody = transform.Find("WizardBody"))
            {
                Debug.Log("set wizard body to not active");
                wizardBody.gameObject.SetActive(false);
                player_invisible = false;
            }
            else
            {
                Debug.Log("can't find wizard body");
            }
        }
    }
    */
}
