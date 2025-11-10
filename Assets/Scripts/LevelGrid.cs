using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LevelGrid
{
    private Vector2Int foodGridPosition;
    private GameObject foodGameObject;
    private int width;
    private int height;
    private Snake snake;
    public void Setup(Snake snake)
    {
        this.snake = snake;
        SpawnFood();
    }
    public LevelGrid(int width, int height)
    {
        this.width = width;
        this.height = height;
        
    }

    private void SpawnFood()
    {
        do
        {
            foodGridPosition = new Vector2Int(Random.Range(0, width), Random.Range(0, height));
        } while (snake.GetFullSnakeGridPositionList().IndexOf(foodGridPosition) != -1);
        
        foodGameObject = new GameObject("Food", typeof(SpriteRenderer));
        foodGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.gameAssets.apple;
        foodGameObject.transform.position = new Vector3(foodGridPosition.x, foodGridPosition.y, 0);
    }
    public bool TrySnakeEatFood(Vector2Int snakeGridPosition)
    {
        if(snakeGridPosition == foodGridPosition)
        {
            Object.Destroy(foodGameObject);
            SpawnFood();
            return true;
        }
        else
        {
            return false;
        }
    }
    
}
