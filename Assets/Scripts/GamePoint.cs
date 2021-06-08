using System.Collections.Generic;
public class GamePoint
{
    public Node startingNode;
    public Node endingNode;
    public List<Node> path;
    public GamePoint()
    {
        startingNode = new Node();
        endingNode = new Node();
        path = new List<Node>();
    }
}
