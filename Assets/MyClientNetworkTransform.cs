using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode.Components;

public class MyClientNetworkTransform : NetworkTransform
{
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }
}
