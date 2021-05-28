using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneChangeButtonController : MonoBehaviour
{
    public string nextScene;
    public float fadeOutTime = 1f;

    AudioSource audioSourceSe;
    public AudioClip clickSound;

    // Start is called before the first frame update
    void Start()
    {
        audioSourceSe = GetComponent<AudioSource>();
    }

    public void OnClick()
    {
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
