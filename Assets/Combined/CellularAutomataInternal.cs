using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SceneTemplate;
using UnityEngine;

public class CellularAutomataInternal
{
  
    public struct CellState
    {
        public int xIdx;
        public int yIdx;
        public bool traversable;

        public CellState(int x, int y, bool trav)
        {
            xIdx = x;
            yIdx = y;
            traversable = trav;
        }
    }
      
    public CellState[,] GenerateArea(int dim, int generations, Bias b, int inB, int outB)
    {
        // true is traversible, false is untraversible
        CellState[,] area = new CellState[dim, dim];
        System.Random rand = new System.Random();
        for (int i = 0; i < dim; i++)
        {
            for (int j = 0; j < dim; j++)
            {
                int prob = InBiasZone(dim, b, i, j) ? inB : outB;
                bool trav = !((rand.Next() % 100) > prob);
                area[i, j] = new CellState(i, j, trav);
            }
        }

        for (int i = 0; i < generations; i++)
        {
            ApplyRule(ref area, dim);
        }
        
        CleanEdges(ref area, dim);
        
        return area;
    }
    
    List<CellState> GetNeighbors(ref CellState[,] map, CellState currentTile, int dimensions)
    {
        List<CellState> neighbors = new List<CellState>();
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                // if were ourself skip
                if (i == j && i == 0)
                {
                    continue;
                }

                // if were out of bounds
                if (currentTile.xIdx + i < 0 || currentTile.yIdx + j < 0 || currentTile.xIdx + i >= dimensions ||
                    currentTile.yIdx + j >= dimensions)
                {
                    continue;
                }

                neighbors.Add(map[(int)currentTile.xIdx + i, (int)currentTile.yIdx + j]);
            }
        }

        return neighbors;
    }
 
    
    void ApplyRule(ref CellState[,] map, int dimensions)
    {
        for (int i = 0; i < dimensions; i++)
        {
            for (int j = 0; j < dimensions; j++)
            {
                List<CellState> neighbors = GetNeighbors(ref map, map[i,j], dimensions);
                int T = 0;
                for (int k = 0; k < neighbors.Count; k++)
                {
                    if (!neighbors[k].traversable)
                    {
                        T++;
                    }
                }

                if (T >= 5.0f)
                {
                    map[i,j].traversable = false;
                } else if (neighbors.Count - T >= 5)
                {
                    map[i,j].traversable = true;
                }

            }
        }
    }
    
    bool InBiasZone(int dimensions, Bias currentBias, int x, int y)
    {
        switch (currentBias)
        {
            case Bias.LeftRight:
                // we dont care about the x value
                return (y > dimensions / 4) && (y < dimensions * 3 / 4); 
            case Bias.UpLeftRight:
                return InBiasZone(dimensions, Bias.LeftRight, x, y) 
                           || (y > dimensions * 3 / 4) && (x > dimensions / 4) && (x < dimensions * 3 / 4);
            case Bias.DownLeftRight:
                return InBiasZone(dimensions, Bias.LeftRight, x, y) 
                           || (y < dimensions / 4) && (x > dimensions / 4) && (x < dimensions * 3 / 4);
            default: // bias::none
               return false;
        }
    }
        
        
    void CleanEdges(ref CellState[,] map, int dimensions)
    {
        for (int i = 0; i < dimensions; i++)
        {
            for (int j = 0; j < dimensions; j++)
            {
                if (!OnEdge(dimensions, i, j))
                {
                    continue;
                }

                List<CellState> neighbors = GetNeighbors(ref map, map[i,j], dimensions);

                int emptyCells = 0;
                foreach (CellState neighbor in neighbors)
                {
                    if (neighbor.traversable)
                    {
                        emptyCells++;
                    } 
                }

                if (emptyCells >= 3)
                {
                    map[i,j].traversable = true;
                }
                else
                { 
                    map[i,j].traversable = false;
                }

            }
        }
    }
    
    bool OnEdge(int dimensions, int x, int y)
    {
        bool leftEdge = x == 0;
        bool rightEdge = x == dimensions - 1;
        bool topEdge = y == dimensions - 1;
        bool bottomEdge = y == 0;

        return topEdge || bottomEdge || leftEdge || rightEdge;
    }

}
