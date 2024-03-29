using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateLevel : MonoBehaviour
{
    [SerializeField] private GameObject tile;
    private CreateSpelunkyPath _path;
    private CellularAutomataInternal _nodeGenerator;
    void Start()
    {
        _nodeGenerator = new CellularAutomataInternal();
        GenerateAndDrawCell(50, 4, Bias.LeftRight, 55, 45, 0, 0);
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

    private int[,] GenerateLayout()
    {
        return _path.GenerateLayout(10);
    }
    
    public void DrawCell(CellularAutomataInternal.CellState[,] map, int dimensions, int xStart, int yStart)
    {
        for (int i = 0 + xStart; i < dimensions + xStart; i++)
        {
            for (int j = 0 + yStart; j < dimensions + yStart; j++)
            {
                GameObject current = Instantiate(tile);
                var spr = current.GetComponent<SpriteRenderer>();

                current.transform.position = new Vector3(i, j, 0);
                spr.color = map[i, j].traversable ? Color.white : Color.black;
            }
        }
    }
    
}
