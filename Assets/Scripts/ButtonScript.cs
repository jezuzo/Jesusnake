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

    private int indexModo;
    private int indexVelocidad;
    private int indexTablero;
    private int indexObstacles;
    private int indexColor;
    private int indexAjustes;
    private GameObject mainScreenGameObject;
    private GameObject optionsScreenGameObject;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainScreenGameObject = GameObject.FindGameObjectWithTag("MainScreen");
        optionsScreenGameObject = GameObject.FindGameObjectWithTag("OptionsScreen");
        
        spritesBotonModo = new List<Sprite>() { 
            GameAssets.gameAssets.apple,
            GameAssets.gameAssets.box,
            GameAssets.gameAssets.locker,
            GameAssets.gameAssets.highScoreArrows,
            GameAssets.gameAssets.randomHighScore
        };

        gameModes = new List<string>() { "NormalMode", "BoxMode", "ChainedApplesMode", "ArrowsMode", "RandomMode" };

        spritesBotonVelocidad = new List<Sprite>() {
            GameAssets.gameAssets.slowVelocity,
            GameAssets.gameAssets.mediumVelocity,
            GameAssets.gameAssets.highVelocity 
        };

        snakeSpeeds = new List<float>() { 0.15f, 0.125f, 0.1f };
        spritesBotonTablero = new List<string>() { "10x10", "15x15", "20x20" };
        tableSizes = new List<int>() {10,15,20 };

        spritesBotonObstacles = new List<Sprite>() { 
            GameAssets.gameAssets.noObstacles, 
            GameAssets.gameAssets.yesObstacles 
        };
        spritesBotonColor = new List<Sprite>() { 
            GameAssets.gameAssets.snakeBlue, 
            GameAssets.gameAssets.snakeRed, 
            GameAssets.gameAssets.snakeGreen, 
            GameAssets.gameAssets.snakePink 
        };
        
        indexModo = PlayerPrefs.GetInt("IndexModo");
        indexVelocidad = PlayerPrefs.GetInt("IndexVelocidad");
        indexTablero = PlayerPrefs.GetInt("IndexTablero");
        indexObstacles = PlayerPrefs.GetInt("Obstacles");
        indexColor = PlayerPrefs.GetInt("IndexColor");
        
        if(gameObject.name == "BotonModo")
        {
            transform.GetChild(1).gameObject.transform.GetComponent<Image>().sprite = spritesBotonModo[indexModo];
            PlayerPrefs.SetString("GameMode", gameModes[indexModo]);
        }
        else if(gameObject.name == "BotonVelocidad")
        {
            transform.GetChild(1).gameObject.transform.GetComponent<Image>().sprite = spritesBotonVelocidad[indexVelocidad];
            PlayerPrefs.SetFloat("SnakeSpeed", snakeSpeeds[indexVelocidad]);

        }else if(gameObject.name == "BotonTablero")
        {
            
            transform.GetChild(1).gameObject.transform.GetComponent<TMP_Text>().text = spritesBotonTablero[indexTablero] ;
            PlayerPrefs.SetInt("TableSize", tableSizes[indexTablero]);
        }
        else if(gameObject.name == "BotonObstaculos")
        {
            transform.GetChild(1).gameObject.transform.GetComponent<Image>().sprite = spritesBotonObstacles[indexObstacles];
            PlayerPrefs.SetInt("Obstacles",indexObstacles);
        }
        else if(gameObject.name == "BotonColor")
        {
           
            transform.GetChild(1).gameObject.transform.GetComponent<Image>().sprite = spritesBotonColor[indexColor];
            PlayerPrefs.SetInt("IndexColor",indexColor);
        }
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
        else if(gameObject.name == "BotonAtras")
        {
            optionsScreenGameObject.GetComponent<Animator>().SetBool("QuitOptionsScreen", false);
            mainScreenGameObject.GetComponent<Animator>().SetBool("QuitMainScreen", false);
        }
        else if(gameObject.name == "BotonCerrar")
        {
            PlayerPrefs.SetInt($"highScore{PlayerPrefs.GetString("GameMode")}", 0);
            Application.Quit();
        }
        else if(gameObject.name == "BotonModo")
        {
            indexModo++;
            if(indexModo == spritesBotonModo.Count)
            {
                indexModo = 0;
            }
            
            transform.GetChild(1).gameObject.transform.GetComponent<Image>().sprite = spritesBotonModo[indexModo];
            PlayerPrefs.SetInt("IndexModo",indexModo);
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
            PlayerPrefs.SetInt("IndexVelocidad",indexVelocidad);
            PlayerPrefs.SetFloat("SnakeSpeed", snakeSpeeds[indexVelocidad]);

        }else if(gameObject.name == "BotonTablero")
        {
            indexTablero++;
            if (indexTablero == spritesBotonTablero.Count)
            {
                indexTablero = 0;
            }
            transform.GetChild(1).gameObject.transform.GetComponent<TMP_Text>().text = spritesBotonTablero[indexTablero] ;
            PlayerPrefs.SetInt("IndexTablero",indexTablero);
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
            PlayerPrefs.SetInt("IndexColor",indexColor);
        }else if(gameObject.name == "BotonJugar2" )
        {
            SceneManager.LoadScene("GameScene");
        }else if (gameObject.name == "BotonReplay")
        {
            SceneManager.LoadScene("GameScene");
        }
        else if (gameObject.name == "BotonAjustes")
        {
            if (indexAjustes == 2)
            {
                indexAjustes = 0;
            }
            transform.parent.parent.gameObject.GetComponent<Animator>().SetBool("DeathScreen", indexAjustes == 0);
            indexAjustes++;
            
        }
    }
}
