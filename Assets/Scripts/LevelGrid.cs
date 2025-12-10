using System.Collections.Generic;
using System.Collections;
using UnityEditor;
using UnityEngine;
using System.Linq;

public class LevelGrid
{
    public int width;
    public int height;
    
    

    public void GenerateGrid(int width, int height)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var spawnedTile = GameObject.Instantiate(GameAssets.gameAssets.tile, new Vector3(x, y), Quaternion.identity, GameAssets.gameAssets.parentTiles);
                spawnedTile.name = $"Tile {x} {y}";

                var isOffset = (x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0);
                var isBorder = (x == 0 || y == 0 || x == width-1 || y == height-1);
                spawnedTile.GetComponent<SpriteRenderer>().sortingOrder = isBorder ? 1 : 0;
                spawnedTile.Init(isOffset, isBorder);
            }
        }
        Camera.main.transform.position = new Vector3((float)width / 2 - 0.5f, (float)height / 2 - 0.5f,-10);
        Camera.main.orthographicSize = width/2 + 1;
    }
    
    public LevelGrid(int width, int height)
    {
        this.width = width;
        this.height = height;
        GenerateGrid(width, height);
        
    }
    
}