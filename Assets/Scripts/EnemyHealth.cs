using SpaceMath;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public float health;
    public float maxHealth;

    public Slider slider;
    public GameObject healthBarUI;

    public GameBoardManager gameBoardManager;

    Ships currentEnemy;


    private void Awake()
    {
        gameBoardManager = GameObject.Find("GameBoard").GetComponent<GameBoardManager>();
    }

    private void Start()
    {
        List<Ships> enemies = gameBoardManager.GetEnemyShips();
        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i].ShipObject == this.gameObject)
            {
                currentEnemy = enemies[i];
            }
        }

        health = currentEnemy.Health;
        maxHealth = currentEnemy.Health;
        slider = this.gameObject.GetComponentInChildren<Slider>();
        healthBarUI.SetActive(true);
        slider.value = CalculateHealth();
    }

    private void Update()
    {
        health = currentEnemy.Health;
        slider.value = CalculateHealth();
    }

    float CalculateHealth()
    {
        return health / maxHealth;
    }


}
