using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellularAutomata : MonoBehaviour
{
    [SerializeField] private GameObject tile;
    [SerializeField] private int dimensions;
    [SerializeField] private Camera cam;
    private float time;
    private GameObject[,] _tiles;
    private bool _finished;

    // Start is called before the first frame update
    void Start()
    {
        time = 0;
        _finished = false;
        _tiles = new GameObject[dimensions,dimensions];
        cam.transform.position = new Vector3(dimensions / 2.0f, dimensions / 2.0f,-10);
        System.Random rand = new System.Random();
        for (int i = 0; i < dimensions; i++)
        {
            for (int j = 0; j < dimensions; j++)
            {
                _tiles[i, j] = Instantiate(tile);
                _tiles[i, j].transform.position = new Vector3(i, j, 0);
                if (rand.Next() % 2 == 1)
                {
                    _tiles[i, j].GetComponent<SpriteRenderer>().color = Color.black;
                }
                else
                {
                    _tiles[i, j].GetComponent<SpriteRenderer>().color = Color.white;
                }
            }
            
        }
    }

    List<GameObject> GetNeighbors(GameObject currentTile)
    {
        List<GameObject> neighbors = new List<GameObject>();
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
                GameObject currentCell = _tiles[i, j];
                List<GameObject> neighbors = GetNeighbors(currentCell);
                int T = 0;
                for (int k = 0; k < neighbors.Count; k++)
                {
                    if (neighbors[k].GetComponent<SpriteRenderer>().color == Color.black)
                    {
                        T++;
                    }
                }

                if (T >= 5.0f)
                {
                    currentCell.GetComponent<SpriteRenderer>().color = Color.black;
                } else if (neighbors.Count - T >= 5)
                {
                    currentCell.GetComponent<SpriteRenderer>().color = Color.white;
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
                if (_tiles[i, j].GetComponent<SpriteRenderer>().color != Color.black)
                {
                    continue;
                }
                List<GameObject> neighbors = GetNeighbors(_tiles[i,j]);
                if (neighbors.FindIndex(a => a.GetComponent<SpriteRenderer>().color == Color.white) != -1)
                {
                    // this should be a wall
                    _tiles[i,j].GetComponent<SpriteRenderer>().color = Color.gray;
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
}
