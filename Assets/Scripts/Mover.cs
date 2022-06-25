using UnityEngine;
using UnityEngine.Tilemaps;

public class Mover : MonoBehaviour
{

    [SerializeField] private Tilemap collisionTileMap;

    // Start is called before the first frame update
    void Start()
    {
        collisionTileMap.CompressBounds();
        collisionTileMap.GetComponent<TilemapRenderer>().enabled = false;  // Makes obstacle tilemap layer invisible
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Z position is offset by 2 
    public void MoveLeft()
    {
        if (checkMove(new Vector3(-1f, 0f, 2f)))
        {
            transform.position = transform.position + new Vector3(-1f, 0f, 0.0f);
        }
    }

    public void MoveRight()
    {

        if (checkMove(new Vector3(1f, 0f, 2f)))
        {
            transform.position = transform.position + new Vector3(1f, 0f, 0.0f);
        }
    }

    public void MoveUp()
    {

        if (checkMove(new Vector3(0f, 1f, 2f)))
        {
            transform.position = transform.position + new Vector3(0f, 1f, 0f);
        }

    }

    public void MoveDown()
    {

        if (checkMove(new Vector3(0f, -1f, 2f)))
        {
            transform.position = transform.position + new Vector3(0f, -1f, 0f);
        }
    }


    // Detect obstacle tilemap tiles
    private bool checkMove(Vector3 direction)
    {
        //Debug.Log(transform.position);        
        Vector3Int gridPosition = collisionTileMap.WorldToCell(transform.position + direction);

        if (collisionTileMap.HasTile(gridPosition))
        {
            Debug.Log("Cannot move to obstacle tile");
            return false;
        }
        return true;

    }


}
