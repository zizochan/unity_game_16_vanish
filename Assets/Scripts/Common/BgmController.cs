using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgmController : MonoBehaviour
{
    AudioSource audioSource;
    public AudioClip titleBgm;
    public AudioClip normalBgm;
    public AudioClip timeAttackBgm;
    public AudioClip onlineVsBgm;

    const int STATUS_INITIAL = 0;
    const int STATUS_PLAY = 1;
    const int STATUS_STOP = 2;
    int status = STATUS_INITIAL;

    void Awake()
    {
        Singleton();
        SetInstances();
    }

    void Singleton()
    {
        if (GameObject.FindGameObjectsWithTag("AudioManager").Length == 1)
        {
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }

    void Start()
    {
    }

    void SetInstances()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void SetBgm(int bgmId)
    {
        AudioClip clip;

        switch (bgmId) {
            case Data.BGM_TITLE:
            default:
                clip = titleBgm;
                break;
            case Data.BGM_MAIN_ENDLESS:
                clip = normalBgm;
                break;
            case Data.BGM_MAIN_TIMEATTACK:
                clip = timeAttackBgm;
                break;
            case Data.BGM_MAIN_ONLINE_VS:
                clip = onlineVsBgm;
                break;
        }

        if (clip != audioSource.clip)
        {
            status = STATUS_INITIAL;
        }

        audioSource.clip = clip;

        PlayBgm();
    }

    public void PlayBgm()
    {
        if (Data.IsBgmStop()) {
            Stop();
        } else {
            Play();
        }
    }

    void Play()
    {
        if (audioSource == null) {
            return;
        }

        if (status == STATUS_PLAY)
        {
            return;
        }

        audioSource.Play();
        status = STATUS_PLAY;
    }

    void Stop()
    {
        audioSource.Stop();
        status = STATUS_STOP;
    }
}
