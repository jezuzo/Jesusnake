
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
    public float spawnRateRef = 0.1f;
    private float spawnRate;
    private Snake snake;
    private List<Object> objectsList;
    private int width;
    private int height;

    private bool chainedApplesEnabled = false;
    private bool boxesEnabled = false;
    private bool appleArrowsEnabled = false;
    private bool normalModeEnabled = false;
    
    public bool enableChainedApples = false;
    public bool enableBoxes = false;
    public bool enableAppleArrows = false;
    public bool enableNormalMode = false;
    public bool enableObstacles = false;
    public bool enableRandomMode = true;

    public void Setup(Snake snake, LevelGrid levelGrid)
    {
        
        this.snake = snake;
        objectsList = new List<Object>();
        width = levelGrid.width - 1;
        height = levelGrid.height - 1;
        

        spawnRate = spawnRateRef;

    }
    private void Start()
    {
        if (enableRandomMode)
        {
            EnableRandomMode();
            GameAssets.gameAssets.trophy.sprite = GameAssets.gameAssets.RandomHighScore;
            GameAssets.gameAssets.trophy.rectTransform.sizeDelta = new Vector2(90, 100);
        }
       
    }
    public class Object
    {
        private Vector2Int objectGridPosition;
        private GameObject objectGameObject;
        private string type;
        private Direction arrowDirection;
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
        
       
    }

    private void EnableRandomMode()
    {
        enableChainedApples = false;
        enableBoxes = false;
        enableAppleArrows = false;
        enableNormalMode = false;

        int randNum = UnityEngine.Random.Range(0, 4);
        Debug.Log(randNum);
        if(randNum == 0)
        {
            enableAppleArrows = true;

        }else if(randNum == 1)
        {
            enableBoxes = true;
        }else if (randNum == 2)
        {
            enableChainedApples = true;
        }
        else if(randNum == 3)
        {
            enableNormalMode = true;
        }
        
        

    }
    private void EnableObstacles()
    {

        if (spawnTimer > spawnRate)
        {
            SpawnObject("Obstacle");
            spawnTimer = 0f;
            Invoke("DeleteObstacle", 5f);
            //spawnRate = UnityEngine.Random.Range(spawnRateRef, spawnRateRef + 1f);
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
        
        for(int i = 0; i<objectsList.Count; i++)
        {
            if (objectsList[i].GetObjectType() == "BoxUnlock" )
            {
                isBox = true;
            }
            
        }
        if (!isBox)
        {
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

    }
    private void EnableChainedApples()
    {
        bool isChainedApple = false;
        if (!chainedApplesEnabled)
        {
            SpawnObject("Key");
            SpawnObject("ChainedApple");

            
            chainedApplesEnabled = true;

        }

        for (int i = 0; i < objectsList.Count; i++)
        {
            if (objectsList[i].GetObjectType() == "UnchainedApple" || objectsList[i].GetObjectType()=="ChainedApple")
            {
                isChainedApple = true;
            }

        }
        if (!isChainedApple)
        {
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

    }

    private void EnableNormalMode()
    {

        bool isFood = false;
        if (!normalModeEnabled)
        {
            SpawnObject("Food");
            
            normalModeEnabled = true;

        }

        for (int i = 0; i < objectsList.Count; i++)
        {
            if (objectsList[i].GetObjectType() == "Food")
            {
                isFood = true;
            }

        }
        if (!isFood)
        {
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
    }

    private void EnableArrowApples()
    {
        bool isAppleArrow = false;
        if (!appleArrowsEnabled)
        {

            SpawnObject("ArrowApple");
            
           
            appleArrowsEnabled = true;
        }
        for (int i = 0; i < objectsList.Count; i++)
        {
            if (objectsList[i].GetObjectType() == "ArrowApple")
            {
                isAppleArrow = true;
            }

        }
        if (!isAppleArrow)
        {
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
    }

    private GameObject CreateGameObject(string name, Vector2Int position, Sprite sprite, int sortOrder = 1, 
        string sortingLayerName = "Default")
    {
        
        GameObject gameObject;
        gameObject = new GameObject(name, typeof(SpriteRenderer));
        gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
        gameObject.GetComponent<SpriteRenderer>().sortingOrder = sortOrder;
        gameObject.GetComponent<SpriteRenderer>().sortingLayerName = sortingLayerName;
        
        gameObject.transform.position = new Vector3(position.x, position.y, 0);
        return gameObject;

    }

    public List<Vector2Int> GetFullObjectsGridPositionList(string type = "None")
    {

        List<Vector2Int> gridPositionList = new List<Vector2Int>();
        foreach (Object GameObject in objectsList)
        {
            if(type == "None")
            {
                gridPositionList.Add(GameObject.GetObjectGridPosition());
            }
            else
            {
                if(GameObject.GetObjectType() == type)
                {
                    gridPositionList.Add(GameObject.GetObjectGridPosition());
                }
            }
            
        }
        return gridPositionList;
    }

    public List<Object> GetFullObjectsTypeList(string type = "None")
    {

        List<Object> ObjectList = new List<Object>();
        foreach (Object GameObject in objectsList)
        {
            if (type == "None")
            {
                ObjectList.Add(GameObject);
            }
            else
            {
                if (GameObject.GetObjectType() == type)
                {
                    ObjectList.Add(GameObject);
                }
            }

        }
        return ObjectList;
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



        if (type == "Obstacle")
        {
            if (objectGridPosition.x == width / 2 && objectGridPosition.y == height / 2)
            {
                return false;
            }
            else if (objectGridPosition.x == width / 2 + 1 && objectGridPosition.y == height / 2 + 1)
            {
                return false;
            }
            else if (objectGridPosition.x == width / 2 && objectGridPosition.y == height / 2 + 1)
            {
                return false;
            }
            else if (objectGridPosition.x == width / 2 + 1 && objectGridPosition.y == height / 2)
            {
                return false;
            }
            else if (objectGridPosition.x == width / 2 - 1 && objectGridPosition.y == height / 2 - 1)
            {
                return false;
            }
            else if (objectGridPosition.x == width / 2 && objectGridPosition.y == height / 2 - 1)
            {
                return false;
            }
            else if (objectGridPosition.x == width / 2 - 1 && objectGridPosition.y == height / 2)
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
        GameObject objectGameObject;
        Sprite sprite = null;
        Direction snakeGridDirection = default;
        int sortOrder = 0;
        string sortingLayerName = "Objects";
        if(name == "Box")
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
        }
        else if(name == "BoxUnlock")
        {
            if (objectGridPosition == default)
            {
                do
                {
                    objectGridPosition = new Vector2Int(UnityEngine.Random.Range(1, width), UnityEngine.Random.Range(1, height));
                } while (snake.GetFullSnakeGridPositionList().IndexOf(objectGridPosition) != -1 || GetFullObjectsGridPositionList().IndexOf(objectGridPosition) != -1);
            }
            sprite = GameAssets.gameAssets.boxUnlock;
            
        }else if(name == "ChainedApple")
        {
            if (objectGridPosition == default)
            {
                do
                {
                    objectGridPosition = new Vector2Int(UnityEngine.Random.Range(1, width), UnityEngine.Random.Range(1, height));
                } while (!CheckRestrictions(objectGridPosition, name));
            }
            sprite = GameAssets.gameAssets.locker;
            
        }else if(name == "Food" || name == "UnchainedApple")
        {
            if (objectGridPosition == default)
            {
                do
                {
                    objectGridPosition = new Vector2Int(UnityEngine.Random.Range(1, width), UnityEngine.Random.Range(1, height));
                } while (!CheckRestrictions(objectGridPosition, name));
            }
            sprite = GameAssets.gameAssets.apple;
        }else if(name == "Key")
        {
            if (objectGridPosition == default)
            {
                do
                {
                    objectGridPosition = new Vector2Int(UnityEngine.Random.Range(1, width), UnityEngine.Random.Range(1, height));
                } while (!CheckRestrictions(objectGridPosition,name));
            }
            sprite = GameAssets.gameAssets.key;
        }else if(name == "Obstacle")
        {
            if(objectGridPosition == default)
            {
               
                do
                {
                    objectGridPosition = new Vector2Int(UnityEngine.Random.Range(1, width), UnityEngine.Random.Range(1, height));
                } while (!CheckRestrictions(objectGridPosition, name));

            }

            
            sprite = GameAssets.gameAssets.obstacle;
            


        }
        else if(name == "ArrowApple")
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




        }

        
        objectGameObject = CreateGameObject(name, objectGridPosition, sprite, sortOrder, sortingLayerName);
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

    private IEnumerator moveBox(Object boxObject, Vector2Int snakeGridPosition, Vector2Int snakeGridMoveDirectionVector,Vector2Int targetGrid = default)
    {
        
        Vector3 startPos = new Vector3(snakeGridPosition.x, snakeGridPosition.y);
        if(targetGrid == default)
        {
            targetGrid = snakeGridPosition + snakeGridMoveDirectionVector;
        }
       
        Vector3 targetPos = new Vector3(targetGrid.x, targetGrid.y, 0);
            
        for (float t = 0f; t < 0.2f; t += Time.deltaTime)
        {

            if (boxObject.GetObjectGameObject() != null && boxObject.GetObjectType() == "Box")
            {
                float lerp = t / 0.2f;
                boxObject.GetObjectGameObject().transform.position = Vector3.Lerp(startPos, targetPos, lerp);
                yield return null;
            }
                    
                    



        }
        if(boxObject.GetObjectGameObject() != null && boxObject.GetObjectType() == "Box")
        {
            boxObject.SetObjectGridPosition(targetGrid);
        }
                

            
            
            
        
        
    }
    public string TrySnakeCollisionObject(Vector2Int snakeGridPosition, Direction snakeGridMoveDirection, Vector2Int snakeGridMoveDirectionVector)
    {
        if(objectsList.Count > 0)
        {
            for (int i = 0; i < objectsList.Count; i++)
            {

                if (objectsList[i].GetObjectGridPosition() == snakeGridPosition)
                {
                    string objectType = objectsList[i].GetObjectType();
                    if (objectType == "Food" || objectType == "UnchainedApple")
                    {
                        Destroy(objectsList[i].GetObjectGameObject());
                        objectsList.Remove(objectsList[i]);

                    }
                    if (objectType == "Key")
                    {
                        Destroy(objectsList[i].GetObjectGameObject());
                        objectsList.Remove(objectsList[i]);
                        if (objectsList.Count > 0)
                        {
                            for (int k = 0; k < objectsList.Count; k++)
                            {
                                if (objectsList[k].GetObjectType() == "ChainedApple")
                                {
                                    SpawnObject("UnchainedApple", objectsList[k].GetObjectGridPosition());
                                    Destroy(objectsList[k].GetObjectGameObject());
                                    objectsList.Remove(objectsList[k]);

                                    
                                }
                            }

                        }
                        
                        
                    }
                    if (objectType == "ArrowApple")
                    {
                        
                        
                        if (objectsList[i].GetArrowDirection() == snakeGridMoveDirection)
                        {
                            
                            Destroy(objectsList[i].GetObjectGameObject());
                            objectsList.Remove(objectsList[i]);
                            return "Food";
                           
                        }
                    }
                    return objectType;
                }
                

            }

        }
        
        if (ObjectCollisionBorder(snakeGridPosition))
        {
            return "Border";
        }
        return "";
    }

    public void TrySnakeCollisionBox(Vector2Int snakeGridPosition, Direction snakeGridMoveDirection, Vector2Int snakeGridMoveDirectionVector)
    {

        for (int i = 0; i < objectsList.Count; i++)
        {
            if (objectsList[i].GetObjectGridPosition() == snakeGridPosition)
            {
                string objectType = objectsList[i].GetObjectType();
                if (objectType == "Box")
                {

                    if (objectsList.Count > 0)
                    {
                        
                        StartCoroutine(moveBox(objectsList[i], snakeGridPosition, snakeGridMoveDirectionVector));
                        
                        



                    }

                }

            }
            

        }

        for (int i = 0; i < objectsList.Count; i++)
        {
            
            string objectType = objectsList[i].GetObjectType();
            if (objectType == "Box")
            {

                if (objectsList.Count > 0)
                {

                    for (int j = 0; j < objectsList.Count; j++)
                    {


                        string secondObjectType = objectsList[j].GetObjectType();

                        if (secondObjectType == "BoxUnlock" && objectsList[i].GetObjectGridPosition() == objectsList[j].GetObjectGridPosition())
                        {

                            objectsList[j].SetObjectSprite(GameAssets.gameAssets.apple);
                            objectsList[j].SetObjectType("Food");

                        }

                        if (((secondObjectType == "Obstacle" || secondObjectType == "ChainedApple" || secondObjectType == "BoxUnlock") && objectsList[j].GetObjectGridPosition() == objectsList[i].GetObjectGridPosition()) ||
                            ObjectCollisionBorder(objectsList[i].GetObjectGridPosition()) || BoxCollisionSnake(objectsList[i].GetObjectGridPosition()))
                        {

                            
                            Vector2Int boxGridPosition;
                            do
                            {
                                boxGridPosition = new Vector2Int(UnityEngine.Random.Range(2, width - 1), UnityEngine.Random.Range(2, height - 1));
                            } while (snake.GetFullSnakeGridPositionList().IndexOf(boxGridPosition) != -1 || GetFullObjectsGridPositionList().IndexOf(boxGridPosition) != -1);

                            StartCoroutine(moveBox(objectsList[i], snakeGridPosition, snakeGridMoveDirectionVector,boxGridPosition));
                            

                            break;

                        }


                    }


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
