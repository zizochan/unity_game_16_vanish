using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DescriptionSceneController : MonoBehaviour
{
    TextController labelText;
    TextController descriptionText;

    AudioSource audioSourceSe;
    public AudioClip clickSound;

    int page;
    string[] textData;

    void Start()
    {
        FadeManager.FadeIn(0.5f);

        page = 0;
        SetTextData();
        SetInstances();
        SetText();
    }

    void Update()
    {
        KeyOperation();
    }

    void KeyOperation()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            PageChange(-1);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            PageChange(1);
        }
    }

    void SetInstances()
    {
        audioSourceSe = GetComponent<AudioSource>();
        labelText = GameObject.Find("DescriptionLableText").GetComponent<TextController>();
        descriptionText = GameObject.Find("DescriptionText").GetComponent<TextController>();
    }

    void SetTextData()
    {
        textData = new string[] {
            TextData0(),
            TextData1(),
            TextData2(),
            TextData3(),
            TextData4()
        };
    }

    string TextData0()
    {
        string text = "";

        text += "① 上下左右キーでブロックを動かします\n";
        text += "② <color='red'>色も数字も同じ</color>ブロックは合体します\n";
        text += "③ <color='red'>数字が16になった</color>ブロックは消滅します\n";
        text += "\n";
        text += "基本ルールはこれだけです！シンプル！";

        return text;
    }

    string TextData1()
    {
        string text = "";

        text += "シンプルなゲームなので、一度遊んでもらえれば\n";
        text += "すぐに分かると思います。\n";
        text += "\n";
        text += "感想もらえるととても嬉しいです。\n";
        text += "お待ちしています！";

        return text;
    }

    string TextData2()
    {
        string text = "";

        text += "・お邪魔ブロックは<color='red'>16ブロックが消滅</color>した時に\n";
        text += "　一緒に消えます\n";
        text += "・動かすスペースが１つも無くなると\n";
        text += "　ゲームオーバーです";

        return text;
    }

    string TextData3()
    {
        string text = "";

        text += "・ver1.2.0で追加したモードです\n";
        text += "・行動するとライフが減少します\n";
        text += "・ただし<color='red'>1つでもブロックを消せば</color>、\n";
        text += "　そのターンはライフが減りません（重要！）\n";
        text += "・ライフが0になるとゲームオーバーです";

        return text;
    }

    string TextData4()
    {
        string text = "";

        text += "・ver1.4.0で追加したモードです\n";
        text += "・プレイヤー同士で対戦します\n";
        text += "・16ブロックを消すと、相手にお邪魔ブロックを\n";
        text += "　送れます\n";
        text += "・先に1000点取った方が勝利者です";

        return text;
    }

    public void PageChange(int pageDiff)
    {
        int beforePage = page;
        page += pageDiff;
        if (page < 0) {
            page = 0;
        } else if (page >= textData.Length) {
            page = textData.Length - 1;
        }

        SetText();

        if (beforePage != page)
        {
            PageChangeSound();
        }
    }

    void SetText()
    {
        SetLabelText();
        SetDescriptionText();
    }

    void SetLabelText()
    {
        string text = GetLabelText() + " (" + (page + 1).ToString() + "/" + textData.Length.ToString() + ")";
        labelText.SetText(text);
    }

    string GetLabelText()
    {
        string text = "";

        switch (page)
        {
            case 0:
                text = "あそびかた";
                break;
            case 1:
                text = "あそびかた";
                break;
            case 2:
                text = "その他・補足説明";
                break;
            case 3:
                text = "20Life Mode 説明";
                break;
            case 4:
                text = "Online Battle 説明";
                break;
        }

        return text;
    }

    void SetDescriptionText()
    {
        string text = textData[page];
        descriptionText.SetText(text);
    }

    void PageChangeSound()
    {
        if (Data.IsBgmStop())
        {
            return;
        }

        audioSourceSe.PlayOneShot(clickSound);
    }
}
