using SpaceMath;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float health;
    public float maxHealth;

    public Slider slider;
    public GameObject healthBarUI;    

    public GameBoardManager gameBoardManager;
   

    private void Awake()
    {
        gameBoardManager = GameObject.Find("GameBoard").GetComponent<GameBoardManager>();
    }

    private void Start()
    {
        health = gameBoardManager.GetPlayer().Health;
        maxHealth = gameBoardManager.GetPlayer().Health;        
        slider = GameObject.FindGameObjectWithTag("healthSlider").GetComponent<Slider>();
        healthBarUI.SetActive(true);
        slider.value = CalculateHealth();                
    }

    private void Update()
    {
        health = gameBoardManager.GetPlayer().Health;
        slider.value = CalculateHealth();
    }

    float CalculateHealth()
    {
        return health / maxHealth;
    }


}
