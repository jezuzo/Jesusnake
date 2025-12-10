using UnityEngine;

public class GameAssetsTitleScreen : MonoBehaviour
{
    public static GameAssetsTitleScreen gameAssetsTitleScreen;
    private void Awake()
    {
        gameAssetsTitleScreen = this;
    }
    public Sprite highScoreArrows;
    public Sprite randomHighScore;
    public Sprite slowVelocity;
    public Sprite mediumVelocity;
    public Sprite highVelocity;
    public Sprite noObstacles;
    public Sprite yesObstacles;
    public Sprite apple;
    public Sprite locker;
    public Sprite snakeBlue;
    public Sprite snakeGreen;
    public Sprite snakePink;
    public Sprite snakeRed;
    public Sprite box;
    

}
