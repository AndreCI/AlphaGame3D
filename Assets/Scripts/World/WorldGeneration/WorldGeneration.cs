using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System;

public class WorldGeneration:MonoBehaviour
{
    public NodeMesh nodeMesh;
    private List<Node> nodes;
    public FractalBrownianMotion treeNoiseGeneration;

    public enum BIOME_TYPES
    {
        DEFAULT,
        MOUNTAIN,
        WATER,
        FOREST_NOTREE,
        FOREST_TREE
    };

    void Start()
    {
        nodeMesh.GenerateMesh();
        nodes = new List<Node>();
        List<Node> orderedNodes = new List<Node>();
        nodes.AddRange(GetComponentsInChildren<Node>());
        Debug.Log(nodes.Count);
        for(int i=0;i<nodeMesh.submeshesTriangles.Count; i++) { 
            orderedNodes.Add(GetNodeFromSubMesh(i));
        }
        foreach(Node n in orderedNodes)
        {
            n.biome = BIOME_TYPES.DEFAULT;
        }
        for(int i=0; i<7; i++)
        {
            //MountainBiome.CreateBiome(orderedNodes[Random.Range(0, orderedNodes.Count)], Random.Range(4, 6));
        }
        AssignBiome(orderedNodes);
        nodeMesh.LoadMaterial(orderedNodes.Select(node=>node.biome).ToList());
    }

    private void AssignBiome(List<Node> nodes)
    {
        foreach(Node n in nodes)
        {
            List<Vector2> vectrices = GetVectricesFromNodes(n);
            List<float> heights = new List<float>();
            foreach(Vector2 v in vectrices)
            {
                heights.Add(nodeMesh.xzToHeight[v]);
            }
            if (heights.Average() > 0.3f )//|| heights.Max()>0.45f)
            {
                n.biome = BIOME_TYPES.MOUNTAIN;
            }else if (heights.Average() < 0f)
            {
                n.biome = BIOME_TYPES.WATER;
            }
        }
        List<Node> forestSource = (nodes.Where(n =>n.biome != BIOME_TYPES.MOUNTAIN && n.biome!=BIOME_TYPES.WATER)).OrderBy(n => Guid.NewGuid()).Take(3).ToList();
        foreach (Node n in forestSource) {
            
            ForestBiome forestBiome = new ForestBiome(treeNoiseGeneration, nodeMesh.prefabTreeList);
            forestBiome.CreateBiome(n, UnityEngine.Random.Range(4, 6));
         }
    }
    private List<Vector2> GetVectricesFromNodes(Node n)
    {
        Vector3 position = n.position;
        position =nodeMesh.nodeResolution* position / nodeMesh.nodeSize;
        List<Vector2> vectrices = new List<Vector2>();
        float tileSize = (float)nodeMesh.nodeSize / (float)nodeMesh.nodeResolution;

        for (int x=0;x<nodeMesh.nodeResolution; x ++)
        {
            for(int z = 0; z < nodeMesh.nodeResolution; z++)
            {
                vectrices.Add(new Vector2((position.x + x)*tileSize, (position.z + z)*tileSize));
            }
        }
        return vectrices;

    }
    private Node GetNodeFromSubMesh(params int[] values)
    {
        int idx;
        int x;
        int z;
        if (values.Length == 2)
        {
            x = values[0];
            z = values[1];
            idx = values[0] * nodeMesh.nodeNumberX + values[1];
        }else if(values.Length == 1)
        {
            idx = values[0];
            x = idx % nodeMesh.nodeNumberX;
            z = (idx - x) / nodeMesh.nodeNumberX;
        }
        else
        {
            throw new System.IndexOutOfRangeException();
        }
        Vector3 position = new Vector3((x) * nodeMesh.nodeSize, 100f, (z) * nodeMesh.nodeSize);
        foreach(Node n in nodes)
        {
            if (n.position == position)
            {
                return n;
            }
        }
        throw new KeyNotFoundException();

    }
}