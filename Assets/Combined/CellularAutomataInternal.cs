using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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

    public enum Side
    {
        Up,
        Right,
        Down,
        Left,
        None
    };

    CellState[,] GenerateInitial(int dim, Bias b, int inB, int outB, bool[] initSide = null, Side side = Side.None)
    {
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

        if (initSide != null && side != Side.None)
        {
           SetRow(ref area, side, dim, initSide); 
        }

        return area;
    }
    public CellState[,] GenerateArea(int dim, int generations, Bias b, int inB, int outB)
    {
        // true is traversible, false is untraversible
        CellState[,] area = GenerateInitial(dim, b, inB, outB);
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

    bool OnEdge(int dimensions, int x, int y, Side side)
    {
        switch (side)
        {
            case Side.Up:
                return y == dimensions - 1;
            case Side.Down:
                return y == 0;
            case Side.Left:
                return x == 0;
            case Side.Right:
                return x == dimensions - 1;
            default:
                throw new InvalidEnumArgumentException();
        }
    }

    // stores left -> right and
    // up -> down
    bool[] GetRow(ref CellState[,] map, Side side, int dimensions)
    {
        bool[] row = new bool[dimensions];
        int count = 0;
        for (int i = 0; i < dimensions; i++)
        {
            for (int j = 0; j < dimensions; j++)
            {
                if (OnEdge(dimensions, i, j, side))
                {
                    row[count] = map[i, j].traversable;
                    count++;
                }
            }
        }

        return row;

    }

    void SetRow(ref CellState[,] map, Side side, int dimensions, bool[] row)
    {
        int xStart = 0;
        int yStart = 0;
        // if true, we increment x, if false, we increment y
        bool xIncrement = true;
        switch (side)
        {
           case Side.Up:
               xStart = 0;
               yStart = dimensions - 1;
               xIncrement = true;
               break;
            case Side.Down:
               xStart = 0;
               yStart = 0;
               xIncrement = true;
               break;
            case Side.Left:
               xStart = 0;
               yStart = 0;
               xIncrement = false;
               break;
            case Side.Right:
                xStart = dimensions - 1;
               yStart = 0;
               xIncrement = false;
               break;
            default:
                throw new InvalidEnumArgumentException();
        }

        int count = 0;
        while (xStart < dimensions && yStart < dimensions && count < dimensions)
        {
            map[xStart, yStart].traversable = row[count];
            count++;
            if (xIncrement)
            {
                xStart++;
            }
            else
            {
                yStart++;
            }
        }
    }

}
