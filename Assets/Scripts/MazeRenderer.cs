﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class MazeRenderer : MonoBehaviour
{

    [SerializeField]
    [Range(1, 50)]
    private int width = 10;

    [SerializeField]
    [Range(1, 50)]
    private int height = 10;

    private float size = 1f;

    [SerializeField]
    private Transform wallPrefab = null;

    [SerializeField]
    private Transform floorPrefab = null;

    [SerializeField]
    private Transform playerPrefab = null;
    [SerializeField]
    private Transform goalPrefab = null;
    [SerializeField]
    public bool loadPrevPrefab = false;
    [SerializeField]
    WallState[,] theMaze;

    // Start is called before the first frame update
    void Start()
    {
        if(loadPrevPrefab == false)
        {
            theMaze = MazeGenerator.Generate(width, height);
            Draw(theMaze);
            ObjectSerialize(theMaze,"Assets/Maze","mazeV1","cool");
        }
        else
        {
            theMaze = DeSerialization("Assets/Maze", "mazeV1", "cool");
            Draw(theMaze);  
        }

    }

    public static void ObjectSerialize(object obj,string path, string filename, string ext)
    {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(path + "/" + filename + ext);
            bf.Serialize(file, obj);
            file.Close();
    }
    public WallState[,] DeSerialization(string path, string filename, string ext)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(path + "/" + filename + ext, FileMode.Open);
        WallState[,] deserialized;
        if (file != null && file.Length > 0)
        {
            deserialized = (WallState[,])bf.Deserialize(file);
        }
        else
        {
            deserialized = default(WallState[,]);
        }

        file.Close();
        return deserialized;
    }

    private void Draw(WallState[,] maze)
    {

        var floor = Instantiate(floorPrefab, transform);
        Instantiate(playerPrefab,transform);
        Instantiate(goalPrefab,new Vector3((width-1)/2,0.25f,(height-1)/2), Quaternion.identity);
        floor.localScale = new Vector3(width,1, height);

        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                var cell = maze[i, j];
                var position = new Vector3(-width / 2 + i, 0, -height / 2 + j);

                if (cell.HasFlag(WallState.UP))
                {
                    var topWall = Instantiate(wallPrefab, transform) as Transform;
                    topWall.position = position + new Vector3(0, 0, size / 2);
                    topWall.localScale = new Vector3(size, topWall.localScale.y, topWall.localScale.z);
                }

                if (cell.HasFlag(WallState.LEFT))
                {
                    var leftWall = Instantiate(wallPrefab, transform) as Transform;
                    leftWall.position = position + new Vector3(-size / 2, 0, 0);
                    leftWall.localScale = new Vector3(size, leftWall.localScale.y, leftWall.localScale.z);
                    leftWall.eulerAngles = new Vector3(0, 90, 0);
                }

                if (i == width - 1)
                {
                    if (cell.HasFlag(WallState.RIGHT))
                    {
                        var rightWall = Instantiate(wallPrefab, transform) as Transform;
                        rightWall.position = position + new Vector3(+size / 2, 0, 0);
                        rightWall.localScale = new Vector3(size, rightWall.localScale.y, rightWall.localScale.z);
                        rightWall.eulerAngles = new Vector3(0, 90, 0);
                    }
                }

                if (j == 0)
                {
                    if (cell.HasFlag(WallState.DOWN))
                    {
                        var bottomWall = Instantiate(wallPrefab, transform) as Transform;
                        bottomWall.position = position + new Vector3(0, 0, -size / 2);
                        bottomWall.localScale = new Vector3(size, bottomWall.localScale.y, bottomWall.localScale.z);
                    }
                }
            }

        }

    }

    // Update is called once per frame
    void Update()
    {

    }
}
