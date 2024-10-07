using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridMap : MonoBehaviour
{
    private GameObject gridBlock;
    private int width;
    private int height;
    private float cellSize;
    private int[,] gridArray;

    public GridMap(int width, int height, GameObject gridBlock, float cellSize) {
        this.width = width;
        this.height = height;
        this.gridBlock = gridBlock;
        this.cellSize = cellSize;

        gridArray = new int [width, height];

        for (int x = 0; x<gridArray.GetLength(0); x++)
        {
            for (int y=0; y< gridArray.GetLength(1); y++)
            {

                Debug.Log(GetHexWorldPostition(new Vector2Int(x,y)));
                Instantiate(gridBlock, GetHexWorldPostition(new Vector2Int(x, y)), Quaternion.identity);
            }
        }
    }
    private Vector3 GetWorldPostition(int x, int y)
    {
        return new Vector3(x, 0, y) * cellSize;
    }
    private Vector3 GetHexWorldPostition(Vector2Int coordinate) {
        int column = coordinate.x;
        int row = coordinate.y;
        float width;
        float height;
        float xPosition;
        float yPosition;
        bool shouldOffset;
        float horizontalDistance;
        float verticalDistance;
        float offset;
        float size = cellSize;

        ///
        shouldOffset = (row % 2) == 0;
        width = Mathf.Sqrt(3) * size;
        height = 2f * size;
        horizontalDistance = width;
        verticalDistance = height * (3f / 4f);

        offset = (shouldOffset) ? width/2 :0;

        xPosition = (column * (horizontalDistance)) + offset;
        yPosition = (row * (verticalDistance));


        return new Vector3(xPosition, 0, -yPosition);
    }

}
