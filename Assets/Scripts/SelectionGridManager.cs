using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SelectionGridManager : MonoBehaviour
{
    [SerializeField] Ships player;
    [SerializeField] Tilemap selectionGrid;
    // [SerializeField] Tilemap obstacleMap;

    GameBoardManager gameBoardManager = new GameBoardManager();

    // enum CellType { Ship, Wall, Empty };
    // CellType[,] _cells;
    Vector2Int playerPosition;

    // Start is called before the first frame update
    void Start()
    {

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
        
      

        Vector3 WorldPos = player.shipObject.transform.position;
        float WorldPositionX = WorldPos.x;
        float WorldPositionY = WorldPos.y;

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

        Vector3 localPos = player.shipObject.transform.localPosition;
        int LocalPosX = (int)localPos.x;
        int LocalPosZ = (int)localPos.z;
        Debug.Log("local pos x"+ LocalPosX);
        int [,] obstacleArray  = new int[5,5];

         for(int z = LocalPosZ - 2; z < LocalPosZ + 2; z++){
            for(int x = LocalPosX -2; x < LocalPosX + 2; x++){
                //issue with line 65
              if(obstacleMap.HasTile(new Vector3Int(x, 0, z))){

                obstacleArray[z + 2, x + 2] = 1;
              }else{
                obstacleArray[z + 2, x + 2] = 0;
              }
            }
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


