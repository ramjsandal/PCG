using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class GenerateLevel : MonoBehaviour
{
    [SerializeField] private GameObject tile;
    private SpelunkyPathInternal _path;
    private CellularAutomataInternal _nodeGenerator;
    void Start()
    {
        _nodeGenerator = new CellularAutomataInternal();
        _path = new SpelunkyPathInternal();
        /*
        GenerateAndDrawCell(100, 4, Bias.LeftRight, 55, 45, 0, 0);
        GenerateAndDrawCell(100, 4, Bias.DownLeftRight, 55, 45, 100, 0);
        */
        CreateMap(4, 100);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void GenerateAndDrawCell(int dimensions, int generations, Bias bias, int inBiasPercent, int outBiasPercent, int xStart, int yStart)
    {
        CellularAutomataInternal.CellState[,] nodeOne = _nodeGenerator.GenerateArea(dimensions, generations, bias, inBiasPercent, outBiasPercent);
        DrawCell(nodeOne, dimensions, xStart, yStart);
 
    }

    CellularAutomataInternal.CellState[,] GenerateCell(int dimensions, int generations, Bias b, int inBiasPercent, int outBiasPercent, (bool[], CellularAutomataInternal.Side)[] sides)
    {
        return _nodeGenerator.GenerateArea(dimensions, generations, b, inBiasPercent, outBiasPercent, sides);
    }

    private int[,] GenerateLayout(int dimensions)
    {
        return _path.GenerateLayout(dimensions);
    }

    void CreateMap(int mapDimensions, int nodeDimensions, int generations = 5, int inBiasPercent = 56, int outBiasPercent = 44)
    {
        CellularAutomataInternal.CellState[,][,] cells = new CellularAutomataInternal.CellState[nodeDimensions,nodeDimensions][,];
        int[,] layout = GenerateLayout(mapDimensions);
        _path.DrawTextLayout(layout, mapDimensions);
        for (int i = 0; i < mapDimensions; i++)
        {
            for (int j = 0; j < mapDimensions; j++)
            {
                (bool[], CellularAutomataInternal.Side)[] sides = new (bool[], CellularAutomataInternal.Side)[4];
                int x = j;
                int y = (mapDimensions - 1 - i);
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
                
                cells[i, j] = GenerateCell(nodeDimensions, generations, numberToBias(layout[i, j]), inBiasPercent,
                    outBiasPercent, sides);
            }
        }
        
        for (int i = 0; i < mapDimensions; i++)
        {
            for (int j = 0; j < mapDimensions; j++)
            {
                DrawCell(cells[i,j], nodeDimensions, j * nodeDimensions, ((mapDimensions - 1 - i) * nodeDimensions));
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
    
    public void DrawCell(CellularAutomataInternal.CellState[,] map, int dimensions, int xStart, int yStart)
    {
        for (int i = 0; i < dimensions; i++)
        {
            for (int j = 0; j < dimensions; j++)
            {
                GameObject current = Instantiate(tile);
                var spr = current.GetComponent<SpriteRenderer>();

                current.transform.position = new Vector3(i + xStart,  yStart + j, 0);
                spr.color = map[i, j].traversable ? Color.white : Color.black;
            }
        }
    }
    
}
