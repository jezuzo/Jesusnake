using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    public GameObject apple;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void spawnFood()
    {
        float randomx = Random.Range(-8.49f, 8.49f);
        float randomy = Random.Range(-4.49f,4.49f);
        Instantiate(apple, new Vector3(Mathf.Round(randomx), Mathf.Round(randomy), 0), Quaternion.identity);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameObject.FindGameObjectWithTag("Food") == null)
        {
            spawnFood();
        }
        
    }
}
