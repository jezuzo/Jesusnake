using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;

public class ButtonScript : MonoBehaviour
{

    private List<Sprite> spritesBotonModo;
    private List<Sprite> spritesBotonVelocidad;
    private List<string> spritesBotonTablero;
    private List<Sprite> spritesBotonObstacles;
    private List<Sprite> spritesBotonColor;

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

        spritesBotonModo = new List<Sprite>() { 
            GameAssetsTitleScreen.gameAssetsTitleScreen.apple,
            GameAssetsTitleScreen.gameAssetsTitleScreen.box,
            GameAssetsTitleScreen.gameAssetsTitleScreen.locker,
            GameAssetsTitleScreen.gameAssetsTitleScreen.highScoreArrows,
            GameAssetsTitleScreen.gameAssetsTitleScreen.randomHighScore
        };

        gameModes = new List<string>() { "NormalMode", "BoxMode", "ChainedApplesMode", "ArrowsMode", "RandomMode" };

        spritesBotonVelocidad = new List<Sprite>() {
            GameAssetsTitleScreen.gameAssetsTitleScreen.slowVelocity,
            GameAssetsTitleScreen.gameAssetsTitleScreen.mediumVelocity,
            GameAssetsTitleScreen.gameAssetsTitleScreen.highVelocity 
        };

        snakeSpeeds = new List<float>() { 0.2f, 0.15f, 0.1f };

        spritesBotonTablero = new List<string>() { "10x10", "15x15", "20x20" };

        tableSizes = new List<int>() {10,15,20 };

        spritesBotonObstacles = new List<Sprite>() { 
            GameAssetsTitleScreen.gameAssetsTitleScreen.noObstacles, 
            GameAssetsTitleScreen.gameAssetsTitleScreen.yesObstacles 
        };
        spritesBotonColor = new List<Sprite>() { 
            GameAssetsTitleScreen.gameAssetsTitleScreen.snakeBlue, 
            GameAssetsTitleScreen.gameAssetsTitleScreen.snakeRed, 
            GameAssetsTitleScreen.gameAssetsTitleScreen.snakeGreen, 
            GameAssetsTitleScreen.gameAssetsTitleScreen.snakePink 
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
        if (gameObject.name == "BotonJugar")
        {
            mainScreenGameObject.GetComponent<Animator>().SetBool("QuitMainScreen", true);
            optionsScreenGameObject.GetComponent<Animator>().SetBool("QuitOptionsScreen", true);
            //Destroy(mainScreen.gameObject);

        }

        if(gameObject.name == "BotonAtras")
        {
           
            optionsScreenGameObject.GetComponent<Animator>().SetBool("QuitOptionsScreen", false);
            mainScreenGameObject.GetComponent<Animator>().SetBool("QuitMainScreen", false);
        }
        if(gameObject.name == "BotonCerrar")
        {
            Application.Quit();
        }
        

       
        if(gameObject.name == "BotonModo")
        {
            indexModo++;
            if(indexModo == spritesBotonModo.Count)
            {
                indexModo = 0;
            }
            
            transform.GetChild(1).gameObject.transform.GetComponent<Image>().sprite = spritesBotonModo[indexModo];
            
            PlayerPrefs.SetString("GameMode", gameModes[indexModo]);
        }
        else if(gameObject.name == "BotonVelocidad")
        {
            indexVelocidad++;
            if (indexVelocidad == spritesBotonVelocidad.Count )
            {
                indexVelocidad = 0;
            }

            transform.GetChild(1).gameObject.transform.GetComponent<Image>().sprite = spritesBotonVelocidad[indexVelocidad];
            
            PlayerPrefs.SetFloat("SnakeSpeed", snakeSpeeds[indexVelocidad]);

        }else if(gameObject.name == "BotonTablero")
        {
            indexTablero++;
            if (indexTablero == spritesBotonTablero.Count)
            {
                indexTablero = 0;
            }
            transform.GetChild(1).gameObject.transform.GetComponent<TMP_Text>().text = spritesBotonTablero[indexTablero] ;
            
            PlayerPrefs.SetInt("TableSize", tableSizes[indexTablero]);


        }
        else if(gameObject.name == "BotonObstaculos")
        {
            indexObstacles++;
            if (indexObstacles == spritesBotonObstacles.Count)
            {
                indexObstacles = 0;
            }
            transform.GetChild(1).gameObject.transform.GetComponent<Image>().sprite = spritesBotonObstacles[indexObstacles];
            PlayerPrefs.SetInt("Obstacles",indexObstacles);


        }
        else if(gameObject.name == "BotonColor")
        {
            indexColor++;
            if (indexColor == spritesBotonColor.Count)
            {
                indexColor = 0;
            }
            transform.GetChild(1).gameObject.transform.GetComponent<Image>().sprite = spritesBotonColor[indexColor];

        }else if(gameObject.name == "BotonJugar2")
        {
            
            SceneManager.LoadScene("GameScene");
        }
    }
}
