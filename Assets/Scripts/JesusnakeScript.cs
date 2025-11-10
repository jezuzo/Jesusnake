
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.U2D;
using System.Collections;
using Unity.VisualScripting;

public class JesusnakeScript : MonoBehaviour
{
    public float moveSpeed;
    private Vector2 direction;
    private Rigidbody2D rb;
    private List<Transform> segments;
    public Transform segmentPrefab;
    public Sprite tail;
    public Sprite normal;
    public Sprite leftDownCorner;
    public Sprite rightDownCorner;
    public Sprite leftUpCorner;
    public Sprite rightUpCorner;
    private bool canChangeDirection = true;
    string orientation;
    string lastOrientation;
    Vector3 posEsquina;
    

      class CornerData
    {
        public Vector3 pos;
        public string orientation;
        public Sprite sprite;
    }

    List<CornerData> corners = new List<CornerData>();

    void RegisterCorner(string orientation, Vector3 pos, Sprite sprite)
    {
        CornerData c = new CornerData();
        c.pos = pos;
        c.orientation = orientation;
        c.sprite = sprite;
        corners.Add(c);
    }
    
    void ManageCorners(string orientation, string lastOrientation)
    {
        
        for (int i = 0; i < corners.Count; i++)
        {
            for (int e = 1; e < segments.Count - 1; e++)
            {
                //Debug.Log(i);
                if (segments[e].position == corners[i].pos)
                {
                    segments[e].GetComponent<SpriteRenderer>().sprite = corners[i].sprite;
                    segments[e].rotation = Quaternion.identity;
                }
                else
                {
                    if(segments[e].GetComponent<SpriteRenderer>().sprite == normal)
                    {
                        
                        if (orientation == "up" && segments[e].position.y > corners[i].pos.y)
                        {
                            orientation = "up";
                            Quaternion rotation = GetSegmentRotation(orientation);
                            segments[e].rotation = rotation;
                            
                        }
                        else if (orientation == "down" && segments[e].position.y < corners[i].pos.y)
                        {
                            orientation = "down";
                            Quaternion rotation = GetSegmentRotation(orientation);
                            segments[e].rotation = rotation;
                        }
                        else if (orientation == "left" && segments[e].position.x < corners[i].pos.x)
                        {
                            orientation = "left";
                            Quaternion rotation = GetSegmentRotation(orientation);
                            segments[e].rotation = rotation;
                        }
                        else if (orientation == "right" && segments[e].position.x > corners[i].pos.x)
                        {
                            orientation = "right";
                            Quaternion rotation = GetSegmentRotation(orientation);
                            segments[e].rotation = rotation;
                        }
                        else
                        {
                            Quaternion lastRotation = GetSegmentRotation(orientation);
                            segments[e].rotation = lastRotation;
                        }
                    }
                    
                        
                }

            }

        }
        if(corners.Count != 0)
        {
            
            if (segments[segments.Count - 1].position == corners[0].pos)
            {
                segments[segments.Count - 1].rotation = segments[segments.Count - 2].rotation;
                Debug.Log("Esquina eliminada");
                corners.Remove(corners[0]);
                Debug.Log(corners.Count);
            }
            
        }
    }
    void Grow()
    {
        Transform newSegment = Instantiate(segmentPrefab);
        newSegment.position = segments[segments.Count - 1].position;
        newSegment.GetComponent<SpriteRenderer>().enabled = false;
        segments.Add(newSegment);

    }
    void InitialState()
    {
        if (transform.position == Vector3.zero)
        {
            segments = new List<Transform>();
            segments.Add(transform);
            Vector3 posInit = new Vector3(1.1f, 0, 0);
            Transform newSegment = Instantiate(segmentPrefab,posInit, Quaternion.identity);
            //Debug.Log(newSegment.position);
            segments.Add(newSegment);
            //Debug.Log(newSegment.position);
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        InitialState();
    }

    // Update is called once per frame
    void Update()
    {
        if (!canChangeDirection) return;

        if (Keyboard.current.leftArrowKey.wasPressedThisFrame && direction.x != 1)
        {
            
            lastOrientation = orientation;
            orientation = "left";
            if(segments.Count != 2)
            {
                fixCornersReal(orientation);
            }
            
            direction = Vector2.left * moveSpeed;
            canChangeDirection = false;
        }
        else if (Keyboard.current.rightArrowKey.wasPressedThisFrame && direction.x != -1)
        {
           
            lastOrientation = orientation;
            orientation = "right";
            if(segments.Count != 2)
            {
                fixCornersReal(orientation);
            }
            direction = Vector2.right * moveSpeed;
            canChangeDirection = false;
        }
        else if (Keyboard.current.upArrowKey.wasPressedThisFrame && direction.y != -1)
        {
            
            lastOrientation = orientation;
            orientation = "up";
            if(segments.Count != 2)
            {
                fixCornersReal(orientation);
            }
            direction = Vector2.up * moveSpeed;
            canChangeDirection = false;
        }
        else if (Keyboard.current.downArrowKey.wasPressedThisFrame && direction.y != 1)
        {
           
            lastOrientation = orientation;
            orientation = "down";
            if(segments.Count != 2)
            {
                fixCornersReal(orientation);
            }
            direction = Vector2.down * moveSpeed;
            canChangeDirection = false;
        }
        
    }

    Sprite GetCornerSprite(string last, string now)
    {
        if (last == "left" && now == "up") return leftUpCorner;
        if (last == "left" && now == "down") return leftDownCorner;
        if (last == "right" && now == "up") return rightUpCorner;
        if (last == "right" && now == "down") return rightDownCorner;
        if (last == "up" && now == "left") return rightDownCorner;
        if (last == "up" && now == "right") return leftDownCorner;
        if (last == "down" && now == "left") return rightUpCorner;
        if (last == "down" && now == "right") return leftUpCorner;

        return null;
    }

    Quaternion GetSegmentRotation(string o)
    {
        return o switch
        {
            "left" => Quaternion.Euler(0, 0, 0),
            "right" => Quaternion.Euler(0, 0, 180),
            "up" => Quaternion.Euler(0, 0, -90),
            "down" => Quaternion.Euler(0, 0, 90),
            _ => Quaternion.identity
        };
    }

    void fixCornersReal(string orientation)
    {
        posEsquina = segments[0].position;
        Sprite sprite = GetCornerSprite(lastOrientation, orientation);
        Debug.Log("Esquina creada");
        RegisterCorner(orientation, posEsquina, sprite);
    }

    // void fixCorners(Sprite sprite, Quaternion rotation, string orientation)
    // {
    //     if (firstCorner)
    //     {
    //         posEsquina = segments[1].position;
    //         firstCorner = false;
    //         RegisterCorner(orientation, rotation, posEsquina,sprite);
    //     }
        
    //     if (segments[segments.Count - 1].position != posEsquina)
    //     {
    //         for (int i = 1; i < segments.Count - 1; i++)
    //         {
    //             if (segments[i].position == posEsquina)
    //             {
    //                 segments[i].GetComponent<SpriteRenderer>().sprite = sprite;
    //                 segments[i].rotation = Quaternion.identity;
    //             }
    //             else if ((orientation == "down" && segments[i].position.y < posEsquina.y) || (orientation == "up" && segments[i].position.y > posEsquina.y))
    //             {

    //                 segments[i].rotation = rotation;
    //                 segments[i].GetComponent<SpriteRenderer>().sprite = normal;
    //             }

    //         }
    //     }
    //     else
    //     {
    //         firstCorner = true;
    //         lastOrientation = orientation;
    //         segments[1].rotation = rotation;
    //         segments[segments.Count - 1].rotation = rotation;
    //         segments[segments.Count - 2].rotation = rotation;
    //     }
        
        

    // }

    // void fixOrientation(string orientation, string lastOrientation)
    // {
    //     Quaternion rotation;
    //     rotation = GetSegmentRotation(orientation);
    //     segments[0].rotation = rotation;
        
    //     if (segments.Count == 2)
    //     {
    //         segments[1].rotation = rotation;
    //     }
    //     for (int i = 0; i < segments.Count-1; i++)
    //     {
    //         if(segments[i].GetComponent<SpriteRenderer>().sprite == normal)
    //         {
    //             segments[i].rotation = rotation;
    //         }
            
    //     }
    // }
    
    void createTail()
    {
        segments[segments.Count - 1].GetComponent<SpriteRenderer>().sprite = tail;
        //segments[segments.Count - 1].tag = "Tail";
        segments[segments.Count - 1].GetComponent<SpriteRenderer>().sortingOrder = 1;
        
    }

    void FixedUpdate()
    {
        //Debug.Log(firstCorner);
        segments[0].rotation = GetSegmentRotation(orientation);
        Debug.Log("Orientation: " + orientation + " " + "LastOrientation: " + lastOrientation);
        if (direction != Vector2.zero)
        {
            for (int i = segments.Count - 1; i > 0; i--)
            {
                
                segments[i].position = segments[i - 1].position;
                segments[i].GetComponent<SpriteRenderer>().sprite = normal;
                segments[i].GetComponent<SpriteRenderer>().enabled = true;
                
            }
            rb.position = new Vector2(Mathf.Round(rb.position.x) + direction.x, Mathf.Round(rb.position.y) + direction.y);
            //Debug.Log(transform.position);
        }
        canChangeDirection = true;
        createTail();
        //fixOrientation(orientation,lastOrientation);
        ManageCorners(orientation,lastOrientation);
        
    }
    void endGame()
    {
        for (int i = 1; i < segments.Count ; i++)
        {
            Destroy(segments[i].gameObject);
        }
        Debug.Log("muerte");
        segments.Clear();
        corners.Clear();
        transform.position = Vector3.zero;
        direction = Vector2.zero;
        InitialState();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log(other.tag);
        if (other.tag == "Food")
        {
            Grow();
            Destroy(other.gameObject);

        }else if(other.tag == "SnakeSegment" || other.tag == "Border")
        {
            endGame();
        }
    }
}
