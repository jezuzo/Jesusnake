using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;

public class ButtonScript : MonoBehaviour
{

    private List<Sprite> spritesBotónModo;
    private List<Sprite> spritesBotónVelocidad;
    private List<string> spritesBotónTablero;
    private List<Sprite> spritesBotónObstacles;
    private List<Sprite> spritesBotónColor;

    private List<float> snakeSpeeds;
    private List<int> tableSizes;
    private List<string> gameModes;

    private int indexModo = 0;
    private int indexVelocidad = 0;
    private int indexTablero = 0;
    private int indexObstacles = 0;
    private int indexColor = 0;
    private GameObject mainScreenGameObject;
    private GameObject optionsScreenGameObject;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainScreenGameObject = GameObject.FindGameObjectWithTag("MainScreen");
        optionsScreenGameObject = GameObject.FindGameObjectWithTag("OptionsScreen");

        spritesBotónModo = new List<Sprite>() { 
            GameAssets.gameAssets.apple,
            GameAssets.gameAssets.box,
            GameAssets.gameAssets.locker,
            GameAssets.gameAssets.highScoreArrows,
            GameAssets.gameAssets.RandomHighScore
        };

        gameModes = new List<string>() { "NormalMode", "BoxMode", "ChainedApplesMode", "ArrowsMode", "RandomMode" };

        spritesBotónVelocidad = new List<Sprite>() {
            GameAssets.gameAssets.slowVelocity,
            GameAssets.gameAssets.mediumVelocity,
            GameAssets.gameAssets.highVelocity 
        };

        snakeSpeeds = new List<float>() { 0.2f, 0.15f, 0.1f };

        spritesBotónTablero = new List<string>() { "10x10", "15x15", "20x20" };

        tableSizes = new List<int>() {10,15,20 };

        spritesBotónObstacles = new List<Sprite>() { 
            GameAssets.gameAssets.noObstacles, 
            GameAssets.gameAssets.yesObstacles 
        };
        spritesBotónColor = new List<Sprite>() { 
            GameAssets.gameAssets.snakeBlue, 
            GameAssets.gameAssets.snakeRed, 
            GameAssets.gameAssets.snakeGreen, 
            GameAssets.gameAssets.snakePink 
        };

        PlayerPrefs.SetString("GameMode", gameModes[indexModo]);
        PlayerPrefs.SetFloat("SnakeSpeed", snakeSpeeds[indexVelocidad]);
        PlayerPrefs.SetInt("TableSize", tableSizes[indexTablero]);
        PlayerPrefs.SetInt("Obstacles",indexObstacles);

    }

    // Update is called once per frame
    void Update()
    {
        
        
    }

    public void OnClick()
    {
        GetComponent<Animator>().SetTrigger("TriggerClick");
        if (gameObject.name == "BotónJugar")
        {
            mainScreenGameObject.GetComponent<Animator>().SetBool("QuitMainScreen",true);
            
            optionsScreenGameObject.GetComponent<Animator>().SetBool("QuitOptionsScreen", true);
            //Destroy(mainScreen.gameObject);

        }

        if(gameObject.name == "BotónAtrás")
        {
           
            optionsScreenGameObject.GetComponent<Animator>().SetBool("QuitOptionsScreen", false);
            mainScreenGameObject.GetComponent<Animator>().SetBool("QuitMainScreen", false);
        }
        if(gameObject.name == "BotónCerrar")
        {
            Application.Quit();
        }
        

       
        if(gameObject.name == "BotónModo")
        {
            indexModo++;
            if(indexModo == spritesBotónModo.Count)
            {
                indexModo = 0;
            }
            
            transform.GetChild(1).gameObject.transform.GetComponent<Image>().sprite = spritesBotónModo[indexModo];
            
            PlayerPrefs.SetString("GameMode", gameModes[indexModo]);
        }
        else if(gameObject.name == "BotónVelocidad")
        {
            indexVelocidad++;
            if (indexVelocidad == spritesBotónVelocidad.Count )
            {
                indexVelocidad = 0;
            }

            transform.GetChild(1).gameObject.transform.GetComponent<Image>().sprite = spritesBotónVelocidad[indexVelocidad];
            
            PlayerPrefs.SetFloat("SnakeSpeed", snakeSpeeds[indexVelocidad]);

        }else if(gameObject.name == "BotónTablero")
        {
            indexTablero++;
            if (indexTablero == spritesBotónTablero.Count)
            {
                indexTablero = 0;
            }
            transform.GetChild(1).gameObject.transform.GetComponent<TMP_Text>().text = spritesBotónTablero[indexTablero] ;
            
            PlayerPrefs.SetInt("TableSize", tableSizes[indexTablero]);


        }
        else if(gameObject.name == "BotónObstáculos")
        {
            indexObstacles++;
            if (indexObstacles == spritesBotónObstacles.Count)
            {
                indexObstacles = 0;
            }
            transform.GetChild(1).gameObject.transform.GetComponent<Image>().sprite = spritesBotónObstacles[indexObstacles];
            PlayerPrefs.SetInt("Obstacles",indexObstacles);


        }
        else if(gameObject.name == "BotónColor")
        {
            indexColor++;
            if (indexColor == spritesBotónColor.Count)
            {
                indexColor = 0;
            }
            transform.GetChild(1).gameObject.transform.GetComponent<Image>().sprite = spritesBotónColor[indexColor];

        }else if(gameObject.name == "BotónJugar2")
        {
            
            SceneManager.LoadScene("GameScene");
        }
    }
}
