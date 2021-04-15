using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    GridGenerator grid;
    public static bool resultFound;
    public static bool[] resultsFound;

    private void Awake()
    {
        resultFound = false;
        grid = GetComponent<GridGenerator>();
        resultsFound = new bool[grid.noOfColors];
        for (int i = 0; i < grid.noOfColors; i++)
        {
            resultsFound[i] = false;
        }

    }
    public void Update()
    {
      
    }
    public void findPath()
    {
        if (!resultFound)
        {
            grid.nodes[grid.startPositionX, grid.startPositionY].isColored = false;
            grid.nodes[grid.endPositionX, grid.endPositionY].isColored = false;
            findPath(grid.nodes[grid.startPositionX, grid.startPositionY], grid.nodes[grid.endPositionX, grid.endPositionY]);
        }
    }
    public void findPath(Node startNode, Node endNode)
    {

        List<Node> openList = new List<Node>();
        HashSet<Node> closedList = new HashSet<Node>();
        openList.Add(startNode);
        while (openList.Count > 0)
        {
            Node currentNode = openList[0];
            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].fCost < currentNode.fCost || openList[i].fCost == currentNode.fCost && openList[i].hCost < currentNode.hCost)
                {
                    currentNode = openList[i];

                }
            }
            openList.Remove(currentNode);
            closedList.Add(currentNode);
            if (currentNode == endNode)
            {
                Debug.Log("Result Found!");
                retracePath(startNode, endNode);
                resultFound = true;
                return;
            }
            Debug.Log("Before the FOREACH LOOP");
            foreach (Node neighbour in grid.generateNeighbours(currentNode))
            {
                //||!grid.isAdjacent(neighbour.x,neighbour.y)
                Debug.Log("In Foreach loop");
                if (neighbour.isColored || closedList.Contains(neighbour)) 
                {
                    Debug.Log("Blockage Found at " +neighbour.x+" "+neighbour.y);
                    continue;
                }
                int movementCostToNeighbour = currentNode.gCost + getDistance(currentNode, neighbour);
                
               
                if ((movementCostToNeighbour < neighbour.gCost || !openList.Contains(neighbour)))
                {
                    neighbour.gCost = movementCostToNeighbour;
                    neighbour.hCost = getDistance(neighbour, endNode);
                 
                    neighbour.parent = currentNode;
                    //if ((neighbour.x == grid.startPositionX && neighbour.y == grid.startPositionY) ||( neighbour.x == grid.endPositionX || neighbour.y == grid.endPositionY)
                    //    continue;
                    if (!openList.Contains(neighbour))
                        openList.Add(neighbour);
                }
                Debug.Log("In Foreach loop");

            }
        } 
    }
    void retracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;
        Debug.Log("In retrace path");
        while (currentNode != startNode)
        {
            Debug.Log("In retrace path while");
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        grid.path = path;
        Debug.Log("Out of Retrace Path While");

    }
    int getDistance(Node startNode, Node endNode)
    {

        int distX = Mathf.Abs(startNode.x - endNode.x);
        int distY = Mathf.Abs(startNode.y - endNode.y);
        //if (isDiagonal(startNode, endNode))
        //    return 14000000 * distX + 10 * (distY - distX);
        if (distX > distY)
            return 10 * distY + 10 * (distX - distY);
        return 14* distX + 10 * (distY - distX);
    }
    //bool isDiagonal(Node startNode,  Node endNode)
    //{
    //    if (startNode.x + 1 == endNode.x && startNode.y + 1 == endNode.y)
    //        return true;
    //    else if (startNode.x - 1 == endNode.x && startNode.y - 1 == endNode.y)
    //        return true;
    //    else if (startNode.x + 1 == endNode.x && startNode.y - 1 == endNode.y)
    //        return true;
    //    else if (startNode.x - 1 == endNode.x && startNode.y + 1 == endNode.y)
    //        return true;
    //    return false;
    //}
}
