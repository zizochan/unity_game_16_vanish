using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class MainScenePhotonController : MonoBehaviourPunCallbacks
{
    GameObject canvas;
    GameController gameController;

    OnlineVsDataController myData;
    OnlineVsDataController enemyData;

    void Start()
    {
        if (!Data.IsOnlineMode())
        {
            return;
        }

        SetInstances();
        CreateNetworkObject();
    }

    void SetInstances()
    {
        canvas = GameObject.Find("Canvas");
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
    }

    void CreateNetworkObject()
    {
        PhotonNetwork.IsMessageQueueRunning = true;
        PhotonNetwork.Instantiate("OnlineVsData", Vector2.zero, Quaternion.identity);
    }

    public void SetMyData(OnlineVsDataController vsData)
    {
        myData = vsData;
    }

    public void SetEnemyData(OnlineVsDataController vsData)
    {
        enemyData = vsData;
    }

    public void SetScore(int score)
    {
        myData.SetScore(score);
        myData.SendProperties();
    }

    public void SetVanishCount(int vanishCount)
    {
        myData.SetVanishCount(vanishCount);
        myData.SendProperties();
    }

    public void Attack(int count)
    {
       myData.SendAttackCount(count);
    }

    public void ReceiveAttack(int count)
    {
        gameController.AddGarbagePanelByAttack(count);
    }

    public void SendLose()
    {
        myData.SendLose();
    }

    public void SendWin()
    {
        myData.SendWin();
    }

    public void GetLose()
    {
        Data.SetTmpText("対戦相手がクリアしました。");
        gameController.OnlineVsLose();
    }

    public void GetWin()
    {
        Data.SetTmpText("対戦相手がゲームオーバーになりました。");
        gameController.OnlineVsWin();
    }

    public void Disconnect()
    {
        PhotonNetwork.Disconnect();
    }

    public (string, int, int) GetOnlineVsInfoData()
    {
        OnlineVsDataController data;
        if (Data.CHEAT_OFF_LINE)
        {
            data = myData;
        } else {
            data = enemyData;
        }

        // TODO: この辺雑なので直したい
        if (data == null) {
            return ("", -1, -1);
        } else {
            return (data.playerName, data.score, data.vanishCount);
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        GetWin();
    }
}
