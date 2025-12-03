using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameAssets : MonoBehaviour
{
    public static GameAssets gameAssets;
    private void Awake()
    {
        gameAssets = this;
    }
    public Sprite snakeHeadSide;
    public Sprite snakeBodySide;
    public Sprite snakeTailSide;
    public Sprite snakeBodyUpDown;
    public Sprite leftDownCorner;
    public Sprite rightDownCorner;
    public Sprite leftUpCorner;
    public Sprite rightUpCorner;

    public Sprite upArrow;
    public Sprite downArrow;
    public Sprite leftArrow;
    public Sprite rightArrow;

    public Sprite snakeHeadUp;
    public Sprite snakeHeadDown;

    public Sprite snakeTailUp;
    public Sprite snakeTailDown;

    public Sprite apple;
    public Sprite locker;
    public Sprite key;

    public Sprite highScoreArrows;
    public Sprite RandomHighScore;
    public Sprite slowVelocity;
    public Sprite mediumVelocity;
    public Sprite highVelocity;
    public Sprite noObstacles;
    public Sprite yesObstacles;

    public Sprite snakeBlue;
    public Sprite snakeGreen;
    public Sprite snakePink;
    public Sprite snakeRed;

    public Tile tile;
    public Transform parentTiles;
    public Transform parentBodyParts;
    public TMP_Text scoreText;
    public TMP_Text highScoreText;
    public Sprite box;
    public Sprite boxUnlock;
    public Sprite obstacle;
    public Image trophy;

}
