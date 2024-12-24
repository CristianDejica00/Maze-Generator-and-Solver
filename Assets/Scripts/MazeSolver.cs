using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MazeSolver : MonoBehaviour
{
    Node[,] solverMap;
    List<Node> openList;
    List<Node> closedList;
    Vector2Int start;
    Vector2Int end;
    int cellSize;
    Vector2Int size;


    bool canRetrace = false;

    public void InitializeSolver(int[,] mazeArray, Vector2Int sz, int cSize, Vector2Int startPoint, Vector2Int endPoint) {
        start = startPoint*new Vector2Int(cSize, cSize) + new Vector2Int(cSize/2, cSize/2);
        end = endPoint*new Vector2Int(cSize, cSize) + new Vector2Int(cSize/2, cSize/2);
        cellSize = cSize;
        size = sz;
        solverMap = new Node[size.x*cellSize, size.y*cellSize];
        openList = new List<Node>();
        closedList = new List<Node>();

        for(int x=0;x<sz.x*cellSize;x++) {
            for(int y=0;y<sz.y*cellSize;y++) {
                solverMap[x, y] = new Node(new Vector2Int(x, y));
                if(mazeArray[x, y] != 1) solverMap[x, y].isWall = true;
            }
        }
        
        InitializeNode(start);

        StartCoroutine(SolveMaze(start));
    }

    IEnumerator SolveMaze(Vector2Int pos) {
        DisplayMaze();
        yield return new WaitForSeconds(0.01f);
        if(pos != end) {
            var minVal = GetMinValueInOpen();
            Node chosenNode = null;
            foreach(Node n in openList) {
                if(n.f == minVal) {
                    chosenNode = n;
                }
            }

            if(pos.x>0 && !solverMap[pos.x-1, pos.y].isWall && !closedList.Contains(solverMap[pos.x-1, pos.y])) InitializeNode(new Vector2Int(pos.x-1, pos.y), chosenNode);
            if(pos.x<size.x*cellSize && !solverMap[pos.x+1, pos.y].isWall && !closedList.Contains(solverMap[pos.x+1, pos.y])) InitializeNode(new Vector2Int(pos.x+1, pos.y), chosenNode);
            if(pos.y>0 && !solverMap[pos.x, pos.y-1].isWall && !closedList.Contains(solverMap[pos.x, pos.y-1])) InitializeNode(new Vector2Int(pos.x, pos.y-1), chosenNode);
            if(pos.y<size.y*cellSize && !solverMap[pos.x, pos.y+1].isWall && !closedList.Contains(solverMap[pos.x, pos.y+1])) InitializeNode(new Vector2Int(pos.x, pos.y+1), chosenNode);
            
            if(pos.x>0 && pos.y>0 && !solverMap[pos.x-1, pos.y-1].isWall && !closedList.Contains(solverMap[pos.x-1, pos.y-1])) InitializeNode(new Vector2Int(pos.x-1, pos.y-1), chosenNode);
            if(pos.x>0 && pos.y<size.y*cellSize && !solverMap[pos.x-1, pos.y+1].isWall && !closedList.Contains(solverMap[pos.x-1, pos.y+1])) InitializeNode(new Vector2Int(pos.x-1, pos.y+1), chosenNode);
            if(pos.x<size.x*cellSize && pos.y>0 && !solverMap[pos.x+1, pos.y-1].isWall && !closedList.Contains(solverMap[pos.x+1, pos.y-1])) InitializeNode(new Vector2Int(pos.x+1, pos.y-1), chosenNode);
            if(pos.x<size.x*cellSize && pos.y<size.y*cellSize && !solverMap[pos.x+1, pos.y+1].isWall && !closedList.Contains(solverMap[pos.x+1, pos.y+1])) InitializeNode(new Vector2Int(pos.x+1, pos.y+1), chosenNode);
            
            
            openList.Remove(solverMap[pos.x, pos.y]);
            if(!closedList.Contains(solverMap[pos.x, pos.y])) {
                closedList.Add(solverMap[pos.x, pos.y]);
            }

            StartCoroutine(SolveMaze(chosenNode.pos));

        } else if(openList.Count == 0) {
            Debug.Log("NO PATH EXISTS");
        } else {
            canRetrace = true;
            Debug.Log("RETRACING");
            StartCoroutine(Retrace(end));
        }
    }

    IEnumerator Retrace(Vector2Int pos) {
        solverMap[pos.x, pos.y].retraced = true;
        Vector2Int selectedNode = new Vector2Int(-1, -1);
        if(pos == end) {
            for(int x=-1;x<=1;x++) {
                for(int y=-1;y<=1;y++) {
                    if(!(x==0 && y==0) && pos.x+x>=0 && pos.x+x<=size.x*cellSize && pos.y+y>=0 && pos.y+y<=size.y*cellSize && !solverMap[pos.x+x, pos.y+y].isWall) {
                        if(closedList.Contains(solverMap[pos.x+x, pos.y+y])) {
                            selectedNode = new Vector2Int(pos.x+x, pos.y+y);
                        }
                    }
                }
            }
        } else {
            for(int x=-1;x<=1;x++) {
                for(int y=-1;y<=1;y++) {
                    if(!(x==0 && y==0) && pos.x+x>=0 && pos.x+x<=size.x*cellSize && pos.y+y>=0 && pos.y+y<=size.y*cellSize && !solverMap[pos.x+x, pos.y+y].isWall && !solverMap[pos.x+x, pos.y+y].retraced) {
                        if(closedList.Contains(solverMap[pos.x+x, pos.y+y]) && (solverMap[pos.x+x, pos.y+y] == solverMap[pos.x, pos.y].parent || solverMap[pos.x+x, pos.y+y].g <= solverMap[pos.x, pos.y].g)) {
                            selectedNode = new Vector2Int(pos.x+x, pos.y+y);
                        }
                    }
                }
            }
        }
        
        DisplayMaze();
        yield return new WaitForSeconds(0.01f);
        if(selectedNode != new Vector2Int(-1, -1)) StartCoroutine(Retrace(selectedNode));
        else if(selectedNode == start) Debug.Log("RETRACE COMPLETED");
        else Debug.Log("CAN'T BE RETRACED");
    }



    void InitializeNode(Vector2Int pos, Node n = null) {
        if(!openList.Contains(solverMap[pos.x, pos.y])) {
            openList.Add(solverMap[pos.x, pos.y]);
        }
        if(solverMap[pos.x, pos.y].parent == null) solverMap[pos.x, pos.y].parent = n;
        solverMap[pos.x, pos.y].g = GetLowestG(pos);
        solverMap[pos.x, pos.y].h = Mathf.Abs(pos.x - end.x) + Mathf.Abs(pos.y - end.y);
        solverMap[pos.x, pos.y].f = solverMap[pos.x, pos.y].g+2f*solverMap[pos.x, pos.y].h;
    }

    float GetLowestG(Vector2Int pos) {
        if(pos == start) return 0;
        float val = System.Single.MaxValue;
        float gAddition = 0;
        for(int x=-1;x<=1;x++) {
            for(int y=-1;y<=1;y++) {
                if(pos.x+x>=0 && pos.x+x<=size.x*cellSize && pos.y+y>=0 && pos.y+y<=size.y*cellSize && !(x==0 && y==0) && !solverMap[pos.x+x, pos.y+y].isWall) {
                    if(val > solverMap[pos.x+x, pos.y+y].g) {
                        val = solverMap[pos.x+x, pos.y+y].g;
                        if(x==0 || y==0) gAddition = 1f;
                        else gAddition = Mathf.Sqrt(2f);
                    }
                }
            }
        }
        return val + gAddition;
    }

    float GetMinValueInOpen() {
        float val = System.Single.MaxValue;
        foreach(Node n in openList) {
            if(n.f < val) val = n.f;
        }
        return val;
    }

    void DisplayMaze() {
        Texture2D texture = new Texture2D(size.x*cellSize, size.y*cellSize);
        texture.filterMode = FilterMode.Point;
        GetComponent<MazeGenerator>().displayImage.GetComponent<RawImage>().texture = texture;

        for(int x=0;x<size.x*cellSize;x++) {
            for(int y=0;y<size.y*cellSize;y++) {
                Color c = Color.black;
                if(!canRetrace) {
                    if(openList.Contains(solverMap[x, y])) c = Color.green;
                    else if(closedList.Contains(solverMap[x, y])) c = Color.red;
                    else if(!solverMap[x, y].isWall) c = Color.white;
                } else {
                    if(solverMap[x, y].retraced) c = Color.blue;
                    else if(closedList.Contains(solverMap[x, y])) c = new Color(1f, 0.7f, 0.7f);
                    else if(!solverMap[x, y].isWall) c = Color.white;
                    
                }
                

                texture.SetPixel(x, y, c);
            }
        }
        
        texture.Apply();

    }
}

public class Node {
    public Vector2Int pos;
    public bool isWall;
    public bool retraced;
    public float g;
    public float h;
    public float f;
    public Node parent;

    public Node(Vector2Int p) {
        pos = p;
        g = System.Single.MaxValue;
        parent = null;
    }
}