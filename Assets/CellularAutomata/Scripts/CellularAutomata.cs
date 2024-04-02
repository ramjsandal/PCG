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
    [SerializeField] private GameObject tile;
    [SerializeField] private int dimensions;
    [SerializeField] private Camera cam;
    [SerializeField] private Bias bias;
    [SerializeField] private int inBiasPercent;
    [SerializeField] private int outBiasPercent;
    [SerializeField] private float rowPercent;
    [SerializeField] private CellularAutomataInternal.Side side;
    private float time;
    private bool _finished;
    private CellularAutomataInternal _cellularAutomataInternal;
    private CellularAutomataInternal.CellState[,] _cellStates;
    private GameObject[,] _visualCells;
    private SpriteRenderer[,] _spriteRenderers;



    // Start is called before the first frame update
    void Start()
    {
        _visualCells = new GameObject[dimensions, dimensions];
        _spriteRenderers = new SpriteRenderer[dimensions, dimensions];
        for (int i = 0; i < dimensions; i++)
        {
            for (int j = 0; j < dimensions; j++)
            {
                _visualCells[i, j] = Instantiate(tile);
                _visualCells[i, j].transform.position = new Vector3(i, j, 0);
                _spriteRenderers[i, j] = _visualCells[i, j].GetComponent<SpriteRenderer>();
            }
        }
        _cellularAutomataInternal = new CellularAutomataInternal();
        time = 0;
        _finished = false;
        cam.transform.position = new Vector3(dimensions / 2.0f, dimensions / 2.0f, -10);
        _cellStates = _cellularAutomataInternal.GenerateInitial(dimensions, bias, inBiasPercent, outBiasPercent, CreateRow(rowPercent), side);
        DrawCells();

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
                _cellularAutomataInternal.CleanEdges(ref _cellStates, dimensions);
                DrawCells();
            }

            _finished = true;
        }

        if (time > 2)
        {
            time = 0;
            _cellularAutomataInternal.ApplyRule(ref _cellStates, dimensions);
            DrawCells();
        }
    }

    private void DrawCells()
    {
        for (int i = 0; i < dimensions; i++)
        {
            for (int j = 0; j < dimensions; j++)
            {
                _spriteRenderers[i, j].color = _cellStates[i, j].traversable ? Color.white : Color.black;
            }
        }
    }

    private bool[] CreateRow(float middlePercent = 0.66f)
    {
        bool[] ret = new bool[dimensions];
        for (int i = 0; i < dimensions; i++)
        {
            ret[i] = (i >= dimensions - (middlePercent * dimensions) && i <= (middlePercent * dimensions));
        }

        return ret;
    }
}