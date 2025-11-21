using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    private float spawnTimer;
    public float spawnRateRef = 2f;
    private float spawnRate;
    private Snake snake;
    private Vector2Int foodGridPosition;
    private Vector2Int boxGridPosition;
    private Vector2Int boxUnlockGridPosition;
    private Vector2Int obstacleGridPosition;
    private List<Vector2Int> objectsGridPositionList;
    private List<Obstacle> obstaclesList;
    private List<Food> foodList;
    private GameObject foodGameObject;
    private GameObject boxGameObject;
    private GameObject boxUnlockGameObject;
    private GameObject obstacleGameObject;
    private int width;
    private int height;
    public void Setup(Snake snake, LevelGrid levelGrid)
    {
        
        this.snake = snake;
        objectsGridPositionList = new List<Vector2Int>();
        obstaclesList = new List<Obstacle>();
        foodList = new List<Food>();
        width = levelGrid.width - 1;
        height = levelGrid.height - 1;
        
        
        spawnRate = spawnRateRef;

    }
    private void Start()
    {
        SpawnBox();
    }
    private class Food
    {
        private Vector2Int foodGridPosition;
        private GameObject foodGameObject;

        public Food(Vector2Int foodGridPosition, GameObject foodGameObject)
        {
            this.foodGridPosition = foodGridPosition;
            this.foodGameObject = foodGameObject;
        }

        public Vector2Int GetFoodGridPosition()
        {
            return foodGridPosition;
        }
        public GameObject GetFoodGameObject()
        {
            return foodGameObject; 
        }
    }

    private class Obstacle
    {
        public Vector2Int obstacleGridPosition;
        public GameObject obstacleGameObject;
        public Obstacle(Vector2Int obstacleGridPosition, GameObject obstacleGameObject)
        {
            this.obstacleGridPosition = obstacleGridPosition;
            this.obstacleGameObject = obstacleGameObject;
        }
        public Vector2Int GetObstacleGridPosition()
        {
            return obstacleGridPosition;
        }
        public GameObject GetObstacleGameObject()
        {
            return obstacleGameObject;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is create

    // Update is called once per frame
    private void Update()
    {
        //if (spawnTimer > spawnRate)
        //{

        //    SpawnObstacle();
        //    spawnTimer = 0f;
        //    Invoke("DeleteObstacle",20f);
        //}
        //spawnTimer += Time.deltaTime;

        bool boxCollisionedUnlock = TryBoxCollisionUnlock();
        bool boxCollisionedObstacle = TryBoxCollisionObstacle();
        if (boxCollisionedUnlock)
        {
            SpawnFood(boxGridPosition);
            SpawnBox();
        }
        if (boxCollisionedObstacle)
        {
            
            boxGridPosition = new Vector2Int(Random.Range(2, width - 1), Random.Range(2, height - 1));
            boxGameObject.transform.position = new Vector3(boxGridPosition.x, boxGridPosition.y, 0);
            objectsGridPositionList.Add(boxGridPosition);
        }
    }

    private void SpawnFood(Vector2Int foodPosition = default)
    {
        if(foodPosition == default)
        {
            do
            {
                foodGridPosition = new Vector2Int(Random.Range(1, width), Random.Range(1, height));
            } while (snake.GetFullSnakeGridPositionList().IndexOf(foodGridPosition) != -1 || objectsGridPositionList.Contains(foodGridPosition));

        }
        else
        {
            foodGridPosition = foodPosition;
        }

        
        objectsGridPositionList.Add(foodGridPosition);
        
        foodGameObject = new GameObject("Food", typeof(SpriteRenderer));
        foodGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.gameAssets.apple;
        foodGameObject.GetComponent<SpriteRenderer>().sortingOrder = 1;
        foodGameObject.transform.position = new Vector3(foodGridPosition.x, foodGridPosition.y, 0);
        foodList.Add(new Food(foodGridPosition, foodGameObject));

    }
    private void SpawnBox()
    {

        do
        {
            boxGridPosition = new Vector2Int(Random.Range(2, width-1), Random.Range(2, height-1));
        } while ((snake.GetFullSnakeGridPositionList().IndexOf(boxGridPosition) != -1) || objectsGridPositionList.Contains(boxGridPosition));
        objectsGridPositionList.Add(boxGridPosition);
        do
        {
            boxUnlockGridPosition = new Vector2Int(Random.Range(1, width), Random.Range(1, height));
        } while ((snake.GetFullSnakeGridPositionList().IndexOf(boxUnlockGridPosition) != -1) || objectsGridPositionList.Contains(boxUnlockGridPosition));

        objectsGridPositionList.Add(boxUnlockGridPosition);
        boxGameObject = new GameObject("Box", typeof(SpriteRenderer));
        boxGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.gameAssets.box;
        boxGameObject.GetComponent<SpriteRenderer>().sortingOrder = 1;
        boxGameObject.transform.position = new Vector3(boxGridPosition.x, boxGridPosition.y, 0);

        boxUnlockGameObject = new GameObject("BoxUnlock", typeof(SpriteRenderer));
        boxUnlockGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.gameAssets.boxUnlock;
        boxUnlockGameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Background";
        boxUnlockGameObject.GetComponent<SpriteRenderer>().sortingOrder = 1;
        boxUnlockGameObject.transform.position = new Vector3(boxUnlockGridPosition.x, boxUnlockGridPosition.y, 0);

    }

    private void SpawnObstacle()
    {
        spawnRate = Random.Range(spawnRateRef, spawnRateRef + 15f);
        do
        {
            obstacleGridPosition = new Vector2Int(Random.Range(1, width), Random.Range(1, height));
        } while ((snake.GetFullSnakeGridPositionList().IndexOf(obstacleGridPosition) != -1) || objectsGridPositionList.Contains(obstacleGridPosition));
        
        objectsGridPositionList.Add(obstacleGridPosition);
        obstacleGameObject = new GameObject("Bush", typeof(SpriteRenderer));
        obstacleGameObject.transform.SetParent(GameAssets.gameAssets.obstacles.transform);
        obstacleGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.gameAssets.obstacle;
        obstacleGameObject.GetComponent<SpriteRenderer>().sortingOrder = 1;
        obstacleGameObject.GetComponent<SpriteRenderer>().color = new Color(0.1254902f, 0.3490196f, 0f);
        obstacleGameObject.transform.position = new Vector3(obstacleGridPosition.x, obstacleGridPosition.y, 0);
        obstaclesList.Add(new Obstacle(obstacleGridPosition, obstacleGameObject));



    }
    private void DeleteObstacle()
    {
        Destroy(obstaclesList[0].GetObstacleGameObject());
        objectsGridPositionList.Remove(obstaclesList[0].GetObstacleGridPosition());
        obstaclesList.Remove(obstaclesList[0]);
        
    }
    public void MoveBox(Vector2Int gridMoveDirection)
    {
        boxGridPosition += gridMoveDirection;
        boxGameObject.transform.position = new Vector3(boxGridPosition.x, boxGridPosition.y, 0);
        objectsGridPositionList.Add(boxGridPosition);
    }
    public bool TrySnakeEatFood(Vector2Int snakeGridPosition)
    {
        for(int i = 0; i < foodList.Count; i++)
        {
            if (foodList[i].GetFoodGridPosition() == snakeGridPosition)
            {
                Object.Destroy(foodList[i].GetFoodGameObject());
                objectsGridPositionList.Remove(foodList[i].GetFoodGridPosition());
                foodList.Remove(foodList[i]);
                return true;
            }
        }
        return false;

    }
    public bool TrySnakeCollisionBorder(Vector2Int snakeGridPosition)
    {

        if (snakeGridPosition.x == 0 || snakeGridPosition.y == 0 || snakeGridPosition.x == width|| snakeGridPosition.y == height)
        {
            return true;
        }

        return false;
    }
    public bool TrySnakeCollisioningBox(Vector2Int snakeGridPosition)
    {
        if (snakeGridPosition == boxGridPosition)
        {
            objectsGridPositionList.Remove(boxGridPosition);
            return true;
        }
        else
        {
            return false;
        }

    }
    public bool TrySnakeCollisionObstacle(Vector2Int snakeGridPosition)
    {
        for (int i = 0; i < obstaclesList.Count; i++)
        {
            if (obstaclesList[i].GetObstacleGridPosition() == snakeGridPosition)
            {
                return true;
            }
        }
        return false;
    }

    private bool TryBoxCollisionUnlock()
    {
        if (boxGridPosition == boxUnlockGridPosition)
        {
            Object.Destroy(boxGameObject);
            Object.Destroy(boxUnlockGameObject);
            objectsGridPositionList.Remove(boxGridPosition);
            objectsGridPositionList.Remove(boxUnlockGridPosition);
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool TryBoxCollisionObstacle()
    {
        for (int i = 0; i < obstaclesList.Count; i++)
        {
            if (obstaclesList[i].GetObstacleGridPosition() == boxGridPosition)
            {
                objectsGridPositionList.Remove(boxGridPosition);
                return true;
            }
            
        }
        if (boxGridPosition.x == 0 || boxGridPosition.y == 0 || boxGridPosition.x == width || boxGridPosition.y == height)
        {
            return true;
        }
        if(snake.GetFullSnakeGridPositionList().IndexOf(boxGridPosition) != -1)
        {
            return true;
        }
        return false;
    }
}
