using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ships : MonoBehaviour
{
    [SerializeField]
    GameObject _gameBoard;
    public GameObject shipObject;
    public Vector2Int cellIndex;
    private int energyPoints;
    private int healthPoints;
    private bool shieldsOn;
    GameBoardManager _gameBoardManager;

    public int HealthPoints
    {
        get => healthPoints;
    }

    public int EnergyPoints
    {
        get => energyPoints;
    }

    public bool isShieldsOn
    {
        get => shieldsOn;
    }
    // Start is called before the first frame update
    void Start()
    {
        _gameBoardManager = _gameBoard.GetComponent<GameBoardManager>();
        energyPoints = 100;
        healthPoints = 100;
        shieldsOn = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void decreaseHealth()
    {
        healthPoints = healthPoints - 10;
        if (isShipDead())
        {
            _gameBoardManager.removeTargetShip(this);
            resetPlayerAfterDeath();
        }
    }

    public void increaseHealth()
    {
        healthPoints = healthPoints + 10;
    }

    public bool isShipDead()
    {
        if (healthPoints < 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void decreaseEnergy()
    {
        energyPoints = energyPoints - 10;

        if (isShipOutOfEnergy())
        {
            //TO-DO: interact with gameboard manager
        }
    }

    public bool isShipOutOfEnergy()
    {
        if (energyPoints < 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void increaseEnergy()
    {
        energyPoints = energyPoints + 10;
    }


    private void resetPlayerAfterDeath()
    {
        energyPoints = 100;
        healthPoints = 100;
        shieldsOn = false;
    }

    private void toggleShields()
    {
        shieldsOn = !shieldsOn;
    }
}
