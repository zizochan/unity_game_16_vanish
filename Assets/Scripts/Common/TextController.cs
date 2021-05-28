using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextController : MonoBehaviour
{
    private void Start()
    {
    }

    public void SetText(string text)
    {
        this.GetComponent<Text>().text = text;
    }
}
