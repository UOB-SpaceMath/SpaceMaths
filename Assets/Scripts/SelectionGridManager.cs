using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SelectionGridManager : MonoBehaviour
{
    [SerializeField] Ships player;
    [SerializeField] Tilemap selectionGrid;
    [SerializeField] Tilemap obstacleMap;

    enum CellType { Ship, Wall, Empty };
    CellType[,] _cells;
    Vector2Int playerPosition;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(player.position);
        buildGrid();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void buildGrid()
    {
        // TODO: retrieve actual player position to determine 5x5 squares around player; at the moment player.position returns cell position
        Vector2Int pos = player.position;

        //for (int y=pos.y-2; y <= pos.y+2; y++)
        //{
        //    for (int x=pos.x-2; x <= pos.x+2; x++)
        //    {
        //        //if (obstacleMap.HasTile(new Vector3Int(x, y, 0))){
        //        //    SetTileColour(Color.red, new Vector3Int(x, y, 0), selectionGrid);
        //        //}
        //        SetTileColour(Color.green, new Vector3Int(x, y, 0), selectionGrid);
        //    }
        //}

        // Own SelectionGrid
        // Offset: x-coordinate from -6 to -2; y-coordinate no offset
        for (int y =0; y <= 4; y++)
        {
            for (int x = -6; x <= -2; x++)
            {

                SetTileColour(Color.green, new Vector3Int(x, y, 0), selectionGrid);
            }
        }

        // Player position is centred on our Selection Grid
        Vector3Int playerPositionInV3 = new Vector3Int(-4, 2, 0);       
        SetTileColour(Color.red, playerPositionInV3, selectionGrid);
    }

    
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


