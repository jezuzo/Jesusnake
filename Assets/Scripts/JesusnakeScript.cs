
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

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
    bool firstCorner1 = true;
    bool firstCorner2 = true;
    Vector3 posEsquina1;
    Vector3 posEsquina2;

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

    void fixCorners1(Sprite sprite, Quaternion rotation, string orientation)
    {

        if (firstCorner1)
        {
            posEsquina1 = segments[1].position;
            firstCorner1 = false;


        }

        for (int i = 1; i < segments.Count - 1; i++)
        {
            if (segments[i].position == posEsquina1)
            {
                segments[i].GetComponent<SpriteRenderer>().sprite = sprite;
                segments[i].rotation = Quaternion.identity;
            }
            else if ((orientation == "down" && segments[i].position.y < posEsquina1.y) || (orientation == "up" && segments[i].position.y > posEsquina1.y))
            {

                segments[i].rotation = rotation;
                segments[i].GetComponent<SpriteRenderer>().sprite = normal;

            }
            if (segments[segments.Count - 1].position == posEsquina1)
            {
                segments[segments.Count - 1].rotation = rotation;
            }

        }


    }
    
    void fixCorners2(Sprite sprite, Quaternion rotation, string orientation)
    {
        
        if (firstCorner2)
        {
            posEsquina2 = segments[1].position;
            firstCorner2 = false;
            
            
        }

        for (int i = 1; i < segments.Count - 1; i++)
        {
            if (segments[i].position == posEsquina2)
            {
                segments[i].GetComponent<SpriteRenderer>().sprite = sprite;
                segments[i].rotation = Quaternion.identity;
            }
            else if ((orientation == "down" && segments[i].position.y < posEsquina2.y) || (orientation == "up" && segments[i].position.y > posEsquina2.y))
            {

                segments[i].rotation = rotation;
                segments[i].GetComponent<SpriteRenderer>().sprite = normal;

            }
            if (segments[segments.Count - 1].position == posEsquina2)
            {
                segments[segments.Count - 1].rotation = rotation;
            }

        }
        
       
    }
    void fixOrientation(string orientation)
    {
        Quaternion rotation = Quaternion.identity;
        switch (orientation)
        {
            case "left":
                rotation = Quaternion.Euler(0, 0, 0);
                break;
            case "right":
                rotation = Quaternion.Euler(0, 0, 180);
                break;
            case "up":
                rotation = Quaternion.Euler(0, 0, -90f);
                break;
            case "down":
                rotation = Quaternion.Euler(0, 0, 90f);
                break;
        }
        segments[0].rotation = rotation;
        //segments[segments.Count -1].rotation = rotation;
        if (segments.Count == 2)
        {
            segments[1].rotation = rotation;
        }
        // for (int i = 0; i < segments.Count; i++)
        // {
        //     segments[i].rotation = rotation;
        // }

        if (orientation == "down" && lastOrientation == "left")
        {
            fixCorners1(leftDownCorner, rotation, orientation);
        }
        else if (orientation == "down" && lastOrientation == "right")
        {
            fixCorners1(rightDownCorner, rotation, orientation);
        }
        else if (orientation == "up" && lastOrientation == "left")
        {
            fixCorners1(leftUpCorner, rotation, orientation);
        }
        else if (orientation == "up" && lastOrientation == "right")
        {
            fixCorners1(rightUpCorner, rotation, orientation);
        }
        else
        {
            firstCorner1 = true;
        }

        if (orientation == "right" && lastOrientation == "down")
        {
            fixCorners2(leftUpCorner, rotation, orientation);
        }
        else if (orientation == "left" && lastOrientation == "down")
        {
            fixCorners2(rightUpCorner, rotation, orientation);
        }
        else if (orientation == "right" && lastOrientation == "up")
        {
            fixCorners2(leftDownCorner, rotation, orientation);
        }
        else if (orientation == "left" && lastOrientation == "up")
        {
            fixCorners2(rightDownCorner, rotation, orientation);
        }
        else
        {
            firstCorner2 = true;
        }



    }
    
    void createTail()
    {
        segments[segments.Count - 1].GetComponent<SpriteRenderer>().sprite = tail;
        segments[segments.Count - 1].tag = "Tail";
        segments[segments.Count - 1].GetComponent<SpriteRenderer>().sortingOrder = 1;
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
