using UnityEngine;

public class TitleScreenController : MonoBehaviour
{
    private float timer;
    private GameObject mainScreenGameObject;
    private bool mainScreenAppear = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainScreenGameObject = GameObject.FindGameObjectWithTag("MainScreen");
        mainScreenGameObject.GetComponent<Animator>().SetBool("QuitMainScreen", true);

    }

    // Update is called once per frame
    void Update()
    {
        if (!mainScreenAppear)
        {
            timer += Time.deltaTime;
            if (timer > 2.5f)
            {
                mainScreenGameObject.GetComponent<Animator>().SetBool("QuitMainScreen", false);
                mainScreenAppear=true;

            }

        }
        
    }
}
