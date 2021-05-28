using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldController : MonoBehaviour
{
    int mapX;
    int mapY;
    int maxPanelCount;

    GameController gameController;
    GameObject panelObject;
    GameObject emptyPanelObject;
    GameObject fieldBackground;

    GameObject[] panels;

    int[,] panelCoordinateIndex;
    const int INACTIVE_INDEX = -1;
    const int INVALID_COORDINATE = -1;
    const int EMPTY_PANEL_VALUE = -1;

    int panelCount = 0;

    public const int MOVE_DIRECTION_LEFT = 1;
    public const int MOVE_DIRECTION_RIGHT = 2;
    public const int MOVE_DIRECTION_UP = 3;
    public const int MOVE_DIRECTION_DOWN = 4;
    public const int MOVE_DIRECTION_LR = 1;
    public const int MOVE_DIRECTION_UD = 2;

    // 一時処理用フラグ
    bool isBeforePanelOverlaped = false;

    void Update()
    {
    }

    public void Initialze(GameController _gameController)
    {
        gameController = _gameController;

        SetInstances();
        SetMapSize();

        CreateEmptyPanels();

        panels = new GameObject[maxPanelCount];
        CreatePanels();

        panelCount = 0;
        ResetPanelCoordinateIndex();

        SetPosition();
        SetBackgroundPosition();

        isBeforePanelOverlaped = false;
    }

    void SetInstances()
    {
        panelObject = (GameObject)Resources.Load("Panel");
        emptyPanelObject = (GameObject)Resources.Load("EmptyPanel");
        fieldBackground = GameObject.Find("FieldBackground");
    }

    void CreateEmptyPanels()
    {
        for (var x = 0; x < mapX; x++)
        {
            for (var y = 0; y < mapY; y++)
            {
                CreateEmptyPanel(x, y);
            }
        }
    }

    void CreateEmptyPanel(int x, int y)
    {
        string objectName = "emptyPanel_" + x.ToString() + "_" + y.ToString();
        CreateObject(emptyPanelObject, x, y, objectName, this.gameObject);
    }

    void CreatePanels()
    {
        int index = 0;
        for (var x = 0; x < mapX; x++)
        {
            for (var y = 0; y < mapY; y++)
            {
                GameObject panel = CreatePanel(index);
                panels[index] = panel;
                index ++;
            }
        }
    }

    GameObject CreatePanel(int index)
    {
        string objectName = "panel_" + index.ToString();
        GameObject panel = CreateObject(panelObject, 0, 0, objectName, this.gameObject);
        panel.GetComponent<PanelController>().Initialze(this.gameController, index);
        return panel;
    }

    GameObject CreateObject(GameObject baseObject, float x, float y, string objectName = null, GameObject parentObject = null, float scaleX = 1f, float scaleY = 1f, float rotation = 0f)
    {
        Quaternion rote = Quaternion.Euler(0f, 0f, rotation);
        GameObject instance = (GameObject)Instantiate(baseObject, new Vector2(x, y), rote);
        instance.transform.localScale = new Vector2(scaleX, scaleY);

        if (objectName != null)
        {
            instance.name = objectName;
        }

        if (parentObject != null) {
            instance.transform.parent = parentObject.transform;
        }

        return instance;
    }

    void SetPosition()
    {
        float x = mapX * 0.5f * -1 + 0.5f;
        float y = mapY * 0.5f * -1 + 0.5f;
        transform.localPosition = new Vector2(x, y);
    }

    void ResetPanelCoordinateIndex()
    {
        panelCoordinateIndex = new int[mapX, mapY];
        for (var x = 0; x < mapX; x++)
        {
            for (var y = 0; y < mapY; y++)
            {
                panelCoordinateIndex[x, y] = INACTIVE_INDEX;
            }
        }
    }

    public void AddPanel(int numberCount, int color)
    {
        var (x, y) = ChoiceEmptyPosition();
        AddPanelWithCoordinate(numberCount, x, y, color);
    }

    public void AddPanelWithCoordinate(int numberCount, int x, int y, int color)
    {
        WakeupPanel(x, y, numberCount, color);
        AddPanelAfterProcess();
    }

    void AddPanelAfterProcess()
    {
        panelCount++;
        CheckAllPanelsScoreVanish();
        CheckGameOver();
    }

    public void AddGarbagePanel()
    {
        var (x, y) = ChoiceEmptyPosition();

        // 一旦適当な値で起動して、Garbageパネルに置き換える
        WakeupPanel(x, y, PanelController.INITIAL_NUMBER_COUNT, PanelController.INITIAL_COLOR);
        PanelController panel = GetPanelController(x, y);
        panel.ChangeToGarbage();

        AddPanelAfterProcess();
    }

    void WakeupPanel(int x, int y, int numberCount, int color)
    {
        int nextPanelIndex = GetNextPanelIndex();
        GetPanelController(nextPanelIndex).Wakeup(x, y, numberCount, color);
        panelCoordinateIndex[x, y] = nextPanelIndex;
    }

    int GetNextPanelIndex()
    {
        for (int i = 0; i < maxPanelCount; i ++) {
            if (GetPanelController(i).IsActive() == false) {
                return i;
            }
        }

        return 0;
    }

    public (int, int) ChoiceEmptyPosition()
    {
        int i = 0;
        int x = 0;
        int y = 0;

        while (true) {
            x = Random.Range(0, mapX);
            y = Random.Range(0, mapY);

            if (panelCoordinateIndex[x, y] == INACTIVE_INDEX)
            {
                break;
            }

            i++;
            if (i > 1000) {
                Debug.Log("無限ループが発生しています");
                break;
            }
        }

        return (x, y);
    }

    public bool KeyOperation(int moveDirection)
    {
        bool isNoChange = true;
        gameController.ResetTmpRemoveCount();

        switch (moveDirection) {
            case MOVE_DIRECTION_LEFT:
                isNoChange = MovePanels(moveDirection, MOVE_DIRECTION_LR, false);
                break;
            case MOVE_DIRECTION_RIGHT:
                isNoChange = MovePanels(moveDirection, MOVE_DIRECTION_LR, true);
                break;
            case MOVE_DIRECTION_UP:
                isNoChange = MovePanels(moveDirection, MOVE_DIRECTION_UD, true);
                break;
            case MOVE_DIRECTION_DOWN:
                isNoChange = MovePanels(moveDirection, MOVE_DIRECTION_UD, false);
                break;
        }

        return isNoChange;
    }

    bool MovePanels(int moveDirection, int lrudDirection, bool isPositiveDirection)
    {
        bool isNoChange = true;
        gameController.ResetTmpMovingPanelCount();;

        if (lrudDirection == MOVE_DIRECTION_LR) {
            for (var y = 0; y < mapY; y++) {
                isBeforePanelOverlaped = false;
                for (var x = (isPositiveDirection ? mapX - 2 : 1); (isPositiveDirection ? x >= 0 : x < mapX); x += (isPositiveDirection ? -1 : 1))
                {
                    if (MovePanel(x, y, moveDirection))
                    {
                        isNoChange = false;
                    }
                }
            }
        } else {
            for (var x = 0; x < mapX; x++) {
                isBeforePanelOverlaped = false;
                for (var y = (isPositiveDirection ? mapY - 2 : 1); (isPositiveDirection ? y >= 0 : y < mapY); y += (isPositiveDirection ? -1 : 1))
                {
                    if (MovePanel(x, y, moveDirection))
                    {
                        isNoChange = false;
                    }
                }
            }
        }

        return isNoChange;
    }

    bool MovePanel(int x, int y, int moveDirection)
    {
        PanelController currentPanel = GetPanelController(x, y);
        if (currentPanel == null)
        {
            return false;
        }

        int nextX = INVALID_COORDINATE;
        int nextY = INVALID_COORDINATE;
        bool overlap = false;

        switch (moveDirection) {
            case MOVE_DIRECTION_RIGHT:
                (nextX, nextY, overlap) = SearchNextPosition(currentPanel, x + 1, y, MOVE_DIRECTION_LR, true);
                break;
            case MOVE_DIRECTION_LEFT:
                (nextX, nextY, overlap) = SearchNextPosition(currentPanel, x - 1, y, MOVE_DIRECTION_LR, false);
                break;
            case MOVE_DIRECTION_UP:
                (nextX, nextY, overlap) = SearchNextPosition(currentPanel, x, y + 1, MOVE_DIRECTION_UD, true);
                break;
            case MOVE_DIRECTION_DOWN:
                (nextX, nextY, overlap) = SearchNextPosition(currentPanel, x, y - 1, MOVE_DIRECTION_UD, false);
                break;
        }

        if (nextX == INVALID_COORDINATE || nextY == INVALID_COORDINATE)
        {
            return false;
        }

        currentPanel.MoveToCoordinate(nextX, nextY);

        if (overlap) {
            // パネル消滅がある場合
            gameController.AddPanelRemoveScore(currentPanel);
            currentPanel.InactiveReservation();
            RemovePanel(x, y);

            int nextPanelIndex = panelCoordinateIndex[nextX, nextY];
            PanelController nextPanel = GetPanelController(nextPanelIndex);
            nextPanel.AddNumberCount();

            isBeforePanelOverlaped = true;
        } else {
            // パネル移動のみの場合
            panelCoordinateIndex[nextX, nextY] = panelCoordinateIndex[x, y];
            panelCoordinateIndex[x, y] = INVALID_COORDINATE;

            isBeforePanelOverlaped = false;
        }

        gameController.AddTmpMovingPanelCount();

        return true;
    }

    (int, int, bool) SearchNextPosition(PanelController currentPanel, int startX, int startY, int lrudDirection, bool isPositiveDirection)
    {
        int nextX = INVALID_COORDINATE;
        int nextY = INVALID_COORDINATE;
        bool overlap = false;
        bool isMove, isOverLap, isBreak;

        if (lrudDirection == MOVE_DIRECTION_LR)
        {
            int y = startY;
            for (var x = startX; (isPositiveDirection ? x < mapX : x >= 0); x += (isPositiveDirection ? 1 : -1))
            {
                (isMove, isOverLap, isBreak) = CheckNextPanelNumber(currentPanel, x, y);
                if (isMove) {
                    nextX = x; nextY = y;
                }

                if (isBreak) {
                    overlap = isOverLap;
                    break;
                }
            }
        } else { // UD
            int x = startX;
            for (var y = startY; (isPositiveDirection ? y < mapY : y >= 0); y += (isPositiveDirection ? 1 : -1))
            {
                (isMove, isOverLap, isBreak) = CheckNextPanelNumber(currentPanel, x, y);
                if (isMove) {
                    nextX = x; nextY = y;
                }

                if (isBreak) {
                    overlap = isOverLap;
                    break;
                }
            }
        }

        return (nextX, nextY, overlap);
    }

    (bool, bool, bool) CheckNextPanelNumber(PanelController currentPanel, int x, int y)
    {
        bool isMove = false;
        bool isOverlap = false;
        bool isBreak = false;

        PanelController nextPanel = GetPanelController(x, y);
        if (nextPanel == null)
        {
            isMove = true;
        } else {
            int nextNumber = nextPanel.numberCount;
            int nextColor = nextPanel.color;

            if (!isBeforePanelOverlaped && CanNextPanelOverlap(currentPanel, nextPanel))
            {
                isMove = true;
                isOverlap = true;
            }
            isBreak = true;
        }

        return (isMove, isOverlap, isBreak);
    }

    bool CanNextPanelOverlap(PanelController currentPanel, PanelController nextPanel)
    {
        if (currentPanel.IsGarbage() || nextPanel.IsGarbage())
        {
            return false;
        }

        return (nextPanel.numberCount == currentPanel.numberCount && nextPanel.color == currentPanel.color);
    }

    void CheckGameOver()
    {
        if (GetEmptyPanelCount() > 0)
        {
            return;
        }

        if (CheckAnyPanelCanMove())
        {
            return;
        }

        gameController.PanelNotMoveGameOver();
    }

    PanelController GetPanelController(int x, int y)
    {
        if (x < 0 || x >= mapX)
        {
            return null;
        }
        if (y < 0 || y >= mapY)
        {
            return null;
        }

        int panelIndex = panelCoordinateIndex[x, y];
        if (panelIndex == INACTIVE_INDEX)
        {
            return null;
        }

        return GetPanelController(panelIndex);
    }

    PanelController GetPanelController(int index)
    {
        return panels[index].GetComponent<PanelController>();
    }

    int GetPanelValue(int x, int y)
    {
        PanelController panelController = GetPanelController(x, y);
        if (panelController == null)
        {
            return EMPTY_PANEL_VALUE;
        }

        return panelController.numberCount;
    }

    void CheckAllPanelsScoreVanish()
    {
        bool isAnyVanished = false;

        for (var x = 0; x < mapX; x++)
        {
            for (var y = 0; y < mapY; y++)
            {
                if (CheckPanelScoreVanish(x, y)) {
                    isAnyVanished = true;
                }
            }
        }

        if (!isAnyVanished) {
            return;
        }

        RemoveGarbagePanels();
        gameController.PanelVanish();
    }

    void RemoveGarbagePanels()
    {
        for (var x = 0; x < mapX; x++)
        {
            for (var y = 0; y < mapY; y++)
            {
                RemoveGarbagePanel(x, y);
            }
        }
    }

    bool CheckPanelScoreVanish(int x, int y)
    {
        PanelController panel = GetPanelController(x, y);
        if (panel == null) {
            return false;
        }

        if (!panel.IsScoreVanish()) {
            return false;
        }

        gameController.AddPanelVanishScore();
        panel.VanishReservation();
        RemovePanel(x, y);

        return true;
    }

    void RemovePanel(int x, int y)
    {
        panelCount--;
        panelCoordinateIndex[x, y] = INVALID_COORDINATE;
    }

    void RemoveGarbagePanel(int x, int y)
    {
        PanelController panel = GetPanelController(x, y);
        if (panel == null) {
            return;
        }

        if (!panel.IsGarbage()) {
            return;
        }

        panel.VanishReservation();
        RemovePanel(x, y);
    }

    bool CheckAnyPanelCanMove()
    {
        for (var x = 0; x < mapX; x++)
        {
            for (var y = 0; y < mapY; y++)
            {
                if (CheckPanelCanMove(x, y))
                {
                    return true;
                }
            }
        }
        return false;
    }

    bool CheckPanelCanMove(int x, int y)
    {
        PanelController currentPanel = GetPanelController(x, y);
        if (currentPanel == null)
        {
            return true;
        }
        if (CanCoordinatePanelOverlap(currentPanel, x - 1, y))
        {
            return true;
        }
        if (CanCoordinatePanelOverlap(currentPanel, x + 1, y))
        {
            return true;
        }
        if (CanCoordinatePanelOverlap(currentPanel, x, y - 1))
        {
            return true;
        }
        if (CanCoordinatePanelOverlap(currentPanel, x, y + 1))
        {
            return true;
        }
        return false;
    }

    bool CanCoordinatePanelOverlap(PanelController currentPanel, int x, int y)
    {
        PanelController nextPanel = GetPanelController(x, y);
        if (nextPanel == null)
        {
            return false;
        }
        return CanNextPanelOverlap(currentPanel, nextPanel);
    }

    void SetMapSize()
    {
        (int x, int y) = Data.GetMapSize();

        if (Data.CHEAT_NEAR_GAME_OVER) {
            x = 2;
            y = 2;
        }

        mapX = x;
        mapY = y;
        maxPanelCount = mapX * mapY;
    }

    void SetBackgroundPosition()
    {
        fieldBackground.transform.localScale = new Vector2(mapX + 1, mapY + 1);

        // 中心に起き直す
        float x = Mathf.Abs(transform.localPosition.x);
        float y = Mathf.Abs(transform.localPosition.y);
        fieldBackground.transform.localPosition = new Vector2(x, y);
    }

    public int GetEmptyPanelCount()
    {
        return maxPanelCount - panelCount;
    }
}
