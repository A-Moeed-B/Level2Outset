using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePoint
{
    public Node startingNode;
    public Node endingNode;
    public GamePoint()
    {
        startingNode = new Node();
        endingNode = new Node();
    }
}
