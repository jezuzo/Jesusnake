
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UIElements;
using static UnityEngine.Tilemaps.TilemapRenderer;

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
    private Vector2Int chainedGridPosition;
    private Vector2Int keyGridPosition;
    private Vector2Int arrowGridPosition;
    private List<Vector2Int> objectsGridPositionList;
    private List<Obstacle> obstaclesList;
    private List<Food> foodList;
    private GameObject foodGameObject;
    private GameObject boxGameObject;
    private GameObject boxUnlockGameObject;
    private GameObject obstacleGameObject;
    private GameObject chainedGameObject;
    private GameObject keyGameObject;
    private GameObject arrowGameObject;
    private int width;
    private int height;
    private bool chainedApplesEnabled = false;
    private bool boxesEnabled = false;
    private bool appleArrowsEnabled = false;
    private Arrow arrow;

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

    private class Arrow
    {
        public Vector2Int arrowGridPosition;
        public GameObject arrowGameObject;
        private Direction arrowDirection;
        private Sprite arrowSprite;

        public Arrow(Vector2Int arrowGridPosition, GameObject arrowGameObject, Direction arrowDirection)
        {
            this.arrowGridPosition = arrowGridPosition;
            this.arrowGameObject = arrowGameObject;
            this.arrowDirection = arrowDirection;
            SetArrowSprite();
            SetArrowGameObject();

        }
         public Direction GetArrowDirection()
        {
            return arrowDirection;
        }

        public GameObject GetArrowGameObject()
        {
            return arrowGameObject; 
        }

        public Vector2Int GetArrowGridPosition()
        {
            return arrowGridPosition;
        }

        public void SetArrowSprite()
        {
            switch (arrowDirection)
            {
                case Direction.Left:
                    arrowSprite = GameAssets.gameAssets.leftArrow;
                    break;
                case Direction.Right:
                    arrowSprite = GameAssets.gameAssets.rightArrow;
                    break;
                case Direction.Up:
                    arrowSprite = GameAssets.gameAssets.upArrow;
                    break;
                case Direction.Down:
                    arrowSprite = GameAssets.gameAssets.downArrow;
                    break;
            }
            
        }

        public void SetArrowGameObject()
        {
            arrowGameObject = new GameObject("Arrow", typeof(SpriteRenderer));
            arrowGameObject.GetComponent<SpriteRenderer>().sprite = arrowSprite;
            arrowGameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Objects";
            arrowGameObject.GetComponent<SpriteRenderer>().sortingOrder = 1;
            arrowGameObject.transform.position = new Vector3(arrowGridPosition.x, arrowGridPosition.y, 0);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is create

    // Update is called once per frame
    private void Update()
    {

        EnableArrowApples();
        EnableObstacles();
        //EnableChainedApples();
        //EnableBoxes();
        
    }

    private void EnableObstacles()
    {

        if (spawnTimer > spawnRate)
        {
            SpawnObstacle();
            spawnTimer = 0f;
            Invoke("DeleteObstacle", 20f);
        }
        spawnTimer += Time.deltaTime;

    }

    private void EnableBoxes()
    {
        if (!boxesEnabled)
        {
            SpawnBox();
            boxesEnabled = true;
           
        }
        
        bool boxCollisionedUnlock = TryBoxCollisionUnlock();
        bool boxCollisionedObstacle = TryBoxCollisionObstacle();
        if (boxCollisionedUnlock)
        {
            SpawnFood(boxGridPosition);
            SpawnBox();
        }
        if (boxCollisionedObstacle)
        {

            boxGridPosition = new Vector2Int(UnityEngine.Random.Range(2, width - 1), UnityEngine.Random.Range(2, height - 1));
            boxGameObject.transform.position = new Vector3(boxGridPosition.x, boxGridPosition.y, 0);
            objectsGridPositionList.Add(boxGridPosition);
        }

    }

   

    private void EnableChainedApples()
    {
        if (!chainedApplesEnabled)
        {
            SpawnAppleChained();
            chainedApplesEnabled = true;
        }
        
    }

    private void EnableNormalMode()
    {

        if(foodGameObject == null)
        {
            SpawnFood();
        }
    }

    private void EnableArrowApples()
    {
        if (!appleArrowsEnabled)
        {
            SpawnArrowApple();
            appleArrowsEnabled = true;
        }
    }

    private GameObject CreateGameObject(string name, Vector2Int position, Sprite sprite, int sortOrder = 1, 
        string sortingLayerName = "Default")
    {
        objectsGridPositionList.Add(position);
        GameObject gameObject;
        gameObject = new GameObject(name, typeof(SpriteRenderer));
        gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
        gameObject.GetComponent<SpriteRenderer>().sortingLayerName = sortingLayerName;
        gameObject.GetComponent<SpriteRenderer>().sortingOrder = sortOrder;
        gameObject.transform.position = new Vector3(position.x, position.y, 0);
        return gameObject;

    }

    


    private void SpawnFood(Vector2Int foodPosition = default)
    {
        if(foodPosition == default)
        {
            do
            {
                foodGridPosition = new Vector2Int(UnityEngine.Random.Range(1, width), UnityEngine.Random.Range(1, height));
            } while (snake.GetFullSnakeGridPositionList().IndexOf(foodGridPosition) != -1 || objectsGridPositionList.Contains(foodGridPosition));

        }
        else
        {
            foodGridPosition = foodPosition;
        }

        foodGameObject = CreateGameObject("Food", foodGridPosition, GameAssets.gameAssets.apple, 0, "Objects");

        foodList.Add(new Food(foodGridPosition, foodGameObject));

    }
    private void SpawnBox()
    {

        do
        {
            boxGridPosition = new Vector2Int(UnityEngine.Random.Range(2, width-1), UnityEngine.Random.Range(2, height-1));
        } while ((snake.GetFullSnakeGridPositionList().IndexOf(boxGridPosition) != -1) || objectsGridPositionList.Contains(boxGridPosition));

        boxGameObject = CreateGameObject("Box", boxGridPosition, GameAssets.gameAssets.box, 1, "Objects");

        do
        {
            boxUnlockGridPosition = new Vector2Int(UnityEngine.Random.Range(1, width), UnityEngine.Random.Range(1, height));
        } while ((snake.GetFullSnakeGridPositionList().IndexOf(boxUnlockGridPosition) != -1) || objectsGridPositionList.Contains(boxUnlockGridPosition));

        boxUnlockGameObject = CreateGameObject("BoxUnlock",boxUnlockGridPosition, GameAssets.gameAssets.boxUnlock,0, "Objects");


    }

    private void SpawnAppleChained()
    {

        do
        {
            chainedGridPosition = new Vector2Int(UnityEngine.Random.Range(2, width - 1), UnityEngine.Random.Range(2, height - 1));
        } while ((snake.GetFullSnakeGridPositionList().IndexOf(chainedGridPosition) != -1) || objectsGridPositionList.Contains(chainedGridPosition));

        chainedGameObject = CreateGameObject("Locker", chainedGridPosition, GameAssets.gameAssets.locker, 1, "Objects");

        do
        {
            keyGridPosition = new Vector2Int(UnityEngine.Random.Range(1, width), UnityEngine.Random.Range(1, height));
        } while ((snake.GetFullSnakeGridPositionList().IndexOf(keyGridPosition) != -1) || objectsGridPositionList.Contains(keyGridPosition));

        if(obstaclesList.Count > 0)
        {
            for (int i = 0; i < obstaclesList.Count; i++)
            {
                if ((obstaclesList[i].GetObstacleGridPosition().x == width - 2 && obstaclesList[i].GetObstacleGridPosition().y == height - 1 && keyGridPosition.x == width -1 && keyGridPosition.y ==height-1)
                    || (obstaclesList[i].GetObstacleGridPosition().x == 2 && obstaclesList[i].GetObstacleGridPosition().y == height - 1 && keyGridPosition.x == 1 && keyGridPosition.y == height - 1)
                    || (obstaclesList[i].GetObstacleGridPosition().x == 2  && obstaclesList[i].GetObstacleGridPosition().y ==  1 && keyGridPosition.x == 1 && keyGridPosition.y == 1)
                    || (obstaclesList[i].GetObstacleGridPosition().x == width-1 && obstaclesList[i].GetObstacleGridPosition().y == 2 && keyGridPosition.x == width -1 && keyGridPosition.y == 1))
                {
                    keyGridPosition = new Vector2Int(UnityEngine.Random.Range(1, width), UnityEngine.Random.Range(1, height));
                }
            }

        }
        

        keyGameObject = CreateGameObject("Key", keyGridPosition, GameAssets.gameAssets.key, 0, "Objects");
        SpawnFood(chainedGridPosition);

        

    }

    private void SpawnObstacle()
    {
        spawnRate = UnityEngine.Random.Range(spawnRateRef, spawnRateRef + 15f);
        do
        {
            obstacleGridPosition = new Vector2Int(UnityEngine.Random.Range(1, width), UnityEngine.Random.Range(1, height));
        } while ((snake.GetFullSnakeGridPositionList().IndexOf(obstacleGridPosition) != -1) 
        || objectsGridPositionList.Contains(obstacleGridPosition)
        || (appleArrowsEnabled && (obstacleGridPosition.x == arrow.GetArrowGridPosition().x + 1 && arrow.GetArrowDirection() == Direction.Left)
        || (appleArrowsEnabled && (obstacleGridPosition.x == arrow.GetArrowGridPosition().x - 1 && arrow.GetArrowDirection() == Direction.Right))
        || (appleArrowsEnabled && (obstacleGridPosition.y == arrow.GetArrowGridPosition().y + 1 && arrow.GetArrowDirection() == Direction.Down))
        || (appleArrowsEnabled && (obstacleGridPosition.y == arrow.GetArrowGridPosition().y - 1 && arrow.GetArrowDirection() == Direction.Up)))
        || (chainedApplesEnabled && ( keyGridPosition.x == width -1 && keyGridPosition.y == height -1 && (obstacleGridPosition.x == width - 2 || obstacleGridPosition.y == height - 2)))
        || (chainedApplesEnabled && (keyGridPosition.x == 1 && keyGridPosition.y == 1 && (obstacleGridPosition.x == 2 || obstacleGridPosition.y == 2)))
        );

        obstacleGameObject = CreateGameObject("Obstacle", obstacleGridPosition, GameAssets.gameAssets.obstacle,0,"Objects");
       
        obstacleGameObject.transform.SetParent(GameAssets.gameAssets.obstacles.transform);
        obstacleGameObject.GetComponent<SpriteRenderer>().color = new Color(0.1254902f, 0.3490196f, 0f);
        obstaclesList.Add(new Obstacle(obstacleGridPosition, obstacleGameObject));



    }
    Direction GetRandomDirection()
    {
        Direction[] values = (Direction[])Enum.GetValues(typeof(Direction));
        int index = UnityEngine.Random.Range(0, values.Length);
        return values[index];
    }

    private void SpawnArrowApple()
    {
        do
        {
            arrowGridPosition = new Vector2Int(UnityEngine.Random.Range(1, width), UnityEngine.Random.Range(1, height));
        } while ((snake.GetFullSnakeGridPositionList().IndexOf(arrowGridPosition) != -1) || objectsGridPositionList.Contains(arrowGridPosition));

        Direction randomDir = GetRandomDirection();
        for(int i = 0; i < objectsGridPositionList.Count; i++)
        {
            if(arrowGridPosition.x == objectsGridPositionList[i].x + 1)
            {
                do
                {
                    randomDir = GetRandomDirection();
                } while (randomDir == Direction.Right);

            }
            if(arrowGridPosition.x == objectsGridPositionList[i].x - 1)
            {

                do
                {
                    randomDir = GetRandomDirection();
                } while (randomDir == Direction.Left);

            }
            if(arrowGridPosition.y == objectsGridPositionList[i].y + 1)
            {
                do
                {
                    randomDir = GetRandomDirection();
                } while (randomDir == Direction.Up);

            }
            if(arrowGridPosition.y == objectsGridPositionList[i].y - 1)
            {
                do
                {
                    randomDir = GetRandomDirection();
                } while (randomDir == Direction.Down);

            }
        }
        if(arrowGridPosition.x == width -1)
        {
            Debug.Log("true");
            do
            {
                randomDir = GetRandomDirection();
            } while( randomDir == Direction.Left);

        }if(arrowGridPosition.x == 1)
        {
            do
            {
                randomDir = GetRandomDirection();
            } while (randomDir == Direction.Right);

        }if(arrowGridPosition.y == height - 1)
        {
            do
            {
                randomDir = GetRandomDirection();
            } while (randomDir == Direction.Down);

        }if(arrowGridPosition.y == 1)
        {
            do
            {
                randomDir = GetRandomDirection();
            } while (randomDir == Direction.Up);

        }
            arrow = new Arrow(arrowGridPosition, arrowGameObject, randomDir);

        SpawnFood(arrowGridPosition);

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
    public bool TrySnakeEatFood(Vector2Int snakeGridPosition, Direction snakeGridDirection)
    {
        for(int i = 0; i < foodList.Count; i++)
        {
            if (foodList[i].GetFoodGridPosition() == snakeGridPosition && (foodList[i].GetFoodGridPosition() != chainedGridPosition || chainedGameObject == null)
                && (arrow == null || foodList[i].GetFoodGridPosition() != arrow.GetArrowGridPosition() || arrow.GetArrowDirection() == snakeGridDirection))
            {
                Destroy(foodList[i].GetFoodGameObject());
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
            Destroy(boxGameObject);
            Destroy(boxUnlockGameObject);
            objectsGridPositionList.Remove(boxGridPosition);
            objectsGridPositionList.Remove(boxUnlockGridPosition);
            return true;
        }
        else
        {
            return false;
        }
    }
    public void TrySnakeCollisionKey(Vector2Int snakeGridPosition)
    {
        if (snakeGridPosition == keyGridPosition)
        {
            Destroy(keyGameObject);
            Destroy(chainedGameObject);
            objectsGridPositionList.Remove(keyGridPosition);
            
        }
        
    }

    public void TrySnakeCollisionArrowApple(Vector2Int snakeGridPosition, Direction snakeDirection)
    {
        if (appleArrowsEnabled)
        {
            if (snakeGridPosition == arrowGridPosition && snakeDirection == arrow.GetArrowDirection())
            {
                Destroy(arrow.GetArrowGameObject());
                objectsGridPositionList.Remove(arrow.GetArrowGridPosition());
                SpawnArrowApple();

            }

        }
        
       
     
    }

    public bool TrySnakeCollisionChainedApple(Vector2Int snakeGridPosition)
    {
        if (chainedApplesEnabled)
        {
            if (snakeGridPosition == chainedGridPosition && chainedGameObject != null)
            {
                return true;
            }
            else if (snakeGridPosition == chainedGridPosition && chainedGameObject == null)
            {
                objectsGridPositionList.Remove(chainedGridPosition);
                SpawnAppleChained();
                return false;
            }
            else
            {
                return false;
            }

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
        if(snake.GetFullSnakeGridPositionList().IndexOf(boxGridPosition) != -1 && snake.GetFullSnakeGridPositionList().IndexOf(boxGridPosition) != 0)
        {
            return true;
        }
        return false;
    }
}
