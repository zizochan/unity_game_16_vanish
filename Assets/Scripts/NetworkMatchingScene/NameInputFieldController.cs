using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NameInputFieldController : MonoBehaviour
{
    InputField playerNameInputField;
    MatchingScenePhotonController photonController;

    void Start()
    {
        SetInstances();
        SetPlayerNameText();
    }

    void SetInstances()
    {
        photonController = GameObject.Find("MatchingScenePhotonController").GetComponent<MatchingScenePhotonController>();
        playerNameInputField = GetComponent<InputField>();
    }

    void SetPlayerNameText()
    {
        playerNameInputField.text = Data.GetNetworkPlayerName();
    }

    public void OnTextChange()
    {
        Data.SetNetworkPlayerName(playerNameInputField.text);
        photonController.SetNickName();
    }
}
