using UnityEngine;

public class GameHandler : MonoBehaviour
{
    [SerializeField] private Snake snake;
    [SerializeField] private ObjectSpawner objectSpawner;
    private LevelGrid levelGrid;
    
    private void Start()
    {
        levelGrid = new LevelGrid(20, 20);
        snake.Setup(levelGrid, objectSpawner);
        objectSpawner.Setup(snake);
    }

    
}
