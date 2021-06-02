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
    public Color []colors;
    public Dictionary<Color, GamePoint> gamePointDict;
    GamePoint gp;
    GamePoint gp2;
    List<Node> pathRed = new List<Node>();
    Color currentColor;
    private void Awake()
    {
        //noOfColors = (Random.Range(rows - 1, rows + 1));
        tiles = new GameObject[rows, columns];
        nodes = new Node[rows, columns];
        generateGrid();
        gp=new GamePoint();
        gp2 = new GamePoint();
        gp.startingNode = nodes[0, 0];
        nodes[0, 0].renderer.color = Color.red;
        nodes[0, 0].isColored = true;
        nodes[0, 0].isGamePoint = true;
        gp.endingNode = nodes[4, 4];
        nodes[4, 4].renderer.color = Color.red;
        nodes[4, 4].isColored = true;
        nodes[4, 4].isGamePoint = true;
        nodes[4, 0].renderer.color = Color.yellow;
        nodes[4, 0].isColored = true;
        nodes[4, 0].isGamePoint = true;
        nodes[0, 4].renderer.color = Color.yellow;
        nodes[0, 4].isColored = true;
        nodes[0, 4].isGamePoint = true;
        gp2.startingNode = nodes[4, 0];
        gp2.endingNode = nodes[0,4];
        currentColor = new Color();
    }
    bool isSameColored(Node firstNode, Node secondNode)
    {
        if (firstNode.renderer.color == secondNode.renderer.color)
        {
            Debug.Log("Color Test True");
            return true;
           
        }
        Debug.Log("Color Test False");

        return false;
    }
    void startingSetup()
    {
        gamePointDict = new Dictionary<Color, GamePoint>();
        noOfColors = 5;
        colors = new Color[] { Color.red, Color.green, Color.yellow, Color.blue, Color.cyan };
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
     
    }
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if(touch.phase==TouchPhase.Moved)
            {
                Node currentNode = nodeFromPosition(gameObject.transform.InverseTransformPoint(Camera.main.ScreenToWorldPoint(touch.position)));
                Debug.Log("Current Node: X:" + currentNode.x + " Y:" + currentNode.y);
                //Debug.Log("Color: " + currentColor.ToString());
                if(isGamePoint(currentNode))
                {
                    currentColor = getColor(currentNode);
                }
                if (isGamePoint(currentNode)|| (isAdjacent(currentNode.x, currentNode.y,currentColor)&&pathRed.Count>0))
                {
                    changeTileColor(currentNode, currentColor);
                    if (!pathRed.Contains(currentNode))
                        pathRed.Add(currentNode);
                }
                else
                    checkPathValidity();
            }
            if (touch.phase == TouchPhase.Ended)
                checkPathValidity();
        }
        //if(pathRed.Contains(gp.startingNode))
        //    checkPathValidity();
    }
    /***
     *
     *Check if isAdjacent is the same colored  as current path IF IT IS colored at all. 
     * */
    public void checkPathValidity()
    {
        //if (pathRed[pathRed.Count - 1] != gp.endingNode)
        if (pathRed[pathRed.Count - 1] != gp.endingNode)
        {
            if(pathRed.Count>1)
            {
                for (int a = 1; a < pathRed.Count; a++)
                {
                    changeTileColor(pathRed[a], Color.white);
                    pathRed[a].isColored = false;
                    //Debug.Log("Called Times: " + a);
                }
            }
            
            pathRed.Clear();
        }
    }

    void changeTileColor(Node node,Color color)
    {
        node.renderer.color = color;
        node.isColored = true;
        
        
       
    }
    bool isGamePoint(Node node)
    {
        if (node.isGamePoint)
            return true;
        return false;
    }
    /*for (int a=0;a<noOfColors;a++)
    *{
        GamePoint point;
        gamePointDict.TryGetValue(colors[a], out point);

        Debug.Log("Color: " + colors[a].ToString() + "Starting Node X:" + point.startingNode.x +"Y:"+ point.startingNode.y);
        Debug.Log("Color: " + colors[a].ToString() + "Ending Node X " + point.endingNode.x +"Y:"+ point.endingNode.y);
    }*/
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
    public void changeColorToBlackDebug()
    {
        for (int a=0;a<pathRed.Count;a++)
        {
            changeTileColor(pathRed[a], Color.black);
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
   
    //stop this much overloading??
    public void move(int x, int y)
    {

        //if (isAdjacent(x, y))
        //{
        //    nodes[x, y].renderer.color = Color.red;
        //    nodes[x, y].isColored = true;
        //}

    }
    bool isRightColored(int x,int y,Color color)
    {
        if (x == rows - 1)
            return false;
        if (nodes[x+1,y].isColored&&nodes[x+1,y].isSameColored(color))
        {
            Debug.Log("Right is same colored");
            return true;
        }
            return false;
    }
    bool isLeftColored(int x, int y,Color color)
    {
        if (x == 0)
            return false;
        if (nodes[x - 1, y].isColored&&nodes[x-1,y].isSameColored(color))
        {
            Debug.Log("Left is same colored");
            return true;
        }
        return false;
    }
    bool isAboveColored(int x, int y,Color color)
    {
        if (y == 0)
            return false;
    
        if (nodes[x, y-1].isColored&&nodes[x,y-1].isSameColored(color))
        {
            Debug.Log("Above is same colored");
            return true;
        }
        return false;
    }
    bool isBelowColored(int x, int y,Color color)
    {
        if (y == rows - 1)
            return false;
        if (nodes[x, y+1].isColored&&nodes[x,y+1].isSameColored(color))
        {
            Debug.Log("Bottom is same colored");
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
    public bool isAdjacent(int x, int y,Color color)
    {
        Debug.Log("X: " + x + "Y" + y);
        if (isLeftColored(x, y,color))
            return true;
        else if (isBelowColored(x, y,color))
            return true;
        else if (isAboveColored(x, y,color))
            return true;
        return isRightColored(x, y,color);
    }
    public void regenerateValues()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    // Update is called once per frame
   
    void oldMove()
    {
        if (Input.GetMouseButtonDown(0))
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
        //if (PathFinding.resultFound)
        //{
        //    for (int a = 0; a < path.Count; a++)
        //        path[a].renderer.color = Color.red;
        //    nodes[startPositionX, startPositionY].isColored = true;
        //    nodes[endPositionX, endPositionY].isColored = true;
        //}
        //if(PathFinding.resultsFound[PathFinding.currentIndex])
        //{
        //    for (int a = 0; a < path.Count; a++)
        //        path[a].renderer.color = colors[PathFinding.currentIndex];
        //    for (int a = 0; a < noOfColors; a++)
        //    {
        //        GamePoint point;
        //        gamePointDict.TryGetValue(colors[i], out point);
        //       nodes[point.startingNode.x, point.startingNode.y].isColored = true;
        //       nodes[point.endingNode.x, point.endingNode.y].isColored = true;
        //    }
        //}
    }
    public void setPathColor(List<Node> path,int currentIndex,bool[] resultsFound)
    {
        if (resultsFound[currentIndex])
        {
            for (int a = 0; a < path.Count; a++)
            {
                path[a].renderer.color = colors[currentIndex];
                path[a].isColored = true;
            }

            GamePoint point;
            gamePointDict.TryGetValue(colors[currentIndex], out point);
            nodes[point.startingNode.x, point.startingNode.y].isColored = true;
            nodes[point.endingNode.x, point.endingNode.y].isColored = true;
            path.Clear();
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
    Color getColor(Node node)
    {
        return node.renderer.color;
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
        if (x < 0 || y < 0||x > columns || y > rows)
            return null;
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
                    neighbours.Add(nodes[checkX, checkY]);
                }
            }
           
        }
        return neighbours;
    }
    
    public void checkNeighbours(Node node)
    {
        for (int a=-1;a<=1;a++)
        {
            for(int b=-1;b<=1;b++)
            {
                if (a == 0 && b == 0||node.renderer.color!=gp.endingNode.renderer.color)
                    continue;
                int checkX = node.x + a;
                int checkY = node.y + a;
                if ((checkX == node.x + 1 && checkY == node.y + 1)||(node.x - 1 == checkX && node.y - 1 == checkY) ||(node.x + 1 == checkX && node.y - 1 == checkY) ||(node.x - 1 == checkX && node.y + 1 == checkY))
                    continue;
                if(checkX >= 0 && checkX < rows && checkY >= 0 && checkY < columns)
                {
                }
            }
        }
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
      
        return gamePoints;
    }
    //for (int a = 0; a < noOfColors; a++)
    //{
    //    Debug.Log("GamePoint: " + (a + 1) + "Starting Node X: " + gamePoints[a].startingNode.x + "Starting Node Y" + gamePoints[a].startingNode.y + "EndingNode X: " + gamePoints[a].endingNode.x + "Ending Node Y" + gamePoints[a].endingNode.y);
    //}

    //for (int a = 0; a < noOfColors; a++)
    //{
    //    Debug.Log(a);
    //    Debug.Log("GamePoint: " + (a + 1) + "Starting Node X: " + gamePoints[a].startingNode.x + "Starting Node Y" + gamePoints[a].startingNode.y + "EndingNode X: " + gamePoints[a].endingNode.x + "Ending Node Y" + gamePoints[a].endingNode.y);
    //}
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
       
        //dont put seed in this randomizer. it breaks
        startX = Random.Range(0, rows);
        startY = Random.Range(0, columns);
        endX = Random.Range(0, rows);
        endY = Random.Range(0, columns);
        //maybe add more conditions so start and end dont spawn next to each other.
        // Debug.Log("Random Numbers are : a" +a + "b" + b + "c" + c + "d" + d);
        if ((startX == endX) && (startY == endY))
            setRandomNumbers(ref startX, ref startY, ref endX, ref endY);
        else
            return;
    }
}
