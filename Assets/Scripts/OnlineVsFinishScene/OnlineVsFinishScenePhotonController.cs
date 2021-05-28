using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class OnlineVsFinishScenePhotonController : MonoBehaviourPunCallbacks
{
    void Start()
    {
        Disconnect();
    }

    public void Disconnect()
    {
        PhotonNetwork.Disconnect();
    }
}
