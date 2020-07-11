using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [Header("Level setup:")]
    public LevelData data;
    private const float CAMERA_OFFSET= -0.5F;

    private void Start()
    {
        GameObject board = GameObject.FindGameObjectWithTag("Board");
        SetupTiles(board);
    }

    public void SetupTiles(GameObject board)
    {
        if (board == null)
            return;

        float bottom_x = -data.boardWidth / 2;
        float bottom_y = -data.boardHeight / 2;

        for (int i = 0; i < data.boardHeight; i++)
        {
            for (int j = 0; j < data.boardWidth; j++)
            {
                Vector3 target = new Vector3(bottom_x + j, bottom_y + i, -2);
                board.transform.GetChild(j + i * data.boardWidth).gameObject.transform.localPosition = target;
            }
        }

        SetupCamera();
    }

    private void SetupCamera()
    {
        //camera setup
        Camera.main.orthographicSize = data.boardWidth;
        Vector3 pos = Camera.main.transform.position;
        Camera.main.transform.position = new Vector3(CAMERA_OFFSET + (float)data.boardWidth % 2/ 2, pos.y, pos.z);

        //mask setup
        Camera.main.transform.GetChild(0).transform.localScale = new Vector3(data.boardWidth, data.boardHeight, 1);
    }


}
