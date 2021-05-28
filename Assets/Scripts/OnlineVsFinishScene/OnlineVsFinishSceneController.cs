using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlineVsFinishSceneController : MonoBehaviour
{
    TextController titleText;
    TextController gameoverText;

    void Start()
    {
        FadeManager.FadeIn(1f);
        SetInstances();
        SetTitleText();
        SetGameoverText();
    }

    void SetInstances()
    {
        titleText = GameObject.Find("GameOverTitle").GetComponent<TextController>();
        gameoverText = GameObject.Find("GameOverText").GetComponent<TextController>();
    }

    void SetTitleText()
    {
        string text = "";

        switch (Data.tmpCauseGameover)
        {
            case Data.CAUSE_GAMEOVER_ONLINE_WIN:
                text = "YOU WIN!";
                break;
            case Data.CAUSE_GAMEOVER_ONLINE_LOSE:
                text = "YOU LOSE!";
                break;
        }

        titleText.SetText(text);
    }

    void SetGameoverText()
    {
        string text = "";

        string tmpText = Data.GetTmpText();
        if (tmpText != "")
        {
            text = text + tmpText + "\n";
        }

        switch (Data.tmpCauseGameover)
        {
            case Data.CAUSE_GAMEOVER_ONLINE_WIN:
                text += "あなたの勝ちです。";
                break;
            case Data.CAUSE_GAMEOVER_ONLINE_LOSE:
                text += "あなたの負けです。";
                break;
        }

        gameoverText.SetText(text);
    }
}
