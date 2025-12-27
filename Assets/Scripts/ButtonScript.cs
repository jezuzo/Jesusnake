using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

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
    private GameObject creditsScreenGameObject;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainScreenGameObject = GameObject.FindGameObjectWithTag("MainScreen");
        optionsScreenGameObject = GameObject.FindGameObjectWithTag("OptionsScreen");
        creditsScreenGameObject = GameObject.FindGameObjectWithTag("CreditsScreen");
        
        
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
        
        SetOptions();
    }

    private IEnumerator Wait(string name)
    {
        yield return new WaitForSeconds(0.15f);
        if (name == "BotonJugar")
        {
            mainScreenGameObject.GetComponent<Animator>().SetBool("QuitMainScreen", true);
            optionsScreenGameObject.GetComponent<Animator>().SetBool("QuitOptionsScreen", true);
        }else if (name == "BotonAtras")
        {
            optionsScreenGameObject.GetComponent<Animator>().SetBool("QuitOptionsScreen", false);
            mainScreenGameObject.GetComponent<Animator>().SetBool("QuitMainScreen", false);
        }else if (name == "BotonCerrar")
        {
            Application.Quit();
        }else if (name == "BotonJugar2" || name == "BotonReplay")
        {
            DontDestroyOnLoad(SoundManager.soundManager.gameObject);
            SceneManager.LoadScene("GameScene");
        }
        else if (name == "BotonAjustes")
        {
            yield return new WaitForSeconds(0.2f);
            gameObject.GetComponent<Button>().interactable = true;
        }else if (name == "GitHubButton")
        {
            yield return new WaitForSeconds(0.2f);
            Application.OpenURL("https://github.com/jezuzo/Jesusnake");
            
        }else if (name == "ItchioButton")
        {
            yield return new WaitForSeconds(0.2f);
            Application.OpenURL("https://github.com/jezuzo/Jesusnake");
            
        }else if (name == "CreditsButton")
        {
            yield return new WaitForSeconds(0.1f);
            creditsScreenGameObject.GetComponent<Animator>().SetBool("CreditsScreen", true);
            
        }else if (name == "BotonAtrasCredits")
        {
            yield return new WaitForSeconds(0.1f);
            creditsScreenGameObject.GetComponent<Animator>().SetBool("CreditsScreen", false);
            
        }
        
    }
    public void OnClick()
    {
        GetComponent<Animator>().SetTrigger("TriggerClick");
        SoundManager.soundManager.PlaySound(Audios.Click);
        if (gameObject.name == "BotonJugar")
        {

            StartCoroutine(Wait(gameObject.name));
            //Destroy(mainScreen.gameObject);
        }
        else if(gameObject.name == "BotonAtras")
        {
            StartCoroutine(Wait(gameObject.name));
            
        }
        else if(gameObject.name == "BotonCerrar")
        {
            StartCoroutine(Wait(gameObject.name));
            
        }
        else if(gameObject.name == "BotonModo")
        {
            indexModo = PlayerPrefs.GetInt("IndexModo");
            indexModo++;
            if(indexModo == spritesBotonModo.Count)
            {
                indexModo = 0;
            }
            Debug.Log(indexModo);
            transform.GetChild(1).gameObject.transform.GetComponent<Image>().sprite = spritesBotonModo[indexModo];
            PlayerPrefs.SetInt("IndexModo",indexModo);
            PlayerPrefs.SetString("GameMode", gameModes[indexModo]);
        }
        else if(gameObject.name == "BotonVelocidad")
        {
            indexVelocidad = PlayerPrefs.GetInt("IndexVelocidad");
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
            indexTablero = PlayerPrefs.GetInt("IndexTablero");
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
            indexObstacles = PlayerPrefs.GetInt("Obstacles");
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
            StartCoroutine(Wait(gameObject.name));
            
            
            
        }else if (gameObject.name == "BotonReplay")
        {
            StartCoroutine(Wait(gameObject.name));
            
        }
        else if (gameObject.name == "BotonAjustes")
        {
            gameObject.GetComponent<Button>().interactable = false;
            if (indexAjustes == 2)
            {
                indexAjustes = 0;
            }
            transform.parent.parent.gameObject.GetComponent<Animator>().SetBool("DeathScreen", indexAjustes == 0);
            indexAjustes++;
            StartCoroutine(Wait(gameObject.name));
            
        }else if (gameObject.name == "GitHubButton")
        {
            StartCoroutine(Wait(gameObject.name));
        }else if (gameObject.name == "ItchioButton")
        {
            StartCoroutine(Wait(gameObject.name));
        }else if (gameObject.name == "CreditsButton")
        {
            StartCoroutine(Wait(gameObject.name));
        }else if (gameObject.name == "BotonAtrasCredits")
        {
            StartCoroutine(Wait(gameObject.name));
        }else if (gameObject.name == "BotonRandom")
        {
            Debug.Log("boton random");
            SelectRandom();
        }

       
    }
    private void SelectRandom()
    {
        
        indexModo = Random.Range(0, spritesBotonModo.Count);
        indexVelocidad = Random.Range(0, spritesBotonVelocidad.Count);
        indexTablero = Random.Range(0, spritesBotonTablero.Count);
        indexObstacles = Random.Range(0, spritesBotonObstacles.Count);
        
        PlayerPrefs.SetInt("IndexModo",indexModo);
        PlayerPrefs.SetInt("IndexVelocidad",indexVelocidad);
        PlayerPrefs.SetInt("IndexTablero",indexTablero);
        PlayerPrefs.SetInt("Obstacles",indexObstacles);
        
        foreach (Transform boton in transform.parent.GetComponentsInChildren<Transform>())
        {
            if(boton.name == "BotonModo")
            {
                boton.GetChild(1).gameObject.transform.GetComponent<Image>().sprite = spritesBotonModo[indexModo];
                PlayerPrefs.SetString("GameMode", gameModes[indexModo]);
            
            }
            else if(boton.name == "BotonVelocidad")
            {
                boton.GetChild(1).gameObject.transform.GetComponent<Image>().sprite = spritesBotonVelocidad[indexVelocidad];
                PlayerPrefs.SetFloat("SnakeSpeed", snakeSpeeds[indexVelocidad]);

            }else if(boton.name == "BotonTablero")
            {
                boton.GetChild(1).gameObject.transform.GetComponent<TMP_Text>().text = spritesBotonTablero[indexTablero] ;
                PlayerPrefs.SetInt("TableSize", tableSizes[indexTablero]);
            }
            else if(boton.name == "BotonObstaculos")
            {
                boton.GetChild(1).gameObject.transform.GetComponent<Image>().sprite = spritesBotonObstacles[indexObstacles];
                PlayerPrefs.SetInt("Obstacles",indexObstacles);
            }
            else if(boton.name == "BotonColor")
            {
                boton.GetChild(1).gameObject.transform.GetComponent<Image>().sprite = spritesBotonColor[indexColor];
                PlayerPrefs.SetInt("IndexColor",indexColor);
            }
        }
        
    }

    private void SetOptions()
    {
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
}
