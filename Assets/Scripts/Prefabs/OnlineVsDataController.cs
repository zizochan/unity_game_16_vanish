using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class OnlineVsDataController : MonoBehaviourPunCallbacks
{
    MainScenePhotonController photonController;

    public int score = 0;
    public int vanishCount = 0;
    public string playerName = "";

    const string ScoreKey = "Score";
    const string VanishCountKey = "VanishCount";
    const string AttackCountKey = "AttackCount";
    const string ResultKey = "Result";

    const int ResultValueWin = 1;
    const int ResultValueLose = 2;

    void Awake()
    {
        SetInstances();
        SetDataInstance();
        playerName = photonView.Owner.NickName;
    }

    void Start()
    {
    }

    void SetInstances()
    {
        photonController = GameObject.Find("MainScenePhotonController").GetComponent<MainScenePhotonController>();
    }

    void SetDataInstance()
    {
        if (photonView.IsMine)
        {
            photonController.SetMyData(this);
        } else {
            photonController.SetEnemyData(this);
        }
    }

    public void SetScore(int score)
    {
        this.score = score;
    }

    public void SetVanishCount(int vanishCount)
    {
        this.vanishCount = vanishCount;
    }

    public void SendProperties()
    {
        var hashtable = new ExitGames.Client.Photon.Hashtable();

        hashtable[ScoreKey] = score;
        hashtable[VanishCountKey] = vanishCount;

        PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);
    }

    void SendProperty(string key, int value)
    {
        var hashtable = new ExitGames.Client.Photon.Hashtable();
        hashtable[key] = value;
        PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);
    }

    public void SendAttackCount(int count)
    {
        SendProperty(AttackCountKey, count);
    }

    public void SendWin()
    {
        SendProperty(ResultKey, ResultValueWin);
    }

    public void SendLose()
    {
        SendProperty(ResultKey, ResultValueLose);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (targetPlayer.ActorNumber != photonView.OwnerActorNr) {
            return;
        }

        foreach (var prop in changedProps)
        {
            SetPropertyValue((string)prop.Key, (int)prop.Value);
        }
    }

    void SetPropertyValue(string key, int value)
    {
        switch (key) {
            case ScoreKey:
                SetScore(value);
                break;
            case VanishCountKey:
                SetVanishCount(value);
                break;
            case AttackCountKey:
                ReceiveAttack(value);
                break;
            case ResultKey:
                ReceiveResult(value);
                break;
        }
    }

    void ReceiveAttack(int value) {
        if (photonView.IsMine && !Data.CHEAT_OFF_LINE)
        {
            return;
        }

        photonController.ReceiveAttack(value);
    }

    void ReceiveResult(int value)
    {
        if (photonView.IsMine)
        {
            return;
        }

        if (value == ResultValueWin)
        {
            // 対戦相手が勝利した
            photonController.GetLose();
        } else if (value == ResultValueLose) {
            // 対戦相手が敗北した
            photonController.GetWin();
        }
    }
}
