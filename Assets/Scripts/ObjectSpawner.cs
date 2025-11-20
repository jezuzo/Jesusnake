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
    private Vector2Int bushGridPosition;
    private List<Vector2Int> objectsGridPositionList;
    private List<Vector2Int> obstaclesGridPositionList;
    private List<Vector2Int> foodGridPositionList;
    private GameObject foodGameObject;
    private GameObject boxGameObject;
    private GameObject boxUnlockGameObject;
    private GameObject bushGameObject;
    private int width = 20;
    private int height = 20;
    public void Setup(Snake snake)
    {
        
        this.snake = snake;
        objectsGridPositionList = new List<Vector2Int>();
        obstaclesGridPositionList = new List<Vector2Int>();
        foodGridPositionList = new List<Vector2Int>();
       
        SpawnBox();
        spawnRate = spawnRateRef;

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is create

    // Update is called once per frame
    private void Update()
    {
        if (spawnTimer > spawnRate)
        {

            SpawnBush();
            spawnTimer = 0f;
            Invoke("DeleteBush",20f);
        }
        spawnTimer += Time.deltaTime;

        bool boxCollisionedUnlock = TryBoxCollisionUnlock();
        if (boxCollisionedUnlock)
        {
            SpawnFood(boxGridPosition);
            SpawnBox();
        }
    }

    private void SpawnFood(Vector2Int foodPosition = default)
    {
        if(foodPosition == default)
        {
            do
            {
                foodGridPosition = new Vector2Int(Random.Range(0, width), Random.Range(0, height));
            } while (snake.GetFullSnakeGridPositionList().IndexOf(foodGridPosition) != -1 || objectsGridPositionList.Contains(foodGridPosition));

        }
        else
        {
            foodGridPosition = foodPosition;
        }

        
        objectsGridPositionList.Add(foodGridPosition);
        foodGridPositionList.Add(foodGridPosition);
        foodGameObject = new GameObject("Food", typeof(SpriteRenderer));
        foodGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.gameAssets.apple;
        foodGameObject.GetComponent<SpriteRenderer>().sortingOrder = 1;
        foodGameObject.transform.position = new Vector3(foodGridPosition.x, foodGridPosition.y, 0);

    }
    private void SpawnBox()
    {

        do
        {
            boxGridPosition = new Vector2Int(Random.Range(1, width-1), Random.Range(1, height-1));
        } while ((snake.GetFullSnakeGridPositionList().IndexOf(boxGridPosition) != -1) || objectsGridPositionList.Contains(boxGridPosition));
        do
        {
            boxUnlockGridPosition = new Vector2Int(Random.Range(0, width), Random.Range(0, height));
        } while ((snake.GetFullSnakeGridPositionList().IndexOf(boxUnlockGridPosition) != -1) || objectsGridPositionList.Contains(boxUnlockGridPosition));

        objectsGridPositionList.Add(boxGridPosition);
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

    private void SpawnBush()
    {
        spawnRate = Random.Range(spawnRateRef, spawnRateRef + 15f);
        do
        {
            bushGridPosition = new Vector2Int(Random.Range(0, width), Random.Range(0, height));
        } while ((snake.GetFullSnakeGridPositionList().IndexOf(bushGridPosition) != -1) || objectsGridPositionList.Contains(bushGridPosition));
        
        objectsGridPositionList.Add(bushGridPosition);
        obstaclesGridPositionList.Add(bushGridPosition);
        bushGameObject = new GameObject("Bush", typeof(SpriteRenderer));
        bushGameObject.transform.SetParent(GameAssets.gameAssets.obstacles.transform);
        bushGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.gameAssets.obstacle;
        bushGameObject.GetComponent<SpriteRenderer>().sortingOrder = 1;
        bushGameObject.GetComponent<SpriteRenderer>().color = new Color(0.1254902f, 0.3490196f, 0f);
        bushGameObject.transform.position = new Vector3(bushGridPosition.x, bushGridPosition.y, 0);


    }
    private void DeleteBush()
    {
        Transform[] obstaclesChilds = GameAssets.gameAssets.obstacles.GetComponentsInChildren<Transform>();
        obstaclesGridPositionList.Remove(new Vector2Int((int)obstaclesChilds[1].position.x, (int)obstaclesChilds[1].position.y));
        Destroy(obstaclesChilds[1].gameObject);
    }
    public void MoveBox(Vector2Int gridMoveDirection)
    {
        boxGridPosition += gridMoveDirection;
        boxGameObject.transform.position = new Vector3(boxGridPosition.x, boxGridPosition.y, 0);
        objectsGridPositionList.Add(boxGridPosition);
    }
    public bool TrySnakeEatFood(Vector2Int snakeGridPosition)
    {
        if (foodGridPositionList.Contains(snakeGridPosition))
        {
            Object.Destroy(foodGameObject);
            objectsGridPositionList.Remove(foodGridPosition);
            foodGridPositionList.Remove(foodGridPosition);
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool TrySnakeCollisionBorder(Vector2Int snakeGridPosition)
    {
        Transform[] borderChilds = GameAssets.gameAssets.borders.GetComponentsInChildren<Transform>();

        Transform borderHorizontal1 = borderChilds[1];
        Transform borderHorizontal2 = borderChilds[2];
        Transform borderVertical1 = borderChilds[3];
        Transform borderVertical2 = borderChilds[4];

        if ((snakeGridPosition.y > borderHorizontal1.position.y) || (snakeGridPosition.y < borderHorizontal2.position.y) ||
        (snakeGridPosition.x < borderVertical1.position.x) || (snakeGridPosition.x > borderVertical2.position.x))
        {
            return true;
        }

        return false;
    }
    public bool TrySnakeCollisionSnake(Vector2Int snakeGridPosition)
    {
        Transform[] snakeBodyParts = GameAssets.gameAssets.parentBodyParts
        .GetComponentsInChildren<Transform>()
        .Where(t => t != GameAssets.gameAssets.parentBodyParts.transform)
        .ToArray();

        foreach (Transform snakeBodyPart in snakeBodyParts)
        {
            Vector2Int snakeBodyPartPosition = new Vector2Int((int)snakeBodyPart.position.x, (int)snakeBodyPart.position.y);
            if (snakeBodyPartPosition == snakeGridPosition)
            {
                return true;
            }
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
        if (obstaclesGridPositionList.Contains(snakeGridPosition))
        {
            objectsGridPositionList.Remove(bushGridPosition);
            return true;
        }
        else
        {
            return false;
        }

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
}
