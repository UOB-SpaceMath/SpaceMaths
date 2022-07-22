using System.Collections;
using System.Collections.Generic;
using SpaceMath;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class LevelGenerator : MonoBehaviour
{
    // the player's ship
    public Ships PlayerShip;

    [NonReorderable]
    // the enemies' ships
    public List<Ships> EnemyShips;
}