using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SpaceMath;

public class Scores : MonoBehaviour
{
    
    [SerializeField] private GameBoardManager _gbm;    
    [SerializeField] private int scoreIncrement = 10;
    [SerializeField] private GameObject highScoreScreen;

    public Text HS;
    public Text currentScore;

    //private Ships player;
    private int score;
    private string currentLevel;
       

    // Start is called before the first frame update
    void Start()
    {
        currentLevel = _gbm.GetCurrentLevel();
        Debug.Log(currentLevel);
    }

    // Update is called once per frame
    void Update()
    {
        // Display current score and high score
        HS.text = "High Score: " + getHighScore();
        currentScore.text = "Current Score: " + getScore();
    }

    // Checks if player's score at end of game is higher than high score, taking into account remaining energy
    // High score saved to PlayerPrefs by key-value pair currentLevel: score
    // 10pts per correct answer + remaining energy
    public void checkScore(int remainingEnergy)
    {

        if (score + remainingEnergy > PlayerPrefs.GetInt(currentLevel))
        {
            PlayerPrefs.SetInt(currentLevel, score + remainingEnergy);
            highScoreScreen.SetActive(true);
        }

    }

    public void incrementScore()
    {
        score += scoreIncrement;
    }    

    public int getScore()
    {
        return score;
    }

    // Returns high score for currentLevel
    public int getHighScore()
    {
        return PlayerPrefs.GetInt(currentLevel);
    }


}
