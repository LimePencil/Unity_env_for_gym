using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class MazeRenderer : MonoBehaviour
{

    [Range(1, 50)]
    public int width = 10;

    [Range(1, 50)]
    public int height = 10;

    public int seed = 0;
    private float size = 1f;


    [SerializeField]
    private Transform wallPrefab = null;

    [SerializeField]
    private Transform floorPrefab = null;

    [SerializeField]
    private Transform playerPrefab = null;




    [SerializeField]
    WallState[,] theMaze;

    // Start is called before the first frame update
    void Start()
    {
        theMaze = MazeGenerator.Generate(width, height, seed);
        Draw(theMaze);
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
        Random.InitState(seed);
        var floor = Instantiate(floorPrefab, transform, false);
        Instantiate(playerPrefab,transform,false);
        floor.localScale = new Vector3(width/4.0f,1, height/4.0f);

        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                var cell = maze[i, j];
                var position = new Vector3(-width / 2 + i, 0, -height / 2 + j);

                if (cell.HasFlag(WallState.UP))
                {
                    var topWall = Instantiate(wallPrefab, transform) as Transform;
                    topWall.localPosition = position + new Vector3(0, 0, size / 2);
                    topWall.localScale = new Vector3(size, topWall.localScale.y, topWall.localScale.z);
                    topWall.GetComponent<Renderer>().material.color = new Color(Random.value, Random.value, Random.value);
                }

                if (cell.HasFlag(WallState.LEFT))
                {
                    var leftWall = Instantiate(wallPrefab, transform) as Transform;
                    leftWall.localPosition = position + new Vector3(-size / 2, 0, 0);
                    leftWall.localScale = new Vector3(size, leftWall.localScale.y, leftWall.localScale.z);
                    leftWall.eulerAngles = new Vector3(0, 90, 0);
                    leftWall.GetComponent<Renderer>().material.color = new Color(Random.value, Random.value, Random.value);
                }

                if (i == width - 1)
                {
                    if (cell.HasFlag(WallState.RIGHT))
                    {
                        var rightWall = Instantiate(wallPrefab, transform) as Transform;
                        rightWall.localPosition = position + new Vector3(+size / 2, 0, 0);
                        rightWall.localScale = new Vector3(size, rightWall.localScale.y, rightWall.localScale.z);
                        rightWall.eulerAngles = new Vector3(0, 90, 0);
                        rightWall.GetComponent<Renderer>().material.color = new Color(Random.Range(0, 255), Random.value, Random.value);

                    }
                }

                if (j == 0)
                {
                    if (cell.HasFlag(WallState.DOWN))
                    {
                        var bottomWall = Instantiate(wallPrefab, transform) as Transform;
                        bottomWall.localPosition = position + new Vector3(0, 0, -size / 2);
                        bottomWall.localScale = new Vector3(size, bottomWall.localScale.y, bottomWall.localScale.z);
                        bottomWall.GetComponent<Renderer>().material.color = new Color(Random.value, Random.value, Random.value);
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

