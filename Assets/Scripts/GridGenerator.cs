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
    public List<Node> pathfindingPath;
    [HideInInspector]
    public int startPositionX, startPositionY;
    [HideInInspector]
    public int endPositionX, endPositionY;
    Node currentNode;
    public int noOfColors;
    public Color []colors;
    public Dictionary<Color, GamePoint> gamePointDict;
    GamePoint gp;
    GamePoint gp2;
    List<Node> pathRed = new List<Node>();
    Color currentColor;
    List<Node> currentPath;
    Color collisionColor;
    bool colorTaken;

    private void Awake()
    {
        //noOfColors = (Random.Range(rows - 1, rows + 1));
        startingSetup();
        //movementDebugStartConditions();
        
    }
    void debug()
    {
        Debug.Log("In Debug Function");
        for (int i = 0; i < columns; i++)
            for (int j = 0; j < rows; j++)
                if (nodes[i, j].renderer == null)
                    Debug.Log("Problem at X: " + i + "Y: " + j);
        for (int i=0;i<noOfColors;i++)
        {
            Debug.Log("Game Points Starting X:" + gamePointDict[colors[i]].startingNode.x + "Y: " + gamePointDict[colors[i]].startingNode.y);
            Debug.Log("Game Points Ending X:" + gamePointDict[colors[i]].endingNode.x + "Y: " + gamePointDict[colors[i]].endingNode.y);
            gamePointDict[colors[i]].startingNode.renderer.color = Color.black;
            nodes[gamePointDict[colors[i]].startingNode.x, gamePointDict[colors[i]].startingNode.y].renderer.color = Color.black;
        }
    }
  
    void movementDebugStartConditions()
    {
        tiles = new GameObject[rows, columns];
        nodes = new Node[rows, columns];
        generateGrid();
        gp = new GamePoint();
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
        gp2.endingNode = nodes[0, 4];
        gamePointDict = new Dictionary<Color, GamePoint>();
        colors = new Color[] { Color.red, Color.yellow };
        gamePointDict.Add(Color.red, gp);
        gamePointDict.Add(Color.yellow, gp2);
        currentPath = new List<Node>();
        currentColor = new Color();
        currentNode = new Node();
        collisionColor = new Color();
        colorTaken = false;
    }
    
    bool checkColors(Color firstColor, Color secondColor)
    {

        return firstColor.r == secondColor.r && firstColor.g == secondColor.g && firstColor.b == secondColor.b && firstColor.a == secondColor.a;
    }
   
    void checkPathValidity(Color color)
    {
        GamePoint currentGamePoint = new GamePoint();
        gamePointDict.TryGetValue(color, out currentGamePoint);
        List<Node> path = new List<Node>();
        path = currentGamePoint.path;
        if(path.Count!=0)
            if(!checkGamePositions(path[path.Count-1],color))
            {
                if(path.Count>1)
                {
                    for (int i = 0; i < path.Count; i++)
                    {
                        if (path[i] == currentGamePoint.startingNode || path[i] == currentGamePoint.endingNode)
                        {
                            if (path[i] == currentGamePoint.startingNode)
                            {
                                Debug.Log("Index: " + i);
                                Debug.Log("Total Count" + path.Count);
                            }
                            continue;
                           
                        }
                        changeTileColor(path[i], Color.white);
                        path[i].isColored = false;
                    }
                }
            }
        path.Clear();
  
        
    }
    bool isSameColored(Node firstNode, Node secondNode)
    {
       return firstNode.isSameColored(secondNode.renderer.color);
    }
    void startingSetup()
    {
        tiles = new GameObject[rows, columns];
        nodes = new Node[rows, columns];
        generateGrid();
        gamePointDict = new Dictionary<Color, GamePoint>();
        noOfColors = 4;
        colors = new Color[] { Color.red, Color.green, Color.yellow, Color.blue };
        pathfindingPath = new List<Node>();
        generateAllStartingPoints();
        currentPath = new List<Node>();
        currentColor = new Color();
        currentNode = new Node();
        colorTaken = false;
        //debug();
    }
    bool checkGamePositions(Node node,Color color)
    {
        if (node == gamePointDict[color].startingNode || node == gamePointDict[color].endingNode)
        {
            Debug.Log("This is true");
            return true;
        }
        Debug.Log("This is false");
        return false;
    }
    void generateGamePointColors()
    {
        //Debug.Log("In Generate GamePoint Colors");
        GamePoint[] gpArray = new GamePoint[noOfColors];
        gpArray = checkGamePoint();
        for (int a=0;a<noOfColors;a++)
        {
            if (!gamePointDict.ContainsKey(colors[a]))
            {
                gamePointDict.Add(colors[a], gpArray[a]);
                if(gamePointDict[colors[a]].startingNode.renderer==null)
                {
                    //Debug.Log("WTF IS THIS SHIT");
                }
                //changeTileColor(gamePointDict[colors[a]].startingNode, colors[a]);
                //changeTileColor(gamePointDict[colors[a]].endingNode, colors[a]);
                //Debug.Log("Game Points Starting X:" + gamePointDict[colors[a]].startingNode.x + "Y: " + gamePointDict[colors[a]].startingNode.y);
                //Debug.Log("Game Points Ending X:" + gamePointDict[colors[a]].endingNode.x + "Y: " + gamePointDict[colors[a]].endingNode.y);
            }
            else
                continue;
        }
        GamePoint testpoint;
        gamePointDict.TryGetValue(Color.red, out testpoint);
        startPositionX = testpoint.startingNode.x;
        startPositionY = testpoint.startingNode.y;
        endPositionX = testpoint.endingNode.x;
        endPositionY = testpoint.endingNode.y;
     
    }
    bool checkSameGamePoint(Node node,Color color)
    {
        if (node == gamePointDict[color].startingNode || node == gamePointDict[color].endingNode)
            return true;
        return false;
    }
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved)
            {
                currentNode = nodeFromPosition(gameObject.transform.InverseTransformPoint(Camera.main.ScreenToWorldPoint(touch.position)));
                //Debug.Log("Color: " + currentColor.ToString());
                //checkPathCollision();
                //Debug.Log("Current Node is colored:" + currentNode.isColored);
                checkPathCollision();
                if (isGamePoint(currentNode) && !colorTaken)
                {
                    currentColor = getColor(currentNode);
                    colorTaken = true;
                }
                if ((isGamePoint(currentNode) && !checkSameGamePoint(currentNode, currentColor)) && colorTaken)
                {
                    Debug.Log("Error is here");
                    checkPathValidity(currentColor);
                }

                if ((isGamePoint(currentNode) && currentPath.Count == 0) || (isAdjacent(currentNode.x, currentNode.y, currentColor) && currentPath.Count > 0))
                {
                    //Debug.Log("In this function");
                    changeTileColor(currentNode, currentColor);
                    //Debug.Log(" By this point color should be changed");
                    if (!currentPath.Contains(currentNode))
                    {
                        currentPath.Add(currentNode);
                        gamePointDict[currentColor].path.Add(currentNode);
                        //Debug.Log("Current Color RED  path has nodes: " + gamePointDict[currentColor].path.Count.ToString());
                    }

                }
                else
                {
                   
                    checkPathValidity(currentColor);
                }
                    
            }
            if (touch.phase == TouchPhase.Ended)
            {
                //checkPathValidity(currentColor);
                if (!checkGamePositions(currentPath[currentPath.Count - 1], currentColor))
                {
                    //Debug.Log("Third Error");
                    checkPathValidity(currentColor);

                }
                //Debug.Log("Current Color RED path has nodes: " + gamePointDict[currentColor].path.Count.ToString());
                //Debug.Log("Current path has Nodes:" + currentPath.Count.ToString());
                currentPath.Clear();
                colorTaken = false;
            }
        }
    }
  
    public void checkPathCollision()
    {
        if (currentPath.Count != 0)
        {
            if (currentNode.isColored && !isSameColored(currentNode, currentPath[0]))
            {
                changePathColor(gamePointDict[currentNode.renderer.color].path, Color.white,currentNode.renderer.color);
                
            }
        }
    }
    void changePathColor(List<Node> path,Color color,Color dictColor)
    {
       if(path.Count>1)
        {
            Debug.Log("In change path color");
            for(int a=0;a<path.Count;a++)
            {
                if (path[a] == gamePointDict[dictColor].startingNode || path[a] == gamePointDict[dictColor].endingNode)
                    continue;
                changeTileColor(path[a], color);
                path[a].isColored = false;
            }
        }
        path.Clear();
    }

    /***
     *
     *Check if isAdjacent is the same colored  as current path IF IT IS colored at all. 
     * */

    void changeTileColor(Node node,Color color)
    {
        //Debug.Log("In change color");
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
        //Debug.Log("Starting Point Red:" + gamePointDict[Color.red].endingNode.x + " Y: " + gamePointDict[Color.red].endingNode.y);
        //Debug.Log("Number of nodes in path red:" + gamePointDict[Color.red].path.Count);
        //Debug.Log("Number of nodes in path yellow:" + gamePointDict[Color.yellow].path.Count);
        for(int i=0;i<noOfColors;i++)
        {
            Debug.Log("Game Points Starting X:" + gamePointDict[colors[i]].startingNode.x + "Y: " + gamePointDict[colors[i]].startingNode.y);
            Debug.Log("Game Points Ending X:" + gamePointDict[colors[i]].endingNode.x + "Y: " + gamePointDict[colors[i]].endingNode.y);
            gamePointDict[colors[i]].startingNode.renderer.color = Color.black;
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
        
    }
   
    //stop this much overloading??
    
    bool isRightColored(int x,int y,Color color)
    {
        if (x == rows - 1)
            return false;
        if (nodes[x+1,y].isColored&&nodes[x+1,y].isSameColored(color))
        {
            //Debug.Log("Right is same colored");
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
            //Debug.Log("Left is same colored");
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
            //Debug.Log("Above is same colored");
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
            //Debug.Log("Bottom is same colored");
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
        //Debug.Log("X: " + x + "Y" + y);
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
        gamePointDict.Clear();
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
            //Debug.Log("Previous X: " + i + "Previous Y: " + j);
            //Debug.Log("Rounded value x " + Mathf.RoundToInt(worldPosition.x));
            //Debug.Log("Rounded Value y " + Mathf.RoundToInt(worldPosition.y));

            //i,j is the previous index.
            //if (nodes[i, j].isHead)//if previous clicked is the head
            //{
            //    Debug.Log("Current head is " + nodes[i, j].isHead);
            //    move(Mathf.RoundToInt(worldPosition.x), -(Mathf.RoundToInt(worldPosition.y)));
            //    //then set previous head to false;
            //    i = Mathf.RoundToInt(worldPosition.x);
            //    j = -(Mathf.RoundToInt(worldPosition.y));//take current clicked value
            //    setHead(i, j);//set current clicked value as the head.
            //    Debug.Log("New head is " + nodes[i, j].isHead);

            //}
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
                //path[a].renderer.color = colors[currentIndex];
                path[a].isColored = true;
            }

            GamePoint point;
            gamePointDict.TryGetValue(colors[currentIndex], out point);
            nodes[point.startingNode.x, point.startingNode.y].isColored = true;
            nodes[point.endingNode.x, point.endingNode.y].isColored = true;
            path.Clear();
        }
    }
    public void resetColoredValues()
    {
        for(int i=0;i<rows;i++)
        {
            for(int j=0;j<columns;j++)
            {
                if (isGamePoint(nodes[i, j]))
                {
                    nodes[i, j].isColored = true;
                    continue;
                }
                nodes[i, j].isColored = false;
            }
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
        //Debug.Log("In Generate Neighbours");
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
            gamepoint.startingNode = nodes[gamepoint.startingNode.x,gamepoint.startingNode.y];
            gamepoint.endingNode = nodes[gamepoint.endingNode.x, gamepoint.endingNode.y];

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
