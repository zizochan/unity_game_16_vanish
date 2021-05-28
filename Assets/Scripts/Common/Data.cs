using System.Collections.Generic;

public static class Data
{
    // version
    public static string VERSION = "1.4.1";

    // マップサイズ
    public const int MAP_X_MODE_ENDLESS = 4;
    public const int MAP_Y_MODE_ENDLESS = 4;
    public const int MAP_X_MODE_TIMEATTACK = 4;
    public const int MAP_Y_MODE_TIMEATTACK = 5;
    public const int MAP_X_MODE_ONLINE = 4;
    public const int MAP_Y_MODE_ONLINE = 5;

    // バランス関連
    public const int PANEL_SCORE_VANISH = 3; // 16
    public const int NEW_COLOR_TERM = 4;
    public const int GARBAGE_PANEL_TERM_MAX = 15;
    public const int GARBAGE_PANEL_TERM_MIN = 7;
    public const int LEVEL_PER_VANISH = 4;
    public const float TIME_ATTACK_LIMIT = 120f;
    public const int SCORE_ATTACK_LIFE_MAX = 20;

    // 色関連
    public const int MAX_COLOR = 2;

    // SOUND
    public static bool CONFIG_SOUND_PLAY = true;
    public const int SOUND_GAME_OVER_BOMB = 1;
    public const int SOUND_PANEL_MOVE = 2;
    public const int SOUND_PANEL_VANISH = 3;
    public const int SOUND_ONLINE_ATTACK = 4;

    // BGM
    public const int BGM_TITLE = 0;
    public const int BGM_MAIN_ENDLESS = 1;
    public const int BGM_MAIN_TIMEATTACK = 2;
    public const int BGM_MAIN_ONLINE_VS = 3;

    // mode
    public const int GAME_MODE_ENDLESS = 0;
    public const int GAME_MODE_TIMEATTACK = 1;
    public const int GAME_MODE_SCOREATTACK = 2;
    public const int GAME_MODE_ONLINE_VS = 10;
    public static int gameMode = GAME_MODE_ONLINE_VS;

    // ゲームオーバー関連
    public static int tmpScore = 0;
    public static int tmpVanishCount = 0;
    public static int tmpCauseGameover = 0;
    public const int CAUSE_GAMEOVER_PANEL_NOT_MOVE = 0; // パネルを動かせない
    public const int CAUSE_GAMEOVER_TIMEOVER = 1; // 時間切れ
    public const int CAUSE_GAMEOVER_LIFEOVER = 2; // Lifeが0
    public const int CAUSE_GAMEOVER_ONLINE_WIN = 10;
    public const int CAUSE_GAMEOVER_ONLINE_LOSE = 11;
    public static bool isHighScoreUpdated = false;

    // スコア関連
    public static int[] highScores;
    public const int HIGH_SCORE_COUNT = 3;

    // オンライン関連
    static string networkPlayerName = "no name";
    public static int ONLINE_SCORE_NORMA = 1000;
    public static int ONLINE_ATTACK_COUNT_PER_VANISH = 2; // vanish1回で何個お邪魔が作られるか
    public static string tmpText = "";

    // チート
    public static bool CHEAT_AUTO_PLAY = false;
    public static bool CHEAT_NEAR_GAME_OVER = false;
    public static bool CHEAT_OFF_LINE = false;

    static Data()
    {
        Start();
        ResetAllData();
    }

    // ゲーム開始時に一度だけ呼ばれる
    static void Start()
    {
        InitializeHighScores();
    }

    public static void InitializeHighScores()
    {
        highScores = new int[HIGH_SCORE_COUNT];
        for (int i = 0; i < HIGH_SCORE_COUNT; i ++) {
            highScores[i] = 0;
        }
    }

    public static void ResetAllData()
    {
        tmpScore = 0;
        tmpVanishCount = 0;
        tmpCauseGameover = CAUSE_GAMEOVER_PANEL_NOT_MOVE;
        tmpText = "";
        isHighScoreUpdated = false;
    }

    public static bool IsBgmStop()
    {
        return CONFIG_SOUND_PLAY == false;
    }

    public static void SetGameMode(int mode)
    {
        gameMode = mode;
    }

    public static (int, int) GetMapSize()
    {
        int x, y;

        switch (gameMode)
        {
            case GAME_MODE_ENDLESS:
            case GAME_MODE_SCOREATTACK:
            default:
                x = MAP_X_MODE_ENDLESS;
                y = MAP_Y_MODE_ENDLESS;
                break;
            case GAME_MODE_TIMEATTACK:
                x = MAP_X_MODE_TIMEATTACK;
                y = MAP_Y_MODE_TIMEATTACK;
                break;
            case GAME_MODE_ONLINE_VS:
                x = MAP_X_MODE_ONLINE;
                y = MAP_Y_MODE_ONLINE;
                break;
        }

        return (x, y);
    }

    public static bool IsTimeAttackMode()
    {
        return gameMode == GAME_MODE_TIMEATTACK;
    }

    public static bool IsScoreAttackMode()
    {
        return gameMode == GAME_MODE_SCOREATTACK;
    }

    public static bool IsOnlineMode()
    {
        return gameMode == GAME_MODE_ONLINE_VS;
    }

    public static bool IsUseHighScore()
    {
        return gameMode != GAME_MODE_ONLINE_VS;
    }

    public static int CalcLevel(int vanishCount)
    {
        return (int)(vanishCount / Data.LEVEL_PER_VANISH);
    }

    public static int GetHighScore() {
        if (!IsUseHighScore()) {
            return 0;
        }

        return Data.highScores[Data.gameMode];
    }

    public static void SetHighScore(int score)
    {
        Data.highScores[Data.gameMode] = score;
    }

    public static string GetNetworkPlayerName()
    {
        return networkPlayerName;
    }

    public static void SetNetworkPlayerName(string name)
    {
        networkPlayerName = name;
    }

    public static void SetTmpText(string text)
    {
        tmpText = text;
    }

    public static string GetTmpText()
    {
        return tmpText;
    }
}
