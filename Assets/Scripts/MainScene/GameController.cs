using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    FieldController fieldController;

    public const int STATUS_INITIAL = 0;
    public const int STATUS_PLAY = 10;
    public const int STATUS_PANEL_MOVE = 11;
    public const int STATUS_GAMEOVER = 99;
    int status = STATUS_INITIAL;

    private AudioSource audioSource;
    public AudioClip gameOverBombSound;
    public AudioClip panelMoveSound;
    public AudioClip panelVanishSound;
    public AudioClip onlineAttackSound;
    const float soundVolume = 3f;

    public CameraShake shake;
    const int MAX_TURN = 100000;

    int turn = 0;
    int score = 0;
    int level = 0;
    int vanishCount = 0;
    int garbageWaitTrun = 0;
    int garbagePanelPerTurn;
    int new_color_per_turn;
    float restTime;
    int life;
    int gameMode;
    bool isTimeAttack;
    bool isScoreAttack;
    float onlineInfoUpdateRestTime = 0.5f;

    const int INVALID_COLOR = -1;
    TextController scoreText;
    TextController restTimeText;
    TextController restTurnText;
    TextController onlineVsInfoText;
    BgmController audioManager;

    MainScenePhotonController photonController;

    // 一時的処理用
    int tmpMovingPanelCount = 0;
    int tmpPanelRemoveCount = 0;
    int tmpPanelVanishCount = 0;

    void Start()
    {
        FadeManager.FadeIn(0.5f);
        status = STATUS_INITIAL;

        SetInstances();
        OnlineModeSetup();
        SetGameMode();
        ResetScores();
        SetBgm();

        fieldController.Initialze(this);
        AddInitialPanels();
        ResetScoresAfterAddInitialPanels();

        ResetRestTime();
        ResetRestTurn();
        ResetOnlineVsInfoText();
        SetScoreText();

        status = STATUS_PLAY;
    }

    void SetInstances()
    {
        fieldController = GameObject.Find("Field").GetComponent<FieldController>();
        scoreText = GameObject.Find("ScoreText").GetComponent<TextController>();
        restTimeText = GameObject.Find("RestTimeText").GetComponent<TextController>();
        restTurnText = GameObject.Find("RestTurnText").GetComponent<TextController>();
        onlineVsInfoText = GameObject.Find("OnlineVsInfoText").GetComponent<TextController>();

        audioSource = GetComponent<AudioSource>();
        audioManager = GameObject.Find("AudioManager").GetComponent<BgmController>();
    }

    // Update is called once per frame
    void Update()
    {
        float deltaTime = Time.deltaTime;

        // TODO: この辺直したい
        if (Data.IsOnlineMode()) {
            onlineInfoUpdateRestTime -= deltaTime;
            if (onlineInfoUpdateRestTime <= 0f) {
                onlineInfoUpdateRestTime = 0.5f;
                SetOnlineVsInfoText();
            }
        }

        if (status == STATUS_PANEL_MOVE)
        {
            UpdatePanelMovePhase();
        }

        if (!IsGameoverStatus())
        {
            ReduceRestTime(deltaTime);
        }

        if (!IsGamePlay())
        {
            return;
        }

        KeyOperation();

        if (Data.CHEAT_AUTO_PLAY) {
            CheatAutoPlay();
        }
    }

    void AddInitialPanels()
    {
        for (int i = 0; i < Data.MAX_COLOR; i ++) {
            AddPanel(1, i);
        }

        turn = 0;
        AddPanel(0);
    }

    void AddPanel(int numberCount = 0, int panelColor = INVALID_COLOR)
    {
        if (panelColor == INVALID_COLOR) {
            panelColor = GetNextPanelColor(); ;
        }

        fieldController.AddPanel(numberCount, panelColor);
        AddTurn();
    }

    void AddGarbagePanel()
    {
        fieldController.AddGarbagePanel();
        AddTurn();
    }

    void KeyOperation()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            KeyOperationLeft();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            KeyOperationRight();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            KeyOperationUp();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            KeyOperationDown();
        }
    }

    public void KeyOperationLeft()
    {
        KeyPush(FieldController.MOVE_DIRECTION_LEFT);
    }

    public void KeyOperationRight()
    {
        KeyPush(FieldController.MOVE_DIRECTION_RIGHT);
    }

    public void KeyOperationUp()
    {
        KeyPush(FieldController.MOVE_DIRECTION_UP);
    }

    public void KeyOperationDown()
    {
        KeyPush(FieldController.MOVE_DIRECTION_DOWN);
    }

    void KeyPush(int moveDirection)
    {
        if (!IsGamePlay())
        {
            return;
        }

        bool isNoChange = fieldController.KeyOperation(moveDirection);
        if (isNoChange)
        {
            return;
        }

        PlaySound(Data.SOUND_PANEL_MOVE);

        if (tmpMovingPanelCount > 0)
        {
            status = STATUS_PANEL_MOVE;
        }
    }

    public void GameOver(int causeGameover = Data.CAUSE_GAMEOVER_PANEL_NOT_MOVE)
    {
        if (IsGameoverStatus())
        {
            return;
        }
        status = STATUS_GAMEOVER;

        PlaySound(Data.SOUND_GAME_OVER_BOMB);

        Data.tmpScore = score;
        Data.tmpVanishCount = vanishCount;
        Data.tmpCauseGameover = causeGameover;

        if (Data.IsUseHighScore()) {
            UpdateHighScore();
        }

        string nextScene = "GameOverScene";
        if (Data.IsOnlineMode()) {
            nextScene = "OnlineVsFinishScene";
        }
        FadeManager.FadeOut(nextScene, 1f);
    }

    void UpdateHighScore()
    {
        if (score <= Data.GetHighScore())
        {
            return;
        }

        Data.isHighScoreUpdated = true;
        Data.SetHighScore(score);
    }

    bool IsGameoverStatus()
    {
        return status == STATUS_GAMEOVER;
    }

    public bool IsGamePlay()
    {
        return status == STATUS_PLAY;
    }

    public void PlaySound(int soundNumber)
    {
        if (Data.IsBgmStop())
        {
            return;
        }

        AudioClip audioClip = null;

        switch (soundNumber)
        {
            case Data.SOUND_GAME_OVER_BOMB:
                audioClip = gameOverBombSound;
                break;
            case Data.SOUND_PANEL_MOVE:
                audioClip = panelMoveSound;
                break;
            case Data.SOUND_PANEL_VANISH:
                audioClip = panelVanishSound;
                break;
            case Data.SOUND_ONLINE_ATTACK:
                audioClip = onlineAttackSound;
                break;
        }

        if (audioClip == null)
        {
            return;
        }

        PlaySoundOneShot(audioClip);
    }

    void PlaySoundOneShot(AudioClip clip)
    {
        audioSource.PlayOneShot(clip, soundVolume);
    }

    int GetNextPanelColor()
    {
        int nextColor;

        if (turn % new_color_per_turn == (new_color_per_turn - 1))
        {
            nextColor = PanelController.COLOR_PINK;
        } else {
            nextColor = PanelController.COLOR_YELLOW;
        }

        return nextColor;
    }

    void AddTurn()
    {
        turn++;
        if (turn > MAX_TURN)
        {
            turn = 0;
        }

        garbageWaitTrun++;
    }

    void AddScore(int add)
    {
        score += add;
        SetScoreText();
        CheckOnlineVsWin();
    }

    public void AddPanelRemoveScore(PanelController panel)
    {
        int addScore = panel.NumberValue();
        AddScore(addScore);
        AddTmpRemoveCount();
    }

    public void AddPanelVanishScore()
    {
        AddVanishCount();
        AddScore(100);
        AddTmpVanishCount();
    }

    void AddVanishCount()
    {
        vanishCount ++;
        SetLevel();
        SetOnlineVanishCount();
    }

    void SetScoreText()
    {
        string text = "";

        text += vanishCount.ToString() + " Vanish\n";
        text += "SCORE: " + score.ToString();

        scoreText.SetText(text);

        SetOnlineScore();
    }

    public void ResetTmpMovingPanelCount()
    {
        tmpMovingPanelCount = 0;
    }

    public void AddTmpMovingPanelCount()
    {
        tmpMovingPanelCount += 1;
    }

    public void ReduceTmpMovingPanelCount()
    {
        tmpMovingPanelCount -= 1;
        if (tmpMovingPanelCount < 0) {
            tmpMovingPanelCount = 0;
        }
    }

    public bool IsPanelMovingStatus()
    {
        return status == STATUS_PANEL_MOVE;
    }

    void UpdatePanelMovePhase()
    {
        if (tmpMovingPanelCount > 0)
        {
            return;
        }

        AddPanel();
        CheckAddGarbagePanel();
        ReduceLifePhase();

        if (!IsGameoverStatus()) {
            status = STATUS_PLAY;
        }
    }

    void CheckAddGarbagePanel()
    {
        // 強制ゲームオーバーにはしない
        if (fieldController.GetEmptyPanelCount() <= 1) {
            return;
        }

        // オンラインモードでは出さない
        if (Data.IsOnlineMode()) {
            return;
        }

        if (garbageWaitTrun < garbagePanelPerTurn)
        {
            return;
        }

        garbageWaitTrun = -1; // お邪魔ブロック出現後に0にするため
        AddGarbagePanel();
    }

    public void PanelVanish()
    {
        PlaySound(Data.SOUND_PANEL_VANISH);
    }

    void SetGameMode()
    {
        gameMode = Data.gameMode;
        isTimeAttack = Data.IsTimeAttackMode();
        isScoreAttack = Data.IsScoreAttackMode();
    }

    void ResetRestTime()
    {
        GameObject restTimeObject = GameObject.Find("RestTime");

        if (!isTimeAttack) {
            restTimeObject.SetActive(false);
            return;
        }

        restTimeObject.SetActive(true);
        restTime = Data.TIME_ATTACK_LIMIT;

        SetRestTimeText();
    }

    void SetRestTimeText()
    {
        int minute = (int)Mathf.Floor(restTime / 60);
        int second = (int)Mathf.Floor(restTime % 60);
        int mSecond = (int)((restTime - Mathf.Floor(restTime)) * 100);

        string text = minute.ToString("D2") + ":" + second.ToString("D2") + ":" + mSecond.ToString("D2");
        restTimeText.SetText(text);
    }

    void ReduceRestTime(float deltaTime)
    {
        if (!isTimeAttack)
        {
            return;
        }

        restTime -= deltaTime;
        if (restTime < 0f) {
            restTime = 0f;
            GameOver(Data.CAUSE_GAMEOVER_TIMEOVER);
        }

        SetRestTimeText();
    }

    void ResetScores()
    {
        score = 0;
        vanishCount = 0;
        SetLevel();
        new_color_per_turn = Data.NEW_COLOR_TERM;

        ResetLife(); // AddInitialPanelsでゲームオーバーにならないようにする
    }

    void ResetScoresAfterAddInitialPanels()
    {
        garbageWaitTrun = 0;
        ResetLife();
    }

    void SetLevel()
    {
        this.level = Data.CalcLevel(vanishCount);
        garbagePanelPerTurn = CalcGarbagePanelPerTurn();

        // test debug
        // Debug.Log("level:" + level.ToString() + " color:" + new_color_per_turn.ToString() + " garbage:" + garbage_panel_per_turn.ToString());
    }

    int CalcGarbagePanelPerTurn()
    {
        int value = Data.GARBAGE_PANEL_TERM_MAX - level;
        if (value < Data.GARBAGE_PANEL_TERM_MIN) {
            value = Data.GARBAGE_PANEL_TERM_MIN;
        }
        return value;
    }

    void CheatAutoPlay()
    {
        int moveDirection = Random.Range(0, 4);
        switch (moveDirection) {
            case 0:
            default:
                KeyOperationLeft();
                break;
            case 1:
                KeyOperationRight();
                break;
            case 2:
                KeyOperationUp();
                break;
            case 3:
                KeyOperationDown();
                break;
        }
    }

    void SetBgm()
    {
        int bgmId;

        if (Data.IsTimeAttackMode())
        {
            bgmId = Data.BGM_MAIN_TIMEATTACK;
        } else if (Data.IsOnlineMode()) {
            bgmId = Data.BGM_MAIN_ONLINE_VS;
        }
        else
        {
            bgmId = Data.BGM_MAIN_ENDLESS;
        }

        audioManager.SetBgm(bgmId);
    }

    void ResetRestTurn()
    {
        GameObject restTurnObject = GameObject.Find("RestTurn");

        if (!isScoreAttack)
        {
            restTurnObject.SetActive(false);
            return;
        }

        restTurnObject.SetActive(true);
        SetLifeText();
    }

    void SetLifeText()
    {
        string text = "Life " + life.ToString();
        restTurnText.SetText(text);
    }

    void ReduceLifePhase()
    {
        if (!isScoreAttack)
        {
            return;
        }

        ReduceLife();
        RecoverLife();
    }

    void ReduceLife()
    {
        if (tmpPanelRemoveCount > 0) {
            return;
        }

        life--;

        if (life > Data.SCORE_ATTACK_LIFE_MAX)
        {
            life = Data.SCORE_ATTACK_LIFE_MAX;
        }

        SetLifeText();

        if (life > 0) {
            return;
        }

        life = 0;
        GameOver(Data.CAUSE_GAMEOVER_LIFEOVER);
    }

    void ResetLife()
    {
        if (!isScoreAttack)
        {
            return;
        }

        life = Data.SCORE_ATTACK_LIFE_MAX;
        SetLifeText();
    }

    void RecoverLife()
    {
        if (tmpPanelVanishCount <= 0) {
            return;
        }

        AddLife(tmpPanelVanishCount);
    }

    void AddLife(int additional)
    {
        life += additional;
        SetLifeText();
    }

    public void ResetTmpRemoveCount()
    {
        tmpPanelRemoveCount = 0;
        tmpPanelVanishCount = 0;
    }

    void AddTmpRemoveCount()
    {
        tmpPanelRemoveCount++;
    }

    void AddTmpVanishCount()
    {
        tmpPanelVanishCount++;
    }

    void OnlineModeSetup()
    {
        if (!Data.IsOnlineMode())
        {
            return;
        }

        photonController = GameObject.Find("MainScenePhotonController").GetComponent<MainScenePhotonController>();
    }

    void SetOnlineScore()
    {
        if (!Data.IsOnlineMode()) {
            return;
        }

        photonController.SetScore(score);
    }

    void SetOnlineVanishCount()
    {
        if (!Data.IsOnlineMode())
        {
            return;
        }

        photonController.Attack(Data.ONLINE_ATTACK_COUNT_PER_VANISH);
        photonController.SetVanishCount(vanishCount);
    }

    public void AddGarbagePanelByAttack(int count)
    {
        for (int i = 0; i < count; i ++) {
            if (fieldController.GetEmptyPanelCount() <= 0)
            {
                return;
            }

            AddGarbagePanel();
        }

        PlaySound(Data.SOUND_ONLINE_ATTACK);
    }

    void CheckOnlineVsWin()
    {
        if (!Data.IsOnlineMode())
        {
            return;
        }

        if (score < Data.ONLINE_SCORE_NORMA)
        {
            return;
        }

        Data.SetTmpText("スコアが" + Data.ONLINE_SCORE_NORMA.ToString() + "点になりました。");
        photonController.SendWin();
        OnlineVsWin();
    }

    public void OnlineVsWin()
    {
        OnlineGameOver(Data.CAUSE_GAMEOVER_ONLINE_WIN);
    }

    public void OnlineVsLose()
    {
        OnlineGameOver(Data.CAUSE_GAMEOVER_ONLINE_LOSE);
    }

    void OnlineGameOver(int gameoverCause)
    {
        if (IsGameoverStatus())
        {
            return;
        }

        SetOnlineVsInfoText();
        GameOver(gameoverCause);
    }

    public void PanelNotMoveGameOver()
    {
        if (Data.IsOnlineMode())
        {
            Data.SetTmpText("ゲームオーバーになりました。");
            photonController.SendLose();
            OnlineVsLose();
        } else {
            GameOver(Data.CAUSE_GAMEOVER_PANEL_NOT_MOVE);
        }
    }

    void ResetOnlineVsInfoText()
    {
        GameObject textObject = GameObject.Find("OnlineVsInfoText");

        if (!Data.IsOnlineMode())
        {
            textObject.SetActive(false);
            return;
        }

        textObject.SetActive(true);
        SetOnlineVsInfoText();
    }

    public void SetOnlineVsInfoText()
    {
        string text = "";
        (string playerName, int score, int vanishCount) = photonController.GetOnlineVsInfoData();

        // まだデータが取れない場合
        if (score == -1) {
            // noopo
        } else {
            text = "ONLINE BATTLE\n";
            text += playerName + "\n";
            text += vanishCount.ToString() + " Vanish\n";
            text += "SCORE: " + score.ToString();
        }

        onlineVsInfoText.SetText(text);
    }
}
