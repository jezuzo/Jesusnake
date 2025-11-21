using UnityEngine;

public class GameHandler : MonoBehaviour
{
    [SerializeField] private Snake snake;
    [SerializeField] private ObjectSpawner objectSpawner;
    private LevelGrid levelGrid;
    
    private void Awake()
    {
        
        levelGrid = new LevelGrid(21,21);
        snake.Setup(objectSpawner, levelGrid);
        objectSpawner.Setup(snake, levelGrid);
        
    }

    
}
