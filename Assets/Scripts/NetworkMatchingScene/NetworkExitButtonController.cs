using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkExitButtonController : MonoBehaviour
{
    public string nextScene;
    public float fadeOutTime = 1f;

    AudioSource audioSourceSe;
    public AudioClip clickSound;

    NetworkMatchingController networkMatchingController;

    // Start is called before the first frame update
    void Start()
    {
        audioSourceSe = GetComponent<AudioSource>();
        networkMatchingController = GameObject.Find("NetworkMatchingController").GetComponent<NetworkMatchingController>();
    }

    public void OnClick()
    {
        networkMatchingController.Exit();
        PlayClickSound();
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
