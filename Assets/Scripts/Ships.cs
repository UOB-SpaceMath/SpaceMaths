using UnityEngine;


namespace SpaceMath
{
    [System.Serializable]
    public class Ships
    {
        [SerializeField]
        GameObject _shipObject;
        [SerializeField]
        private Vector2Int _cellIndex;

        GameBoardManager _gameBoardManager;

        // Must be initialized in the inspector for the player and the enemies respectively

        // Weapon relatives
        [SerializeField]
        private int attackDamage = 1;
        [SerializeField]
        private float weaponRange = 3.0f;
        //[SerializeField]
        //private Transform weaponEnd; // Where the weapon is.

        // Ship itself relatives
        [SerializeField]
        private int _energy = 100;
        [SerializeField]
        private int _health = 100;
        [SerializeField]
        private int _energyOpenShield = 5;
        [SerializeField]
        private int energyConsumption;

        private bool _isShieldsOn = false;

        public int AttackDamage { get => attackDamage; }

        public float WeaponRange { get => weaponRange; }

        //public Transform WeaponEnd { get => weaponEnd; }

        public int Health { get => _health; }

        public int Energy { get => _energy; }

        public bool ShieldsEnabled { get => _isShieldsOn; }

        public Vector2Int CellIndex { get => _cellIndex; set => _cellIndex = value; }

        public GameObject ShipObject { get => _shipObject; set => _shipObject = value; }

        public GameBoardManager GameBoardManager { get => _gameBoardManager; set => _gameBoardManager = value; }

        // Return value false means the player lose the game
        public void ApplyDamage(int amount)
        {
            if (_isShieldsOn)
            {
                DecreaseEnergy(10 * amount);
            }
            else
            {
                _health -= amount; 
            }
            if (IsShipDead())
            {
                _gameBoardManager.RemoveTargetShip(this);
            }
        }

        public void IncreaseHealth(int amount)
        {
            _health += amount;
        }

        public bool IsShipDead()
        {
            if (_health <= 0 || _energy <= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void DecreaseEnergy(int amount)
        {
            _energy -= amount;
        }

        public void ConsumeEnergyByTurn()
        {
            _energy -= energyConsumption;
        }

        public bool IsShipOutOfEnergy()
        {
            if (_energy <= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void IncreaseEnergy(int amount)
        {
            _energy += amount;
        }

        public bool OpenShields()
        {
            // Can only raise shield when the energy is enough and shield isn't on
            if (!_isShieldsOn && _energy > _energyOpenShield)
            {
                DecreaseEnergy(_energyOpenShield);
                _isShieldsOn = !_isShieldsOn;
                GameObject shield = GameObject.Find("Shield");
                shield.transform.localScale = new Vector3(0.46f, 0.24f, 0.15f);
                return true;
            }
            return false;
        }

        public void CloseShields()
        {
            _isShieldsOn = !_isShieldsOn;
            GameObject shield = GameObject.Find("Shield");
            shield.transform.localScale = new Vector3(0, 0, 0);
        }
    }
}

