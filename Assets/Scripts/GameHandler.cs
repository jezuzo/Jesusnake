using UnityEngine;

public class GameHandler : MonoBehaviour
{
    [SerializeField] private Snake snake;
    [SerializeField] private ObjectSpawner objectSpawner;
    private LevelGrid levelGrid;
    
    private void Awake()
    {
        
        levelGrid = new LevelGrid(PlayerPrefs.GetInt("TableSize"), PlayerPrefs.GetInt("TableSize"));
        snake.Setup(objectSpawner, levelGrid);
        objectSpawner.Setup(snake, levelGrid);
        
    }

    
}
