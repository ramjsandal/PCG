using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpelunkyPathInternal  
{
    
    int[,] GenerateRooms(int dimensions)
    {
        int[,] roomTypes = new int[dimensions, dimensions];
                // Fill the room array with empty's (0)
        for (int i = 0; i < dimensions; i++)
        {
            for (int j = 0; j < dimensions; j++)
            {
                roomTypes[i, j] = 0;
            }
        }

        return roomTypes;
    }
    
    // modifies the given roomTypes to generate the starting row
    void GenerateStartRow(ref int[,] roomTypes, int dimensions)
    {
       // 1. Pick a room in the top row to be the start 
       int startRoom = Random.Range(0, dimensions - 1);

       roomTypes[0, startRoom] = 1;
       
       // 2. Pick left or right, -1 is left 1 is right
       int direction = Random.Range(1, 2) == 1 ? -1 : 1;
       if (startRoom == 0)
       {
           direction = 1;
       } else if (startRoom == dimensions - 1)
       {
           direction = -1;
       }

       int currentRoom = startRoom + direction;

       while (currentRoom > 0 && currentRoom < dimensions - 1)
       {
           if (Random.Range(1, dimensions) == 3)
           {
               break;
           }
           roomTypes[0, currentRoom] = 1;
           currentRoom += direction;
       }

       roomTypes[0, currentRoom] = 3;

       GenerateRow(ref roomTypes, dimensions, 1, currentRoom);
    }
    
    
    void GenerateRow(ref int[,] roomTypes, int dimensions, int startRow, int startCol)
    {
        if (startRow == dimensions)
        {
            return; 
        }
        
       //1. Set the starting tile to 3 so we can drop down into it
       // LIES we acutally build up so we need 2 so we cna 'jump up'
       roomTypes[startRow, startCol] = 2;

       // 2. Pick left or right, -1 is left 1 is right
       int direction = Random.Range(1, 2) == 1 ? -1 : 1;
       int column = startCol;
       if (column == 0)
       {
           direction = 1;
       } else if (column == dimensions - 1)
       {
           direction = -1;
       }
       
       int currentRoom = column + direction;
       while (currentRoom > 0 && currentRoom < dimensions - 1)
       {
           if (Random.Range(1, dimensions) == 3)
           {
               break;
           }
           roomTypes[startRow, currentRoom] = 1;
           currentRoom += direction;
       }

       roomTypes[startRow, currentRoom] = 3;

       GenerateRow(ref roomTypes, dimensions, startRow + 1, currentRoom);
    }
    
    public int[,] GenerateLayout(int dimensions)
    {
        int[,] rooms = GenerateRooms(dimensions);
        GenerateStartRow(ref rooms, dimensions);
        return rooms;
    }

    public void DrawTextLayout(int[,] layout, int dimensions)
    {
        string total = "";
        for (int i = 0; i < dimensions; i++)
        {
            string currentLine = "";
            for (int j = 0; j < dimensions; j++)
            {
                currentLine += layout[i, j] + ", ";
            }

            currentLine += "\n";
            total += currentLine;
        }
        Debug.Log(total);
    }

}
