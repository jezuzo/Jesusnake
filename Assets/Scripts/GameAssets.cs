using UnityEngine;

public class GameAssets : MonoBehaviour
{
    public static GameAssets gameAssets;
    private void Awake()
    {
        gameAssets = this;
    }
    public Sprite snakeHead;
    public Sprite snakeBody;
    public Sprite snakeTail;
    public Sprite leftDownCorner;
    public Sprite rightDownCorner;
    public Sprite leftUpCorner;
    public Sprite rightUpCorner;
    public Sprite apple;

}
