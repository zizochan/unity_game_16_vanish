using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DescriptionButtonController : MonoBehaviour
{
    public int type;
    const int TYPE_LEFT = 1;
    const int TYPE_RIGHT = 2;

    DescriptionSceneController descriptionSceneController;

    void Start()
    {
        SetInstances();
    }

    void SetInstances()
    {
        descriptionSceneController = GameObject.Find("DescriptionSceneController").GetComponent<DescriptionSceneController>();
    }

    public void OnClick()
    {
        PageChange();
    }

    void PageChange()
    {
        int pageDiff = 0;
        switch (type) {
            case TYPE_LEFT:
                pageDiff = -1;
                break;
            case TYPE_RIGHT:
                pageDiff = 1;
                break;
        }
        descriptionSceneController.PageChange(pageDiff);
    }
}
