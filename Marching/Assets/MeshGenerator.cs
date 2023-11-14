using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SocialPlatforms.GameCenter;
using UnityEngine.UIElements;

public struct Cube
{
    public int cubeState;
    public int[] position;
    public int[][] cornerPositions;

    public Cube(int x, int y, int z)
    {
        cubeState = 0;
        position = new int[3] { x, y, z };
        //Debug.Log(x + "-" + y + "-" + z);
        cornerPositions = new int[8][];
    }
};

struct triangle
{
    public Vector3 point1;
    public Vector3 point2;
    public Vector3 point3;
}

public class MeshGenerator : MonoBehaviour
{

    int[,] triTable = new int[256,16]
{{-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{0, 8, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{0, 1, 9, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{1, 8, 3, 9, 8, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{1, 2, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{0, 8, 3, 1, 2, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{9, 2, 10, 0, 2, 9, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{2, 8, 3, 2, 10, 8, 10, 9, 8, -1, -1, -1, -1, -1, -1, -1},
{3, 11, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{0, 11, 2, 8, 11, 0, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{1, 9, 0, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{1, 11, 2, 1, 9, 11, 9, 8, 11, -1, -1, -1, -1, -1, -1, -1},
{3, 10, 1, 11, 10, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{0, 10, 1, 0, 8, 10, 8, 11, 10, -1, -1, -1, -1, -1, -1, -1},
{3, 9, 0, 3, 11, 9, 11, 10, 9, -1, -1, -1, -1, -1, -1, -1},
{9, 8, 10, 10, 8, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{4, 7, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{4, 3, 0, 7, 3, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{0, 1, 9, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{4, 1, 9, 4, 7, 1, 7, 3, 1, -1, -1, -1, -1, -1, -1, -1},
{1, 2, 10, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{3, 4, 7, 3, 0, 4, 1, 2, 10, -1, -1, -1, -1, -1, -1, -1},
{9, 2, 10, 9, 0, 2, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1},
{2, 10, 9, 2, 9, 7, 2, 7, 3, 7, 9, 4, -1, -1, -1, -1},
{8, 4, 7, 3, 11, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{11, 4, 7, 11, 2, 4, 2, 0, 4, -1, -1, -1, -1, -1, -1, -1},
{9, 0, 1, 8, 4, 7, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1},
{4, 7, 11, 9, 4, 11, 9, 11, 2, 9, 2, 1, -1, -1, -1, -1},
{3, 10, 1, 3, 11, 10, 7, 8, 4, -1, -1, -1, -1, -1, -1, -1},
{1, 11, 10, 1, 4, 11, 1, 0, 4, 7, 11, 4, -1, -1, -1, -1},
{4, 7, 8, 9, 0, 11, 9, 11, 10, 11, 0, 3, -1, -1, -1, -1},
{4, 7, 11, 4, 11, 9, 9, 11, 10, -1, -1, -1, -1, -1, -1, -1},
{9, 5, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{9, 5, 4, 0, 8, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{0, 5, 4, 1, 5, 0, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{8, 5, 4, 8, 3, 5, 3, 1, 5, -1, -1, -1, -1, -1, -1, -1},
{1, 2, 10, 9, 5, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{3, 0, 8, 1, 2, 10, 4, 9, 5, -1, -1, -1, -1, -1, -1, -1},
{5, 2, 10, 5, 4, 2, 4, 0, 2, -1, -1, -1, -1, -1, -1, -1},
{2, 10, 5, 3, 2, 5, 3, 5, 4, 3, 4, 8, -1, -1, -1, -1},
{9, 5, 4, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{0, 11, 2, 0, 8, 11, 4, 9, 5, -1, -1, -1, -1, -1, -1, -1},
{0, 5, 4, 0, 1, 5, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1},
{2, 1, 5, 2, 5, 8, 2, 8, 11, 4, 8, 5, -1, -1, -1, -1},
{10, 3, 11, 10, 1, 3, 9, 5, 4, -1, -1, -1, -1, -1, -1, -1},
{4, 9, 5, 0, 8, 1, 8, 10, 1, 8, 11, 10, -1, -1, -1, -1},
{5, 4, 0, 5, 0, 11, 5, 11, 10, 11, 0, 3, -1, -1, -1, -1},
{5, 4, 8, 5, 8, 10, 10, 8, 11, -1, -1, -1, -1, -1, -1, -1},
{9, 7, 8, 5, 7, 9, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{9, 3, 0, 9, 5, 3, 5, 7, 3, -1, -1, -1, -1, -1, -1, -1},
{0, 7, 8, 0, 1, 7, 1, 5, 7, -1, -1, -1, -1, -1, -1, -1},
{1, 5, 3, 3, 5, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{9, 7, 8, 9, 5, 7, 10, 1, 2, -1, -1, -1, -1, -1, -1, -1},
{10, 1, 2, 9, 5, 0, 5, 3, 0, 5, 7, 3, -1, -1, -1, -1},
{8, 0, 2, 8, 2, 5, 8, 5, 7, 10, 5, 2, -1, -1, -1, -1},
{2, 10, 5, 2, 5, 3, 3, 5, 7, -1, -1, -1, -1, -1, -1, -1},
{7, 9, 5, 7, 8, 9, 3, 11, 2, -1, -1, -1, -1, -1, -1, -1},
{9, 5, 7, 9, 7, 2, 9, 2, 0, 2, 7, 11, -1, -1, -1, -1},
{2, 3, 11, 0, 1, 8, 1, 7, 8, 1, 5, 7, -1, -1, -1, -1},
{11, 2, 1, 11, 1, 7, 7, 1, 5, -1, -1, -1, -1, -1, -1, -1},
{9, 5, 8, 8, 5, 7, 10, 1, 3, 10, 3, 11, -1, -1, -1, -1},
{5, 7, 0, 5, 0, 9, 7, 11, 0, 1, 0, 10, 11, 10, 0, -1},
{11, 10, 0, 11, 0, 3, 10, 5, 0, 8, 0, 7, 5, 7, 0, -1},
{11, 10, 5, 7, 11, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{10, 6, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{0, 8, 3, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{9, 0, 1, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{1, 8, 3, 1, 9, 8, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1},
{1, 6, 5, 2, 6, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{1, 6, 5, 1, 2, 6, 3, 0, 8, -1, -1, -1, -1, -1, -1, -1},
{9, 6, 5, 9, 0, 6, 0, 2, 6, -1, -1, -1, -1, -1, -1, -1},
{5, 9, 8, 5, 8, 2, 5, 2, 6, 3, 2, 8, -1, -1, -1, -1},
{2, 3, 11, 10, 6, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{11, 0, 8, 11, 2, 0, 10, 6, 5, -1, -1, -1, -1, -1, -1, -1},
{0, 1, 9, 2, 3, 11, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1},
{5, 10, 6, 1, 9, 2, 9, 11, 2, 9, 8, 11, -1, -1, -1, -1},
{6, 3, 11, 6, 5, 3, 5, 1, 3, -1, -1, -1, -1, -1, -1, -1},
{0, 8, 11, 0, 11, 5, 0, 5, 1, 5, 11, 6, -1, -1, -1, -1},
{3, 11, 6, 0, 3, 6, 0, 6, 5, 0, 5, 9, -1, -1, -1, -1},
{6, 5, 9, 6, 9, 11, 11, 9, 8, -1, -1, -1, -1, -1, -1, -1},
{5, 10, 6, 4, 7, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{4, 3, 0, 4, 7, 3, 6, 5, 10, -1, -1, -1, -1, -1, -1, -1},
{1, 9, 0, 5, 10, 6, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1},
{10, 6, 5, 1, 9, 7, 1, 7, 3, 7, 9, 4, -1, -1, -1, -1},
{6, 1, 2, 6, 5, 1, 4, 7, 8, -1, -1, -1, -1, -1, -1, -1},
{1, 2, 5, 5, 2, 6, 3, 0, 4, 3, 4, 7, -1, -1, -1, -1},
{8, 4, 7, 9, 0, 5, 0, 6, 5, 0, 2, 6, -1, -1, -1, -1},
{7, 3, 9, 7, 9, 4, 3, 2, 9, 5, 9, 6, 2, 6, 9, -1},
{3, 11, 2, 7, 8, 4, 10, 6, 5, -1, -1, -1, -1, -1, -1, -1},
{5, 10, 6, 4, 7, 2, 4, 2, 0, 2, 7, 11, -1, -1, -1, -1},
{0, 1, 9, 4, 7, 8, 2, 3, 11, 5, 10, 6, -1, -1, -1, -1},
{9, 2, 1, 9, 11, 2, 9, 4, 11, 7, 11, 4, 5, 10, 6, -1},
{8, 4, 7, 3, 11, 5, 3, 5, 1, 5, 11, 6, -1, -1, -1, -1},
{5, 1, 11, 5, 11, 6, 1, 0, 11, 7, 11, 4, 0, 4, 11, -1},
{0, 5, 9, 0, 6, 5, 0, 3, 6, 11, 6, 3, 8, 4, 7, -1},
{6, 5, 9, 6, 9, 11, 4, 7, 9, 7, 11, 9, -1, -1, -1, -1},
{10, 4, 9, 6, 4, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{4, 10, 6, 4, 9, 10, 0, 8, 3, -1, -1, -1, -1, -1, -1, -1},
{10, 0, 1, 10, 6, 0, 6, 4, 0, -1, -1, -1, -1, -1, -1, -1},
{8, 3, 1, 8, 1, 6, 8, 6, 4, 6, 1, 10, -1, -1, -1, -1},
{1, 4, 9, 1, 2, 4, 2, 6, 4, -1, -1, -1, -1, -1, -1, -1},
{3, 0, 8, 1, 2, 9, 2, 4, 9, 2, 6, 4, -1, -1, -1, -1},
{0, 2, 4, 4, 2, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{8, 3, 2, 8, 2, 4, 4, 2, 6, -1, -1, -1, -1, -1, -1, -1},
{10, 4, 9, 10, 6, 4, 11, 2, 3, -1, -1, -1, -1, -1, -1, -1},
{0, 8, 2, 2, 8, 11, 4, 9, 10, 4, 10, 6, -1, -1, -1, -1},
{3, 11, 2, 0, 1, 6, 0, 6, 4, 6, 1, 10, -1, -1, -1, -1},
{6, 4, 1, 6, 1, 10, 4, 8, 1, 2, 1, 11, 8, 11, 1, -1},
{9, 6, 4, 9, 3, 6, 9, 1, 3, 11, 6, 3, -1, -1, -1, -1},
{8, 11, 1, 8, 1, 0, 11, 6, 1, 9, 1, 4, 6, 4, 1, -1},
{3, 11, 6, 3, 6, 0, 0, 6, 4, -1, -1, -1, -1, -1, -1, -1},
{6, 4, 8, 11, 6, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{7, 10, 6, 7, 8, 10, 8, 9, 10, -1, -1, -1, -1, -1, -1, -1},
{0, 7, 3, 0, 10, 7, 0, 9, 10, 6, 7, 10, -1, -1, -1, -1},
{10, 6, 7, 1, 10, 7, 1, 7, 8, 1, 8, 0, -1, -1, -1, -1},
{10, 6, 7, 10, 7, 1, 1, 7, 3, -1, -1, -1, -1, -1, -1, -1},
{1, 2, 6, 1, 6, 8, 1, 8, 9, 8, 6, 7, -1, -1, -1, -1},
{2, 6, 9, 2, 9, 1, 6, 7, 9, 0, 9, 3, 7, 3, 9, -1},
{7, 8, 0, 7, 0, 6, 6, 0, 2, -1, -1, -1, -1, -1, -1, -1},
{7, 3, 2, 6, 7, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{2, 3, 11, 10, 6, 8, 10, 8, 9, 8, 6, 7, -1, -1, -1, -1},
{2, 0, 7, 2, 7, 11, 0, 9, 7, 6, 7, 10, 9, 10, 7, -1},
{1, 8, 0, 1, 7, 8, 1, 10, 7, 6, 7, 10, 2, 3, 11, -1},
{11, 2, 1, 11, 1, 7, 10, 6, 1, 6, 7, 1, -1, -1, -1, -1},
{8, 9, 6, 8, 6, 7, 9, 1, 6, 11, 6, 3, 1, 3, 6, -1},
{0, 9, 1, 11, 6, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{7, 8, 0, 7, 0, 6, 3, 11, 0, 11, 6, 0, -1, -1, -1, -1},
{7, 11, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{7, 6, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{3, 0, 8, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{0, 1, 9, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{8, 1, 9, 8, 3, 1, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1},
{10, 1, 2, 6, 11, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{1, 2, 10, 3, 0, 8, 6, 11, 7, -1, -1, -1, -1, -1, -1, -1},
{2, 9, 0, 2, 10, 9, 6, 11, 7, -1, -1, -1, -1, -1, -1, -1},
{6, 11, 7, 2, 10, 3, 10, 8, 3, 10, 9, 8, -1, -1, -1, -1},
{7, 2, 3, 6, 2, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{7, 0, 8, 7, 6, 0, 6, 2, 0, -1, -1, -1, -1, -1, -1, -1},
{2, 7, 6, 2, 3, 7, 0, 1, 9, -1, -1, -1, -1, -1, -1, -1},
{1, 6, 2, 1, 8, 6, 1, 9, 8, 8, 7, 6, -1, -1, -1, -1},
{10, 7, 6, 10, 1, 7, 1, 3, 7, -1, -1, -1, -1, -1, -1, -1},
{10, 7, 6, 1, 7, 10, 1, 8, 7, 1, 0, 8, -1, -1, -1, -1},
{0, 3, 7, 0, 7, 10, 0, 10, 9, 6, 10, 7, -1, -1, -1, -1},
{7, 6, 10, 7, 10, 8, 8, 10, 9, -1, -1, -1, -1, -1, -1, -1},
{6, 8, 4, 11, 8, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{3, 6, 11, 3, 0, 6, 0, 4, 6, -1, -1, -1, -1, -1, -1, -1},
{8, 6, 11, 8, 4, 6, 9, 0, 1, -1, -1, -1, -1, -1, -1, -1},
{9, 4, 6, 9, 6, 3, 9, 3, 1, 11, 3, 6, -1, -1, -1, -1},
{6, 8, 4, 6, 11, 8, 2, 10, 1, -1, -1, -1, -1, -1, -1, -1},
{1, 2, 10, 3, 0, 11, 0, 6, 11, 0, 4, 6, -1, -1, -1, -1},
{4, 11, 8, 4, 6, 11, 0, 2, 9, 2, 10, 9, -1, -1, -1, -1},
{10, 9, 3, 10, 3, 2, 9, 4, 3, 11, 3, 6, 4, 6, 3, -1},
{8, 2, 3, 8, 4, 2, 4, 6, 2, -1, -1, -1, -1, -1, -1, -1},
{0, 4, 2, 4, 6, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{1, 9, 0, 2, 3, 4, 2, 4, 6, 4, 3, 8, -1, -1, -1, -1},
{1, 9, 4, 1, 4, 2, 2, 4, 6, -1, -1, -1, -1, -1, -1, -1},
{8, 1, 3, 8, 6, 1, 8, 4, 6, 6, 10, 1, -1, -1, -1, -1},
{10, 1, 0, 10, 0, 6, 6, 0, 4, -1, -1, -1, -1, -1, -1, -1},
{4, 6, 3, 4, 3, 8, 6, 10, 3, 0, 3, 9, 10, 9, 3, -1},
{10, 9, 4, 6, 10, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{4, 9, 5, 7, 6, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{0, 8, 3, 4, 9, 5, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1},
{5, 0, 1, 5, 4, 0, 7, 6, 11, -1, -1, -1, -1, -1, -1, -1},
{11, 7, 6, 8, 3, 4, 3, 5, 4, 3, 1, 5, -1, -1, -1, -1},
{9, 5, 4, 10, 1, 2, 7, 6, 11, -1, -1, -1, -1, -1, -1, -1},
{6, 11, 7, 1, 2, 10, 0, 8, 3, 4, 9, 5, -1, -1, -1, -1},
{7, 6, 11, 5, 4, 10, 4, 2, 10, 4, 0, 2, -1, -1, -1, -1},
{3, 4, 8, 3, 5, 4, 3, 2, 5, 10, 5, 2, 11, 7, 6, -1},
{7, 2, 3, 7, 6, 2, 5, 4, 9, -1, -1, -1, -1, -1, -1, -1},
{9, 5, 4, 0, 8, 6, 0, 6, 2, 6, 8, 7, -1, -1, -1, -1},
{3, 6, 2, 3, 7, 6, 1, 5, 0, 5, 4, 0, -1, -1, -1, -1},
{6, 2, 8, 6, 8, 7, 2, 1, 8, 4, 8, 5, 1, 5, 8, -1},
{9, 5, 4, 10, 1, 6, 1, 7, 6, 1, 3, 7, -1, -1, -1, -1},
{1, 6, 10, 1, 7, 6, 1, 0, 7, 8, 7, 0, 9, 5, 4, -1},
{4, 0, 10, 4, 10, 5, 0, 3, 10, 6, 10, 7, 3, 7, 10, -1},
{7, 6, 10, 7, 10, 8, 5, 4, 10, 4, 8, 10, -1, -1, -1, -1},
{6, 9, 5, 6, 11, 9, 11, 8, 9, -1, -1, -1, -1, -1, -1, -1},
{3, 6, 11, 0, 6, 3, 0, 5, 6, 0, 9, 5, -1, -1, -1, -1},
{0, 11, 8, 0, 5, 11, 0, 1, 5, 5, 6, 11, -1, -1, -1, -1},
{6, 11, 3, 6, 3, 5, 5, 3, 1, -1, -1, -1, -1, -1, -1, -1},
{1, 2, 10, 9, 5, 11, 9, 11, 8, 11, 5, 6, -1, -1, -1, -1},
{0, 11, 3, 0, 6, 11, 0, 9, 6, 5, 6, 9, 1, 2, 10, -1},
{11, 8, 5, 11, 5, 6, 8, 0, 5, 10, 5, 2, 0, 2, 5, -1},
{6, 11, 3, 6, 3, 5, 2, 10, 3, 10, 5, 3, -1, -1, -1, -1},
{5, 8, 9, 5, 2, 8, 5, 6, 2, 3, 8, 2, -1, -1, -1, -1},
{9, 5, 6, 9, 6, 0, 0, 6, 2, -1, -1, -1, -1, -1, -1, -1},
{1, 5, 8, 1, 8, 0, 5, 6, 8, 3, 8, 2, 6, 2, 8, -1},
{1, 5, 6, 2, 1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{1, 3, 6, 1, 6, 10, 3, 8, 6, 5, 6, 9, 8, 9, 6, -1},
{10, 1, 0, 10, 0, 6, 9, 5, 0, 5, 6, 0, -1, -1, -1, -1},
{0, 3, 8, 5, 6, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{10, 5, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{11, 5, 10, 7, 5, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{11, 5, 10, 11, 7, 5, 8, 3, 0, -1, -1, -1, -1, -1, -1, -1},
{5, 11, 7, 5, 10, 11, 1, 9, 0, -1, -1, -1, -1, -1, -1, -1},
{10, 7, 5, 10, 11, 7, 9, 8, 1, 8, 3, 1, -1, -1, -1, -1},
{11, 1, 2, 11, 7, 1, 7, 5, 1, -1, -1, -1, -1, -1, -1, -1},
{0, 8, 3, 1, 2, 7, 1, 7, 5, 7, 2, 11, -1, -1, -1, -1},
{9, 7, 5, 9, 2, 7, 9, 0, 2, 2, 11, 7, -1, -1, -1, -1},
{7, 5, 2, 7, 2, 11, 5, 9, 2, 3, 2, 8, 9, 8, 2, -1},
{2, 5, 10, 2, 3, 5, 3, 7, 5, -1, -1, -1, -1, -1, -1, -1},
{8, 2, 0, 8, 5, 2, 8, 7, 5, 10, 2, 5, -1, -1, -1, -1},
{9, 0, 1, 5, 10, 3, 5, 3, 7, 3, 10, 2, -1, -1, -1, -1},
{9, 8, 2, 9, 2, 1, 8, 7, 2, 10, 2, 5, 7, 5, 2, -1},
{1, 3, 5, 3, 7, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{0, 8, 7, 0, 7, 1, 1, 7, 5, -1, -1, -1, -1, -1, -1, -1},
{9, 0, 3, 9, 3, 5, 5, 3, 7, -1, -1, -1, -1, -1, -1, -1},
{9, 8, 7, 5, 9, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{5, 8, 4, 5, 10, 8, 10, 11, 8, -1, -1, -1, -1, -1, -1, -1},
{5, 0, 4, 5, 11, 0, 5, 10, 11, 11, 3, 0, -1, -1, -1, -1},
{0, 1, 9, 8, 4, 10, 8, 10, 11, 10, 4, 5, -1, -1, -1, -1},
{10, 11, 4, 10, 4, 5, 11, 3, 4, 9, 4, 1, 3, 1, 4, -1},
{2, 5, 1, 2, 8, 5, 2, 11, 8, 4, 5, 8, -1, -1, -1, -1},
{0, 4, 11, 0, 11, 3, 4, 5, 11, 2, 11, 1, 5, 1, 11, -1},
{0, 2, 5, 0, 5, 9, 2, 11, 5, 4, 5, 8, 11, 8, 5, -1},
{9, 4, 5, 2, 11, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{2, 5, 10, 3, 5, 2, 3, 4, 5, 3, 8, 4, -1, -1, -1, -1},
{5, 10, 2, 5, 2, 4, 4, 2, 0, -1, -1, -1, -1, -1, -1, -1},
{3, 10, 2, 3, 5, 10, 3, 8, 5, 4, 5, 8, 0, 1, 9, -1},
{5, 10, 2, 5, 2, 4, 1, 9, 2, 9, 4, 2, -1, -1, -1, -1},
{8, 4, 5, 8, 5, 3, 3, 5, 1, -1, -1, -1, -1, -1, -1, -1},
{0, 4, 5, 1, 0, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{8, 4, 5, 8, 5, 3, 9, 0, 5, 0, 3, 5, -1, -1, -1, -1},
{9, 4, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{4, 11, 7, 4, 9, 11, 9, 10, 11, -1, -1, -1, -1, -1, -1, -1},
{0, 8, 3, 4, 9, 7, 9, 11, 7, 9, 10, 11, -1, -1, -1, -1},
{1, 10, 11, 1, 11, 4, 1, 4, 0, 7, 4, 11, -1, -1, -1, -1},
{3, 1, 4, 3, 4, 8, 1, 10, 4, 7, 4, 11, 10, 11, 4, -1},
{4, 11, 7, 9, 11, 4, 9, 2, 11, 9, 1, 2, -1, -1, -1, -1},
{9, 7, 4, 9, 11, 7, 9, 1, 11, 2, 11, 1, 0, 8, 3, -1},
{11, 7, 4, 11, 4, 2, 2, 4, 0, -1, -1, -1, -1, -1, -1, -1},
{11, 7, 4, 11, 4, 2, 8, 3, 4, 3, 2, 4, -1, -1, -1, -1},
{2, 9, 10, 2, 7, 9, 2, 3, 7, 7, 4, 9, -1, -1, -1, -1},
{9, 10, 7, 9, 7, 4, 10, 2, 7, 8, 7, 0, 2, 0, 7, -1},
{3, 7, 10, 3, 10, 2, 7, 4, 10, 1, 10, 0, 4, 0, 10, -1},
{1, 10, 2, 8, 7, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{4, 9, 1, 4, 1, 7, 7, 1, 3, -1, -1, -1, -1, -1, -1, -1},
{4, 9, 1, 4, 1, 7, 0, 8, 1, 8, 7, 1, -1, -1, -1, -1},
{4, 0, 3, 7, 4, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{4, 8, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{9, 10, 8, 10, 11, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{3, 0, 9, 3, 9, 11, 11, 9, 10, -1, -1, -1, -1, -1, -1, -1},
{0, 1, 10, 0, 10, 8, 8, 10, 11, -1, -1, -1, -1, -1, -1, -1},
{3, 1, 10, 11, 3, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{1, 2, 11, 1, 11, 9, 9, 11, 8, -1, -1, -1, -1, -1, -1, -1},
{3, 0, 9, 3, 9, 11, 1, 2, 9, 2, 11, 9, -1, -1, -1, -1},
{0, 2, 11, 8, 0, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{3, 2, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{2, 3, 8, 2, 8, 10, 10, 8, 9, -1, -1, -1, -1, -1, -1, -1},
{9, 10, 2, 0, 9, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{2, 3, 8, 2, 8, 10, 0, 1, 8, 1, 10, 8, -1, -1, -1, -1},
{1, 10, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{1, 3, 8, 9, 1, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{0, 9, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{0, 3, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}};

    // Start is called before the first frame update

    Mesh mesh;
    List<Vector3> verts = new List<Vector3>();

    [SerializeField] MeshFilter meshFilter;
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] MeshCollider meshCollider;

    int[,] cornersFromEdge = new int[12,2]
    {
        {0 ,1},{1 ,2},{2 ,3},{3 ,0},
        {4 ,5},{5 ,6},{6 ,7},{7 ,4},
        {0 ,4},{1 ,5},{2 ,6},{3 ,7},
    };


    [SerializeField] int gridSize = 40;
    [SerializeField] float noiseFrequency = 1;

    float[,,] grid = new float[4, 4, 4];
    int[,,] gridIDs = new int[4, 4, 4];

    Cube[] cubes;// = new Cube[64];

    [SerializeField] float surface = 0.5f;

    [SerializeField] float terraformStrength;

    [SerializeField] float terrainHeight;

    [SerializeField] private float divideBy = 1;


    [SerializeField] ComputeShader GPUGenerator;

    


    void Start()
    {

        //genera

        int kernel = GPUGenerator.FindKernel("GenerateMesh");

        GPUGenerator.SetInt("width", gridSize);
        GPUGenerator.SetInt("height", gridSize);
        GPUGenerator.SetInt("length", gridSize);

        GPUGenerator.SetFloat("surface", surface);

        mesh = new Mesh();

        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        cubes = new Cube[(gridSize) * (gridSize) * (gridSize)];

        grid = new float[gridSize, gridSize, gridSize];
        gridIDs = new int[gridSize, gridSize, gridSize];

        Vector3 center = new Vector3(gridSize / 2, gridSize / 2, gridSize / 2);

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                for (int z = 0; z < gridSize; z++)
                {
                    //grid[x, y, z] = Mathf.Round(Mathf.Clamp01(Vector3.Distance(center, new Vector3(x, y, z)) / (gridSize * divideBy)));

                    if (y > terrainHeight)
                    {
                        grid[x, y, z] = 1;
                    }
                    else
                    {
                        grid[x, y, z] = 0;
                    }

                    grid[x, y, z] = perlinNoise.get3DPerlinNoise(new Vector3(x, y, z), noiseFrequency) + 0.5f;

                }
            }
        }

        //GenerateMeshGPU();

    }

    void Update()
    {
        GPUGenerator.SetFloat("surface", surface);

        GenerateMeshGPU();
    }

    void GenerateMeshGPU()
    {

        mesh.Clear();
        verts.Clear();

        int id = 0;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool didHit = false;
        if (Physics.Raycast(ray, out hit, 1000))
        {
            didHit = true;
        }

        float[] singleGrid = new float[gridSize * gridSize * gridSize];

        for(int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                for (int z = 0; z < gridSize; z++)
                {
                    singleGrid[x + y * gridSize + z * gridSize * gridSize] = grid[x,y,z]; 
                }
            }
        }

        //GPUGenerator.

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                for (int z = 0; z < gridSize; z++)
                {

                    if (Input.GetMouseButton(0) && didHit)
                    {

                        float distance = Vector3.Distance(hit.point, new Vector3(x, y, z));

                        if (distance < 4)
                        {
                            grid[x, y, z] -= 1 / (0.0001f + (Mathf.Pow(distance, 2f)));
                        }
                    }

                    if (Input.GetMouseButton(1) && didHit)
                    {

                        float distance = Vector3.Distance(hit.point, new Vector3(x, y, z));

                        if (distance < 4)
                        {
                            grid[x, y, z] += 1 / (0.0001f + (Mathf.Pow(distance, 2f)));
                        }
                    }

                    gridIDs[x, y, z] = id;
                    id++;
                }
            }
        }

        for (int i = 0; i < 5; i++)
        {
            Debug.Log(Convert.ToBase64String(BitConverter.GetBytes(singleGrid[i])));
            Debug.Log(singleGrid[i]);
        }


        int kernel = GPUGenerator.FindKernel("GenerateMesh");

        ComputeBuffer valuesBuffer = new ComputeBuffer(grid.Length, sizeof(float));
        valuesBuffer.SetData(singleGrid);
        GPUGenerator.SetBuffer(kernel, "inputMap", valuesBuffer);

        float[] tempArray = new float[grid.Length];
        valuesBuffer.GetData(tempArray);

        for (int i = 0; i < 5; i++)
        {
            Debug.Log(Convert.ToBase64String(BitConverter.GetBytes(tempArray[i])));
            Debug.Log(tempArray[i]);
        }

        ComputeBuffer outputBuffer = new ComputeBuffer((gridSize * gridSize * gridSize * 5 * 3), sizeof(float) * 3, ComputeBufferType.Append);
        GPUGenerator.SetBuffer(kernel, "triangles", outputBuffer);

        var countBuffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.Raw);


        GPUGenerator.Dispatch(kernel, gridSize / 8, gridSize / 8, gridSize / 8);




        ComputeBuffer.CopyCount(outputBuffer, countBuffer, 0);

        int[] counter = new int[1] { 0 };
        countBuffer.GetData(counter);

        Debug.Log(counter[0]);

        triangle[] trianglesOutput = new triangle[counter[0]];

        outputBuffer.GetData(trianglesOutput, 0, 0, counter[0]);

        foreach(triangle tri in trianglesOutput)
        {
            //Debug.Log(tri.point1 + ", " + tri.point2 + ", " + tri.point3);

            verts.Add(tri.point1);
            verts.Add(tri.point2);
            verts.Add(tri.point3);
        }

        //https://discussions.unity.com/t/how-can-i-read-in-the-actual-elements-from-an-append-compute-buffer/147044

        mesh.vertices = verts.ToArray();

        int triCount = verts.Count;

        Debug.Log(triCount);

        List<int> tris = new List<int>();
        for (int i = triCount - 1; i > -1; i--)
        {
            tris.Add(i);
        }

        mesh.triangles = tris.ToArray();

        Vector2[] uvs = new Vector2[verts.Count];

        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(verts[i].x, verts[i].y);
        }
        mesh.uv = uvs;

        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;

        //countBuffer.Dispose();
        //outputBuffer.Dispose();
        //valuesBuffer.Dispose();

    }

    void GenerateMesh()
    {

        //meshFilter.mesh.Clear();

        mesh.Clear();

        verts.Clear();

        int id = 0;

        //grid = new float[30, 30, 30];

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool didHit = false;
        if (Physics.Raycast(ray, out hit, 1000))
        {
            //Debug.Log(hit.transform.name);
            
            didHit = true;
        }

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                for (int z = 0; z < gridSize; z++)
                {

                    if (Input.GetMouseButton(0) && didHit)
                    {

                        float distance = Vector3.Distance(hit.point, new Vector3(x, y, z));

                        if (distance < 4)
                        {
                            grid[x, y, z] -= 1 / (0.0001f + (Mathf.Pow(distance, 2f)));
                        }

                        //if(Vector3.Distance(hit.point, new Vector3(x, y, z)) / (gridSize * divideBy) < divideBy)
                        //{
                        //    grid[x, y, z] = terraformStrength;
                        //}
                    }

                    if (Input.GetMouseButton(1) && didHit)
                    {

                        float distance = Vector3.Distance(hit.point, new Vector3(x, y, z));

                        if (distance < 4)
                        {
                            grid[x, y, z] += 1 / (0.0001f + (Mathf.Pow(distance, 2f)));
                        }
                    }

                    //grid[x, y, z] = Mathf.Round(Mathf.Clamp01(Vector3.Distance(center, new Vector3(x, y, z)) / (gridSize * divideBy)));

                    //grid[x, y, z] = perlinNoise.get3DPerlinNoise(new Vector3(x, y, z), noiseFrequency) + 0.5f;
                    gridIDs[x, y, z] = id;
                    id++;

                    //Debug.Log(grid[x, y, z]);
                }
            }
        }

        //initialize cube structs

        for (int x = 1; x < gridSize; x++)
        {
            for (int y = 1; y < gridSize; y++)
            {
                for (int z = 1; z < gridSize; z++)
                {
                    int currentID = gridIDs[x - 1, y - 1, z - 1];
                    //Debug.Log("cubeLength: " + cubes.Length + "current Index: " + currentID);
                    cubes[currentID] = new Cube(x, y, z);

                    cubes[currentID].cornerPositions[0] = new int[] { x - 1, y - 1, z - 1 };
                    cubes[currentID].cornerPositions[1] = new int[] { x - 1, y - 1, z };
                    cubes[currentID].cornerPositions[2] = new int[] { x, y - 1, z };
                    cubes[currentID].cornerPositions[3] = new int[] { x, y - 1, z - 1 };
                    cubes[currentID].cornerPositions[4] = new int[] { x - 1, y, z - 1 };
                    cubes[currentID].cornerPositions[5] = new int[] { x - 1, y, z };
                    cubes[currentID].cornerPositions[6] = new int[] { x, y, z };
                    cubes[currentID].cornerPositions[7] = new int[] { x, y, z - 1 };
                }
            }
        }

        //

        for (int x = 0; x < gridSize - 1; x++)
        {
            for (int y = 0; y < gridSize - 1; y++)
            {
                for (int z = 0; z < gridSize - 1; z++)
                {
                    int currentID = gridIDs[x, y, z];
                    int cubeState = 0;

                    int[][] pos = cubes[currentID].cornerPositions;


                    //Debug.Log(cubes[currentID]);
                    //Debug.Log(currentID);

                    //Debug.Log()
                    //Debug.Log(cubes[currentID].position[0] + "-" + cubes[currentID].position[1] + "-" + cubes[currentID].position[2]);

                    for (int i = 0; i < 8; i++)
                    {
                        if (grid[pos[i][0], pos[i][1], pos[i][2]] < surface)
                        {
                            cubeState |= 1 << i;
                        }
                    }

                    //Debug.Log(cubeState);

                    cubes[currentID].cubeState = cubeState;

                    int[] triangulation = new int[16];

                    for (int j = 0; j < 16; j++)
                    {
                        triangulation[j] = triTable[cubeState, j];
                    }



                    foreach (int edgeIndex in triangulation)
                    {

                        //Debug.Log(edgeIndex);

                        if (edgeIndex >= 0)
                        {
                            int pointA = cornersFromEdge[edgeIndex, 0];
                            int pointB = cornersFromEdge[edgeIndex, 1];

                            //Debug.Log("WORK??");

                            Vector3 posA = new Vector3(cubes[currentID].cornerPositions[pointA][0],
                                                       cubes[currentID].cornerPositions[pointA][1],
                                                       cubes[currentID].cornerPositions[pointA][2]);

                            Vector3 posB = new Vector3(cubes[currentID].cornerPositions[pointB][0],
                                                       cubes[currentID].cornerPositions[pointB][1],
                                                       cubes[currentID].cornerPositions[pointB][2]);

                            float valA = grid[(int)posA[0], (int)posA[1], (int)posA[2]];
                            float valB = grid[(int)posB[0], (int)posB[1], (int)posB[2]];

                            Vector3 vertexPosInterpolated = posA + ((posB - posA) * ((surface - valA) / (valB - valA)));

                            //Debug.Log(vertexPosInterpolated + " A= " + valA + " B= " + valB + " S= " + surface + " = " + ((surface - valA) / (valB - valA)));

                            //Vector3 vertexPos = (posA + posB) / 2;

                            verts.Add(vertexPosInterpolated);
                        }
                    }



                }
            }
        }

        //Debug.Log(verts.Count);

        mesh.vertices = verts.ToArray();

        int triCount = verts.Count;

        List<int> tris = new List<int>();
        for (int i = triCount - 1; i > -1; i--)
        {
            tris.Add(i);
        }

        mesh.triangles = tris.ToArray();

        Vector2[] uvs = new Vector2[verts.Count];

        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(verts[i].x, verts[i].y);
        }
        mesh.uv = uvs;

        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;

    }

    private void OnDrawGizmos()
    {

        Vector3 center = new Vector3(gridSize / 2, gridSize / 2, gridSize / 2);

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                for (int z = 0; z < gridSize; z++)
                {
                    //grid[x, y, z] = Mathf.Round(Mathf.Clamp01(Vector3.Distance(center, new Vector3(x, y, z)) / (gridSize * divideBy)));
                    //Gizmos.color = new Color(grid[x,y,z], grid[x, y, z], grid[x, y, z], 0.3f);
                    //Gizmos.DrawSphere(new Vector3(x, y, z), 0.1f);
                }
            }
        }
    }
}


//initialize cube structs

//for (int x = 1; x < gridSize; x++)
//{
//    for (int y = 1; y < gridSize; y++)
//    {
//        for (int z = 1; z < gridSize; z++)
//        {
//            int currentID = gridIDs[x - 1, y - 1, z - 1];

//            cubes[currentID] = new Cube(x, y, z);

//            cubes[currentID].cornerPositions[0] = new int[] { x - 1, y - 1, z - 1 };
//            cubes[currentID].cornerPositions[1] = new int[] { x - 1, y - 1, z };
//            cubes[currentID].cornerPositions[2] = new int[] { x, y - 1, z };
//            cubes[currentID].cornerPositions[3] = new int[] { x, y - 1, z - 1 };
//            cubes[currentID].cornerPositions[4] = new int[] { x - 1, y, z - 1 };
//            cubes[currentID].cornerPositions[5] = new int[] { x - 1, y, z };
//            cubes[currentID].cornerPositions[6] = new int[] { x, y, z };
//            cubes[currentID].cornerPositions[7] = new int[] { x, y, z - 1 };
//        }
//    }
//}

//

//for (int x = 0; x < gridSize - 1; x++)
//{
//    for (int y = 0; y < gridSize - 1; y++)
//    {
//        for (int z = 0; z < gridSize - 1; z++)
//        {
//            int currentID = gridIDs[x, y, z];
//            int cubeState = 0;

//            int[][] pos = cubes[currentID].cornerPositions;


//            for (int i = 0; i < 8; i++)
//            {
//                if (grid[pos[i][0], pos[i][1], pos[i][2]] < surface)
//                {
//                    cubeState |= 1 << i;
//                }
//            }

//            cubes[currentID].cubeState = cubeState;

//            int[] triangulation = new int[16];

//            for (int j = 0; j < 16; j++)
//            {
//                triangulation[j] = triTable[cubeState, j];
//            }



//            foreach (int edgeIndex in triangulation)
//            {

//                if (edgeIndex >= 0)
//                {
//                    int pointA = cornersFromEdge[edgeIndex, 0];
//                    int pointB = cornersFromEdge[edgeIndex, 1];

//                    Vector3 posA = new Vector3(cubes[currentID].cornerPositions[pointA][0],
//                                               cubes[currentID].cornerPositions[pointA][1],
//                                               cubes[currentID].cornerPositions[pointA][2]);

//                    Vector3 posB = new Vector3(cubes[currentID].cornerPositions[pointB][0],
//                                               cubes[currentID].cornerPositions[pointB][1],
//                                               cubes[currentID].cornerPositions[pointB][2]);

//                    float valA = grid[(int)posA[0], (int)posA[1], (int)posA[2]];
//                    float valB = grid[(int)posB[0], (int)posB[1], (int)posB[2]];

//                    Vector3 vertexPosInterpolated = posA + ((posB - posA) * ((surface - valA) / (valB - valA)));

//                    verts.Add(vertexPosInterpolated);
//                }
//            }



//        }
//    }
//}