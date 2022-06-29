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

        private int _energy;
        private int _health;
        private bool _isShieldsOn;


        public int Health { get => _health; }

        public int Energy { get => _energy; }

        public bool ShieldsEnabled { get => _isShieldsOn; }

        public Vector2Int CellIndex { get => _cellIndex; set => _cellIndex = value; }

        public GameObject ShipObject { get => _shipObject; set => _shipObject = value; }

        public GameBoardManager GameBoardManager { get => _gameBoardManager; set => _gameBoardManager = value; }

        public void DecreaseHealth(int amount)
        {
            _health -= amount;
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
            if (_health < 0)
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

        public bool IsShipOutOfEnergy()
        {
            if (_energy < 0)
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

        public void ToggleShields()
        {
            _isShieldsOn = !_isShieldsOn;
        }
    }
}

