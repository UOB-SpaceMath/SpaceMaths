using SpaceMath;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class GameBoardManager : MonoBehaviour
{
    // The height of ship according to the game board.
    [SerializeField] float _height;

    // the obstacle tile-map
    [SerializeField] Tilemap _obstacleMap;

    // bias to make ship in the middle of cell rather than corner.
    [SerializeField] Vector2 _playerBias;

    // the player's ship
    [SerializeField] Ships _playerShip;    

    // the enemies' ships
    [NonReorderable] [SerializeField] List<Ships> _enemyShips;    

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

    // Return player ship
    public Ships GetPlayer
    {
        get => _playerShip;
    }

    // Return all enemy ships
    public List<Ships> GetEnemyShips
    {
        get => _enemyShips;
    }

    // Return all enemy ships in range
    public List<Ships> GetEnemiesInRange()
    {
        List<Ships> result = new List<Ships>();
        for (int i = 0; i < _enemyShips.Count; i++)
        {
            if (isInRange(_enemyShips[i]))
            {
                result.Add(_enemyShips[i]);
            }
        }
        return result;
    }

    // Check an enemy is within range of player (5x5)
    private bool isInRange(Ships enemy)
    {
        int playerY = _playerShip.CellIndex.y;
        int playerX = _playerShip.CellIndex.x;
        if (enemy.CellIndex.y < playerY - 2 || enemy.CellIndex.y > playerY + 2)
        {
            return false;
        }
        if (enemy.CellIndex.x < playerX - 2 || enemy.CellIndex.x > playerX + 2)
        {
            return false;
        }
        return true;
    }

    private void InstantiateShip()
    {
        var gridTransform = _obstacleMap.GetComponentInParent<Grid>().transform;
        _playerShip.ShipObject = Instantiate(_playerShip.ShipObject, gridTransform);
        _playerShip.GameBoardManager = this;
        foreach (var ship in _enemyShips)
        {
            ship.ShipObject = Instantiate(ship.ShipObject, gridTransform);
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
        isSetup = isSetup && SetShip(_playerShip, _playerShip.CellIndex.x, _playerShip.CellIndex.y);
        // set enemy ship
        foreach (Ships ship in _enemyShips)
        {
            isSetup = isSetup && SetShip(ship, ship.CellIndex.x, ship.CellIndex.y);
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
            ship.ShipObject.transform.localPosition = GetPosition(x, y);
            ship.CellIndex = new Vector2Int(x, y);
            _cells[x, y] = CellType.Ship;
            return true;
        }
        else
            return false;
    }

    public bool MoveShip(Ships ship, int x, int y)
    {
        _cells[ship.CellIndex.x, ship.CellIndex.y] = CellType.Empty;
        return SetShip(ship, x, y);
    }

    // return the local position of a grid cell by it's index.
    Vector3 GetPosition(int x, int y)
    {
        return new Vector3(x + _tileBias.x + _playerBias.x, _height, y + _tileBias.y + _playerBias.y);
    }

    public Vector2Int GetPlayerIndex()
    {
        return _playerShip.CellIndex;
    }

    bool IsEmpty(int x, int y)
    {
        return _cells[x, y] == CellType.Empty;
    }

    public CellType GetCellType(int x, int y)
    {
        if (IsOutOfBound(x, y))
        {
            return CellType.Wall;
        }

        return _cells[x, y];
    }

    bool IsOutOfBound(int x, int y)
    {
        if (x < 0 || x >= _cells.GetLength(0) ||
            y < 0 || y >= _cells.GetLength(1))
        {
            return true;
        }
        return false;
    }


    public void RemoveTargetShip(Ships currentShip)
    {
        Vector2Int _currentCellIndex = currentShip.CellIndex;
        int targetPosX = _currentCellIndex.x;
        int targetPosY = _currentCellIndex.y;

        // if the ship is not the playership
        if (_currentCellIndex != _playerShip.CellIndex)
        {
            _enemyShips.Remove(currentShip);
        }
        _cells[targetPosX, targetPosY] = CellType.Empty;
        currentShip.ShipObject.SetActive(false);
    }

    // Get Ship from position, if no ship, returns null 
    public Ships GetShip(Vector2Int position)
    {
        int x = position.x;
        int y = position.y;
        //if (_cells[x,y] == CellType.Empty || _cells[x,y] == CellType.Wall)
        //{
        //    return null;
        //}
        if (_playerShip.CellIndex.x == x && _playerShip.CellIndex.y == y)
        {
            return _playerShip;
        }
        for (int i=0; i < _enemyShips.Count; i++)
        {
            if (_enemyShips[i].CellIndex.y == y && _enemyShips[i].CellIndex.x == x)
            {
                return _enemyShips[i];
            }
        }
        return null;
    }
    
}
