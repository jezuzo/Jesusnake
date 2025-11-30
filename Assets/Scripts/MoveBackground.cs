using UnityEngine;

public class MoveBackground : MonoBehaviour
{

    public float speed = 5f;
    public float clampPos;
    private Vector3 startPos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float newPos = Mathf.Repeat(Time.time * speed, clampPos);
        transform.position = startPos + Vector3.left * newPos;
    }
}
