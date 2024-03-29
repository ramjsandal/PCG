using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Bias
{
    None,
    LeftRight,
    UpLeftRight,
    DownLeftRight
}
public class CellularAutomata : MonoBehaviour
{
    [SerializeField] private Cell tile;
    [SerializeField] private int dimensions;
    [SerializeField] private Camera cam;
    [SerializeField] private Bias bias;
    [SerializeField] private int inBiasPercent;
    [SerializeField] private int outBiasPercent;
    private float time;
    private Cell [,] _tiles;
    private bool _finished;



    // Start is called before the first frame update
    void Start()
    {
        time = 0;
        _finished = false;
        _tiles = new Cell[dimensions,dimensions];
        cam.transform.position = new Vector3(dimensions / 2.0f, dimensions / 2.0f,-10);
        System.Random rand = new System.Random();
        int chanceInOneHundred;
        for (int i = 0; i < dimensions; i++)
        {
            for (int j = 0; j < dimensions; j++)
            {
                _tiles[i, j] = Instantiate(tile);
                _tiles[i, j].Init(_tiles[i,j].gameObject, _tiles[i,j].gameObject.GetComponent<SpriteRenderer>(), _tiles[i,j].gameObject.transform);
                _tiles[i, j].SetPosition(i,j);
                chanceInOneHundred = InBiasZone(bias, i, j) ? inBiasPercent : outBiasPercent;
                if (rand.Next() % 100 > chanceInOneHundred)
                {
                    _tiles[i, j].SetTraversable(false);
                }
                else
                {
                    _tiles[i, j].SetTraversable(true);
                }
            }
            
        }
    }

   List<Cell> GetNeighbors(Cell currentTile)
    {
        List<Cell> neighbors = new List<Cell>();
        Vector3 pos = currentTile.transform.position;
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                if (i == j && i == 0)
                {
                    continue;
                }

                if (pos.x + i < 0 || pos.y + j < 0 || pos.x + i >= dimensions || pos.y + j >= dimensions)
                {
                    continue;
                }
                neighbors.Add(_tiles[(int) pos.x + i, (int) pos.y + j]); 
            }
        }

        return neighbors;

    }

    void ApplyRule()
    {
        for (int i = 0; i < dimensions; i++)
        {
            for (int j = 0; j < dimensions; j++)
            {
                Cell currentCell = _tiles[i, j];
                List<Cell> neighbors = GetNeighbors(currentCell);
                int T = 0;
                for (int k = 0; k < neighbors.Count; k++)
                {
                    if (!neighbors[k].Traversable())
                    {
                        T++;
                    }
                }

                if (T >= 5.0f)
                {
                    currentCell.SetTraversable(false);
                } else if (neighbors.Count - T >= 5)
                {
                    currentCell.SetTraversable(true);
                }
                
            }
        }
    }

    private void MakeWalls()
    {
        for (int i = 0; i < dimensions; i++)
        {
            for (int j = 0; j < dimensions; j++)
            {
                if (_tiles[i, j].Traversable())
                {
                    continue;
                }
                List<Cell> neighbors = GetNeighbors(_tiles[i,j]);
                if (neighbors.FindIndex(a => a.Traversable()) != -1)
                {
                    // this should be a wall
                    _tiles[i,j].SetTraversable(false, true);
                }
            }
        }

    }
    private void Update()
    {
        if (_finished)
        {
            return;
        }
        
        time += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (_finished == false)
            {
                CleanEdges();
                MakeWalls();
            }
            _finished = true;
        }
        if (time > 2)
        {
            time = 0;
            ApplyRule();
        }
    }

    bool InBiasZone(Bias currentBias, int x, int y)
    {
        switch (currentBias)
        {
            case Bias.LeftRight:
                // we dont care about the x value
                return (y > dimensions / 4) && (y < dimensions * 3 / 4); 
            case Bias.UpLeftRight:
                return InBiasZone(Bias.LeftRight, x, y) 
                           || (y > dimensions * 3 / 4) && (x > dimensions / 4) && (x < dimensions * 3 / 4);
            case Bias.DownLeftRight:
                return InBiasZone(Bias.LeftRight, x, y) 
                           || (y < dimensions / 4) && (x > dimensions / 4) && (x < dimensions * 3 / 4);
            default: // bias::none
               return false;
        }
    }
    
        void CleanEdges()
    {
        for (int i = 0; i < dimensions; i++)
        {
            for (int j = 0; j < dimensions; j++)
            {
                if (!OnEdge(i, j))
                {
                    continue;
                }

                Cell currentCell = _tiles[i, j];
                List<Cell> neighbors = GetNeighbors(currentCell);

                int emptyCells = 0;
                foreach (Cell neighbor in neighbors)
                {
                    if (neighbor.Traversable())
                    {
                        emptyCells++;
                    } 
                }

                if (emptyCells >= 3)
                {
                    currentCell.SetTraversable(true);
                }
                else
                {
                     currentCell.SetTraversable(false);
                }
            }
        }
        
    }

    bool OnEdge(int x, int y)
    {
        bool leftEdge = x == 0;
        bool rightEdge = x == dimensions - 1;
        bool topEdge = y == dimensions - 1;
        bool bottomEdge = y == 0;

        return topEdge || bottomEdge || leftEdge || rightEdge;
    }
}


