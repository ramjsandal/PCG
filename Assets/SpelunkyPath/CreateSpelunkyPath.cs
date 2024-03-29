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


    private SpelunkyPathInternal _path;
    [SerializeField] private Camera cam;

    [SerializeField] private GameObject tile;

    [SerializeField] private Sprite leftRight;

    [SerializeField] private Sprite upLeftRight;

    [SerializeField] private Sprite downLeftRight;
    // Start is called before the first frame update
    void Start()
    {
        _path = new SpelunkyPathInternal();
        int _dimensions = 10;
        int[,] layout = GenerateLayout(_dimensions);
        DrawRooms(layout, _dimensions);
        cam.transform.position = new Vector3(_dimensions / 2.0f, (_dimensions % 2 == 0) ? (_dimensions / 2.0f) - .5f : (_dimensions / 2.0f) + .5f,-200);
        cam.orthographicSize = _dimensions / 2.0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DrawRooms(int[,] layout, int dimensions)
    {
        GameObject[,] tiles = new GameObject[dimensions, dimensions];
        // Fill the room array with empty's (0)
        for (int i = 0; i < dimensions; i++)
        {
            for (int j = 0; j < dimensions; j++)
            {
               tiles[i, j] = Instantiate(tile);
               tiles[i, j].transform.position = new Vector3(j, i, 0);
               switch (layout[i,j]) 
               {
                  case 0:
                      tiles[i, j].GetComponent<SpriteRenderer>().color = Color.black;
                      break;
                  case 1:
                      tiles[i, j].GetComponent<SpriteRenderer>().sprite = leftRight;
                      break;
                  case 2:
                      tiles[i, j].GetComponent<SpriteRenderer>().sprite = downLeftRight;
                      break;
                  case 3:
                      tiles[i, j].GetComponent<SpriteRenderer>().sprite = upLeftRight;
                      break;
               }
            }
        }
    }

    public int[,] GenerateLayout(int dimensions)
    {
      return  _path.GenerateLayout(dimensions);
    } 
}
