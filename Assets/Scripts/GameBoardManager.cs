using UnityEngine;
using UnityEngine.Tilemaps;
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
    Ships[] _enemyShips;
    public Ships[] EnemyShips
    {
        get => _enemyShips;
    }

    // different types of cell
    public enum CellType { Ship, Wall, Empty };

    // the whole map information
    CellType[,] _cells; // the whole map info
    public CellType[,] Cells
    {
        get => _cells;
    }


    Vector3Int _tileBias;

    // Start is called before the first frame update
    void Start()
    {
        GetWallInfo();
        SetupShip();
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
            ship.shipObject.transform.localPosition = GetPosision(x, y);
            ship.cellIndex.x = x;
            ship.cellIndex.y = y;
            _cells[x, y] = CellType.Ship;
            return true;
        }
        else
            return false;
    }

    bool MoveShip(Ships ship, int x, int y)
    {
        _cells[ship.cellIndex.x, ship.cellIndex.y] = CellType.Empty;
        return SetShip(ship, x, y);
    }

    Vector3 GetPosision(int x, int y)
    {
        return new Vector3(x + _tileBias.x + _playerBias.x, _height, -(y + _tileBias.y + _playerBias.y));
    }

    Vector2Int GetPlayerIndex()
    {
        return _playerShip.cellIndex;
    }

    bool IsEmpty(int x, int y)
    {
        return _cells[x, y] == CellType.Empty;
    }
}

// class for player and enemy 
[System.Serializable]
public class Ships
{
    public GameObject shipObject;
    public Vector2Int cellIndex;
}
