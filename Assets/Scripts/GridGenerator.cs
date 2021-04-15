using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GridGenerator : MonoBehaviour
{
    
    [SerializeField]
    private int rows = 5;
    [SerializeField]
    private int columns = 5;
    public GameObject referenceTile;
    SpriteRenderer renderer;
    [SerializeField]
    private float tileSize = 1;
    public GameObject[,] tiles;
    public Node[,] nodes;
    public List<Node> path;
    int i, j;
    [HideInInspector]
    public int startPositionX, startPositionY;
    [HideInInspector]
    public int endPositionX, endPositionY;
   
    public int noOfColors;
    Color []colors;
    Dictionary<Color, GamePoint> gamePointDict;

    private void Awake()
    {
        //noOfColors = (Random.Range(rows - 1, rows + 1));
        Debug.Log("No of Colors:" + noOfColors);
        gamePointDict = new Dictionary<Color, GamePoint>();
        noOfColors = 5;
        colors = new Color[]{Color.red,Color.green,Color.yellow,Color.blue,Color.cyan };
        tiles = new GameObject[rows, columns];
        nodes = new Node[rows, columns];
        generateGrid();
        generateAllStartingPoints();
     
        path = new List<Node>();
    }
    void generateGamePointColors()
    {
        Debug.Log("In Generate GamePoint Colors");
        GamePoint[] gpArray = new GamePoint[noOfColors];
        gpArray = checkGamePoint();
        for (int a=0;a<noOfColors;a++)
        {
            gamePointDict.Add(colors[a], gpArray[a]);
        }
        GamePoint testpoint;
        gamePointDict.TryGetValue(Color.red, out testpoint);
        startPositionX = testpoint.startingNode.x;
        startPositionY = testpoint.startingNode.y;
        endPositionX = testpoint.endingNode.x;
        endPositionY = testpoint.endingNode.y;
        
        for (int a=0;a<noOfColors;a++)
        {
            GamePoint point;
            gamePointDict.TryGetValue(colors[a], out point);
            Debug.Log("Color: " + colors[a].ToString() + "Starting Node X:" + point.startingNode.x +"Y:"+ point.startingNode.y);
            Debug.Log("Color: " + colors[a].ToString() + "Ending Node X " + point.endingNode.x +"Y:"+ point.endingNode.y);
        }
    }
    
 
    void generateAllStartingPoints()
    {
        generateGamePointColors();
        for (int a = 0; a < noOfColors; a++)
        {
            GamePoint point;
            gamePointDict.TryGetValue(colors[a], out point);
            generateGamePoint(point.startingNode.x, point.startingNode.y, point.endingNode.x, point.endingNode.y,colors[a]);
        }
    }
    void generateGamePoint(int startX, int startY, int endX, int endY,Color color)
    {
        
        changeTileColor(startX, startY,color);
        changeTileColor(endX, endY,color);
        nodes[startX, startY].isGamePoint = true;
        nodes[endX, endY].isGamePoint = true;
        nodes[startX, startY].isColored = true;
        nodes[endX, endY].isColored = true;
        setHead(startX, startY);
        setHead(endX, endY);
        i = startX;
        j = startY;
    }
    void generateGamePoint(ref int startX,ref int startY, ref int endX,ref int endY )
    {
        setRandomNumbers(ref startX, ref startY, ref endX, ref endY);
        changeTileColor(startX, startY);
        changeTileColor(endX, endY);
        nodes[startX, startY].isGamePoint = true;
        nodes[endX, endY].isGamePoint = true;
        nodes[startX, startY].isColored = true;
        nodes[endX, endY].isColored = true;
        setHead(startX, startY);
        setHead(endX, endY);
        i = startX;
        j = startY;
    }
   
    //stop this much overloading??
    public void move(int x, int y)
    {

        if (isAdjacent(x, y))
        {
            Debug.Log("This is adjacent");
            nodes[x, y].renderer.color = Color.red;
            nodes[x, y].isColored = true;
        }

    }
    bool isRightColored(int x,int y)
    {
        Debug.Log("X: " + x + "Y" + y);
        if (x == rows - 1)
            return false;
        if (nodes[x+1,y].isColored&& nodes[x + 1, y].isHead)
        {
            nodes[i, j].isHead = false;
            return true;
        }
            return false;
    }
    bool isLeftColored(int x, int y)
    {
      
        if (x == 0)
            return false;
        if (nodes[x - 1, y].isColored&&nodes[x-1,y].isHead)
        {
            nodes[i, j].isHead = false;
            return true;
        }
        return false;
    }
    bool isAboveColored(int x, int y)
    {
        if (y == 0)
            return false;
        if (nodes[x, y-1].isColored&&nodes[x,y-1].isHead)
        {
            nodes[i, j].isHead = false;
            return true;
        }
        return false;
    }
    bool isBelowColored(int x, int y)
    {
        if (y == rows - 1)
            return false;
        if (nodes[x, y+1].isColored&&nodes[x,y+1].isHead)
        {
            nodes[i, j].isHead = false;
            return true;
        }
        return false;
    }
    //bool isRightColored(int x, int y,Color color)
    //{
    //    Debug.Log("X: " + x + "Y" + y);
    //    if (x == rows - 1)
    //        return false;
    //    if (nodes[x + 1, y].isColored && nodes[x + 1, y].isHead)
    //    {
    //        nodes[i, j].isHead = false;
    //        return true;
    //    }
    //    return false;
    //}
    //bool isLeftColored(int x, int y,Color color)
    //{

    //    if (x == 0)
    //        return false;
    //    if (nodes[x - 1, y].isColored && nodes[x - 1, y].isHead)
    //    {
    //        nodes[i, j].isHead = false;
    //        return true;
    //    }
    //    return false;
    //}
    //bool isAboveColored(int x, int y,Color color)
    //{
    //    if (y == 0)
    //        return false;
    //    if (nodes[x, y - 1].isColored && nodes[x, y - 1].isHead)
    //    {
    //        nodes[i, j].isHead = false;
    //        return true;
    //    }
    //    return false;
    //}
    //bool isBelowColored(int x, int y,Color color)
    //{
    //    if (y == rows - 1)
    //        return false;
    //    if (nodes[x, y + 1].isColored && nodes[x, y + 1].isHead)
    //    {
    //        nodes[i, j].isHead = false;
    //        return true;
    //    }
    //    return false;
    //}
    //public bool isAdjacent(int x,int y)
    //{
    //    if (x == 0&&y==0)
    //    {
    //        if (isBelowColored(x, y))
    //            return true;
    //       return isRightColored(x, y);
    //    }
    //    else if (x==0)
    //    {
    //        if (isAboveColored(x, y))
    //            return true;
    //        else if (isBelowColored(x, y))
    //            return true;
    //        return isRightColored(x, y);
    //    }
    //    else if (y==0)
    //    {
    //        if (isLeftColored(x, y))
    //            return true;
    //        else if (isBelowColored(x, y))
    //            return true;
    //        return isRightColored(x, y);
    //    }
    //    else if (y==rows-1)
    //    {
    //        if (x == columns - 1)
    //        {
    //            if (isLeftColored(x, y))
    //                return true;
    //        }

    //        else if (x >= 0)
    //        {
    //            if (isRightColored(x, y))
    //                return true;
    //        }

    //        return isAboveColored(x, y);
    //    }
    //    else
    //    {
    //        if (isLeftColored(x, y))
    //            return true;
    //        else if (isBelowColored(x, y))
    //            return true;
    //        else if (isAboveColored(x, y))
    //            return true;
    //        return isRightColored(x, y);
    //    }
    //}
    //public bool isAdjacent(int x, int y,Color color)
    //{
    //    if (isLeftColored(x, y))
    //        return true;
    //    else if (isBelowColored(x, y))
    //        return true;
    //    else if (isAboveColored(x, y))
    //        return true;
    //    return isRightColored(x, y);
    //}
    public bool isAdjacent(int x, int y)
    {
        if (isLeftColored(x, y))
            return true;
        else if (isBelowColored(x, y))
            return true;
        else if (isAboveColored(x, y))
            return true;
        return isRightColored(x, y);
    }
    public void regenerateValues()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    // Update is called once per frame
    void Update()
    {
        //move();
        if(Input.GetMouseButtonDown(0))
        {
            
            //Debug.Log("Mouse is down");
            Vector3 mouseInput = Input.mousePosition;
            Vector3 worldPosition = gameObject.transform.InverseTransformPoint(Camera.main.ScreenToWorldPoint(mouseInput));
            //Debug.Log("Original value x "+worldPosition.x);
            //Debug.Log("Original value y "+worldPosition.y);
            Debug.Log("Previous X: " + i + "Previous Y: " + j);
            Debug.Log("Rounded value x " + Mathf.RoundToInt(worldPosition.x));
            Debug.Log("Rounded Value y " + Mathf.RoundToInt(worldPosition.y));
            
            //i,j is the previous index.
            if (nodes[i, j].isHead)//if previous clicked is the head
            {
                Debug.Log("Current head is " + nodes[i, j].isHead);
                move(Mathf.RoundToInt(worldPosition.x), -(Mathf.RoundToInt(worldPosition.y)));
                //then set previous head to false;
                i = Mathf.RoundToInt(worldPosition.x);
                j = -(Mathf.RoundToInt(worldPosition.y));//take current clicked value
                setHead(i, j);//set current clicked value as the head.
                Debug.Log("New head is " + nodes[i, j].isHead);
                
            }
            //nodes[Mathf.RoundToInt(worldPosition.x), Mathf.RoundToInt(worldPosition.y)].isHead = false;
         
        }
        if (PathFinding.resultFound)
        {
            for (int a = 0; a < path.Count; a++)
                path[a].renderer.color = Color.red;
            nodes[startPositionX, startPositionY].isColored = true;
            nodes[endPositionX, endPositionY].isColored = true;
        }
    }
  
   
    void generateGrid()
    {

        for (int currentRow = 0; currentRow < rows; currentRow++)
        {
            for (int currentColumn = 0; currentColumn < columns; currentColumn++)
            {
                tiles[currentRow, currentColumn] = Instantiate(referenceTile, transform);
                float posX = currentRow * tileSize;
                float posY = currentColumn * -tileSize;
                tiles[currentRow, currentColumn].transform.position = new Vector2(posX, posY);
                nodes[currentRow, currentColumn] = new Node(false, tiles[currentRow, currentColumn].transform.position, tiles[currentRow, currentColumn].GetComponent<SpriteRenderer>(), false, false,false,currentRow,currentColumn);
            }
        }
       
        transform.position = new Vector2(-(columns * tileSize) / 2 + tileSize / 2, (rows + tileSize) / 2 - tileSize / 2);
    }
    //helper functions
    void changeTileColor(int x, int y)
    {
        renderer = tiles[x, y].GetComponent<SpriteRenderer>();
        nodes[x, y].isColored = true;
        renderer.color = Color.red;
    }
    void changeTileColor(int x, int y, Color color)
    {
        renderer = tiles[x, y].GetComponent<SpriteRenderer>();
        nodes[x, y].isColored = true;
        renderer.color = color;
    }
    void setHead(int x, int y)
    {
        if (nodes[x, y].isHead)
            nodes[x, y].isHead = false;
        else
            nodes[x, y].isHead = true;
    }
    public Node nodeFromPosition(Vector3 position)
    {
        int x = Mathf.RoundToInt(position.x / tileSize);
        int y = Mathf.RoundToInt(-(position.y) / tileSize);
        Debug.Log("Nodes[" + x + "," + y + "]");
        return nodes[x, y];

    }
    public List<Node> generateNeighbours(Node node)
    {
        Debug.Log("In Generate Neighbours");

        List<Node> neighbours = new List<Node>();

        for (int a = -1; a <= 1; a++)
        {
            for (int b = -1; b <= 1; b++)
            {
                if (a == 0 && b == 0)
                    continue;
                int checkX = node.x + a;
                int checkY = node.y + b;
                if (checkX == node.x + 1 && checkY == node.y + 1)
                    continue;
                else if (node.x - 1 == checkX && node.y - 1 == checkY)
                    continue;
                else if (node.x + 1 == checkX && node.y - 1 == checkY)
                    continue;
                else if (node.x - 1 == checkX && node.y + 1 == checkY)
                    continue;
      

                if (checkX>=0&&checkX<rows&&checkY>=0&&checkY<columns)
                {
                    //if ((checkX == startPositionX && checkY == startPositionY) || (checkX == endPositionX && checkY == endPositionY))
                    //    continue;
                    neighbours.Add(nodes[checkX, checkY]);
                }
            }
           
        }
        return neighbours;
    }
    GamePoint[] checkGamePoint()
    {
        GamePoint[] gamePoints = new GamePoint[noOfColors];
        for (int a = 0; a < noOfColors; a++)
        {
            GamePoint gamepoint = new GamePoint();
            setRandomNumbers(ref gamepoint.startingNode.x, ref gamepoint.startingNode.y, ref gamepoint.endingNode.x, ref gamepoint.endingNode.y);
            for (int b = 0; b < a; b++)
            {
                if (compareGamePoints(gamePoints[b], gamepoint))
                    setRandomNumbers(ref gamepoint.startingNode.x, ref gamepoint.startingNode.y, ref gamepoint.endingNode.x, ref gamepoint.endingNode.y);
            }
            gamePoints[a] = gamepoint;

        }
        for (int a = 0; a < noOfColors; a++)
        {
            Debug.Log("GamePoint: " + (a + 1) + "Starting Node X: " + gamePoints[a].startingNode.x + "Starting Node Y" + gamePoints[a].startingNode.y + "EndingNode X: " + gamePoints[a].endingNode.x + "Ending Node Y" + gamePoints[a].endingNode.y);
        }
        //for (int a=0;a<noOfColors;a++)
        //{
        //    for (int b=a+1;b<noOfColors;b++)
        //    {
        //        if (compareGamePoints(gamePoints[a],gamePoints[b]))
        //            setRandomNumbers(ref gamePoints[a].startingNode.x, ref gamePoints[a].startingNode.y, ref gamePoints[a].endingNode.x, ref gamePoints[a].endingNode.y);
        //    }
        //}
        for (int a = 0; a < noOfColors; a++)
        {
            Debug.Log(a);
            Debug.Log("GamePoint: " + (a + 1) + "Starting Node X: " + gamePoints[a].startingNode.x + "Starting Node Y" + gamePoints[a].startingNode.y + "EndingNode X: " + gamePoints[a].endingNode.x + "Ending Node Y" + gamePoints[a].endingNode.y);
        }
        return gamePoints;
    }
    bool compareGamePoints(GamePoint firstPoint, GamePoint secondPoint)
    {
        if (firstPoint.startingNode.x == secondPoint.startingNode.x && firstPoint.startingNode.y == secondPoint.startingNode.y)
            return true;
        else if (firstPoint.endingNode.x == secondPoint.endingNode.x && firstPoint.endingNode.y == secondPoint.endingNode.y)
            return true;
        else if (firstPoint.startingNode.x == secondPoint.endingNode.x && firstPoint.startingNode.y == secondPoint.endingNode.y)
            return true;
        return false;

    }
    //i dont know how to fix this random function anymore
    //i have spent way too much time in it.
    //using a noise function would have alot better 
    //but whatever.
    //on the offchance someone is actually  reading this 
    //please, fix this. 
    void setRandomNumbers(ref int startX, ref int startY, ref int endX, ref int endY)
    {
        //this random generator is literally the worst randomizer i have ever seen
        //wtf ??
        //FIX RANDOMIZER!!
        //dont put seed in this randomizer. it breaks


        startX = Random.Range(0, rows);
        startY = Random.Range(0, columns);
        endX = Random.Range(0, rows);
        endY = Random.Range(0, columns);
        //maybe add more conditions so start and end dont spawn next to each other.
        //this barely works.
        //also recursive calls???

        // Debug.Log("Random Numbers are : a" +a + "b" + b + "c" + c + "d" + d);
        if ((startX == endX) && (startY == endY))
            setRandomNumbers(ref startX, ref startY, ref endX, ref endY);
        else
            return;
    }

}
