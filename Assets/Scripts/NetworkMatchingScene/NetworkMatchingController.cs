using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkMatchingController : MonoBehaviour
{
    MatchingScenePhotonController photonController;
    TextController statusText;

    AudioSource audioSourceSe;
    public AudioClip startSound;

    public const int NETWORK_STATUS_INITIAL = 0;
    public const int NETWORK_STATUS_WAIT = 1;
    public const int NETWORK_STATUS_MATCH = 2;
    public const int NETWORK_STATUS_EXIT = 3;
    int networkStatus = NETWORK_STATUS_WAIT;

    float pastTime = 0f;
    const float MATCH_CHECK_TERM = 3f;

    string nextScene = "MainScene";
    float fadeOutTime = 2f;

    void Start()
    {
        FadeManager.FadeIn(0.5f);

        SetInstances();
        SetNetworkStatus(NETWORK_STATUS_INITIAL);
    }

    void Update()
    {
        float deltaTime = Time.deltaTime;
        pastTime += deltaTime;

        if (networkStatus == NETWORK_STATUS_WAIT) {
            CheckMatching();
        }
    }

    void SetInstances()
    {
        audioSourceSe = GetComponent<AudioSource>();
        photonController = GameObject.Find("MatchingScenePhotonController").GetComponent<MatchingScenePhotonController>();
        statusText = GameObject.Find("StatusText").GetComponent<TextController>();
    }

    public void SetNetworkStatus(int status)
    {
        networkStatus = status;
        UpdateStatusText();
    }

    void UpdateStatusText()
    {
        string text;

        switch (networkStatus) {
            case NETWORK_STATUS_INITIAL:
            default:
                text = "ネットワークに接続しています";
                break;
            case NETWORK_STATUS_WAIT:
                text = "対戦相手を探しています\nしばらくお待ち下さい";
                break;
            case NETWORK_STATUS_MATCH:
                text = "対戦相手が見つかりました\nゲームを開始します";
                break;
            case NETWORK_STATUS_EXIT:
                text = "ネットワークを切断しました";
                break;
        }

        statusText.SetText(text);
    }

    public void Exit()
    {
        SetNetworkStatus(NETWORK_STATUS_EXIT);
        photonController.Exit();
    }

    void CheckMatching()
    {
        if (pastTime < MATCH_CHECK_TERM) {
            return;
        }
        pastTime = 0f;

        if (!IsMatched()) {
            return;
        }

        StartOnlineVsMode();
    }

    bool IsMatched()
    {
        if (Data.CHEAT_OFF_LINE) {
            return true;
        }

        int matchCount = photonController.GetMatchCount();
        return matchCount > 0;
    }

    void StartOnlineVsMode()
    {
        if (networkStatus == NETWORK_STATUS_MATCH) {
            return;
        }

        photonController.StopQueueRunning();

        SetNetworkStatus(NETWORK_STATUS_MATCH);

        Data.SetGameMode(Data.GAME_MODE_ONLINE_VS);
        Data.ResetAllData();

        PlayStartSound();

        if (Data.CHEAT_OFF_LINE) {
            fadeOutTime = 0.1f;
        }

        FadeManager.FadeOut(nextScene, fadeOutTime);
    }

    void PlayStartSound()
    {
        if (Data.IsBgmStop())
        {
            return;
        }

        if (startSound == null)
        {
            return;
        }

        audioSourceSe.PlayOneShot(startSound);
    }
}
