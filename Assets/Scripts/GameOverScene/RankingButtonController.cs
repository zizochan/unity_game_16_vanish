using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankingButtonController : MonoBehaviour
{
    int currentScore;

    AudioSource audioSourceSe;
    public AudioClip clickSound;

    void Start()
    {
        SetCurrentScore();
        audioSourceSe = GetComponent<AudioSource>();
    }

    void SetCurrentScore()
    {
        currentScore = Data.GetHighScore();
    }

    public void CallRanking()
    {
        PlayClickSound();
        naichilab.RankingLoader.Instance.SendScoreAndShowRanking(currentScore, Data.gameMode);
    }

    void PlayClickSound()
    {
        if (Data.IsBgmStop())
        {
            return;
        }

        if (clickSound == null)
        {
            return;
        }

        audioSourceSe.PlayOneShot(clickSound);
    }
}
