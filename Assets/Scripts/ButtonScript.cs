using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ButtonScript : MonoBehaviour
{
    private List<Sprite> spritesBotónModo;
    private int indexModo = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spritesBotónModo = new List<Sprite>();
        spritesBotónModo.Add(GameAssets.gameAssets.box);
        spritesBotónModo.Add(GameAssets.gameAssets.locker);
        spritesBotónModo.Add(GameAssets.gameAssets.highScoreArrows);
        spritesBotónModo.Add(GameAssets.gameAssets.RandomHighScore);
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }

    public void OnClick()
    {
        GetComponent<Animator>().SetTrigger("TriggerClick");
        if(gameObject.name == "BotónModo")
        {
            indexModo++;
            if(indexModo == spritesBotónModo.Count - 1)
            {
                indexModo = 0;
            }
           
            transform.GetChild(1).gameObject.transform.GetComponent<Image>().sprite = spritesBotónModo[indexModo];
        }
    }
}
