//FadeManager.cs

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FadeManager:MonoBehaviour{

    //フェード用のCanvasとImage
    private static Canvas fadeCanvas;
    private static Image fadeImage;

    //フェード用Imageの透明度
    private static float alpha;

    //フェードインアウトのフラグ
    public static bool isFadeIn = false;
    public static bool isFadeOut = false;

    //フェードしたい時間（単位は秒）
    public static float fadeTime;
    private static float DEFAULT_FADE_TIME = 1.0f;

    //遷移先のシーン番号
    private static string nextScene = "";

    private static float FADE_IN_ALPHA = 1.0f;
    private static float FADE_OUT_ALPHA = 0.0f;

    //フェード用のCanvasとImage生成
    static void Init()
    {
        //フェード用のCanvas生成
        GameObject FadeCanvasObject = new GameObject("CanvasFade");
        fadeCanvas = FadeCanvasObject.AddComponent<Canvas>();
        FadeCanvasObject.AddComponent<GraphicRaycaster>();
        fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        FadeCanvasObject.AddComponent<FadeManager>();

        //最前面になるよう適当なソートオーダー設定
        fadeCanvas.sortingOrder = 100;

        //フェード用のImage生成
        fadeImage = new GameObject("ImageFade").AddComponent<Image>();
        fadeImage.transform.SetParent(fadeCanvas.transform, false);
        fadeImage.rectTransform.anchoredPosition = Vector3.zero;

        //Imageサイズは適当に大きく設定してください
        fadeImage.rectTransform.sizeDelta = new Vector2(9999, 9999);
    }

    //フェードイン開始
    public static void FadeIn(float fadeTime = 0f)
    {
        if (fadeImage == null) Init();
        SetFadeTime(fadeTime);
        alpha = FADE_IN_ALPHA;
        fadeImage.color = Color.black;
        isFadeIn = true;
    } 

    //フェードアウト開始
    public static void FadeOut(string sceneName, float fadeTime = 0f)
    {
        if (fadeImage == null) Init();
        SetFadeTime(fadeTime);
        alpha = FADE_OUT_ALPHA;
        nextScene = sceneName;
        fadeImage.color = Color.clear;
        fadeCanvas.enabled = true;
        isFadeOut = true;
    }

    void Update()
    {
        //フラグ有効なら毎フレームフェードイン/アウト処理
        if (isFadeIn)
        {
            //経過時間から透明度計算
            alpha -= Time.deltaTime / fadeTime;

            //フェードイン終了判定
            if (alpha <= FADE_OUT_ALPHA)
            {
                isFadeIn = false;
                alpha = FADE_OUT_ALPHA;
                fadeCanvas.enabled = false; 
            }

            //フェード用Imageの色・透明度設定
            fadeImage.color = new Color(0.0f, 0.0f, 0.0f, alpha);
        }
        else if (isFadeOut)
        {
            //経過時間から透明度計算
            alpha += Time.deltaTime / fadeTime;

            //フェードアウト終了判定
            if (alpha >= FADE_IN_ALPHA)
            {
                isFadeOut = false;
                alpha = FADE_IN_ALPHA;

                //次のシーンへ遷移
                SceneManager.LoadScene(nextScene);
            }

            //フェード用Imageの色・透明度設定
            fadeImage.color = new Color(0.0f, 0.0f, 0.0f, alpha);
        }
    }

    static void SetFadeTime(float time)
    {
        if (time == 0f)
        {
            time = DEFAULT_FADE_TIME;
        }

        fadeTime = time;
    }
}