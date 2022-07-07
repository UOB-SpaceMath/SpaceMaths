using SpaceMath;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerEnergy : MonoBehaviour
{
    public float energy;
    public float maxEnergy;

    public Slider slider;
    public GameObject energyBarUI;

    public GameBoardManager gameBoardManager;


    private void Awake()
    {
        gameBoardManager = GameObject.Find("GameBoard").GetComponent<GameBoardManager>();
    }

    private void Start()
    {
        energy = gameBoardManager.GetPlayer().Energy;
        maxEnergy = gameBoardManager.GetPlayer().Energy;
        slider = GameObject.FindGameObjectWithTag("energySlider").GetComponent<Slider>();
        energyBarUI.SetActive(true);
        slider.value = CalculateEnergy();
    }

    private void Update()
    {
        energy = gameBoardManager.GetPlayer().Energy;
        slider.value = CalculateEnergy();
    }

    float CalculateEnergy()
    {
        return energy / maxEnergy;
    }


}
