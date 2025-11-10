using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class Snake : MonoBehaviour
{
    private enum Direction
    {
        Left,
        Right,
        Up,
        Down
    }
    private float gridMoveTimer;
    private float gridMoveTimerMax;
    private Vector2Int gridPosition;
    private Direction gridMoveDirection;
    private LevelGrid levelGrid;
    private int snakeBodySize;
    private List<SnakeMovePosition> snakeMovePositionList;
    private List<SnakeBodyPart> snakeBodyPartsList;
    public void Setup(LevelGrid levelGrid)
    {
        this.levelGrid = levelGrid;
    }
    private void Awake()
    {
        gridPosition = new Vector2Int(10, 10);
        gridMoveDirection = Direction.Right;
        gridMoveTimerMax = .2f;
        gridMoveTimer = gridMoveTimerMax;
        snakeMovePositionList = new List<SnakeMovePosition>();
        snakeBodyPartsList = new List<SnakeBodyPart>();
        snakeBodySize = 0;
    }
    private void Update()
    {
        ManageInput();
        ManageGridMovement();
    }

    private void ManageInput()
    {
        if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            if (gridMoveDirection != Direction.Right)
            {
                gridMoveDirection = Direction.Left;
            }
        }
        else if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            if (gridMoveDirection != Direction.Left)
            {
                gridMoveDirection = Direction.Right;
            }
        }
        else if (Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            if (gridMoveDirection != Direction.Down)
            {
                gridMoveDirection = Direction.Up;
            }
        }
        else if (Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            if (gridMoveDirection != Direction.Up)
            {
                gridMoveDirection = Direction.Down;
            }
        }
    }

    private void ManageGridMovement()
    {
        gridMoveTimer += Time.deltaTime;

        if (gridMoveTimer >= gridMoveTimerMax)
        {
            gridMoveTimer -= gridMoveTimerMax;
            SnakeMovePosition previousSnakeMovePosition = null;
            if(snakeMovePositionList.Count > 0)
            {
                previousSnakeMovePosition = snakeMovePositionList[0];
            }
            SnakeMovePosition snakeMovePosition = new SnakeMovePosition(gridPosition, gridMoveDirection, previousSnakeMovePosition);
            snakeMovePositionList.Insert(0, snakeMovePosition);

            Vector2Int gridMoveDirectionVector;
            switch (gridMoveDirection)
            {
                default:
                case Direction.Right: gridMoveDirectionVector = new Vector2Int(1, 0); break;
                case Direction.Left: gridMoveDirectionVector = new Vector2Int(-1, 0); break;
                case Direction.Up: gridMoveDirectionVector = new Vector2Int(0, 1); break;
                case Direction.Down: gridMoveDirectionVector = new Vector2Int(0, -1); break;

            }
            
            gridPosition += gridMoveDirectionVector;

            
            
            bool snakeAteFood = levelGrid.TrySnakeEatFood(gridPosition);
            if (snakeAteFood)
            {
                snakeBodySize++;
                CreateSnakeBody();
            }

            if (snakeMovePositionList.Count >= snakeBodySize + 1)
            {
                snakeMovePositionList.RemoveAt(snakeMovePositionList.Count - 1);
            }

            transform.position = new Vector3(gridPosition.x, gridPosition.y);
            transform.eulerAngles = new Vector3(0, 0, GetAngleFromVector(gridMoveDirectionVector) - 180);

            UpdateSnakeBodyParts();
        }


    }
    private void CreateSnakeBody()
    {
        snakeBodyPartsList.Add(new SnakeBodyPart(snakeBodyPartsList.Count));
    }
    private void UpdateSnakeBodyParts()
    {
        for (int i = 0; i < snakeBodyPartsList.Count; i++)
        {
            snakeBodyPartsList[i].SetSnakeMovePosition(snakeMovePositionList[i]);
        }
    }

    private float GetAngleFromVector(Vector2Int dir)
    {
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;
        return n;
    }

    public Vector2Int GetGridPosition()
    {
        return gridPosition;
    }
    public List<Vector2Int> GetFullSnakeGridPositionList()
    {
        List<Vector2Int> gridPositionList = new List<Vector2Int>() { gridPosition };
        foreach(SnakeMovePosition snakeMovePosition in snakeMovePositionList)
        {
            gridPositionList.Add(snakeMovePosition.GetGridPosition());
        }
        return gridPositionList;
    }

    private class SnakeBodyPart
    {
        private Sprite snakeBodySprite;
        private SnakeMovePosition snakeMovePosition;
        private Transform transform;
        public SnakeBodyPart(int bodyIndex)
        {
            GameObject snakeBodyGameObject = new GameObject("SnakeBody", typeof(SpriteRenderer));
            snakeBodyGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.gameAssets.snakeBody;
            snakeBodyGameObject.GetComponent<SpriteRenderer>().sortingOrder = -bodyIndex;
            transform = snakeBodyGameObject.transform;
        }
        public void SetSnakeMovePosition(SnakeMovePosition snakeMovePosition)
        {
            this.snakeMovePosition = snakeMovePosition;
            transform.position = new Vector3(snakeMovePosition.GetGridPosition().x, snakeMovePosition.GetGridPosition().y);
            float angle;
            // Sprite leftDownCorner = GameAssets.gameAssets.leftDownCorner;
            // Sprite rightDownCorner = GameAssets.gameAssets.rightDownCorner;
            // Sprite leftUpCorner = GameAssets.gameAssets.leftUpCorner;
            // Sprite rightUpCorner = GameAssets.gameAssets.rightUpCorner;
            switch (snakeMovePosition.GetDirection())
            {
                default:
                case Direction.Left: angle = 0; break;
                case Direction.Down: angle = -90; break;
                case Direction.Up: angle = 90; break;
                case Direction.Right:
                    switch (snakeMovePosition.GetPreviousDirection())
                    {
                        default:
                            angle = 180; break;
                        case Direction.Down:
                            angle = 135; break;
                    }
                    break;    
            }
            transform.eulerAngles = new Vector3(0, 0, angle);

            
        }
    }

    private class SnakeMovePosition
    {
        private SnakeMovePosition previousSnakeMovePosition;
        private Vector2Int gridPosition;
        private Direction direction;

        public SnakeMovePosition(Vector2Int gridPosition, Direction direction, SnakeMovePosition previousSnakeMovePosition)
        {
            this.gridPosition = gridPosition;
            this.direction = direction;
            this.previousSnakeMovePosition = previousSnakeMovePosition;
        }

        public Vector2Int GetGridPosition()
        {
            return gridPosition;
        }

        public Direction GetDirection()
        {
            return direction;
        }
        
        public Direction GetPreviousDirection()
        {
            if(previousSnakeMovePosition == null)
            {
                return Direction.Right;
            }
            else
            {
                return previousSnakeMovePosition.direction;
            }
            
        }
    }

}
