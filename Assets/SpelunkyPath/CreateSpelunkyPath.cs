using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateSpelunkyPath : MonoBehaviour
{
    // SPELUNKY PATH ALGORITHM
    // 1. Create a grid of "rooms"
    // 2. Pick one of the rooms in the top row to be the start
    // 3. Pick a direction: left or right
    // 4. Repeat choosing between moving in the chosen direction or dropping down.
        // If you hit an edge, immediately drop down
            // If a node is not along the chosen path, we assign it to zero
            // If a node is on the chosen path, we assign it a nonzero value
                // Openings on left and right side: 1
                // Openings on left and right and bottom: 2
                // Openings on left and right and top: 3


    private int[,] _roomTypes;
    private GameObject[,] _tiles;
    private int _dimensions;
    [SerializeField] private Camera cam;

    [SerializeField] private GameObject tile;
    // Start is called before the first frame update
    void Start()
    {
       GenerateRooms(10); 
       GenerateStartRow();
       DrawRooms();
       cam.transform.position = new Vector3(_dimensions / 2.0f, _dimensions / 2.0f,-100);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GenerateRooms(int dimensions)
    {
        _dimensions = dimensions;
        _roomTypes = new int[_dimensions, _dimensions];
                // Fill the room array with empty's (0)
        for (int i = 0; i < _dimensions; i++)
        {
            for (int j = 0; j < _dimensions; j++)
            {
                _roomTypes[i, j] = 0;
            }
        }
        
    }

    void GenerateStartRow()
    {
       // 1. Pick a room in the top row to be the start 
       int startRoom = Random.Range(0, _dimensions - 1);

       _roomTypes[0, startRoom] = 1;
       
       // 2. Pick left or right, -1 is left 1 is right
       int direction = Random.Range(1, 2) == 1 ? -1 : 1;
       if (startRoom == 0)
       {
           direction = 1;
       } else if (startRoom == _dimensions - 1)
       {
           direction = -1;
       }

       int currentRoom = startRoom + direction;

       while (currentRoom > 0 && currentRoom < _dimensions - 1)
       {
           if (Random.Range(1, 3) == 3)
           {
               break;
           }
           _roomTypes[0, currentRoom] = 1;
           currentRoom += direction;
       }

       _roomTypes[0, currentRoom] = 2;

       GenerateRow(1, currentRoom);
    }

    void GenerateRow(int startRow, int startCol)
    {
        if (startRow == _dimensions)
        {
            return; 
        }
       //1. Set the starting tile to 3 so we can drop down into it
       _roomTypes[startRow, startCol] = 3;

       // 2. Pick left or right, -1 is left 1 is right
       int direction = Random.Range(1, 2) == 1 ? -1 : 1;
       int column = startCol;
       if (column == 0)
       {
           direction = 1;
       } else if (column == _dimensions - 1)
       {
           direction = -1;
       }
       
       int currentRoom = column + direction;
       while (currentRoom > 0 && currentRoom < _dimensions - 1)
       {
           if (Random.Range(1, 5) == 3)
           {
               break;
           }
           _roomTypes[startRow, currentRoom] = 1;
           currentRoom += direction;
       }

       _roomTypes[startRow, currentRoom] = 2;

       GenerateRow(startRow + 1, currentRoom);
    }

    void DrawRooms()
    {
        _tiles = new GameObject[_dimensions, _dimensions];
        // Fill the room array with empty's (0)
        for (int i = 0; i < _dimensions; i++)
        {
            for (int j = 0; j < _dimensions; j++)
            {
               _tiles[i, j] = Instantiate(tile);
               _tiles[i, j].transform.position = new Vector3(j, i, 0);
               if (_roomTypes[i,j] == 0)
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
}
