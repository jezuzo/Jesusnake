using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = System.Random;

public enum Direction
{
    Left,
    Right,
    Up,
    Down
}

public class Snake : MonoBehaviour
{

    private int width;
    private int height;
    private Queue<Direction> inputBuffer;
    public float gridMoveTimerMax;
    private Vector2Int gridPosition;
    private Vector2Int gridMoveDirectionVector;
    private Direction gridMoveDirection;
    private ObjectSpawner objectSpawner;
    private int snakeBodySize;
    private List<SnakeMovePosition> snakeMovePositionList;
    private List<SnakeBodyPart> snakeBodyPartsList;
    private float timeSinceLastInput;
    private Direction nextDirection; 
    private Direction lastDirection;
    private TrailRenderer bodyShadow;
    private TrailRenderer headShadow;
    private TrailRenderer snakeBody;
    private Vector3 eyeRightInitialPosition;
    private Vector3 eyeLeftInitialPosition;
    public bool isAlive = true;
    public bool firstClick = false;
    
    
    public void Setup(ObjectSpawner objectSpawner, LevelGrid levelGrid)
    {

        this.objectSpawner = objectSpawner;
        width = levelGrid.width;
        height = levelGrid.height;
        BeginGame();

    }


    private IEnumerator Start()
    {
        yield return new WaitUntil(() => firstClick);
        while (true)
        {
            
            //gridMoveDirection = nextDirection;
            lastDirection = gridMoveDirection;
            switch (lastDirection)
            {
                case Direction.Right:
                    
                    bodyShadow.emitting = true;
                    headShadow.emitting = true;
                    transform.GetChild(0).localPosition = new Vector3(0f, 0.2f, 0);
                    transform.GetChild(1).localPosition = new Vector3(0.1f, 0.2f, 0);
                    break;
                case Direction.Left:
                    
                    bodyShadow.emitting = true;
                    headShadow.emitting = true;
                    transform.GetChild(0).localPosition = new Vector3(0f, -0.2f, 0);
                    transform.GetChild(1).localPosition = new Vector3(0.1f, -0.2f, 0);
                    break;
                case Direction.Up:
                    
                    bodyShadow.emitting = true;
                    headShadow.emitting = false;
                    transform.GetChild(0).localPosition = new Vector3(0.2f, 0f, 0);
        
                    break;
                case Direction.Down:
                    
                    bodyShadow.emitting = false;
                    headShadow.emitting = true;
                    transform.GetChild(1).localPosition = new Vector3(-0.1f, 0f, 0);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        
            
            Vector3 startPos = transform.position;
            Vector2Int targetGrid = gridPosition + gridMoveDirectionVector;
            Vector3 targetPos = new Vector3(targetGrid.x, targetGrid.y, 0);
            if (isAlive)
            {
                transform.eulerAngles = new Vector3(0, 0, GetAngleFromVector(gridMoveDirectionVector) - 180);
            }
            
        
            objectSpawner.TrySnakeCollisionBox(targetGrid, gridMoveDirection, gridMoveDirectionVector);
            
            
            for (float t = 0f; t < gridMoveTimerMax; t += Time.deltaTime)
            {
                if (isAlive)
                {
                    float lerp = t / gridMoveTimerMax;
                    transform.position = Vector3.Lerp(startPos, targetPos, lerp);
                    
                }
                
                yield return null;
            }

            transform.position = isAlive ? targetPos : new Vector3(transform.position.x, transform.position.y, 0);
            
                ManageGridMovement(targetGrid);
                ManageCollisions(targetGrid);
                
            
            
            
            if (inputBuffer.Count > 0)
            {
                Direction next = inputBuffer.Dequeue();
                if (!AreOpposite(next, nextDirection))
                {
                    nextDirection = next;
                    
                }
            }
        }
    }
    
    public void FreezeSnake()
    {
        isAlive = false;
        
        GetComponent<Animator>().SetBool("SnakeEating",false);
        GetComponent<Animator>().SetTrigger("isDeath");
        GetComponent<TrailRenderer>().autodestruct = false;
        GetComponent<TrailRenderer>().time = Mathf.Infinity;
        bodyShadow.autodestruct = true;
        bodyShadow.time = Mathf.Infinity;
        headShadow.autodestruct = true;
        headShadow.time = Mathf.Infinity;
        
        transform.GetChild(2).GetComponent<Animator>().SetTrigger("TongueDeath");
        transform.GetChild(4).GetComponent<Animator>().SetTrigger("Death");
        
        Transform rightEye = transform.GetChild(3).GetChild(0);
        Transform leftEye = transform.GetChild(3).GetChild(1);
        rightEye.localPosition = new Vector3(-0.15f, 0.25f, 0f);
        leftEye.localPosition = new Vector3(-0.15f, -0.25f, 0f);
    }
    
    private void ManageCollisions(Vector2Int targetGrid)
    {
        bool snakeCollisionedSnake = TrySnakeCollisionSnake();
       
        string collision = objectSpawner.TrySnakeCollisionObject(targetGrid, lastDirection, gridMoveDirectionVector);

        if (collision == "Food" || collision == "UnchainedApple")
        {
            TriggerBulge();
            snakeBodySize++;
            CreateSnakeBody();
            snakeBody.time = (snakeBodyPartsList.Count+1)*gridMoveTimerMax;
            bodyShadow.time = (snakeBodyPartsList.Count+1)*gridMoveTimerMax;
            ScoreManager.scoreManager.addScore(1);
        }
        

        if (snakeCollisionedSnake)
        {
            Debug.Log("murió por serpiente");
        }
    }
    private IEnumerator InitializeTrail()
    {
        //snakeBody.Clear();

        Vector3 headPos = new Vector3(gridPosition.x,gridPosition.y);
        Vector3 dir = Vector3.right;
        headShadow.emitting = false;

        for (int i = 0; i < 2; i++)
        {
            transform.position = headPos + new Vector3(gridMoveDirectionVector.x,gridMoveDirectionVector.y) * i;
            yield return null;
        }

        transform.position = headPos + new Vector3(gridMoveDirectionVector.x, gridMoveDirectionVector.y);
    }

    private void BeginGame()
    {
        transform.GetChild(3).gameObject.SetActive(true);
        transform.GetChild(4).gameObject.SetActive(true);
        gridMoveTimerMax = PlayerPrefs.GetFloat("SnakeSpeed");
        gridPosition = new Vector2Int(width / 2 - 2, height / 2);
        transform.position = new Vector3(gridPosition.x, gridPosition.y, 0);
        tongueOutRate = UnityEngine.Random.Range(0f, 20f);
        blinkRate = UnityEngine.Random.Range(0f, 10f);
        gridMoveDirection = Direction.Right;
        nextDirection = Direction.Right;
        transform.eulerAngles = new Vector3(0, 0, GetAngleFromVector(gridMoveDirectionVector) - 180);
        gridMoveDirectionVector = new Vector2Int(1, 0);
        eyeRightInitialPosition = new Vector3(-0.15f, 0.25f);
        eyeLeftInitialPosition = new Vector3(-0.15f, -0.25f);
        snakeMovePositionList = new List<SnakeMovePosition>();
        snakeBodyPartsList = new List<SnakeBodyPart>();
        snakeBodySize = 0;
        inputBuffer = new Queue<Direction>();
        InitialState();
        bodyShadow = transform.GetChild(0).GetComponent<TrailRenderer>();
        headShadow = transform.GetChild(1).GetComponent<TrailRenderer>();
        snakeBody = transform.GetComponent<TrailRenderer>();
        StartCoroutine(InitializeTrail());
        bodyShadow.time = 0.4f;
        snakeBody.time = 0.4f;
        bodyShadow.emitting = true;
        transform.GetChild(0).localPosition = new Vector3(0f, 0.2f, 0);
        transform.GetChild(1).localPosition = new Vector3(0.1f, 0.2f, 0);
        GameAssets.gameAssets.controls.SetActive(true);
        
        snakeBody.Clear();
        
        Debug.Log("Velocidad de la serpiente: " + PlayerPrefs.GetFloat("SnakeSpeed"));
        Debug.Log("Tamaño del tablero: " + PlayerPrefs.GetInt("TableSize"));
        Debug.Log("Obstáculos: " + PlayerPrefs.GetInt("Obstacles"));
        Debug.Log("Modo de juego: " + PlayerPrefs.GetString("GameMode"));
    }


    private void InitialState()
    {
        CreateSnakeBody();
        snakeBodySize++;
    }
    private void Update()
    {
        if(!firstClick) Time.timeScale = 0f;
        if (!isAlive) return;
        ManageInput();
        TongueOut();
        Blink();
        DoBugle();

    }

    private float tongueTimer;
    private float tongueOutRate;
    private void TongueOut()
    {
        tongueTimer += Time.deltaTime;
        if (!(tongueTimer > tongueOutRate)) return;
        tongueTimer = 0f;
        transform.GetChild(2).GetComponent<Animator>().SetTrigger("TongueOut");
        tongueOutRate = UnityEngine.Random.Range(0f, 20f);
    }

    private float blinkTimer;
    private float blinkRate;
    private void Blink()
    {
        blinkTimer += Time.deltaTime;
        if (!(blinkTimer > blinkRate)) return;
        blinkTimer = 0f;
        transform.GetChild(4).GetComponent<Animator>().SetTrigger("Blink");
        blinkRate = UnityEngine.Random.Range(0f, 10f);
        
    }
    
    private float bulgeWidth = 1.2f;   // tamaño máximo de la protuberancia
    private float travelTime = 0.5f;   // cuánto tarda en recorrer el cuerpo
    private float timerBugle = -1f;
    private float startPos = 0.05f;    // posición inicial de la bola (cerca de la cabeza)
    private float endPos = 1f;     
    private float normalWidth = 1f;
    private float fadeTime = 0.1f;
    private float baseSpread = 0.3f;    // spread base para serpiente pequeña
    private float baseTrailTime = 0.34f;
    private bool isFading = false;        // si la curva actual está desapareciendo
    private float fadeTimer = 0f;         // tiempo de fade de la curva actual
    private bool pendingNewBulge = false; // si hay una nueva curva esperando
    private void DoBugle()
    {
        TrailRenderer trail = GetComponent<TrailRenderer>();
        float trailLength = trail.time;
    
        // Fade de la curva actual
        if (isFading)
        {
            fadeTimer += Time.deltaTime;
            float t = Mathf.Clamp01(fadeTimer / fadeTime);

            // Reducimos el ancho suavemente
            float peakWidth = Mathf.Lerp(bulgeWidth, normalWidth, t);

            float center = Mathf.Clamp01(timerBugle * 2f / trailLength); // posición aproximada, da igual dónde
            ApplyBulge(center, peakWidth);

            if (t >= 1f)
            {
                // Fade completado
                ResetCurve();
                isFading = false;
                timerBugle = -1f;

                if (pendingNewBulge)
                {
                    StartNewBulge();
                }
            }

            return;
        }

        // Curva normal
        if (timerBugle < 0f) return;

        timerBugle += Time.deltaTime;

        float speedUnits = 2f;
        float maxTravelLength = trailLength * 0.6f;
        float posUnits = Mathf.Min(timerBugle * speedUnits, maxTravelLength);
        float peakPos = posUnits / trailLength;

        float peakWidth2;
        if (timerBugle * speedUnits < maxTravelLength)
        {
            peakWidth2 = bulgeWidth;
        }
        else
        {
            float t = (timerBugle * speedUnits - maxTravelLength) / fadeTime;
            t = Mathf.Clamp01(t);
            peakWidth2 = Mathf.Lerp(bulgeWidth, normalWidth, t);
        }

        ApplyBulge(peakPos, peakWidth2);

        if (peakWidth2 <= normalWidth)
        {
            timerBugle = -1f;
            ResetCurve();
        }
        
        
    }
    
    public void TriggerBulge()
    {
        
        if (timerBugle >= 0f)
        {
            // Hay curva activa → iniciar fade de la actual
            isFading = true;
            fadeTimer = 0f;
            pendingNewBulge = true; // señalamos que queremos la nueva curva
            return;
        }

        // No hay curva activa → iniciar normalmente
        StartNewBulge();
    }
    private void StartNewBulge()
    {
        startPos = 0.05f;
        endPos = 1f;
        timerBugle = 0f;
        travelTime = endPos - startPos;
        isFading = false;
        pendingNewBulge = false;
    }

    void ApplyBulge(float center, float peak)
    {

        AnimationCurve curve = new AnimationCurve();

        Keyframe K(float x, float y)
        {
            Keyframe k = new Keyframe(x, y, 0, 0); // tangentes planas para evitar overshoot
            return k;
        }
        float spread = baseSpread * baseTrailTime / transform.GetComponent<TrailRenderer>().time;

        curve.AddKey(K(0f, normalWidth));
        curve.AddKey(K(center - spread, normalWidth));
        curve.AddKey(K(center, peak));
        curve.AddKey(K(center + spread, normalWidth));
        curve.AddKey(K(1f, normalWidth));
        

        transform.GetComponent<TrailRenderer>().widthCurve = curve;
    }

    void ResetCurve()
    {
        AnimationCurve normal = new AnimationCurve();

        Keyframe K(float time, float value)
        {
            Keyframe k = new Keyframe(time, value);
            k.inTangent = 0f;
            k.outTangent = 0f;
            return k;
        }

        normal.AddKey(K(0f, 1f));
        normal.AddKey(K(1f, 1f));
        transform.GetComponent<TrailRenderer>().widthCurve = normal;
    }
    public void EatAnimation(Vector2Int appleGridPosition)
    {
        float dist = Vector2.Distance(new Vector2(transform.position.x,transform.position.y), appleGridPosition);
        
        if (!isAlive) return;
        if(appleGridPosition == default)
        {
            transform.GetComponent<Animator>().SetBool("SnakeEating", false);
            return;
        }

        if (dist < 3f)
        {
            transform.GetComponent<Animator>().SetBool("SnakeBox", false);
            transform.GetComponent<Animator>().SetBool("SnakeBox", false);
            
        }
        transform.GetComponent<Animator>().SetBool("SnakeEating", dist < 3f);
        
        

    }
    private Vector3 lastOffsetRight;
    private Vector3 lastOffsetLeft;
    
    public void EyesFollowingApple(Vector2Int appleGridPosition)
    {
        if (!isAlive) return;
        Transform rightEye = transform.GetChild(3).GetChild(0);
        Transform leftEye = transform.GetChild(3).GetChild(1);
        
        float maxOffset = 0.05f;
        
        // Convertimos la posición del target al espacio local del padre
        Vector3 localTargetPos = transform.InverseTransformPoint(new Vector3(appleGridPosition.x,appleGridPosition.y,0));
        
        // Dirección desde el ojo hacia el target, pero en el espacio del padre
        Vector3 dirRight = (localTargetPos - eyeRightInitialPosition).normalized;
        Vector3 dirLeft = (localTargetPos - eyeLeftInitialPosition).normalized;
        if ((localTargetPos - eyeRightInitialPosition).magnitude < 0.8f || (localTargetPos - eyeLeftInitialPosition).magnitude < 0.8f
            ||  transform.GetComponent<SpriteRenderer>().sprite != GameAssets.gameAssets.snakeHead )
        {
            rightEye.localPosition = eyeRightInitialPosition + lastOffsetRight;
            leftEye.localPosition = eyeLeftInitialPosition + lastOffsetLeft;
            return;
        }
        // Offset limitado
        Vector3 offsetRight = dirRight * maxOffset;
        Vector3 offsetLeft = dirLeft * maxOffset;
        
        lastOffsetRight = offsetRight;
        lastOffsetLeft = offsetRight;
        
        // Movemos el ojo respetando la rotación del padre
        rightEye.localPosition = eyeRightInitialPosition + offsetRight;
        leftEye.localPosition = eyeLeftInitialPosition + offsetLeft;
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
            if (!firstClick) return;
            inputBuffer.Enqueue(Direction.Left);
            timeSinceLastInput = 0f;
        }
        else if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            if (!firstClick)
            {
                Time.timeScale = 1f;
                //snakeBody.emitting = true;
                snakeBody.time = (snakeBodyPartsList.Count+1)*gridMoveTimerMax;
                bodyShadow.time = (snakeBodyPartsList.Count+1)*gridMoveTimerMax;
                GameAssets.gameAssets.controls.SetActive(false);
                firstClick = true;
                
            }
            inputBuffer.Enqueue(Direction.Right);
            timeSinceLastInput = 0f;
            
        }
        else if (Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            if (!firstClick) return;
            inputBuffer.Enqueue(Direction.Up);
            timeSinceLastInput = 0f;
            
        }
        else if (Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            if (!firstClick) return;
            inputBuffer.Enqueue(Direction.Down);
            timeSinceLastInput = 0f;
            
        }
        timeSinceLastInput += Time.deltaTime;
        if (inputBuffer.Count > 0)
        {
            nextDirection = inputBuffer.Peek();
            
            if (!AreOpposite(nextDirection,lastDirection))
            {
                gridMoveDirection = nextDirection;

                switch (gridMoveDirection)
                {
                    case Direction.Right:
                        gridMoveDirectionVector = new Vector2Int(1, 0);
                        
                        break;
                    case Direction.Left:
                        gridMoveDirectionVector = new Vector2Int(-1, 0);
                        
                        break;
                    case Direction.Up:
                        gridMoveDirectionVector = new Vector2Int(0, 1);
                        
                        break;
                    case Direction.Down:
                        gridMoveDirectionVector = new Vector2Int(0, -1);
                        
                        break;
                }


            }
        }
        if (timeSinceLastInput > gridMoveTimerMax)
        {
            inputBuffer.Clear();
        }
       

    }

    private bool detectedKey;

    private void OnTriggerEnter2D(Collider2D other)
    {
        FreezeSnake();
        
    }

    private bool TrySnakeCollisionSnake()
    {

        if (snakeBodyPartsList.Count > 0)
        {
            foreach (var snakeBodyPart in snakeBodyPartsList)
            {
                if (snakeBodyPart.GetSnakeMovePosition() != null)
                {
                    if (snakeBodyPart.GetSnakeMovePosition().GetGridPosition() == gridPosition)
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
        SnakeMovePosition previousSnakeMovePosition = null;
        if (snakeMovePositionList.Count > 0)
        {
            previousSnakeMovePosition = snakeMovePositionList[0];
        }
        
        SnakeMovePosition snakeMovePosition =
            new SnakeMovePosition(gridPosition, gridMoveDirection, previousSnakeMovePosition);
        snakeMovePositionList.Insert(0, snakeMovePosition);
        gridPosition = targetPosition;
        
        UpdateSnakeBodyParts();
        
        if (snakeMovePositionList.Count >= snakeBodySize + 1)
        {
            snakeMovePositionList.RemoveAt(snakeMovePositionList.Count - 1);
        }
    }
    
    private void QuitAction()
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

        gameObject.GetComponent<TrailRenderer>().emitting = false;
        bodyShadow.emitting =false;
        foreach (var snakeBodyPart in snakeBodyPartsList)
        {
            Destroy(snakeBodyPart.GetSnakeBodyGameObject());
        }

        snakeMovePositionList.Clear();
        snakeBodyPartsList.Clear();
        

        BeginGame();
        SnakeMovePosition snakeMovePosition = new SnakeMovePosition(gridPosition, gridMoveDirection, null);
        snakeMovePositionList.Insert(0, snakeMovePosition);
        ScoreManager.scoreManager.setScore(0);


    }
    private IEnumerator MoveSnakeBodyPart(SnakeBodyPart snakeBody,Vector2Int startPosition, Vector2Int targetPosition)
    {
        Vector3 startPos = new Vector3(startPosition.x, startPosition.y, 0);
        Vector3 targetPos = new Vector3(targetPosition.x, targetPosition.y, 0);

        for (float t = 0f; t < gridMoveTimerMax; t += Time.deltaTime)
        {
            if(snakeBody.GetSnakeBodyGameObject() != null)
            {
                float lerp = t / gridMoveTimerMax;
                snakeBody.GetSnakeBodyGameObject().transform.position = Vector3.Lerp(startPos, targetPos, lerp);
                yield return null;

            }
           
        }
        if(snakeBody.GetSnakeBodyGameObject() != null)
        {
            snakeBody.GetSnakeBodyGameObject().transform.position = targetPos;
        }
        
    }
    private void UpdateSnakeBodyParts()
    {


        for (int i = 0; i < snakeBodyPartsList.Count; i++)
        {
            if (snakeBodyPartsList.Count > 0)
            {
                snakeBodyPartsList[i].SetSnakeMovePosition(snakeMovePositionList[i]);
                // Vector2Int startPosition = snakeBodyPartsList[i].GetSnakeMovePosition().GetGridPosition();
                // Vector2Int targetPosition = startPosition + snakeBodyPartsList[i].GetSnakeMovePosition().GetDirectionVector();
                //
                // StartCoroutine(MoveSnakeBodyPart(snakeBodyPartsList[i], startPosition, targetPosition));
                
                    
                
            }


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

        private readonly Transform transform;
        private readonly GameObject snakeBodyGameObject;


        public SnakeBodyPart(int bodyIndex)
        {

            snakeBodyGameObject = new GameObject("SnakeBody", typeof(SpriteRenderer),typeof(BoxCollider2D));
            snakeBodyGameObject.GetComponent<SpriteRenderer>().sortingOrder = -bodyIndex;
            snakeBodyGameObject.GetComponent<BoxCollider2D>().isTrigger = true;
            snakeBodyGameObject.GetComponent<BoxCollider2D>().size = new Vector2(1, 1);

            transform = snakeBodyGameObject.transform;
            transform.SetParent(GameAssets.gameAssets.parentBodyParts);

        }
        


        public GameObject GetSnakeBodyGameObject()
        {
            return snakeBodyGameObject;
        }
        public SnakeMovePosition GetSnakeMovePosition()
        {
            return snakeMovePosition;
        }
        
        public void SetSnakeMovePosition(SnakeMovePosition snakeMovePosition)
        {
            this.snakeMovePosition = snakeMovePosition;
            transform.position = new Vector3(snakeMovePosition.GetGridPosition().x, snakeMovePosition.GetGridPosition().y);
            
            
        }

    }

    private class SnakeMovePosition
    {
        private readonly SnakeMovePosition previousSnakeMovePosition;
        private readonly Vector2Int gridPosition;
        private readonly Direction direction;

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
        public Vector2Int GetDirectionVector()
        {
            switch (direction)
            {
                default: return new Vector2Int(1, 0);
                case Direction.Right: return new Vector2Int(1, 0); 
                case Direction.Left: return new Vector2Int(-1, 0); 
                case Direction.Up: return new Vector2Int(0, 1); 
                case Direction.Down: return new Vector2Int(0, -1); 
            }
            
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