using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager scoreManager;
    private int score;
    private int highScore;
    private void Start()
    {
        scoreManager = this;
        if(PlayerPrefs.GetInt("FirstTime", 0) == 0)
        {

            PlayerPrefs.SetInt("highScore", 0);

            // Guardamos para que no vuelva a pasar
            PlayerPrefs.SetInt("FirstTime", 1);
            PlayerPrefs.Save();
        }
        
        highScore = PlayerPrefs.GetInt("highScore", 0);
        PlayerPrefs.Save();
        GameAssets.gameAssets.highScoreText.text = highScore + "";

    }

    public void addScore(int plus)
    {
        score += plus;
        GameAssets.gameAssets.scoreText.text = score + "";
    }
    public void setScore(int newScore)
    {
        score = newScore;
        GameAssets.gameAssets.scoreText.text = score + "";
    }
    public void setHighScore(int newHighScore)
    {
        highScore = newHighScore;
        PlayerPrefs.SetInt("highScore", highScore);
        PlayerPrefs.Save();
        GameAssets.gameAssets.highScoreText.text = highScore + "";
    }

    void Update()
    {
        if (score > highScore)
        {
            setHighScore(score);
        }
    }
}
