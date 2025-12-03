using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class SnakeContinuous : MonoBehaviour
{
    private int width;
    private int height;
    // private float gridMoveTimer;         
    // private float timeSinceLastInput = 0f; 
    private Queue<Direction> inputBuffer;
    public float gridMoveTimerMax = .1f;
    private Vector2Int gridPosition;
    private Vector2Int gridMoveDirectionVector;
    private Direction gridMoveDirection;
    private ObjectSpawner objectSpawner;
    private int snakeBodySize;
    private List<SnakeMovePosition> snakeMovePositionList;
    private List<SnakeBodyPart> snakeBodyPartsList;
    private float timeSinceLastInput;
    
    public int maxBufferSize = 2;





    public void Setup(ObjectSpawner objectSpawner, LevelGrid levelGrid)
    {

        this.objectSpawner = objectSpawner;
        width = levelGrid.width;
        height = levelGrid.height;
        BeginGame();

    }


    private IEnumerator Start()
    {
        while (true)
        {
            if (inputBuffer.Count > 0)
            {
                Direction next = inputBuffer.Dequeue();
                if (!AreOpposite(next, gridMoveDirection))
                {
                    gridMoveDirection = next;
                }
            }

            // actualizar vector

            Vector3 startPos = transform.position;
            switch (gridMoveDirection)
            {
                case Direction.Right: gridMoveDirectionVector = new Vector2Int(1, 0); break;
                case Direction.Left: gridMoveDirectionVector = new Vector2Int(-1, 0); break;
                case Direction.Up: gridMoveDirectionVector = new Vector2Int(0, 1); break;
                case Direction.Down: gridMoveDirectionVector = new Vector2Int(0, -1); break;
            }
            transform.eulerAngles = new Vector3(0, 0, GetAngleFromVector(gridMoveDirectionVector) - 180);
            Vector2Int targetGrid = gridPosition + gridMoveDirectionVector;
            Vector3 targetPos = new Vector3(targetGrid.x, targetGrid.y, 0);

            for (float t = 0; t < gridMoveTimerMax; t += Time.deltaTime)
            {
                transform.position = Vector3.Lerp(startPos, targetPos, t / gridMoveTimerMax);
                yield return null;
            }

            transform.position = targetPos;

            // update grid + cuerpo + colisiones
            ManageGridMovement(targetGrid);
        }
    }






    public void BeginGame()
    {
        gridPosition = new Vector2Int(width / 2, height / 2);

        gridMoveDirection = Direction.Right;
        gridMoveDirectionVector = new Vector2Int(1, 0);

        //gridMoveTimer = 0f; // opcional, si lo usas para otra cosa visual

        snakeMovePositionList = new List<SnakeMovePosition>();
        snakeBodyPartsList = new List<SnakeBodyPart>();
        snakeBodySize = 0;
        inputBuffer = new Queue<Direction>();
        InitialState();

        var tr = gameObject.GetComponent<UnityEngine.TrailRenderer>();
        tr.emitting = true;
        tr.time = 0.34f;
    }


    public void InitialState()
    {
        CreateSnakeBody();
        snakeBodySize++;
    }
    private void Update()
    {

        if (Keyboard.current == null) return;

        Direction? dir = null;

        if (Keyboard.current.leftArrowKey.wasPressedThisFrame) dir = Direction.Left;
        else if (Keyboard.current.rightArrowKey.wasPressedThisFrame) dir = Direction.Right;
        else if (Keyboard.current.upArrowKey.wasPressedThisFrame) dir = Direction.Up;
        else if (Keyboard.current.downArrowKey.wasPressedThisFrame) dir = Direction.Down;

        if (!dir.HasValue) return;

        Direction newDir = dir.Value;

        // Última dirección relevante (la actual o la última en buffer)
        Direction lastDir = (inputBuffer.Count > 0) ? inputBuffer.Peek() : gridMoveDirection;

        // Evitar direcciones opuestas
        if (AreOpposite(lastDir, newDir))
            return;

        // Evitar rellenar el buffer de más
        if (inputBuffer.Count >= maxBufferSize)
            return;

        inputBuffer.Enqueue(newDir);


    }

    private bool AreOpposite(Direction a, Direction b)
    {
        return (a == Direction.Left && b == Direction.Right) ||
               (a == Direction.Right && b == Direction.Left) ||
               (a == Direction.Up && b == Direction.Down) ||
               (a == Direction.Down && b == Direction.Up);
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
        timeSinceLastInput += Time.deltaTime;
        if (inputBuffer.Count > 0)
        {
            Direction lastExecutedDirection = GetDirectionFromVector(gridMoveDirectionVector);
            Direction nextDirection = inputBuffer.Peek();

            if (!(lastExecutedDirection == Direction.Left && nextDirection == Direction.Right) &&
                !(lastExecutedDirection == Direction.Right && nextDirection == Direction.Left) &&
                !(lastExecutedDirection == Direction.Up && nextDirection == Direction.Down) &&
                !(lastExecutedDirection == Direction.Down && nextDirection == Direction.Up))
            {
                gridMoveDirection = nextDirection;


            }
        }
        if (timeSinceLastInput > gridMoveTimerMax)
        {
            inputBuffer.Clear();
        }



    }
    private Direction GetDirectionFromVector(Vector2Int dir)
    {
        if (dir == Vector2Int.left) return Direction.Left;
        if (dir == Vector2Int.right) return Direction.Right;
        if (dir == Vector2Int.up) return Direction.Up;
        if (dir == Vector2Int.down) return Direction.Down;
        return Direction.Right;
    }


    private bool TrySnakeCollisionSnake()
    {

        if (snakeBodyPartsList.Count > 0)
        {
            for (int i = 0; i < snakeBodyPartsList.Count; i++)
            {

                if (snakeBodyPartsList[i].GetSnakeMovePosition() != null)
                {
                    if (snakeBodyPartsList[i].GetSnakeMovePosition().GetGridPosition() == gridPosition)
                    {
                        return true;
                    }

                }


            }

        }
        return false;

    }

    private void ManageGridMovement(Vector2Int targetPosition)
    {
        // ? Nada de gridMoveTimer aquí
        // gridMoveTimer += Time.deltaTime;

        SnakeMovePosition previousSnakeMovePosition = null;
        if (snakeMovePositionList.Count > 0)
        {
            previousSnakeMovePosition = snakeMovePositionList[0];
        }

        // La cabeza estaba en gridPosition antes de moverse
        SnakeMovePosition snakeMovePosition =
            new SnakeMovePosition(gridPosition, gridMoveDirection, previousSnakeMovePosition);
        snakeMovePositionList.Insert(0, snakeMovePosition);

        // Ahora actualizamos la gridPosition a la celda destino
        gridPosition = targetPosition;

        // Actualizar cuerpo en base a la nueva lista
        UpdateSnakeBodyParts();

        // --- Colisiones ---
        bool snakeCollisionedSnake = TrySnakeCollisionSnake();
        string collision = objectSpawner.TrySnakeCollisionObject(targetPosition, gridMoveDirection, gridMoveDirectionVector);

        if (collision == "Food" || collision == "UnchainedApple")
        {
            snakeBodySize++;
            CreateSnakeBody();
            gameObject.GetComponent<UnityEngine.TrailRenderer>().time += 0.16f;
            ScoreManager.scoreManager.addScore(1);
        }

        if (collision == "Border" || snakeCollisionedSnake || collision == "Obstacle" || collision == "ChainedApple")
        {
            ResetGame();
        }

        if (snakeCollisionedSnake)
        {
            Debug.Log("murió por serpiente");
        }

        if (snakeMovePositionList.Count >= snakeBodySize + 1)
        {
            snakeMovePositionList.RemoveAt(snakeMovePositionList.Count - 1);
        }
    }


    private void quitAction()
    {
        if (inputBuffer.Count > 0)
        {
            inputBuffer.Dequeue();
        }

    }
    private void CreateSnakeBody()
    {
        snakeBodyPartsList.Add(new SnakeBodyPart(snakeBodyPartsList.Count + 1));
    }
    private void ResetGame()
    {

        gameObject.GetComponent<UnityEngine.TrailRenderer>().emitting = false;
        for (int i = 0; i < snakeBodyPartsList.Count; i++)
        {
            Destroy(snakeBodyPartsList[i].GetSnakeBodyGameObject());
        }

        snakeMovePositionList.Clear();
        snakeBodyPartsList.Clear();

        BeginGame();
        SnakeMovePosition previousSnakeMovePosition = null;
        SnakeMovePosition snakeMovePosition = new SnakeMovePosition(gridPosition, gridMoveDirection, previousSnakeMovePosition);
        snakeMovePositionList.Insert(0, snakeMovePosition);
        ScoreManager.scoreManager.setScore(0);


    }
    private void UpdateSnakeBodyParts()
    {


        for (int i = 0; i < snakeBodyPartsList.Count; i++)
        {
            if (snakeBodyPartsList.Count > 0)
            {
                snakeBodyPartsList[i].SetSnakeMovePosition(snakeMovePositionList[i]);
            }



        }



    }
    private void UpdateSnakeTail()
    {
        Sprite snakeTailSide = GameAssets.gameAssets.snakeTailSide;
        Sprite snakeTailDown = GameAssets.gameAssets.snakeTailDown;
        Sprite snakeTailUp = GameAssets.gameAssets.snakeTailUp;
        Sprite snakeTailSprite = GameAssets.gameAssets.snakeTailSide;
        bool flipX = false;
        if (snakeBodyPartsList.Count != 0)
        {

            switch (snakeMovePositionList[snakeBodyPartsList.Count - 1].GetDirection())
            {

                case Direction.Left:
                    snakeTailSprite = snakeTailSide;
                    flipX = false;
                    break;
                case Direction.Right:

                    snakeTailSprite = snakeTailSide;
                    flipX = true;
                    break;
                case Direction.Up:
                    snakeTailSprite = snakeTailUp;
                    flipX = false;
                    break;
                case Direction.Down:
                    snakeTailSprite = snakeTailDown;
                    flipX = false;
                    break;


            }

            snakeBodyPartsList[snakeBodyPartsList.Count - 1].SetSnakeBodySprite(snakeTailSprite, flipX);
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

        Vector2Int nextGridPosition = gridPosition + gridMoveDirectionVector;
        List<Vector2Int> gridPositionList = new List<Vector2Int>() { nextGridPosition, gridPosition };
        foreach (SnakeMovePosition snakeMovePosition in snakeMovePositionList)
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

            snakeBodyGameObject = new GameObject("SnakeBody", typeof(SpriteRenderer));
            snakeBodyGameObject.GetComponent<SpriteRenderer>().sortingOrder = -bodyIndex;

            transform = snakeBodyGameObject.transform;
            transform.SetParent(GameAssets.gameAssets.parentBodyParts);

        }
        public void SetSnakeBodySprite(Sprite sprite, bool flipX)
        {
            snakeBodySprite = sprite;
            snakeBodyGameObject.GetComponent<SpriteRenderer>().sprite = snakeBodySprite;
            snakeBodyGameObject.GetComponent<SpriteRenderer>().flipX = flipX;


        }


        public GameObject GetSnakeBodyGameObject()
        {
            return snakeBodyGameObject;
        }
        public SnakeMovePosition GetSnakeMovePosition()
        {
            return snakeMovePosition;
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

            Sprite snakeBodySide = GameAssets.gameAssets.snakeBodySide;
            Sprite snakeBodyUpDown = GameAssets.gameAssets.snakeBodyUpDown;

            switch (snakeMovePosition.GetDirection())
            {
                default:
                case Direction.Left:
                    snakeBodySprite = snakeBodySide;

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

                    snakeBodySprite = snakeBodyUpDown;

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
                    snakeBodySprite = snakeBodyUpDown;

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
                    snakeBodySprite = snakeBodySide;


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
            SetSnakeBodySprite(snakeBodySprite, false);




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
