using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartButtonController : MonoBehaviour
{
    string nextScene = "MainScene";
    float fadeOutTime = 1f;

    AudioSource audioSourceSe;
    public AudioClip clickSound;

    public int type;

    void Start()
    {
        audioSourceSe = GetComponent<AudioSource>();
    }

    public void OnClick()
    {
        PlayClickSound();

        Data.SetGameMode(type);
        Data.ResetAllData();

        FadeManager.FadeOut(nextScene, fadeOutTime);
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
