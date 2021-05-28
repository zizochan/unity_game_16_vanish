using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverSceneController : MonoBehaviour
{
    TextController titleText;
    TextController gameoverText;
    GameObject highScoreUpdate;

    bool useHighScore = true;

    void Start()
    {
        FadeManager.FadeIn(1f);
        useHighScore = Data.IsUseHighScore();

        SetInstances();
        SetHighScoreUpdate();

        if (!useHighScore) {
            HiddenOfflineButtons();
        }

        SetText();
    }

    void SetInstances()
    {
        titleText = GameObject.Find("GameOverTitle").GetComponent<TextController>();
        gameoverText = GameObject.Find("GameOverText").GetComponent<TextController>();
        highScoreUpdate = GameObject.Find("GameOverHighScoreUpdate");
    }

    void SetText()
    {
        SetTitleText();
        SetGameoverText();
    }

    void SetTitleText()
    {
        string text;

        switch(Data.tmpCauseGameover)
        {
            case Data.CAUSE_GAMEOVER_TIMEOVER:
                text = "TIME UP!";
                break;
            case Data.CAUSE_GAMEOVER_LIFEOVER:
                text = "LIFE EMPTY";
                break;
            default:
                text = "GAME OVER";
                break;
        }

        titleText.SetText(text);
    }

    void SetGameoverText()
    {
        string text = "";

        text += Data.tmpVanishCount.ToString() + " Vanish\n";
        text += "SCORE: " + Data.tmpScore.ToString();
    
        if (useHighScore) {
            text += "\nHIGH SCORE: " + Data.GetHighScore();
        }

        gameoverText.SetText(text);
    }

    void SetHighScoreUpdate()
    {
        bool isUpdated = IsHighScoreUpdated();
        highScoreUpdate.SetActive(isUpdated);
    }

    bool IsHighScoreUpdated()
    {
        if (!useHighScore) {
            return false;
        }

        return Data.isHighScoreUpdated;
    }

    void HiddenOfflineButtons()
    {
        GameObject.Find("RetryButton").SetActive(false);
        GameObject.Find("RankingButton").SetActive(false);
    }
}
