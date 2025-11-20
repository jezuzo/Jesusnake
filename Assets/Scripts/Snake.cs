using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SocialPlatforms.Impl;

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
    private float timeSinceLastInput = 0f;
    private Queue<Direction> inputBuffer;
    private float gridMoveTimerMax;
    private Vector2Int gridPosition;
    private Direction gridMoveDirection;
    private LevelGrid levelGrid;
    private ObjectSpawner objectSpawner;
    private int snakeBodySize;
    private List<SnakeMovePosition> snakeMovePositionList;
    private List<SnakeBodyPart> snakeBodyPartsList;


    public void Setup(LevelGrid levelGrid, ObjectSpawner objectSpawner)
    {
        this.levelGrid = levelGrid;
        this.objectSpawner = objectSpawner;

    }
    public void InitialState()
    {
        CreateSnakeBody();
        snakeBodySize++;

    }
    public void Setup()
    {
        gridPosition = new Vector2Int(10, 10);
        gridMoveDirection = Direction.Right;
        gridMoveTimerMax = .2f;
        gridMoveTimer = gridMoveTimerMax;
        snakeMovePositionList = new List<SnakeMovePosition>();
        snakeBodyPartsList = new List<SnakeBodyPart>();
        snakeBodySize = 0;
        inputBuffer = new Queue<Direction>(); 
        InitialState();
        
    }
    private void Awake()
    {
        Setup();
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
                inputBuffer.Enqueue(Direction.Left);
                timeSinceLastInput = 0f;
                
            }
        }
        else if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            if (gridMoveDirection != Direction.Left)
            {
                inputBuffer.Enqueue(Direction.Right);
                timeSinceLastInput = 0f;
                
               
            }
        }
        else if (Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            if (gridMoveDirection != Direction.Down)
            {
                inputBuffer.Enqueue(Direction.Up);
                timeSinceLastInput = 0f;
                
                
            }
        }
        else if (Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            if (gridMoveDirection != Direction.Up)
            {
                inputBuffer.Enqueue(Direction.Down);
                timeSinceLastInput = 0f;
                
                
            }
        }
        timeSinceLastInput+=Time.deltaTime;
        if (inputBuffer.Count > 0)
        {
            Direction nextDirection = inputBuffer.Peek();
            if (!(gridMoveDirection == Direction.Left && nextDirection == Direction.Right) &&
                !(gridMoveDirection == Direction.Right && nextDirection == Direction.Left) &&
                !(gridMoveDirection == Direction.Up && nextDirection == Direction.Down) &&
                !(gridMoveDirection == Direction.Down && nextDirection == Direction.Up))
            {
                gridMoveDirection = nextDirection;
            }
        }
        if (timeSinceLastInput > gridMoveTimerMax)
        {
            inputBuffer.Clear();
        }
    }

    private void ManageGridMovement()
    {
        gridMoveTimer += Time.deltaTime;

        if (gridMoveTimer >= gridMoveTimerMax)
        {
            gridMoveTimer -= gridMoveTimerMax;
            quitAction();
            

            SnakeMovePosition previousSnakeMovePosition = null;
            if (snakeMovePositionList.Count > 0)
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


            bool snakeAteFood = objectSpawner.TrySnakeEatFood(gridPosition);
            bool snakeCollisionedBorder = objectSpawner.TrySnakeCollisionBorder(gridPosition);
            bool snakeCollisionedSnake = objectSpawner.TrySnakeCollisionSnake(gridPosition);
            bool snakeCollisioningBox = objectSpawner.TrySnakeCollisioningBox(gridPosition);
            bool snakeCollisionedBush = objectSpawner.TrySnakeCollisionObstacle(gridPosition);
            if (snakeAteFood)
            {
                snakeBodySize++;
                CreateSnakeBody();
                ScoreManager.scoreManager.addScore(1);
            }
            if (snakeCollisionedBorder|| snakeCollisionedSnake || snakeCollisionedBush)
            {
                ResetGame();
            }
            if (snakeCollisioningBox)
            {
                objectSpawner.MoveBox(gridMoveDirectionVector);
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
    private void quitAction()
    {
        if(inputBuffer.Count > 0)
        {
            inputBuffer.Dequeue();
        }
        
    }
    private void CreateSnakeBody()
    {
        snakeBodyPartsList.Add(new SnakeBodyPart(snakeBodyPartsList.Count));

    }
    private void ResetGame()
    {
        Transform[] deadBodyParts = GameAssets.gameAssets.parentBodyParts
        .GetComponentsInChildren<Transform>()
        .Where(t => t != GameAssets.gameAssets.parentBodyParts.transform)
        .ToArray();

        foreach (Transform deadBodyPart in deadBodyParts)
        {
            Destroy(deadBodyPart.gameObject);
        }
        
        snakeMovePositionList.Clear();
        snakeBodyPartsList.Clear();
        Setup();

        SnakeMovePosition previousSnakeMovePosition = null;
        SnakeMovePosition snakeMovePosition = new SnakeMovePosition(gridPosition, gridMoveDirection, previousSnakeMovePosition);
        snakeMovePositionList.Insert(0, snakeMovePosition);

        ScoreManager.scoreManager.setScore(0);
        
        
    }
    private void UpdateSnakeBodyParts()
    {
        for (int i = 0; i < snakeBodyPartsList.Count; i++)
        {
            
            snakeBodyPartsList[i].SetSnakeBodySprite(GameAssets.gameAssets.snakeBody);
            snakeBodyPartsList[i].SetSnakeMovePosition(snakeMovePositionList[i]);
        
           
        }
        if (snakeBodyPartsList.Count != 0)
        {
            snakeBodyPartsList[snakeBodyPartsList.Count - 1].SetSnakeBodyAngle(true);
            snakeBodyPartsList[snakeBodyPartsList.Count - 1].SetSnakeBodySprite(GameAssets.gameAssets.snakeTail); 
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
        private int angle;
        private SnakeMovePosition snakeMovePosition;
        private Transform transform;
        private GameObject snakeBodyGameObject;
        public SnakeBodyPart(int bodyIndex)
        {
            //Debug.Log(bodyIndex);
            snakeBodyGameObject = new GameObject("SnakeBody", typeof(SpriteRenderer));
            snakeBodyGameObject.GetComponent<SpriteRenderer>().sortingOrder = -bodyIndex;
            transform = snakeBodyGameObject.transform;
            transform.SetParent(GameAssets.gameAssets.parentBodyParts);
            
        }
        public void SetSnakeBodySprite(Sprite sprite)
        {
            snakeBodySprite = sprite;
            snakeBodyGameObject.GetComponent<SpriteRenderer>().sprite = snakeBodySprite;
        }
        public void SetSnakeBodyAngle(bool tail)
        {
            if (!tail)
            {
                transform.eulerAngles = new Vector3(0, 0, angle);
            }
            else
            {
                switch (snakeMovePosition.GetDirection())
                {
                    case Direction.Left:
                        angle = 0;
                        break;
                    case Direction.Right:
                        angle = -180;
                        break;
                    case Direction.Up:
                        angle = -90;
                        break;
                    case Direction.Down:
                        angle = 90;
                        break;
                }
                transform.eulerAngles = new Vector3(0, 0, angle);
            }
        }
        public int GetSnakeBodyAngle()
        {
            return angle;
        }
        public void SetSnakeMovePosition(SnakeMovePosition snakeMovePosition)
        {
            this.snakeMovePosition = snakeMovePosition;
            transform.position = new Vector3(snakeMovePosition.GetGridPosition().x, snakeMovePosition.GetGridPosition().y);
            Sprite leftDownCorner = GameAssets.gameAssets.leftDownCorner;
            Sprite rightDownCorner = GameAssets.gameAssets.rightDownCorner;
            Sprite leftUpCorner = GameAssets.gameAssets.leftUpCorner;
            Sprite rightUpCorner = GameAssets.gameAssets.rightUpCorner;
            Sprite snakeBody = GameAssets.gameAssets.snakeBody;
            Sprite snakeTail = GameAssets.gameAssets.snakeTail;
            switch (snakeMovePosition.GetDirection())
            {
                default:
                case Direction.Left:
                    snakeBodySprite = snakeBody;
                    switch (snakeMovePosition.GetPreviousDirection())
                    {
                        default:
                            
                            angle = 0; break;
                        case Direction.Down:
                            snakeBodySprite = rightUpCorner;
                            angle = 0;
                            break;
                        case Direction.Up:

                            snakeBodySprite = rightDownCorner;
                            angle = 0;
                            break;
                    }
                    break;
                case Direction.Down:
                    
                    snakeBodySprite = snakeBody;
                    switch (snakeMovePosition.GetPreviousDirection())
                    {
                        default:
                            angle = -90; break;
                        case Direction.Right:

                            snakeBodySprite = rightDownCorner;
                            angle = 0;
                            break;
                        case Direction.Left:

                            snakeBodySprite = leftDownCorner;
                            angle = 0;
                            break;
                    }
                    break;
                case Direction.Up:
                    snakeBodySprite = snakeBody;
                    switch (snakeMovePosition.GetPreviousDirection())
                    {
                        default:
                            angle = 90; break;
                        case Direction.Right:

                            snakeBodySprite = rightUpCorner;
                            angle = 0;
                            break;
                        case Direction.Left:

                            snakeBodySprite = leftUpCorner;
                            angle = 0;
                            break;
                    }
                    break;
                case Direction.Right:
                    snakeBodySprite = snakeBody;
                    switch (snakeMovePosition.GetPreviousDirection())
                    {
                        default:
                            angle = 180; break;
                        case Direction.Down:

                            snakeBodySprite = leftUpCorner;
                            angle = 0;
                            break;
                        case Direction.Up:

                            snakeBodySprite = leftDownCorner;
                            angle = 0;
                            break;
                    }
                    break;
            }
            SetSnakeBodySprite(snakeBodySprite);
            SetSnakeBodyAngle(false);
            
           
            
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
            if (previousSnakeMovePosition == null)
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