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
        Color healthColor = new Color(0.3f, 1.2f, 0.6f, 1);
        Color energyColor = new Color(0.2f, 1f, 0.9f, 1.2f);
        
        healthDisplay.text = HealthTextToDisplay();
        healthDisplay.color = healthColor;
        energyDisplay.text = EnergyTextToDisplay();
        energyDisplay.color = energyColor;
    }

    private string HealthTextToDisplay()
    {

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
