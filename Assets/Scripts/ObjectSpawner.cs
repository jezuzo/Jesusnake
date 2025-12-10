
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    private float spawnTimer;
    public float spawnRateRef = 5f;
    private float spawnRate;
    private Snake snake;
    private List<Object> objectsList;
    private int width;
    private int height;

    private bool chainedApplesEnabled;
    private bool boxesEnabled;
    private bool appleArrowsEnabled;
    private bool normalModeEnabled;
    
    public bool enableChainedApples;
    public bool enableBoxes;
    public bool enableAppleArrows;
    public bool enableNormalMode;
    public bool enableObstacles;
    public bool enableRandomMode;

    public void Setup(Snake snake, LevelGrid levelGrid)
    {
        
        this.snake = snake;
        objectsList = new List<Object>();
        width = levelGrid.width - 1;
        height = levelGrid.height - 1;
        

        

    }
    private void Start()
    {

        enableChainedApples = false;
        enableBoxes = false;
        enableAppleArrows = false;
        enableNormalMode = false;
        enableObstacles = false;
        enableRandomMode = false;
        enableObstacles = PlayerPrefs.GetInt("Obstacles") != 0;

        switch (PlayerPrefs.GetString("GameMode"))
        {
            default:
                enableNormalMode = true;
                break;
            case "NormalMode":
                enableNormalMode = true;
                break;
            case "BoxMode":
                enableBoxes = true;
                break;
            case "ChainedApplesMode":
                enableChainedApples = true;
                break;
            case "ArrowsMode":
                enableAppleArrows = true;
                break;
            case "RandomMode":
                enableRandomMode = true;
                break;
        }
        if (enableRandomMode)
        {
            EnableRandomMode();
            GameAssets.gameAssets.trophy.sprite = GameAssets.gameAssets.randomHighScore;
            GameAssets.gameAssets.trophy.rectTransform.sizeDelta = new Vector2(90, 100);
        }
       
    }
    public class Object
    {
        private Vector2Int objectGridPosition;
        private readonly GameObject objectGameObject;
        private string type;
        private readonly Direction arrowDirection;
        private Sprite objectSprite;
        
        public Object(Vector2Int objectGridPosition, GameObject objectGameObject, string type, Direction arrowDirection, Sprite objectSprite)
        {
            this.objectGridPosition = objectGridPosition;
            this.objectGameObject = objectGameObject;
            this.type = type;
            this.arrowDirection = arrowDirection;
            this.objectSprite = objectSprite;
        }
        public Vector2Int GetObjectGridPosition()
        {
            return objectGridPosition; 
        }
        public GameObject GetObjectGameObject()
        {
            return objectGameObject; 
        }
        public string GetObjectType()
        {
            return type;
        }
        public void SetObjectGridPosition(Vector2Int newObjectGridPosition)
        {
            objectGridPosition = newObjectGridPosition;
            objectGameObject.transform.position = new Vector3(objectGridPosition.x, objectGridPosition.y);
        }
        
        public Direction GetArrowDirection()
        {
            return arrowDirection;
        }
        public void SetObjectSprite(Sprite newObjectSprite)
        {
            objectSprite = newObjectSprite;
            objectGameObject.GetComponent<SpriteRenderer>().sprite = objectSprite;
        }
        public void SetObjectType(string newType)
        {
            type = newType;
            objectGameObject.name = type;
        }
    }
    private void Update()
    {
        if (enableAppleArrows)
        {
            if (!enableRandomMode)
            {
                GameAssets.gameAssets.trophy.sprite = GameAssets.gameAssets.highScoreArrows;
                GameAssets.gameAssets.trophy.rectTransform.sizeDelta = new Vector2(91, 105);

            }
            EnableArrowApples();
        }
        if (enableObstacles)
        {
            EnableObstacles();
        }
        if (enableChainedApples)
        {
            if (!enableRandomMode)
            {
                GameAssets.gameAssets.trophy.sprite = GameAssets.gameAssets.locker;
                GameAssets.gameAssets.trophy.rectTransform.sizeDelta = new Vector2(91, 105);

            }
            EnableChainedApples();
        }
        if (enableBoxes)
        {
            if (!enableRandomMode)
            {
                GameAssets.gameAssets.trophy.sprite = GameAssets.gameAssets.box;
                GameAssets.gameAssets.trophy.rectTransform.sizeDelta = new Vector2(90, 99);
            }
            
            EnableBoxes();
        }
        if (enableNormalMode)
        {
            EnableNormalMode();
        }

        foreach (Object appleObject in objectsList)
        {
            if (appleObject.GetObjectType() != "Food") continue;
            snake.EyesFollowingApple(appleObject.GetObjectGridPosition());
            snake.EatAnimation(appleObject.GetObjectGridPosition());
        }
        
       
    }

    private void EnableRandomMode()
    {
        enableChainedApples = false;
        enableBoxes = false;
        enableAppleArrows = false;
        enableNormalMode = false;

        int randNum = UnityEngine.Random.Range(0, 4);
       
        switch (randNum)
        {
            case 0:
                enableAppleArrows = true;
                break;
            case 1:
                enableBoxes = true;
                break;
            case 2:
                enableChainedApples = true;
                break;
            case 3:
                enableNormalMode = true;
                break;
        }
    }
    private void EnableObstacles()
    {

        if (spawnTimer > spawnRate)
        {
            SpawnObject("Obstacle");
            spawnTimer = 0f;
            Invoke(nameof(DeleteObstacle), 10f);
            spawnRate = UnityEngine.Random.Range(spawnRateRef, spawnRateRef + 10f);
        }
        spawnTimer += Time.deltaTime;

    }
    private void EnableBoxes()
    {
        bool isBox = false;
        if (!boxesEnabled)
        {
            SpawnObject("Box");
            SpawnObject("BoxUnlock");
            
            boxesEnabled = true;

        }
        
        foreach (var myObject in objectsList)
        {
            if (myObject.GetObjectType() == "BoxUnlock" )
            {
                isBox = true;
            }
        }

        if (isBox) return;
        if (!enableRandomMode)
        {
            SpawnObject("BoxUnlock");
        }
        else
        {
            Destroy(GetFullObjectsTypeList("Box")[0].GetObjectGameObject());
            objectsList.Remove((GetFullObjectsTypeList("Box")[0]));
            boxesEnabled = false;
            EnableRandomMode();
        }
    }
    private void EnableChainedApples()
    {
        TryKeyCollisionChainedApple();
        bool isChainedApple = false;
        if (!chainedApplesEnabled)
        {
            SpawnObject("Key");
            SpawnObject("ChainedApple");
            chainedApplesEnabled = true;
        }

        foreach (var myObject in objectsList)
        {
            if (myObject.GetObjectType() == "Food" || myObject.GetObjectType()=="ChainedApple")
            {
                isChainedApple = true;
            }
        }

        if (isChainedApple) return;
        if (!enableRandomMode)
        {
            SpawnObject("Key");
            SpawnObject("ChainedApple");

        }
        else
        {
            chainedApplesEnabled = false;
            EnableRandomMode();
        }
    }

    private void EnableNormalMode()
    {

        bool isFood = false;
        if (!normalModeEnabled)
        {
            SpawnObject("Food");
            
            normalModeEnabled = true;

        }

        foreach (var myObject in objectsList)
        {
            if (myObject.GetObjectType() == "Food")
            {
                isFood = true;
            }
        }

        if (isFood) return;
        if (!enableRandomMode)
        {
            SpawnObject("Food");
        }
        else
        {
            normalModeEnabled = false;
            EnableRandomMode();
        }
    }

    private void EnableArrowApples()
    {
        bool isAppleArrow = false;
        if (!appleArrowsEnabled)
        {
            SpawnObject("ArrowApple");
            appleArrowsEnabled = true;
        }
        foreach (var myObject in objectsList)
        {
            if (myObject.GetObjectType() == "ArrowApple")
            {
                isAppleArrow = true;
            }
        }

        if (isAppleArrow) return;
        if (!enableRandomMode)
        {
            SpawnObject("ArrowApple");
        }
        else
        {
            appleArrowsEnabled = false;
            EnableRandomMode();
        }
    }

    private GameObject CreateGameObject(string name, Vector2Int position, Sprite sprite, int sortOrder = 1, 
        string sortingLayerName = "Default")
    {
        GameObject objectGameObject = new GameObject(name, typeof(SpriteRenderer));
        objectGameObject.GetComponent<SpriteRenderer>().sprite = sprite;
        objectGameObject.GetComponent<SpriteRenderer>().sortingOrder = sortOrder;
        objectGameObject.GetComponent<SpriteRenderer>().sortingLayerName = sortingLayerName;
        
        objectGameObject.transform.position = new Vector3(position.x, position.y, 0);
        return objectGameObject;
    }

    public List<Vector2Int> GetFullObjectsGridPositionList(string type = "None")
    {

        List<Vector2Int> gridPositionList = new List<Vector2Int>();
        foreach (Object myObject in objectsList)
        {
            if(type == "None")
            {
                gridPositionList.Add(myObject.GetObjectGridPosition());
            }
            else
            {
                if(myObject.GetObjectType() == type)
                {
                    gridPositionList.Add(myObject.GetObjectGridPosition());
                }
            }
            
        }
        return gridPositionList;
    }

    public List<Object> GetFullObjectsTypeList(string type = "None")
    {

        List<Object> objectList = new List<Object>();
        foreach (Object myObject in objectsList)
        {
            if (type == "None")
            {
                objectList.Add(myObject);
            }
            else
            {
                if (myObject.GetObjectType() == type)
                {
                    objectList.Add(myObject);
                }
            }

        }
        return objectList;
    }

    private bool CheckRestrictions(Vector2Int objectGridPosition, string type,Direction appleGridDirection = default)
    {
        if ((snake.GetFullSnakeGridPositionList().IndexOf(objectGridPosition) != -1) || GetFullObjectsGridPositionList().IndexOf(objectGridPosition) != -1)
        {
            return false;
        }

        if ((GetFullObjectsGridPositionList("Obstacle").Contains(new Vector2Int(objectGridPosition.x + 1, objectGridPosition.y)))
            && (GetFullObjectsGridPositionList("Obstacle").Contains(new Vector2Int(objectGridPosition.x - 1, objectGridPosition.y)))
            && (GetFullObjectsGridPositionList("Obstacle").Contains(new Vector2Int(objectGridPosition.x, objectGridPosition.y - 1))))
        {
            return false;
        }

        

        if ((objectGridPosition.x == 1 && objectGridPosition.y == 1) && (GetFullObjectsGridPositionList("Obstacle").Contains(new Vector2Int(objectGridPosition.x + 1, objectGridPosition.y)) && GetFullObjectsGridPositionList("Obstacle").Contains(new Vector2Int(objectGridPosition.x, objectGridPosition.y + 1)))
            || (objectGridPosition.x == 1 && objectGridPosition.y == height - 1) && (GetFullObjectsGridPositionList("Obstacle").Contains(new Vector2Int(objectGridPosition.x + 1, objectGridPosition.y)) && GetFullObjectsGridPositionList("Obstacle").Contains(new Vector2Int(objectGridPosition.x, objectGridPosition.y - 1)))
            || (objectGridPosition.x == width - 1 && objectGridPosition.y ==  1) && (GetFullObjectsGridPositionList("Obstacle").Contains(new Vector2Int(objectGridPosition.x - 1, objectGridPosition.y)) && GetFullObjectsGridPositionList("Obstacle").Contains(new Vector2Int(objectGridPosition.x, objectGridPosition.y + 1)))
            || (objectGridPosition.x == width - 1 && objectGridPosition.y == height - 1) && (GetFullObjectsGridPositionList("Obstacle").Contains(new Vector2Int(objectGridPosition.x - 1, objectGridPosition.y)) && GetFullObjectsGridPositionList("Obstacle").Contains(new Vector2Int(objectGridPosition.x, objectGridPosition.y - 1)))
            )
        {
            return false;
        }

        if (type == "ChainedApple")
        {
            if (objectGridPosition.x == width / 2 && objectGridPosition.y == height / 2)
            {
                return false;
            }
            else if (objectGridPosition.x == width / 2 + 1 && objectGridPosition.y == height / 2)
            {
                return false;
            }
        }
        if (type == "Obstacle")
        {
            if (objectGridPosition.x == width / 2 && objectGridPosition.y == height / 2)
            {
                return false;
            }
            else if (objectGridPosition.x == width / 2 + 1 && objectGridPosition.y == height / 2)
            {
                return false;
            }
            

            //(GetFullObjectsGridPositionList("ArrowApple").Contains(new Vector2Int(objectGridPosition.x + 1, objectGridPosition.y)))
            for (int i = 0; i < objectsList.Count; i++)
            {
                if (objectsList[i].GetObjectType() == "ArrowApple" && ((GetFullObjectsGridPositionList("ArrowApple").Contains(new Vector2Int(objectGridPosition.x - 1, objectGridPosition.y))) && objectsList[i].GetArrowDirection() == Direction.Left))
                {
                    return false;
                }
                else if (objectsList[i].GetObjectType() == "ArrowApple" && ((GetFullObjectsGridPositionList("ArrowApple").Contains(new Vector2Int(objectGridPosition.x + 1, objectGridPosition.y))) && objectsList[i].GetArrowDirection() == Direction.Right))
                {
                    return false;
                }
                else if (objectsList[i].GetObjectType() == "ArrowApple" && ((GetFullObjectsGridPositionList("ArrowApple").Contains(new Vector2Int(objectGridPosition.x, objectGridPosition.y-1))) && objectsList[i].GetArrowDirection() == Direction.Down))
                {
                    return false;
                }
                else if (objectsList[i].GetObjectType() == "ArrowApple" && ((GetFullObjectsGridPositionList("ArrowApple").Contains(new Vector2Int(objectGridPosition.x , objectGridPosition.y+1))) && objectsList[i].GetArrowDirection() == Direction.Up))
                {
                    return false;
                }
            }

            for(int i = 0; i < objectsList.Count; i++)
            {
                if((objectGridPosition.x == 2 && objectGridPosition.y == 1) && (objectsList[i].GetObjectGridPosition().x == 1 && objectsList[i].GetObjectGridPosition().y == 1))
                {
                    return false;
                }else if((objectGridPosition.x == 2 && objectGridPosition.y == height - 1) && (objectsList[i].GetObjectGridPosition().x == 1 && objectsList[i].GetObjectGridPosition().y == height - 1)){
                    return false;
                }else if((objectGridPosition.x == width - 2 && objectGridPosition.y == 1) && (objectsList[i].GetObjectGridPosition().x == width - 1 && objectsList[i].GetObjectGridPosition().y == 1)){
                    return false;
                }else if((objectGridPosition.x == width - 2 && objectGridPosition.y == height - 1) && (objectsList[i].GetObjectGridPosition().x == width - 1 && objectsList[i].GetObjectGridPosition().y == height - 1))
                {
                    return false; 
                }

                       
                
            
            }

            return true;
        }

        if(type == "ArrowApple")
        {
            
            if (appleGridDirection == Direction.Right)
            {
                if(objectGridPosition.x == 1)
                {
                    return false;
                }

                for (int i = 0;i< GetFullObjectsTypeList("Obstacle").Count; i++)
                {
                    if((objectGridPosition.x == GetFullObjectsTypeList("Obstacle")[i].GetObjectGridPosition().x + 1) && (objectGridPosition.y == GetFullObjectsTypeList("Obstacle")[i].GetObjectGridPosition().y))
                    {
                        return false;
                    }

                    
                }
            }else if(appleGridDirection == Direction.Left)
            {
                if (objectGridPosition.x == width - 1 )
                {
                    return false;
                }
                for (int i = 0; i < GetFullObjectsTypeList("Obstacle").Count; i++)
                {
                    if ((objectGridPosition.x == GetFullObjectsTypeList("Obstacle")[i].GetObjectGridPosition().x - 1) && (objectGridPosition.y == GetFullObjectsTypeList("Obstacle")[i].GetObjectGridPosition().y))
                    {
                        return false;
                    }


                }
            }
            else if(appleGridDirection == Direction.Up)
            {
                if (objectGridPosition.y == 1)
                {
                    return false;
                }
                for (int i = 0; i < GetFullObjectsTypeList("Obstacle").Count; i++)
                {
                    if ((objectGridPosition.x == GetFullObjectsTypeList("Obstacle")[i].GetObjectGridPosition().x) && (objectGridPosition.y == GetFullObjectsTypeList("Obstacle")[i].GetObjectGridPosition().y + 1))
                    {
                        return false;
                    }
                    if (((objectGridPosition.x == 1 && objectGridPosition.y ==  2) && (GetFullObjectsTypeList("Obstacle")[i].GetObjectGridPosition().x == 2) && GetFullObjectsTypeList("Obstacle")[i].GetObjectGridPosition().y == 1)
                        || ((objectGridPosition.x == width - 1 && objectGridPosition.y ==  2) && (GetFullObjectsTypeList("Obstacle")[i].GetObjectGridPosition().x == width - 2) && GetFullObjectsTypeList("Obstacle")[i].GetObjectGridPosition().y == 1)
                        )
                    {
                        return false;
                    }


                }


            }else if(appleGridDirection == Direction.Down)
            {
                if (objectGridPosition.y == height - 1)
                {
                    return false;
                }

                for (int i = 0; i < GetFullObjectsTypeList("Obstacle").Count; i++)
                {
                    if ((objectGridPosition.x == GetFullObjectsTypeList("Obstacle")[i].GetObjectGridPosition().x) && (objectGridPosition.y == GetFullObjectsTypeList("Obstacle")[i].GetObjectGridPosition().y - 1))
                    {
                        return false;
                    }
                    if(((objectGridPosition.x == 1 && objectGridPosition.y == height -2) && (GetFullObjectsTypeList("Obstacle")[i].GetObjectGridPosition().x == 2) && GetFullObjectsTypeList("Obstacle")[i].GetObjectGridPosition().y == height - 1)
                        || ((objectGridPosition.x == width - 1 && objectGridPosition.y == height - 2) && (GetFullObjectsTypeList("Obstacle")[i].GetObjectGridPosition().x == width - 2) && GetFullObjectsTypeList("Obstacle")[i].GetObjectGridPosition().y == height - 1)
                        )
                    {
                        return false;
                    }


                }

            }
            return true;

        }

        
        return true;
    }

    private void SpawnObject(string name, Vector2Int objectGridPosition = default)
    {
        Sprite sprite = null;
        Direction snakeGridDirection = default;
        int sortOrder = 0;
        string sortingLayerName = "Objects";
        switch (name)
        {
            case "Box":
            {
                sprite = GameAssets.gameAssets.box;
                sortOrder = 1;

                if (objectGridPosition == default)
                {
                    do
                    {
                        objectGridPosition = new Vector2Int(UnityEngine.Random.Range(2, width-1), UnityEngine.Random.Range(2, height-1));
                    } while (snake.GetFullSnakeGridPositionList().IndexOf(objectGridPosition) != -1 || GetFullObjectsGridPositionList().IndexOf(objectGridPosition) != -1);
                }

                break;
            }
            case "BoxUnlock":
            {
                if (objectGridPosition == default)
                {
                    do
                    {
                        objectGridPosition = new Vector2Int(UnityEngine.Random.Range(1, width), UnityEngine.Random.Range(1, height));
                    } while (snake.GetFullSnakeGridPositionList().IndexOf(objectGridPosition) != -1 || GetFullObjectsGridPositionList().IndexOf(objectGridPosition) != -1);
                }
                sprite = GameAssets.gameAssets.boxUnlock;
                break;
            }
            case "ChainedApple":
            {
                if (objectGridPosition == default)
                {
                    do
                    {
                        objectGridPosition = new Vector2Int(UnityEngine.Random.Range(1, width), UnityEngine.Random.Range(1, height));
                    } while (!CheckRestrictions(objectGridPosition, name));
                }
                sprite = GameAssets.gameAssets.locker;
                break;
            }
            case "Food":
            {
                if (objectGridPosition == default)
                {
                    do
                    {
                        objectGridPosition = new Vector2Int(UnityEngine.Random.Range(1, width), UnityEngine.Random.Range(1, height));
                    } while (!CheckRestrictions(objectGridPosition, name));
                }
                sprite = GameAssets.gameAssets.apple;
                break;
            }
            case "Key":
            {
                if (objectGridPosition == default)
                {
                    do
                    {
                        objectGridPosition = new Vector2Int(UnityEngine.Random.Range(1, width), UnityEngine.Random.Range(1, height));
                    } while (!CheckRestrictions(objectGridPosition,name));
                }
                sprite = GameAssets.gameAssets.key;
                sortOrder = 1;
                break;
            }
            case "Obstacle":
            {
                if(objectGridPosition == default)
                {
               
                    do
                    {
                        objectGridPosition = new Vector2Int(UnityEngine.Random.Range(1, width), UnityEngine.Random.Range(1, height));
                    } while (!CheckRestrictions(objectGridPosition, name));

                }

            
                sprite = GameAssets.gameAssets.obstacle;
                break;
            }
            case "ArrowApple":
            {
                snakeGridDirection = GetRandomDirection();
            
                if (objectGridPosition == default)
                {
                    do
                    {
                        objectGridPosition = new Vector2Int(UnityEngine.Random.Range(1, width), UnityEngine.Random.Range(1, height));
                    } while (!CheckRestrictions(objectGridPosition,name,snakeGridDirection));
                }
                switch (snakeGridDirection)
                {
                    case Direction.Left:
                        sprite = GameAssets.gameAssets.leftArrow;

                        break;
                    case Direction.Right:
                        sprite = GameAssets.gameAssets.rightArrow;
                        break;
                    case Direction.Up:
                        sprite = GameAssets.gameAssets.upArrow;
                        break;
                    case Direction.Down:
                        sprite = GameAssets.gameAssets.downArrow;
                        break;
                }

                break;
            }
        }

        
        GameObject objectGameObject = CreateGameObject(name, objectGridPosition, sprite, sortOrder, sortingLayerName);
        objectsList.Add(new Object(objectGridPosition, objectGameObject, name, snakeGridDirection, sprite));
    }

    public bool ObjectCollisionBorder(Vector2Int objectGridPosition)
    {
        if (objectGridPosition.x == 0 || objectGridPosition.y == 0 || objectGridPosition.x == width || objectGridPosition.y == height)
        {
            return true;
        }
        return false;
    }

    public bool BoxCollisionSnake(Vector2Int objectGridPosition)
    {
        
        if (snake.GetFullSnakeGridPositionList().GetRange(1, snake.GetFullSnakeGridPositionList().Count-1).IndexOf(objectGridPosition) != -1)
        {
            return true;
        }
        
        return false;
    }

    private IEnumerator MoveObject(Object myObject, Vector2Int startGrid, Vector2Int targetGrid)
    {
        
        Vector3 startPos = new Vector3(startGrid.x, startGrid.y);
        Vector3 targetPos = new Vector3(targetGrid.x, targetGrid.y, 0);
            
        for (var t = 0f; t < PlayerPrefs.GetFloat("SnakeSpeed"); t += Time.deltaTime)
        {
            if (myObject.GetObjectGameObject() != null)
            {
                float lerp = t / PlayerPrefs.GetFloat("SnakeSpeed");
                myObject.GetObjectGameObject().transform.position = Vector3.Lerp(startPos, targetPos, lerp);
                yield return null;
            }
        }
        if(myObject.GetObjectGameObject() != null)
        {
            myObject.SetObjectGridPosition(targetGrid);
        }
    }
    
    
    public string TrySnakeCollisionObject(Vector2Int snakeGridPosition, Direction snakeGridMoveDirection, Vector2Int snakeGridMoveDirectionVector)
    {
        if (objectsList.Count <= 0) return ObjectCollisionBorder(snakeGridPosition) ? "Border" : "";
        for (int i = 0; i < objectsList.Count; i++)
        {
            if (objectsList[i].GetObjectGridPosition() != snakeGridPosition) continue;
            string objectType = objectsList[i].GetObjectType();
            switch (objectsList[i].GetObjectType())
            {
                case "Food":
                    Destroy(objectsList[i].GetObjectGameObject());
                    objectsList.Remove(objectsList[i]);
                    break;
                case "Key":
                {
                    if (objectsList.Count > 0)
                    {
                        for (int k = 0; k < objectsList.Count; k++)
                        {
                            if (objectsList[k].GetObjectType() != "ChainedApple") continue;
                            if ((objectsList[i].GetObjectGridPosition().x == objectsList[k].GetObjectGridPosition().x + 1 && objectsList[i].GetObjectGridPosition().y == objectsList[k].GetObjectGridPosition().y  )
                                || (objectsList[i].GetObjectGridPosition().x == objectsList[k].GetObjectGridPosition().x - 1 && objectsList[i].GetObjectGridPosition().y == objectsList[k].GetObjectGridPosition().y  )
                                || (objectsList[i].GetObjectGridPosition().y == objectsList[k].GetObjectGridPosition().y + 1 && objectsList[i].GetObjectGridPosition().x == objectsList[k].GetObjectGridPosition().x  )
                                || (objectsList[i].GetObjectGridPosition().y == objectsList[k].GetObjectGridPosition().y - 1 && objectsList[i].GetObjectGridPosition().x == objectsList[k].GetObjectGridPosition().x  ))
                            {
                                objectsList[k].SetObjectSprite(GameAssets.gameAssets.apple);
                                objectsList[k].SetObjectType("Food");
                                
                                Destroy(objectsList[i].GetObjectGameObject());
                                objectsList.Remove(objectsList[i]);
                                Debug.Log("aqui");
                            }
                            else
                            {
                                StartCoroutine(MoveObject(objectsList[i], objectsList[i].GetObjectGridPosition(),
                                    objectsList[k].GetObjectGridPosition()));
                            }
                            
                        }
                    }
                    break;
                }
                case "ArrowApple" when objectsList[i].GetArrowDirection() == snakeGridMoveDirection:
                    Destroy(objectsList[i].GetObjectGameObject());
                    objectsList.Remove(objectsList[i]);
                    return "Food";
            }

            return objectType;
        }
        return ObjectCollisionBorder(snakeGridPosition) ? "Border" : "";
    }

    public void TryKeyCollisionChainedApple()
    {
        foreach (var myObject in objectsList.ToList())
        {
            foreach (var secondObject in objectsList.ToList())
            {
                if (myObject.GetObjectType() == "ChainedApple" && secondObject.GetObjectType() == "Key" &&
                    myObject.GetObjectGridPosition() == secondObject.GetObjectGridPosition())
                {
                    
                    myObject.SetObjectSprite(GameAssets.gameAssets.apple);
                    myObject.SetObjectType("Food");
                    Destroy(secondObject.GetObjectGameObject());
                    objectsList.Remove(secondObject);
                    
                }
            }
        }
    }

    public void TrySnakeCollisionBox(Vector2Int snakeGridPosition, Direction snakeGridMoveDirection, Vector2Int snakeGridMoveDirectionVector)
    {
        foreach (var myObject in objectsList)
        {
            if (myObject.GetObjectGridPosition() != snakeGridPosition) continue;
            string objectType = myObject.GetObjectType();
            if (objectType != "Box") continue;
            if (objectsList.Count > 0)
            {
                StartCoroutine(MoveObject(myObject, snakeGridPosition, snakeGridPosition+snakeGridMoveDirectionVector));
            }
        }

        foreach (var myObject in objectsList)
        {
            var objectType = myObject.GetObjectType();
            if (objectType != "Box") continue;
            if (objectsList.Count <= 0) continue;
            foreach (var secondObject in objectsList)
            {
                string secondObjectType = secondObject.GetObjectType();
                if (secondObjectType == "BoxUnlock" && myObject.GetObjectGridPosition() == secondObject.GetObjectGridPosition())
                {
                    secondObject.SetObjectSprite(GameAssets.gameAssets.apple);
                    secondObject.SetObjectType("Food");
                }
                if (((secondObjectType == "Obstacle" || secondObjectType == "ChainedApple" || secondObjectType == "BoxUnlock") && secondObject.GetObjectGridPosition() == myObject.GetObjectGridPosition()) ||
                    ObjectCollisionBorder(myObject.GetObjectGridPosition()) || BoxCollisionSnake(myObject.GetObjectGridPosition()))
                {
                    Vector2Int boxGridPosition;
                    do
                    {
                        boxGridPosition = new Vector2Int(UnityEngine.Random.Range(2, width - 1), UnityEngine.Random.Range(2, height - 1));
                    } while (snake.GetFullSnakeGridPositionList().IndexOf(boxGridPosition) != -1 || GetFullObjectsGridPositionList().IndexOf(boxGridPosition) != -1);

                    StartCoroutine(MoveObject(myObject, snakeGridPosition,boxGridPosition));
                    break;
                }
            }
        }
    }
    Direction GetRandomDirection()
    {
        Direction[] values = (Direction[])Enum.GetValues(typeof(Direction));
        int index = UnityEngine.Random.Range(0, values.Length);
        return values[index];
    }

   
    private void DeleteObstacle()
    {
        for(int i = 0; i<objectsList.Count; i++)
        {
            if(objectsList[i].GetObjectType() == "Obstacle")
            {
                Destroy(objectsList[i].GetObjectGameObject());
                objectsList.Remove(objectsList[i]);
                break;
            }
        }
        
    }
    
}