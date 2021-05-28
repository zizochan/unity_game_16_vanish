using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BgmToggleController : MonoBehaviour
{
    public Toggle bgmToggle;
    BgmController audioManager;

    // Start is called before the first frame update
    void Start()
    {
        SetInstances();

        audioManager.SetBgm(Data.BGM_TITLE);

        bgmToggle = GetComponent<Toggle>();
        bgmToggle.isOn = Data.CONFIG_SOUND_PLAY;
    }

    void SetInstances()
    {
        audioManager = GameObject.Find("AudioManager").GetComponent<BgmController>();
    }

    public void OnToggleChanged()
    {
        Data.CONFIG_SOUND_PLAY = bgmToggle.isOn;
        audioManager.PlayBgm();
    }
}
