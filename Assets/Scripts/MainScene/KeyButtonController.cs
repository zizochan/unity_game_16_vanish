using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyButtonController : MonoBehaviour
{
    public int type;
    const int TYPE_UP = 1;
    const int TYPE_DOWN = 2;
    const int TYPE_LEFT = 3;
    const int TYPE_RIGHT = 4;

    GameController gameController;

    void Start()
    {
        SetInstances();
    }

    void SetInstances()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
    }

    public void OnClick()
    {
        switch (type)
        {
            case TYPE_UP:
                gameController.KeyOperationUp();
                break;
            case TYPE_DOWN:
                gameController.KeyOperationDown();
                break;
            case TYPE_LEFT:
                gameController.KeyOperationLeft();
                break;
            case TYPE_RIGHT:
                gameController.KeyOperationRight();
                break;
        }
    }
}
