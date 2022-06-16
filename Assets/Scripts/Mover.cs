using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Mover : MonoBehaviour
{
    [SerializeField] float moveSpeed = 10;
    private HashSet<char> LockedDirections; // Record which directions are locked.
    private char currentDirection; // Record which direction just went.

    [SerializeField] private Tilemap groundTileMap;
    [SerializeField] private Tilemap collisionTileMap;

    //public List<Vector3> tileWorldLocations;

    // Start is called before the first frame update
    void Start()
    {        
        LockedDirections = new HashSet<char>();
        var bounds = collisionTileMap.cellBounds;
        Debug.Log(bounds);
        //tileLook();
    }

    // Update is called once per frame
    void Update()
    {
        //MovePlayer();        
    }
    

    //void MovePlayer()
    //{

    //    float xValue = Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed;
    //    float zValue = Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed;
    //    int x = (int)xValue;
    //    int z = (int)zValue;
    //    //if (isOnGround(xValue, zValue))
    //    //{
    //    //if (xValue != 0)
    //    //{
    //    //transform.Translate(0.001f, 0, z);
    //    transform.Translate(0.1f, 0.0f, 0.0f);

    //    //}

    //    //}
    //}

    //public void MoveLeft()
    //{
    //    if (!LockedDirections.Contains('l'))
    //    {
    //        transform.Translate(-0.1f, 0.0f, 0.0f);
    //        currentDirection = 'l';
    //        LockedDirections.Remove('r');
    //    }
    //}

    //public void MoveRight()
    //{
    //    if (!LockedDirections.Contains('r'))
    //    {
    //        transform.Translate(0.1f, 0.0f, 0.0f);
    //        currentDirection = 'r';
    //        LockedDirections.Remove('l');
    //    }
    //}

    //public void MoveUp()
    //{
    //    if (!LockedDirections.Contains('u'))
    //    {
    //        transform.Translate(0f, 0f, 0.1f);
    //        currentDirection = 'u';
    //        LockedDirections.Remove('d');
    //    }
    //}

    //public void MoveDown()
    //{
    //    if (!LockedDirections.Contains('d'))
    //    {
    //        transform.Translate(0f, 0f, -0.1f);
    //        currentDirection = 'd';
    //        LockedDirections.Remove('u');
    //    }
    //}

    public void MoveLeft()
    {       
        if (checkMove(new Vector3 (-1f, 0f, 0.0f)))
        {
            transform.position = transform.position + new Vector3(-1f, 0f, 0.0f);
        }
    }

    public void MoveRight()
    {
            
        if (checkMove(new Vector3(1f, 0f, 0.0f)))
        {
            transform.position = transform.position + new Vector3(1f, 0f, 0.0f);
        }
    }

    public void MoveUp()
    {        
            
        if (checkMove(new Vector3(0f, 0f, 1f)))
        {
            transform.position = transform.position + new Vector3(0f, 0f, 1f);
        }

    }

    public void MoveDown()
    {       
            
        if (checkMove(new Vector3(0f, 0f, -1f)))
        {
            transform.position = transform.position + new Vector3(0f, 0f, -1f);
        }
    }
   

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            LockedDirections.Add(currentDirection);
        }
    }

    private bool checkMove(Vector3 direction)
    {
        Vector3Int gridPosition = collisionTileMap.WorldToCell(transform.position + direction);

        //if (!groundTileMap.HasTile(gridPosition))
        //{
        //    Debug.Log("No ground tile");
        //    return false;
        //}
        if (collisionTileMap.HasTile(gridPosition))
        {
            Debug.Log("Col");
            return false;
        }
        return true;

    }
    

    //private void tileCheck()
    //{
    //    BoundsInt bounds = groundTileMap.cellBounds;
    //    TileBase[] allTiles = groundTileMap.GetTilesBlock(bounds);

    //    for (int x = 0; x < bounds.size.x; x++)
    //    {
    //        for (int y = 0; y < bounds.size.z; y++)
    //        {
    //            TileBase tile = allTiles[x + y * bounds.size.x];
    //            if (tile != null)
    //            {
    //                Debug.Log("x:" + x + " y:" + y + " tile:" + tile.name);
    //            }
    //            else
    //            {
    //                Debug.Log("x:" + x + " y:" + y + " tile: (null)");
    //            }
    //        }
    //    }
    //}

    //private void tileLook()
    //{
        //tileWorldLocations = new List<Vector3>();

        //foreach (var pos in groundTileMap.cellBounds.allPositionsWithin)
        //{
        //    Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
        //    Vector3 place = groundTileMap.CellToWorld(localPlace);
        //    if (groundTileMap.HasTile(localPlace))
        //    {
        //        tileWorldLocations.Add(place);
        //    }
        //}

        //foreach (Vector3 i in tileWorldLocations)
        //{
        //    Debug.Log(i);
        //}
        //Debug.Log(tileWorldLocations);


    //}

}
