using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpaceMath;

public class Scores : MonoBehaviour
{
    [SerializeField] private GameManager _gm;
    [SerializeField] private GameBoardManager _gbm;
    [SerializeField] private int scoreIncrement;
    private Ships player;
    private int score;

    // Start is called before the first frame update
    void Start()
    {
        player = _gbm.GetPlayer();
        Debug.Log(player);
    }

    // Update is called once per frame
    void Update()
    {
        checkAnswer();        
        checkScore();
    }

    private void checkAnswer()
    {

        if (_gm.isCorrectAnswer())
        {
            score += scoreIncrement;
            Debug.Log("Score: " + score);
        }
    }    

    private void checkScore()
    {

        if (_gm.isGameOver() && score + player.Energy > PlayerPrefs.GetInt("highScore"))
        {
            PlayerPrefs.SetInt("highScore", score + player.Energy);
        }

    }

    public int getScore()
    {
        return score;
    }



}
