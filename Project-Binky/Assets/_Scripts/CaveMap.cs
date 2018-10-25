﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Diagnostics;

public class CaveMap : MonoBehaviour {

    [SerializeField] private int width = 64;
    [SerializeField] private int height = 64;
    [SerializeField] private int closingLayers;

    [SerializeField] private GameObject wallTile;
    [SerializeField] private GameObject floorTile;

    private Stopwatch stopwatch;

    private PerlinGenerator perlinGenerator;

    private Node[,] nodeMap;
    private List<Node> floorNodes;
    private List<Room> rooms;

    private void Awake()
    {
        stopwatch = new Stopwatch();
        floorNodes = new List<Node>();
        rooms = new List<Room>();
        nodeMap = new Node[width, height];
        perlinGenerator = GetComponent<PerlinGenerator>();
        InitializeMap();
    }

    private void InitializeMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                nodeMap[x, y] = new Node(new Point(x, y), 0.0f);
            }
        }
    }

    void Start () {
        stopwatch.Start();
        GenerateCaves();
        ApplyGraphics();
        stopwatch.Stop();
        UnityEngine.Debug.Log(stopwatch.ElapsedMilliseconds);
	}

    public Node GetNode(int x, int y)
    {
        if(x < 0 || x >= width)
        {
            UnityEngine.Debug.LogError("Attemtped to retrieve CaveMap Node with invalid x index of " + x.ToString() + ".");
            return new Node();
        }

        if(y < 0 || y >= height)
        {
            UnityEngine.Debug.LogError("Attemtped to retrieve CaveMap Node with invalid y index of " + y.ToString() + ".");
            return new Node();
        }

        return nodeMap[x, y];
    }
	
    private void GenerateCaves()
    {
        PerlinizeNodeMap();
        CloseCaveSystem();
        NegateSuperfluousNodes();

    }

    private void PerlinizeNodeMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float p = perlinGenerator.GetPerlin((float)x / width, (float)y / height, 0.0f);
                nodeMap[x, y].SetValue(p);
                if(p == 0.0f) { floorNodes.Add(nodeMap[x, y]); }
            }
        }
    }

    /// <summary>
    /// Build and feather a border around the cave system.
    /// </summary>
    private void CloseCaveSystem()
    {
        int wallLeft;
        int wallRight;
        int wallBottom;
        int wallTop;
        float wallChance = 1;
        bool placeWall = true;

        for (int i = 0; i < closingLayers; i++)
        {
            if(i > 0)
            {
                wallChance = ((float)i / closingLayers);
            }


            wallLeft = 0 + i;
            wallRight = width - 1 - i;

            for (int j = i; j < height - i; j++)
            {
                if(i > 0)
                {
                    if(GetNode(wallLeft - 1, j).Value == 1)
                    {
                        placeWall = (UnityEngine.Random.Range(0.0f, 1.0f) > wallChance);
                        if (placeWall) { nodeMap[wallLeft, j].SetValue(1); }
                    }

                    if (GetNode(wallRight + 1, j).Value == 1)
                    {
                        placeWall = (UnityEngine.Random.Range(0.0f, 1.0f) > wallChance);
                        if (placeWall) { nodeMap[wallRight, j].SetValue(1); }
                    }
                }
                else
                {
                    nodeMap[wallLeft, j].SetValue(1);
                    nodeMap[wallRight, j].SetValue(1);
                }
            }

            wallBottom = 0 + i;
            wallTop = height - 1 - i;

            for (int j = i; j < width - i; j++)
            {
                if (i > 0)
                {
                    if(GetNode(j, wallBottom - 1).Value == 1)
                    {
                        placeWall = (UnityEngine.Random.Range(0.0f, 1.0f) > wallChance);
                        if (placeWall) { nodeMap[j, wallBottom].SetValue(1); }
                    }

                    if (GetNode(j, wallTop + 1).Value == 1)
                    {
                        placeWall = (UnityEngine.Random.Range(0.0f, 1.0f) > wallChance);
                        if (placeWall) { nodeMap[j, wallTop].SetValue(1); }
                    }
                }
                else
                {
                    nodeMap[j, wallTop].SetValue(1);
                    nodeMap[j, wallBottom].SetValue(1);
                }
            }
        }
    }

    private void ApplyGraphics()
    {
        GameObject toInstantiate = null;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (nodeMap[x, y].Value == 1.0f)
                {
                    toInstantiate = wallTile;
                }
                else if (nodeMap[x, y].Value == 0.0f)
                {
                    toInstantiate = floorTile;
                }
                else
                {
                    toInstantiate = null;
                }

                if(toInstantiate != null)
                {
                    Instantiate(toInstantiate, new Vector3(x, y), Quaternion.identity, transform);
                }
            }
        }
    }

    private bool NeighbourHasValue(Node n, float val)
    {
        bool hasValue = false;
        for(int x = n.Address.x - 1; x <= n.Address.x + 1; x++)
        {
            for (int y = n.Address.y - 1; y <= n.Address.y + 1; y++)
            {
                if(!IsValidCoordinate(x, y)) { continue; }
                if(nodeMap[x, y].Value == val) { hasValue = true; }
            }
            if (hasValue) { break; }
        }

        return hasValue;
    }

    public bool IsValidCoordinate(int x, int y)
    {
        return ((x >= 0) && (x < width) && (y >= 0) && (y < height));
    }

    private void NegateSuperfluousNodes()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if(!NeighbourHasValue(nodeMap[x, y], 0.0f)) { nodeMap[x, y].SetValue(-1.0f); }
            }
        }
    }

}
