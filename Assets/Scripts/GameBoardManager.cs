using SpaceMath;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameBoardManager : MonoBehaviour
{
    // The height of ship according to the game board.
    [SerializeField] private float _height;

    // bias to make ship in the middle of cell rather than corner.
    [SerializeField] private Vector2 _playerBias;

    [NonReorderable] [SerializeField] private List<GameObject> _levelPrefabs;

    // different types of cell
    public enum CellType
    {
        Ship,
        Wall,
        Empty
    };

    // the whole map information
    private CellType[,] _cells; // the whole map info

    private Vector3Int _tileBias;

    // the player's ship
    private Ships _playerShip;

    // the enemies' ships
    private List<Ships> _enemyShips;

    // the obstacle tile-map
    private Tilemap _invisibleTilemap;

    private void Awake()
    {
        ImportLevel();
        GetWallInfo();
        SetupShip();
    }

    private void ImportLevel()
    {
        // instantiate level
        var level = Instantiate(_levelPrefabs[GlobalInformation.CurrentLevelIndex], gameObject.transform, false);
        var levelInformation = level.GetComponent<LevelGenerator>();
        // get information
        _playerShip = levelInformation.PlayerShip;
        _enemyShips = levelInformation.EnemyShips;
        _invisibleTilemap = levelInformation.InvisibleTilemap;
    }

    public bool IsEnemiesRemain()
    {
        return _enemyShips.Count > 0;
    }

    // Return player ship
    public Ships GetPlayer()
    {
        return _playerShip;
    }

    // Return all enemy ships
    public List<Ships> GetEnemyShips()
    {
        return _enemyShips;
    }

    // Return all enemy ships in range
    public List<Ships> GetEnemiesInRange()
    {
        var result = new List<Ships>();
        foreach (var ship in _enemyShips)
            if (IsInRange(ship))
                result.Add(ship);

        return result;
    }

    // Check an enemy is within range of player (5x5)
    private bool IsInRange(Ships enemy)
    {
        var playerY = _playerShip.CellIndex.y;
        var playerX = _playerShip.CellIndex.x;
        if (enemy.CellIndex.y < playerY - 2 || enemy.CellIndex.y > playerY + 2) return false;
        if (enemy.CellIndex.x < playerX - 2 || enemy.CellIndex.x > playerX + 2) return false;
        return true;
    }

    private void InstantiateShip()
    {
        var gridTransform = _invisibleTilemap.GetComponentInParent<Grid>().transform;
        _playerShip.ShipObject = Instantiate(_playerShip.ShipObject, gridTransform);
        _playerShip.GameBoardManager = this;
        foreach (var ship in _enemyShips)
        {
            ship.ShipObject = Instantiate(ship.ShipObject, gridTransform);
            ship.GameBoardManager = this;
        }
    }

    // scan the whole map
    private void GetWallInfo()
    {
        // get map info
        _invisibleTilemap.CompressBounds();
        var obstacleBound = _invisibleTilemap.cellBounds;
        var xSize = obstacleBound.size.x - 2;
        var ySize = obstacleBound.size.y - 2;
        _cells = new CellType[xSize, ySize];
        _tileBias = obstacleBound.position + new Vector3Int(1, 1, 0);
        for (var x = 0; x < _cells.GetLength(0); x++)
        for (var y = 0; y < _cells.GetLength(1); y++)
            _cells[x, y] = _invisibleTilemap.HasTile(new Vector3Int(x, y, 0) + _tileBias)
                ? CellType.Wall
                : CellType.Empty;
    }

    // place ships to specific cells
    private void SetupShip()
    {
        InstantiateShip();
        // set player ship
        var isSetup = SetShip(_playerShip, _playerShip.CellIndex.x, _playerShip.CellIndex.y);
        // set enemy ship
        foreach (var ship in _enemyShips) isSetup = isSetup && SetShip(ship, ship.CellIndex.x, ship.CellIndex.y);
        if (!isSetup)
            throw new System.Exception("Fail to setup ship: the cell is not empty.");
    }

    // set ship to a cell
    // don't use this method to move the ship, use MoveShip()
    private bool SetShip(Ships ship, int x, int y)
    {
        if (IsEmpty(x, y))
        {
            ship.ShipObject.transform.localPosition = GetPosition(x, y);
            ship.CellIndex = new Vector2Int(x, y);
            _cells[x, y] = CellType.Ship;
            return true;
        }

        return false;
    }

    public bool MoveShip(Ships ship, int x, int y)
    {
        _cells[ship.CellIndex.x, ship.CellIndex.y] = CellType.Empty;
        return SetShip(ship, x, y);
    }

    // return the local position of a grid cell by it's index.
    private Vector3 GetPosition(int x, int y)
    {
        return new Vector3(x + _tileBias.x + _playerBias.x, _height, y + _tileBias.y + _playerBias.y);
    }

    public Vector2Int GetPlayerIndex()
    {
        return _playerShip.CellIndex;
    }

    public bool IsEmpty(int x, int y)
    {
        return _cells[x, y] == CellType.Empty;
    }

    public bool IsEmpty(Vector2Int index)
    {
        return _cells[index.x, index.y] == CellType.Empty;
    }

    public bool IsEnemy(int x, int y)
    {
        if (_playerShip.CellIndex.Equals(new Vector2Int(x, y)))
            return false;
        return _cells[x, y] == CellType.Ship;
    }

    public bool IsEnemy(Vector2Int index)
    {
        return IsEnemy(index.x, index.y);
    }

    public CellType GetCellType(int x, int y)
    {
        if (IsOutOfBound(x, y)) return CellType.Wall;

        return _cells[x, y];
    }

    private bool IsOutOfBound(int x, int y)
    {
        if (x < 0 || x >= _cells.GetLength(0) ||
            y < 0 || y >= _cells.GetLength(1))
            return true;
        return false;
    }


    public void RemoveTargetShip(Ships currentShip)
    {
        var _currentCellIndex = currentShip.CellIndex;
        var targetPosX = _currentCellIndex.x;
        var targetPosY = _currentCellIndex.y;

        // if the ship is not the player ship
        if (_currentCellIndex != _playerShip.CellIndex) _enemyShips.Remove(currentShip);
        _cells[targetPosX, targetPosY] = CellType.Empty;
        currentShip.ShipObject.SetActive(false);
    }

    // Get Ship from position, if no ship, returns null 
    public Ships GetShip(Vector2Int position)
    {
        var x = position.x;
        var y = position.y;

        if (_playerShip.CellIndex.x == x && _playerShip.CellIndex.y == y) return _playerShip;
        for (var i = 0; i < _enemyShips.Count; i++)
            if (_enemyShips[i].CellIndex.y == y && _enemyShips[i].CellIndex.x == x)
                return _enemyShips[i];
        return null;
    }

    public int GetLevelCount()
    {
        return _levelPrefabs.Count;
    }
}