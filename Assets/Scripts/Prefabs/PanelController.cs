using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelController : MonoBehaviour
{
    const float INACTIVE_POSITION = -200f;
    const int INACTIVE_COORDINATE = -1;
    public const int INITIAL_NUMBER_COUNT = 0;
    const int GARBAGE_NUMBER_COUNT = -1;

    int index;
    int x;
    int y;

    public int numberCount;
    public int color;
    bool isGarbage = false;

    // sttus
    int status;
    const int STATUS_ACTIVE = 1;
    const int STATUS_SCORE_VANISH = 10;
    const int STATUS_INACTIVE_RESERVATION = 11;
    const int STATUS_INACTIVE = -1;

    // 一時処理用フラグ
    bool isMoving = false;

    // color
    public const int COLOR_YELLOW = 0;
    public const int COLOR_PINK = 1;
    public const int COLOR_GARBAGE = 9;
    public const int INITIAL_COLOR = COLOR_YELLOW;

    public Sprite[] numberSprites;
    public Sprite transparentSprite;
    SpriteRenderer numberSpriteRenderer;
    SpriteRenderer backgroundSpriteRenderer;

    GameController gameController;
    public Animator panelAnimator;

    Vector2 nextPosition;
    const float MOVE_SPEED = 30f;

    void Update()
    {
        float deltaTime = Time.deltaTime;

        if (status == STATUS_SCORE_VANISH)
        {
            if (panelAnimator.GetCurrentAnimatorStateInfo(0).IsName("ScoreVanishFinish"))
            {
                ScoreVanishFinish();
            }
        }

        if (status == STATUS_INACTIVE)
        {
            return;
        }

        if (isMoving && gameController.IsPanelMovingStatus())
        {
            MovePosition(deltaTime);
        }
    }

    public void Initialze(GameController _gameController, int index)
    {
        SetInstances();

        this.gameController = _gameController;

        this.index = index;
        this.numberCount = INITIAL_NUMBER_COUNT;
        this.color = INITIAL_COLOR;

        InActivate();

        SetBackgroundColor();
    }

    void SetInstances()
    {
        numberSpriteRenderer = transform.Find("Number").gameObject.GetComponent<SpriteRenderer>();
        backgroundSpriteRenderer = transform.Find("PanelBackground").gameObject.GetComponent<SpriteRenderer>();
    }

    public void Wakeup(int x, int y, int numberCount, int color)
    {
        // アニメーション途中の場合は一旦強制終了させる
        if (status == STATUS_SCORE_VANISH)
        {
            ScoreVanishFinish();
        }

        isGarbage = false;

        this.numberCount = numberCount;
        SetNumberSprite();

        this.color = color;
        SetBackgroundColor();

        SetCoordinate(x, y);

        Activate();
    }

    void SetCoordinate(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    void Activate()
    {
        gameObject.SetActive(true);

        SetNextPosition(x, y);
        MoveImmediately();

        status = STATUS_ACTIVE;
    }

    void InActivate()
    {
        gameObject.SetActive(false);

        SetNextPosition(INACTIVE_POSITION, INACTIVE_POSITION);
        MoveImmediately();

        SetDefaultSpriteRendererOrder();
        SetCoordinate(INACTIVE_COORDINATE, INACTIVE_COORDINATE);

        x = INACTIVE_COORDINATE;
        y = INACTIVE_COORDINATE;

        status = STATUS_INACTIVE;
    }

    void SetNumberSprite()
    {
        Sprite sprite;

        if (isGarbage)
        {
            sprite = transparentSprite;
        } else {
            sprite = numberSprites[numberCount];
        }

        numberSpriteRenderer.sprite = sprite;

        UpdateSpriteRendererOrder();
    }

    public void MoveToCoordinate(int x, int y)
    {
        SetCoordinate(x, y);
        SetNextPosition(x, y);
    }

    public void AddNumberCount()
    {
        numberCount += 1;
        SetNumberSprite();
    }

    public bool IsActive()
    {
        return status == STATUS_ACTIVE;
    }

    public void SetNextPosition(float x, float y)
    {
        Vector2 position = new Vector2(x, y);
        nextPosition = position;
        isMoving = true;
    }

    void MovePosition(float deltaTime)
    {
        float moveSpeed = MOVE_SPEED * deltaTime;
        float currentX = transform.localPosition.x;
        float currentY = transform.localPosition.y;

        if (currentX == nextPosition.x && currentY == nextPosition.y)
        {
            MovePositionFinish();
            return;
        }

        currentX = CalcNextPosition(currentX, nextPosition.x, moveSpeed);
        currentY = CalcNextPosition(currentY, nextPosition.y, moveSpeed);

        transform.localPosition = new Vector2(currentX, currentY);
    }

    void MovePositionFinish()
    {
        if (status == STATUS_INACTIVE_RESERVATION)
        {
            InActivate();
        }

        isMoving = false;
        gameController.ReduceTmpMovingPanelCount();
    }

    void MoveImmediately()
    {
        transform.localPosition = nextPosition;
        isMoving = false;
    }

    public float CalcNextPosition(float current, float target, float speed)
    {
        float result = current;
        float diff = current - target;

        if (diff == 0f)
        {
            result = current;
        }
        else if (Mathf.Abs(diff) < speed)
        {
            result = target;
        }
        else if (diff > 0f)
        {
            result = current - speed;
        }
        else if (diff < 0f)
        {
            result = current + speed;
        }

        return result;
    }

    // 指定の位置まで動いたら消えるフラグを立てる
    public void InactiveReservation()
    {
        status = STATUS_INACTIVE_RESERVATION;
    }

    public void VanishReservation()
    {
        status = STATUS_SCORE_VANISH;
        panelAnimator.SetTrigger("ScoreVanish");
    }

    string GetBackgroundColorCode()
    {
        string colorCode;

        switch(color) {
            case COLOR_PINK:
                colorCode = "#ffb6c1";
                break;
            case COLOR_GARBAGE:
                colorCode = "#333333";
                break;
            case COLOR_YELLOW:
            default:
                colorCode = "#e2cd99";
                break;
        }

        return colorCode;
    }

    void SetBackgroundColor()
    {
        Color newColor;
        string colorCode = GetBackgroundColorCode();
        ColorUtility.TryParseHtmlString(colorCode, out newColor);

        backgroundSpriteRenderer.color = newColor;
    }

    public bool IsScoreVanish()
    {
        return numberCount >= Data.PANEL_SCORE_VANISH;
    }

    void ScoreVanishFinish()
    {
        panelAnimator.ResetTrigger("ScoreVanish");
        InActivate();
    }

    // 実際に表示される値を返す
    public int NumberValue()
    {
        return (int)Mathf.Pow(2, (numberCount + 1));
    }

    public void ChangeToGarbage()
    {
        isGarbage = true;
        color = COLOR_GARBAGE;
        SetBackgroundColor();
        SetNumberSprite();
    }

    public bool IsGarbage()
    {
        return isGarbage;
    }

    void UpdateSpriteRendererOrder()
    {
        int order;

        if (isGarbage)
        {
            order = 150;
        } else {
            order = numberCount * 10;
        }

        SetSpriteRendererOrder(150);
    }

    void SetDefaultSpriteRendererOrder()
    {
        SetSpriteRendererOrder(0);
    }

    void SetSpriteRendererOrder(int order)
    {
        backgroundSpriteRenderer.sortingOrder = order;
        numberSpriteRenderer.sortingOrder = order + 1;
    }
}
