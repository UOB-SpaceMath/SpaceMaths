using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SpaceMath;

public class DisplayHighScore : MonoBehaviour
{
    public Text HS;
    public Text currentScore;
    public Scores scores;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HS.text = "High Score: " + PlayerPrefs.GetInt("highScore");
        currentScore.text = "Current Score: " + scores.getScore();
    }
}
