using System.Collections.Generic;
using System.Collections;
using UnityEditor;
using UnityEngine;
using System.Linq;

public class LevelGrid
{
    private Vector2Int foodGridPosition;
    private GameObject foodGameObject;
    private int width;
    private int height;
    private Snake snake;
    

    public void GenerateGrid(int width, int height)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var spawnedTile = GameObject.Instantiate(GameAssets.gameAssets.tile, new Vector3(x, y), Quaternion.identity, GameAssets.gameAssets.parentTiles);
                spawnedTile.name = $"Tile {x} {y}";
                var isOffset = (x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0);
                spawnedTile.Init(isOffset);
            }
        }
        Camera.main.transform.position = new Vector3((float)width / 2 - 0.5f, (float)height / 2 - 0.5f,-10);
    }
    public void Setup(Snake snake)
    {
        this.snake = snake;
        SpawnFood();
    }
    public LevelGrid(int width, int height)
    {
        this.width = width;
        this.height = height;
        GenerateGrid(width, height);
        
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
        if (snakeGridPosition == foodGridPosition)
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
    public bool TrySnakeCollisionBorder(Vector2Int snakeGridPosition)
    {
        Transform[] borderChilds = GameAssets.gameAssets.borders.GetComponentsInChildren<Transform>();

        Transform borderHorizontal1 = borderChilds[1];
        Transform borderHorizontal2 = borderChilds[2];
        Transform borderVertical1 = borderChilds[3];
        Transform borderVertical2= borderChilds[4];

        if ((snakeGridPosition.y > borderHorizontal1.position.y)|| (snakeGridPosition.y < borderHorizontal2.position.y) ||
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
}