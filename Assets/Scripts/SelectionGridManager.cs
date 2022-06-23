using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SelectionGridManager : MonoBehaviour
{

    [SerializeField] Tilemap selectionGrid;
    [SerializeField] GameObject gameBoard;

    public enum ActionType { None, Move, Attack };
    Vector2Int playerIndex;
    GameBoardManager.CellType[,] _cells;
    ActionType[,] _selectionCells;


    // Start is called before the first frame update



    void OnEnable()
    {
        var gameBoardManager = gameBoard.GetComponent<GameBoardManager>();
        _cells = gameBoardManager.Cells;
        _selectionCells = new ActionType[5, 5];
        playerIndex = gameBoardManager.PlayerShips.cellIndex;

        buildGrid();
    }
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {

    }

    void buildGrid()
    {
        // TODO: retrieve actual player position to determine 5x5 squares around player; at the moment player.position returns cell position

        Debug.Log(playerIndex.x);
        Debug.Log(playerIndex.y);
        int startX = playerIndex.x + 2;
        int startY = playerIndex.y - 2;
        int selectionX = 0;
        int selectionY = 0;


        for (int x = startX; x < startX - 5; x--)
        {
            for (int y = startY; y < startY + 5; y++)
            {

                if (x < 0 || y < 0 || x >= _cells.GetLength(0) || y >= _cells.GetLength(1) ||
                _cells[x, y] == GameBoardManager.CellType.Wall)
                {
                    _selectionCells[x, y] = ActionType.None;
                }
                else if (_cells[x, y] == GameBoardManager.CellType.Empty)
                {
                    _selectionCells[selectionX, selectionY] = ActionType.Move;
                }
                else if (_cells[x, y] == GameBoardManager.CellType.Ship)
                {
                    _selectionCells[selectionX, selectionY] = ActionType.Attack;
                }

                selectionY++;
            }
            selectionX++;
        }



        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                Debug.Log("Cell type"+ _selectionCells[i, j]);
            }
        }

    }

    // private Vector2Int GetSelectionIndex(Vector2Int playerIndex, Vector2Int cellIndex)
    // {

    // }

    // Set the colour of a tile.    
    private void SetTileColour(Color colour, Vector3Int position, Tilemap tilemap)
    {
        // Flag the tile, inidicating that it can change colour.
        // By default it's set to "Lock Colour".
        tilemap.SetTileFlags(position, TileFlags.None);

        // Set the colour.
        tilemap.SetColor(position, colour);
    }
}




