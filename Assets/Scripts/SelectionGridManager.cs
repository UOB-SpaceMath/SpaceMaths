using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SelectionGridManager : MonoBehaviour
{
    
    [SerializeField] Tilemap selectionGrid;
    [SerializeField] GameObject gameBoard;
    
    Vector2Int playerIndex;
    GameBoardManager.CellType[,] _cells;
    GameBoardManager.CellType[,] _selectionCells;

    // Start is called before the first frame update
    void Start()
    {
        var gameBoardManager =  gameBoard.GetComponent<GameBoardManager>();
        _cells = gameBoardManager.Cells;
        _selectionCells = new GameBoardManager.CellType[5,5];
        playerIndex = gameBoardManager.PlayerShips.cellIndex;

        buildGrid();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void buildGrid()
    {
        // TODO: retrieve actual player position to determine 5x5 squares around player; at the moment player.position returns cell position

       // gameBoardManager.GetWallInfo();
        
      

        // Vector3 WorldPos = player.shipObject.transform.position;
        // float WorldPositionX = WorldPos.x;
        // float WorldPositionY = WorldPos.y;

        // Debug.Log("world Position" + obstacleMap.transform.position);
        // Debug.Log("local position" + obstacleMap.transform.localPosition);

        // Own SelectionGrid
        // Offset: x-coordinate from -6 to -2; y-coordinate no offset
        // for (int y =0; y <= 4; y++)
        // {
        //     for (int x = -6; x <= -2; x++)
        //     {
        //         SetTileColour(Color.green, new Vector3Int(x, y, 0), selectionGrid);
        //     }
        // }

        // Vector3 localPos = player.shipObject.transform.localPosition;
   
        
        int localPosX = playerIndex.x;
        int localPosY = playerIndex.y;
        int startX = playerIndex.x - 2;
        int startY = playerIndex.y - 2;   
]
         for(int x = 0; x < 5; x++){
            for(int y = 0; y < 5; y++){
        
            int currentX = startX + x;
            int currentY = startY + y;

            if(currentX < 0 || currentY < 0 || currentX > _selectionCells.GetLength(0) || )
            // if(_cells[x,y] == GameBoardManager.CellType.Empty){

            // }else if()
                
           




            //     obstacleArray[y + 2, y + 2] = 1;
            //   }else{
            //     obstacleArray[y + 2, y + 2] = 0;
            //   }
            
         }

         Debug.Log(obstacleArray.GetLength(1));

         for(int j = 0; j< obstacleArray.GetLength(0); j++){
            for(int k = 0; k < obstacleArray.GetLength(1); k++){
                Debug.Log(j);
                Debug.Log(k);
               if(obstacleArray[j,k] ==0){
                // SetTileColour()
                
               }else{

               }
            }
         }

        // Player position is centred on our Selection Grid
        Vector3Int playerPositionInV3 = new Vector3Int(-4, 2, 0);       
        SetTileColour(Color.red, playerPositionInV3, selectionGrid);
    }



    Vector2Int GetSelectionIndexFromPlayerIndex (Vector2Int playerIndex){
        Vector2Int result = new Vector2Int(0, 0);
        result.y = playerIndex.y - playerIndex.y - 2;
        result.x = playerIndex.x - playerIndex.x + 2;
        return result;
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


