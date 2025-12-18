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

    public Sprite snakeHead;
    public Sprite snakeBodySide;

    public Sprite upArrow;
    public Sprite downArrow;
    public Sprite leftArrow;
    public Sprite rightArrow;
    public Sprite snakeTongue;
    
    public Sprite apple;
    public Sprite locker;
    public Sprite key;

    public Sprite highScoreArrows;
    public Sprite randomHighScore;

    public Tile tile;
    public Transform parentTiles;
    public Transform parentBodyParts;
    public TMP_Text scoreText;
    public TMP_Text highScoreText;
    public Sprite box;
    public Sprite boxUnlock;
    public Sprite obstacle;
    public Image trophy;
    public GameObject controls;
}
