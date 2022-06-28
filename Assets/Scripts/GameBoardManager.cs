using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
public class GameBoardManager : MonoBehaviour
{
    // The height of ship according to the game board.
    [SerializeField]
    float _height;

    // the obstacle tile-map
    [SerializeField]
    Tilemap _obstacleMap;

    // bias to make ship in the middle of cell rather than corner.
    [SerializeField]
    Vector2 _playerBias;

    // the player's ship
    [SerializeField]
    Ships _playerShip;
    public Ships PlayerShips
    {
        get => _playerShip;
    }

    // the enemies' ships
    [NonReorderable]
    [SerializeField]

    List<Ships> _enemyShips;

    // public Ships[] EnemyShips
    // {
    //     get => _enemyShips;
    // }


    public List<Ships> EnemyShips
    {
        get => _enemyShips;
    }

    // different types of cell
    public enum CellType { Ship, Wall, Empty };

    // the whole map information
    CellType[,] _cells; // the whole map info


    Vector3Int _tileBias;

    void Awake()
    {
        GetWallInfo();
        SetupShip();
    }

    private void InstantiateShip()
    {
        var gridTransform = _obstacleMap.GetComponentInParent<Grid>().transform;
        _playerShip.shipObject = Instantiate(_playerShip.shipObject, gridTransform);
        foreach (var ship in _enemyShips)
        {
            ship.shipObject = Instantiate(ship.shipObject, gridTransform);
        }
    }

    // scan the whole map
    void GetWallInfo()
    {
        // get map info
        _obstacleMap.CompressBounds();
        var obstacleBound = _obstacleMap.cellBounds;
        var xSize = obstacleBound.size.x - 2;
        var ySize = obstacleBound.size.y - 2;
        _cells = new CellType[xSize, ySize];
        _tileBias = obstacleBound.position + new Vector3Int(1, 1, 0);
        for (int x = 0; x < _cells.GetLength(0); x++)
        {
            for (int y = 0; y < _cells.GetLength(1); y++)
            {
                _cells[x, y] = _obstacleMap.HasTile(new Vector3Int(x, y, 0) + _tileBias) ?
                    CellType.Wall : CellType.Empty;
            }
        }
    }



    // place ships to specific cells
    void SetupShip()
    {
        InstantiateShip();
        bool isSetup = true;
        // set player ship
        isSetup = isSetup && SetShip(_playerShip, _playerShip.cellIndex.x, _playerShip.cellIndex.y);
        // set enemy ship
        foreach (Ships ship in _enemyShips)
        {
            isSetup = isSetup && SetShip(ship, ship.cellIndex.x, ship.cellIndex.y);
        }
        if (!isSetup)
            throw new System.Exception("Fail to setup ship: the cell is not empty.");
    }

    // set ship to a cell
    // don't use this method to move the ship, use MoveShip()
    bool SetShip(Ships ship, int x, int y)
    {
        if (IsEmpty(x, y))
        {
            ship.shipObject.transform.localPosition = GetPosition(x, y);
            ship.cellIndex.x = x;
            ship.cellIndex.y = y;
            _cells[x, y] = CellType.Ship;
            return true;
        }
        else
            return false;
    }

    public bool MoveShip(Ships ship, int x, int y)
    {
        _cells[ship.cellIndex.x, ship.cellIndex.y] = CellType.Empty;
        return SetShip(ship, x, y);
    }

    // return the local position of a grid cell by it's index.
    Vector3 GetPosition(int x, int y)
    {
        return new Vector3(x + _tileBias.x + _playerBias.x, _height, y + _tileBias.y + _playerBias.y);
    }

    public Vector2Int GetPlayerIndex()
    {
        return _playerShip.cellIndex;
    }

    bool IsEmpty(int x, int y)
    {
        return _cells[x, y] == CellType.Empty;
    }

    public CellType getCellType(int x, int y)
    {
        if (isOutOfBound(x, y))
        {
            return CellType.Wall;
        }

        return _cells[x, y];
    }

    bool isOutOfBound(int x, int y)
    {
        if (x < 0 || x >= _cells.GetLength(0) ||
            y < 0 || y >= _cells.GetLength(1))
        {
            return true;
        }
        return false;
    }


    public void removeTargetShip(Ships currentShip)
    {
        GameObject _currentShipObject = currentShip.shipObject;
        Vector2Int _currentCellIndex = currentShip.cellIndex;
        int targetPosX = _currentCellIndex.x;
        int targetPosY = _currentCellIndex.y;

        // if the ship is the playership
        if (_currentCellIndex == _playerShip.cellIndex)
        {
            disablePlayerOnGameBoard();
            _cells[targetPosX, targetPosY] = CellType.Empty;
        }
        else
        {
            for (int i = 0; i < _enemyShips.Count; i++)
            {
                if (_enemyShips[i].cellIndex.Equals(_currentCellIndex))
                {
                    _enemyShips.Remove(currentShip);
                    _cells[targetPosX, targetPosY] = CellType.Empty;
                }
            }
        }
    }

    void disablePlayerOnGameBoard()
    {
        _playerShip.shipObject.SetActive(false);
    }


}



// class for player and enemy 
[System.Serializable]
public class Ships
{
    public GameObject shipObject;
    public Vector2Int cellIndex;
}
