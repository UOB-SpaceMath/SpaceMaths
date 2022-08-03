using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

namespace SpaceMath
{
    // https://en.wikipedia.org/wiki/A*_search_algorithm
    public class AStarPathFinder
    {
        private readonly GameBoardManager _gbm;

        public AStarPathFinder(GameBoardManager gbm)
        {
            _gbm = gbm;
        }

        // find path from node "from" to node "to"
        public Stack<Vector2Int> FindPath(Vector2Int from, Vector2Int to)
        {
            // the nodes to be calculate.
            var openSet = new HashSet<Vector2Int>();

            // maps to record gValue and fValue.
            // gValue indicate the distance from current node to starting node.
            // hValue is the heuristic distance from current node to ending node.
            // fValue = gValue + hValue
            var mapSize = _gbm.GetMapSize();
            var gMap = new int[mapSize.x, mapSize.y];
            var fMap = new int[mapSize.x, mapSize.y];

            // each element indicate that where is its parent node.
            var cameFormDict = new Dictionary<Vector2Int, Vector2Int>();
            // set default as maximum 
            for (var i = 0; i < mapSize.x * mapSize.y; i++) gMap[i % mapSize.x, i / mapSize.x] = int.MaxValue;
            for (var i = 0; i < mapSize.x * mapSize.y; i++) fMap[i % mapSize.x, i / mapSize.x] = int.MaxValue;

            // first open node is where it from.
            gMap[from.x, from.y] = 0;
            fMap[from.x, from.y] = GetHeuristic(from, to);
            openSet.Add(from);

            // if there is still a open node, keep finding.
            while (openSet.Count != 0)
            {
                // get the node with minimum fValue.
                var current = GetNodeWithMinimalF(openSet, fMap);
                if (current.Equals(to))
                    return GetResultFromCameFromDict(cameFormDict, current);

                openSet.Remove(current);
                
                foreach (var neighbor in GetNeighbors(current))
                {
                    var gValue = gMap[current.x, current.y] + 1;
                    // if true, this path to this neighbor is better than previous one. Record it.
                    if (gValue < gMap[neighbor.x, neighbor.y])
                    {
                        
                        cameFormDict.Add(neighbor, current);
                        gMap[neighbor.x, neighbor.y] = gValue;
                        fMap[neighbor.x, neighbor.y] = gValue + GetHeuristic(neighbor, to);
                        
                        // it's already on open set, ignore it.
                        if (!openSet.Contains(neighbor))
                            openSet.Add(neighbor);
                    }
                }
            }

            // empty stack means fail
            return new Stack<Vector2Int>();
        }

        // iterating to find the node with minimal fValue
        private Vector2Int GetNodeWithMinimalF(HashSet<Vector2Int> nodes, int[,] fMap)
        {
            var minimum = nodes.First();
            foreach (var node in nodes.Where(node => fMap[node.x, node.y] < fMap[minimum.x, minimum.y]))
                minimum = node;
            return minimum;
        }

        // get the path result in form of stack, the end node is in the bottom.
        private Stack<Vector2Int> GetResultFromCameFromDict(Dictionary<Vector2Int, Vector2Int> cameFromDict,
            Vector2Int end)
        {
            var result = new Stack<Vector2Int>();
            var current = end;
            while (cameFromDict.ContainsKey(current))
            {
                result.Push(current);
                current = cameFromDict[current];
            }

            return result;
        }

        // get available neighbors around center 
        private List<Vector2Int> GetNeighbors(Vector2Int center)
        {
            var neighbors = new List<Vector2Int>();
            var left = new Vector2Int(center.x - 1, center.y);
            var right = new Vector2Int(center.x + 1, center.y);
            var up = new Vector2Int(center.x, center.y - 1);
            var down = new Vector2Int(center.x, center.y + 1);

            if (_gbm.IsEmpty(left))
                neighbors.Add(left);
            if (_gbm.IsEmpty(right))
                neighbors.Add(right);
            if (_gbm.IsEmpty(up))
                neighbors.Add(up);
            if (_gbm.IsEmpty(down))
                neighbors.Add(down);

            return neighbors;
        }
        
        // the heuristic function.
        // Use Manhattan Distance here.
        private static int GetHeuristic(Vector2Int from, Vector2Int to)
        {
            return Math.Abs(from.x - to.x) + Math.Abs(from.y - to.y);
        }
    }
}