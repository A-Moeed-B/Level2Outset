using UnityEngine;
public class Node
{
    public bool isColored;
    public Vector3 worldPosition;
    public SpriteRenderer renderer;
    bool isClicked;
    public int gCost;
    public int hCost;
    public bool isGamePoint;
    Color color;
    public bool isHead;
    public int x;
    public int y;
    public Node parent;
    public Node(bool isColored, Vector3 worldPosition,SpriteRenderer renderer,bool isClicked,bool isGamePoint,bool isHead,int x, int y)
    {
        this.renderer = new SpriteRenderer();
        this.isColored = isColored;
        this.worldPosition = worldPosition;
        this.renderer = renderer;
        this.isClicked = isClicked;
        this.isGamePoint = isGamePoint;
        this.isHead = isHead;
        this.x = x;
        this.y = y;
        color = renderer.color;
    }
    public Node()
    {
        renderer = new SpriteRenderer();
        isColored = false;
        isClicked = false;
    }
    public Color getColor()
    {
        return color;
    }
    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }
    public void changeColor(SpriteRenderer renderer)
    {
        this.renderer = renderer;
    }
    public bool isSameColored(Color color)
    {
        return renderer.color.r == color.r && renderer.color.g == color.g && renderer.color.b == color.b && renderer.color.a == color.a;
    }
    public void setColor(Color color)
    {
        this.color = color;
    }

}