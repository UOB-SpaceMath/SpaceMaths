using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
    [SerializeField] float moveSpeed = 10;

    // Start is called before the first frame update
    void Start()
    {
        PrintInstructions();
    }

    // Update is called once per frame
    void Update()
    {
        //MovePlayer();        
    }

    void PrintInstructions()
    {
        //Debug.Log("Welcome to the game");
        //Debug.Log("Move your player with WASD or arrow keys\n Don't hit the walls!");
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

    public void MoveLeft()
    {
        transform.Translate(-0.1f, 0.0f, 0.0f);
    }

    public void MoveRight()
    {
        transform.Translate(0.1f, 0.0f, 0.0f);
    }

    public void MoveUp()
    {
        transform.Translate(0f, 0f, 0.1f);
    }

    public void MoveDown()
    {
        transform.Translate(0f, 0f, -0.1f);
    }

    bool isOnGround(float xVal, float zVal)
    {
        if ((xVal >= 0 && xVal <= 5) && (zVal >= 0 && zVal <= 5))
        {
            return true;
        }
        return false;
    }

}
