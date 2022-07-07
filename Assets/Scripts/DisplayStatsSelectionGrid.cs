using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayStatsSelectionGrid : MonoBehaviour
{
    public Text healthDisplay;
    public Text energyDisplay;
    public float energy;
    public float health;

    public GameBoardManager gameBoardManager;

    private void Awake()
    {
        gameBoardManager = GameObject.Find("GameBoard").GetComponent<GameBoardManager>();
    }
    

    // Update is called once per frame
    void Update()
    {

        healthDisplay.text = HealthTextToDisplay();
        healthDisplay.color = Color.green;
        energyDisplay.text = EnergyTextToDisplay();
        energyDisplay.color = Color.blue;
    }

    private string HealthTextToDisplay()    {        
        
        health = gameBoardManager.GetPlayer().Health;
        string result = "Health: " + health.ToString();
        return result;
    }

    private string EnergyTextToDisplay()
    {

        energy = gameBoardManager.GetPlayer().Energy;        
        string result = "Energy: " + energy.ToString();
        return result;
    }

}
