using UnityEngine;
using UnityEngine.Tilemaps;
public class GameBoardManager : MonoBehaviour
{
    [SerializeField]
    float _height;

    [SerializeField]
    Ships _playerShip;
    public Ships PlayerShips
    {
        get => _playerShip;
    }

    [NonReorderable]
    [SerializeField]
    Ships[] _enermyShips;
    public Ships[] EnermyShips
    {
        get => _enermyShips;
    }

    enum CellType { Ship, Wall, Empty };
    CellType[,] _cells;
    static readonly Vector3 _PLAYER_BIAS = new Vector3(0.5f, 0.5f, 0);
    Vector3Int _tileBias;

    // Start is called before the first frame update
    void Start()
    {
        GetWallInfo();
        SetupShip();
    }

    void GetWallInfo()
    {
        // get map info
        var obstacleMap = transform.Find("Grid/Obstacle Tilemap").GetComponent<Tilemap>();
        obstacleMap.CompressBounds();
        var obstacleBound = obstacleMap.cellBounds;
        var xSize = obstacleBound.size.x - 2;
        var ySize = obstacleBound.size.y - 2;
        _cells = new CellType[xSize, ySize];
        _tileBias = obstacleBound.position + new Vector3Int(1, 1, 0);
        for (int x = 0; x < _cells.GetLength(0); x++)
        {
            for (int y = 0; y < _cells.GetLength(1); y++)
            {
                _cells[x, y] = obstacleMap.HasTile(new Vector3Int(x, y, 0) + _tileBias) ?
                    CellType.Wall : CellType.Empty;
            }
        }
    }

    void SetupShip()
    {
        bool isSetup = true;
        // set player ship
        isSetup = isSetup && SetShip(_playerShip, _playerShip.position.x, _playerShip.position.y);
        // set enemy ship
        foreach (Ships ship in _enermyShips)
        {
            isSetup = isSetup && SetShip(ship, ship.position.x, ship.position.y);
        }
        if (!isSetup)
            throw new System.Exception("Fail to setup ship: the cell is not empty.");
    }

    bool SetShip(Ships ship, int x, int y)
    {
        if (IsEmpty(x, y))
        {
            ship.shipObject.transform.localPosition = GetPosision(x, y);
            ship.position.x = x;
            ship.position.y = y;
            _cells[x, y] = CellType.Ship;
            return true;
        }
        else
            return false;
    }

    Vector3 GetPosision(int x, int y)
    {
        return new Vector3(x + _tileBias.x + _PLAYER_BIAS.x, _height, -(y + _tileBias.y + _PLAYER_BIAS.y));
    }

    bool IsEmpty(int x, int y)
    {
        return _cells[x, y] == CellType.Empty;
    }
}

[System.Serializable]
public class Ships
{
    public GameObject shipObject;
    public Vector2Int position;
}
