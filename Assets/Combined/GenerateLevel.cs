using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Cryptography;
using UnityEngine;

public class GenerateLevel : MonoBehaviour
{
    [SerializeField] private GameObject tile;
    [SerializeField] private Sprite traversableSprite;
    [SerializeField] private Sprite untraversableSprite;
    [SerializeField] private bool textured;
    [SerializeField] private bool player;
    private SpelunkyPathInternal _path;
    private CellularAutomataInternal _nodeGenerator;
    void Start()
    {
        _nodeGenerator = new CellularAutomataInternal();
        _path = new SpelunkyPathInternal();
        int mapDimensions = 4;
        int nodeDimensions = 100;
        if (!player)
        {
          float position = (mapDimensions * nodeDimensions) / 2;
          Camera.main.transform.position = new Vector3(position, position, -10);
          Camera.main.orthographicSize = position;    
        }
        CreateMap(mapDimensions,nodeDimensions, 7, 3);
    }

    CellularAutomataInternal.CellState[,] GenerateCell(int dimensions, int generations, Bias b, int inBiasPercent, int outBiasPercent, (bool[], CellularAutomataInternal.Side)[] sides)
    {
        return _nodeGenerator.GenerateArea(dimensions, generations, b, inBiasPercent, outBiasPercent, sides);
    }

    private int[,] GenerateLayout(int dimensions)
    {
        return _path.GenerateLayout(dimensions);
    }

    (bool[], CellularAutomataInternal.Side)[] GetSides(int x, int y, int mapDimensions, int nodeDimensions, ref CellularAutomataInternal.CellState[,][,] cells)
    {
        (bool[], CellularAutomataInternal.Side)[] sides = new (bool[], CellularAutomataInternal.Side)[4];
          int count = 0;
          // check above
          if (y + 1 < mapDimensions && cells[x, y + 1] != null)
          {
              sides[count] = (
                  _nodeGenerator.GetRow(ref cells[x, y + 1], CellularAutomataInternal.Side.Down, nodeDimensions),
                  CellularAutomataInternal.Side.Up);
              count++;
          }
          // check below 
          if (y - 1 >= 0 && cells[x, y - 1] != null)
          {
              sides[count] = (
                  _nodeGenerator.GetRow(ref cells[x, y - 1], CellularAutomataInternal.Side.Up, nodeDimensions),
                  CellularAutomataInternal.Side.Down);
              count++;
          }
          // check left 
          if (x - 1 >= 0 && cells[x - 1, y] != null)
          {
              sides[count] = (
                  _nodeGenerator.GetRow(ref cells[x - 1, y], CellularAutomataInternal.Side.Right, nodeDimensions),
                  CellularAutomataInternal.Side.Left);
              count++;
          }
          // check right 
          if (x + 1 >= 0 && cells[x + 1, y] != null)
          {
              sides[count] = (
                  _nodeGenerator.GetRow(ref cells[x + 1, y], CellularAutomataInternal.Side.Left, nodeDimensions),
                  CellularAutomataInternal.Side.Right);
              count++;
          }

          return sides;
    }
    void CreateMap(int mapDimensions, int nodeDimensions, int generations = 3, int finalRule = 3, int inBiasPercent = 56, int outBiasPercent = 44)
    {
        // Initialize grid to empty
        CellularAutomataInternal.CellState[,][,] cells = new CellularAutomataInternal.CellState[nodeDimensions,nodeDimensions][,];
        
        // Generate layout using path component 
        int[,] layout = GenerateLayout(mapDimensions);
        
        // loop through map dimensions, generating cells
        for (int i = 0; i < mapDimensions; i++)
        {
            for (int j = 0; j < mapDimensions; j++)
            {
                (bool[], CellularAutomataInternal.Side)[] sides = GetSides(j, mapDimensions - 1 - i, mapDimensions,
                    nodeDimensions, ref cells); 
                cells[i, j] = GenerateCell(nodeDimensions, generations, numberToBias(layout[i, j]), inBiasPercent,
                    outBiasPercent, sides);
            }
        }
        
        // Put it all into one big grid
        CellularAutomataInternal.CellState[,] bigGrid = CoalesceCellState(cells, mapDimensions, nodeDimensions);
        
        // Apply the rule on the whole grid to smooth it out
        for (int i = 0; i < finalRule; i++)
        {
            _nodeGenerator.ApplyRule(ref bigGrid, mapDimensions * nodeDimensions);
        }
        
        _nodeGenerator.ApplyEnemyRule(ref bigGrid, mapDimensions * nodeDimensions);
        FinalizeEnemies(ref bigGrid, mapDimensions * nodeDimensions);
        
        _nodeGenerator.ApplyChestRule(ref bigGrid, mapDimensions * nodeDimensions);
        FinalizeChests(ref bigGrid, mapDimensions * nodeDimensions);

        // Draw the big grid
        DrawBigGrid(bigGrid, mapDimensions * nodeDimensions, textured);
    }

    CellularAutomataInternal.CellState[,] CoalesceCellState(CellularAutomataInternal.CellState[,][,] cellStates, int mapDimensions, int nodeDimensions)
    {
        CellularAutomataInternal.CellState[,] cells = new CellularAutomataInternal.CellState[mapDimensions * nodeDimensions, mapDimensions * nodeDimensions];
        
        for (int i = 0; i < mapDimensions; i++)
        {
            for (int j = 0; j < mapDimensions; j++)
            {
                CellularAutomataInternal.CellState[,]current = cellStates[i, j];
                int xStart = j * nodeDimensions;
                int yStart = ((mapDimensions - 1 - i) * nodeDimensions);
             
                for (int k = 0; k < nodeDimensions; k++)
                {
                    for (int l = 0; l < nodeDimensions; l++)
                    {
                        cells[k + xStart, l + yStart].traversable = current[k, l].traversable;
                        cells[k + xStart, l + yStart].xIdx = k + xStart;
                        cells[k + xStart, l + yStart].yIdx = l + yStart;
                        cells[k + xStart, l + yStart].enemy = current[k, l].enemy;
                        cells[k + xStart, l + yStart].chest = current[k, l].chest;
                    }
                    
                }
            }
        }
        return cells;
    }
    
    CellularAutomataInternal.CellState[,][,] SplitCellState(CellularAutomataInternal.CellState[,] cellStates, int mapDimensions, int nodeDimensions)
    {
        CellularAutomataInternal.CellState[,][,] cells = new CellularAutomataInternal.CellState[nodeDimensions,nodeDimensions][,];

        for (int i = 0; i < mapDimensions; i++)
        {
            for (int j = 0; j < mapDimensions; j++)
            {
                CellularAutomataInternal.CellState[,] current = new CellularAutomataInternal.CellState[nodeDimensions,nodeDimensions];
                int xStart = j * nodeDimensions;
                int yStart = ((mapDimensions - 1 - i) * nodeDimensions);
             
                for (int k = 0; k < nodeDimensions; k++)
                {
                    for (int l = 0; l < nodeDimensions; l++)
                    {
                        current[k, l].traversable = current[k, l].traversable;
                        current[k, l].xIdx = k - xStart;
                        current[k, l].yIdx = l - yStart;
                        current[k, l].enemy = current[k, l].enemy;
                        current[k, l].chest= current[k, l].chest;
                    }
                }

                cells[i, j] = current;
            }
        }
        return cells;
    }

    void DrawBigGrid(CellularAutomataInternal.CellState[,] cells, int dimensions, bool textured)
    {
        for (int i = 0; i < dimensions; i++)
        {
            for (int j = 0; j < dimensions; j++)
            {
                GameObject current = Instantiate(tile);
                var spr = current.GetComponent<SpriteRenderer>();

                current.transform.position = new Vector3(i,  j, 0);
                if (textured)
                {
                    spr.sprite = cells[i, j].traversable ? traversableSprite : untraversableSprite;
                }
                else
                {
                    spr.color = cells[i, j].traversable ? Color.white : Color.black;
                    spr.color = cells[i, j].spawnEnemy ? Color.red : spr.color;
                    spr.color = cells[i, j].spawnChest ? Color.green: spr.color;
                }
                
                if (!cells[i, j].traversable)
                {
                    current.AddComponent<BoxCollider2D>();
                }
            }
        }
    }
    
    void FinalizeEnemies(ref CellularAutomataInternal.CellState[,] cells, int dimensions)
    {
        for (int i = 0; i < dimensions; i++)
        {
            for (int j = 0; j < dimensions; j++)
            {
                // if we dont have a perfect enemy spawn
                // move on
                if (cells[i, j].enemy < 7)
                {
                    cells[i, j].spawnEnemy = false;
                    continue;
                }

                cells[i, j].spawnEnemy = Random.Range(0, dimensions) > j && Random.Range(0, 2) == 1;
            }
        }
    }
    
    void FinalizeChests(ref CellularAutomataInternal.CellState[,] cells, int dimensions)
    {
        for (int i = 0; i < dimensions; i++)
        {
            for (int j = 0; j < dimensions; j++)
            {
                // if we dont have a perfect chest spawn
                // move on
                if (cells[i, j].chest < 5)
                {
                    cells[i, j].spawnChest = false;
                    continue;
                }

                cells[i, j].spawnChest = Random.Range(0, dimensions) > j && Random.Range(0, 2) == 1;
            }
        }
    }
 
    
    Bias numberToBias(int num)
    {
        switch (num)
        {
            case 0:
                return Bias.None;
            case 1:
                return Bias.LeftRight;
            case 2:
                return Bias.DownLeftRight;
            case 3:
                return Bias.UpLeftRight;
            default:
                throw new InvalidEnumArgumentException();
        }
    }
   
}
