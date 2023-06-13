using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using VolumetricLines;

public class RazerSynchronization : NetworkBehaviour
{
    public struct ClientRazer
    {
        VolumetricLineBehavior razerInstance;
        VolumetricLineBehavior[] blastRazerInstance;
    }

    public Dictionary<ulong, ClientRazer> clientRazer = new Dictionary<ulong, ClientRazer>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
