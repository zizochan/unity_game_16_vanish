using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class MatchingScenePhotonController : MonoBehaviourPunCallbacks
{
    NetworkMatchingController networkMatchingController;

    void Start()
    {
        SetInstances();

        // 2重起動を防ぐために一度切断する
        PhotonNetwork.Disconnect();

        SetNickName();
        PhotonNetwork.ConnectUsingSettings();
    }

    void SetInstances()
    {
        networkMatchingController = GameObject.Find("NetworkMatchingController").GetComponent<NetworkMatchingController>();
    }

    public void SetNickName()
    {
        PhotonNetwork.NickName = Data.GetNetworkPlayerName();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message) {
        CreateNewRoom();
    }

    void CreateNewRoom()
    {
        var roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        PhotonNetwork.CreateRoom(null, roomOptions);
    }

    public override void OnJoinedRoom()
    {
        networkMatchingController.SetNetworkStatus(NetworkMatchingController.NETWORK_STATUS_WAIT);
    }

    public void Exit()
    {
        PhotonNetwork.Disconnect();
    }

    public int GetMatchCount()
    {
        var others = PhotonNetwork.PlayerListOthers;
        return others.Length;
    }

    public void StopQueueRunning()
    {
        PhotonNetwork.IsMessageQueueRunning = false;
    }
}
