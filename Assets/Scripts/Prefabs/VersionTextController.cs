using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VersionTextController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SetDayText();
    }

    void SetDayText()
    {
        string text = "Version " + Data.VERSION.ToString();
        this.GetComponent<Text>().text = text;
    }
}
