
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.U2D;

public class JesusnakeScript : MonoBehaviour
{
    public float moveSpeed;
    private Vector2 direction;
    private Rigidbody2D rb;
    private List<Transform> segments;
    public Transform segmentPrefab;
    bool side = false;
    bool vertical = false;
    public Sprite tail;
    public Sprite normal;
    public Sprite leftDownCorner;
    public Sprite rightDownCorner;
    public Sprite leftUpCorner;
    public Sprite rightUpCorner;
    string orientation;
    string lastOrientation;
    bool firstCorner = true;
    List<bool> firstCorners;
    List<Vector3> posEsquinas;
    Vector3 posEsquina;
    

      class CornerData
    {
        public Vector3 pos;
        public string orientation;
        public Quaternion rotation;
    }

    List<CornerData> corners = new List<CornerData>();

    void RegisterCorner(string orientation, Quaternion rotation, Vector3 pos)
    {
        CornerData c = new CornerData();
        c.pos = pos ;
        c.orientation = orientation;
        c.rotation = rotation;
        corners.Add(c);
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
        if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            if (!side)
            {
                lastOrientation = orientation;
                orientation = "left";
                vertical = false;
                direction = Vector2.left * moveSpeed;
            }
            side = true;
        }
        else if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            if (!side)
            {
                lastOrientation = orientation;
                orientation = "right";
                vertical = false;
                direction = Vector2.right * moveSpeed;
            }
            side = true;
        }
        else if (Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            if (!vertical)
            {
                lastOrientation = orientation;
                orientation = "up";
                side = false;
                direction = Vector2.up * moveSpeed;

            }
            vertical = true;
        }
        else if (Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            if (!vertical)
            {
                lastOrientation = orientation;
                orientation = "down";
                side = false;
                direction = Vector2.down * moveSpeed;

            }
            vertical = true;
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


    void fixCorners(Sprite sprite, Quaternion rotation, string orientation)
    {
        if (firstCorner)
        {
            posEsquina = segments[1].position;
            firstCorner = false;
            RegisterCorner(orientation, rotation, posEsquina);
        }

        if(segments[segments.Count -1].position != posEsquina)
        {
            for (int i = 1; i < segments.Count - 1; i++)
            {
                if (segments[i].position == posEsquina)
                { 
                    segments[i].GetComponent<SpriteRenderer>().sprite = sprite;
                    segments[i].rotation = Quaternion.identity;
                }
                else if ((orientation == "down" && segments[i].position.y < posEsquina.y) || (orientation == "up" && segments[i].position.y > posEsquina.y))
                {

                    segments[i].rotation = rotation;
                    segments[i].GetComponent<SpriteRenderer>().sprite = normal;
                }

            }
        }
        else
        {
            firstCorner = true;
            lastOrientation = orientation;
            segments[1].rotation = rotation;
            segments[segments.Count - 1].rotation = rotation;
            segments[segments.Count - 2].rotation = rotation;
        }

    }


    void fixOrientation(string orientation)
    {
        Quaternion rotation;
        rotation = GetSegmentRotation(orientation);
        segments[0].rotation = rotation;
        if (segments.Count == 2)
        {
            segments[1].rotation = rotation;
        }

        if (lastOrientation != orientation)
        {
            Sprite sprite = GetCornerSprite(lastOrientation, orientation);
            fixCorners(sprite, rotation, orientation);
        }
        else
        {
            for (int i = 0; i < segments.Count; i++)
            {
                segments[i].rotation = rotation;
            }
        }


    }
    
    void createTail()
    {
        segments[segments.Count - 1].GetComponent<SpriteRenderer>().sprite = tail;
        //segments[segments.Count - 1].tag = "Tail";
        segments[segments.Count - 1].GetComponent<SpriteRenderer>().sortingOrder = 1;
        segments[segments.Count - 1].rotation = segments[segments.Count - 2].rotation;
    }

    void FixedUpdate()
    {
        //Debug.Log(firstCorner);

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
        createTail();
        fixOrientation(orientation);
    }
    void endGame()
    {
        for (int i = 1; i < segments.Count ; i++)
        {
            Destroy(segments[i].gameObject);
        }
        Debug.Log("muerte");
        segments.Clear();
        transform.position = Vector3.zero;
        direction = Vector2.zero;
        side = false;
        vertical = false;
        InitialState();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.tag);
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
