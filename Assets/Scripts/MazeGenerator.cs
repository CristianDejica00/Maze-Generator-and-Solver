using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MazeGenerator : MonoBehaviour
{
    public GameObject displayImage;
    public int cellSize;
    public Vector2Int size;
    public Cell[,] mazeMap;
    List<Vector2Int> stack;
    List<Vector2Int> completedCells;
    public Vector2Int startPoint;
    public Vector2Int endPoint;
    public int[,] solvingMap;

    public int seed;


    void Start() {
        Random.InitState(seed);
        solvingMap = new int[size.x*cellSize, size.y*cellSize];
        mazeMap = new Cell[size.x, size.y];
        for(int x=0;x<size.x;x++) {
            for(int y=0;y<size.y;y++) {
                mazeMap[x, y] = new Cell(new Vector2Int(x, y));
            }
        }
        stack = new List<Vector2Int>();
        completedCells = new List<Vector2Int>();
        startPoint = Vector2Int.zero;
        stack.Add(startPoint);
        mazeMap[0, 0].current = true;
        InitializeMaze(startPoint);
        DisplayMap();
        endPoint = new Vector2Int(size.x-1, size.y-1);
        GetComponent<MazeSolver>().InitializeSolver(solvingMap, size, cellSize, startPoint, endPoint);
    }

    void InitializeMaze(Vector2Int pos) {
        if(completedCells.Count < size.x * size.y) {
            
            mazeMap[pos.x, pos.y].occupied = true;
            if(!completedCells.Contains(new Vector2Int(pos.x, pos.y))) completedCells.Add(new Vector2Int(pos.x, pos.y));
            mazeMap[pos.x, pos.y].current = false;

            if(GetRandomNeighbor(pos) == new Vector2Int(-1, -1)) {
                stack.RemoveAt(stack.Count - 1);
                mazeMap[stack[stack.Count - 1].x, stack[stack.Count - 1].y].current = true;
                InitializeMaze(stack[stack.Count - 1]);
            } else {
                var newPos = GetRandomNeighbor(pos);
                stack.Add(newPos);
                mazeMap[newPos.x, newPos.y].current = true;
                mazeMap[newPos.x, newPos.y].connections.Add(new Vector2Int(pos.x, pos.y));
                mazeMap[pos.x, pos.y].connections.Add(new Vector2Int(newPos.x, newPos.y));
                InitializeMaze(newPos);
            }
        } else {
            Debug.Log("Completed generation!");
        }
    }

    Vector2Int GetRandomNeighbor(Vector2Int pos) {
        List<Vector2Int> neighbors = new List<Vector2Int>();
        if(pos.x>0 && !mazeMap[pos.x-1, pos.y].occupied) neighbors.Add(new Vector2Int(pos.x-1, pos.y));
        if(pos.x<size.x-1 && !mazeMap[pos.x+1, pos.y].occupied) neighbors.Add(new Vector2Int(pos.x+1, pos.y));
        if(pos.y>0 && !mazeMap[pos.x, pos.y-1].occupied) neighbors.Add(new Vector2Int(pos.x, pos.y-1));
        if(pos.y<size.y-1 && !mazeMap[pos.x, pos.y+1].occupied) neighbors.Add(new Vector2Int(pos.x, pos.y+1));
        if(neighbors.Count == 0) return new Vector2Int(-1, -1);
        return neighbors[Random.Range(0, neighbors.Count)];
    }

    void DisplayMap() {
        Texture2D texture = new Texture2D(size.x*cellSize, size.y*cellSize);
        texture.filterMode = FilterMode.Point;
        displayImage.GetComponent<RawImage>().texture = texture;


        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Color color = Color.gray;
                if(mazeMap[x, y].occupied) {
                    for(int m=0;m<cellSize;m++) {
                        for(int n=0;n<cellSize;n++) {
                            if(m==0 || m==cellSize-1 || n==0 || n==cellSize-1) {
                                color = Color.black;
                            } else {
                                color = mazeMap[x, y].current ? Color.green : Color.white;
                            }
                            solvingMap[x*cellSize+m, y*cellSize+n] = color == Color.black ? 0 : 1;
                            texture.SetPixel(x*cellSize+m, y*cellSize+n, color);
                        }
                    }

                    if(mazeMap[x, y].connections.Contains(new Vector2Int(x-1, y))) {
                        color = mazeMap[x, y].current ? Color.green : Color.white;
                        for(int n=1;n<cellSize-1;n++) {
                            solvingMap[x*cellSize, y*cellSize+n] = 1;
                            texture.SetPixel(x*cellSize, y*cellSize+n, color);
                        }
                    }

                    if(mazeMap[x, y].connections.Contains(new Vector2Int(x+1, y))) {
                        color = mazeMap[x, y].current ? Color.green : Color.white;
                        for(int n=1;n<cellSize-1;n++) {
                            solvingMap[x*cellSize+cellSize-1, y*cellSize+n] = 1;
                            texture.SetPixel(x*cellSize+cellSize-1, y*cellSize+n, color);
                        }
                    }

                    if(mazeMap[x, y].connections.Contains(new Vector2Int(x, y-1))) {
                        color = mazeMap[x, y].current ? Color.green : Color.white;
                        for(int n=1;n<cellSize-1;n++) {
                            solvingMap[x*cellSize+n, y*cellSize] = 1;
                            texture.SetPixel(x*cellSize+n, y*cellSize, color);
                        }
                    }

                    if(mazeMap[x, y].connections.Contains(new Vector2Int(x, y+1))) {
                        color = mazeMap[x, y].current ? Color.green : Color.white;
                        for(int n=1;n<cellSize-1;n++) {
                            solvingMap[x*cellSize+n, y*cellSize+cellSize-1] = 1;
                            texture.SetPixel(x*cellSize+n, y*cellSize+cellSize-1, color);
                        }
                    }

                } else {
                    for(int m=0;m<cellSize;m++) {
                        for(int n=0;n<cellSize;n++) {
                            texture.SetPixel(x*cellSize+m, y*cellSize+n, color);
                        }
                    }
                }
            }
        }

        texture.Apply();
    }
}

public class Cell {
    public Vector2Int position;
    public bool occupied;
    public bool current;
    public List<Vector2Int> connections;

    public Cell(Vector2Int p) {
        position = p;
        occupied = false;
        current = false;
        connections = new List<Vector2Int>();
    }
}