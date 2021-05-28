using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[Serializable]
public class JsonData
{
}

[DefaultExecutionOrder(-10)]
public class MasterData : MonoBehaviour
{
    static JsonData jsonData;

    // Start is called before the first frame update
    void Start()
    {
        LoadJsonFile();
    }

    void LoadJsonFile()
    {
        string inputString = Resources.Load<TextAsset>("MasterData").ToString();
        jsonData = JsonUtility.FromJson<JsonData>(inputString);
    }
}
