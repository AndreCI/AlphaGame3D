using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class WorldGeneration:MonoBehaviour
{
    public NodeMesh nodeMesh;
    private List<Node> nodes;

    public enum BIOME_TYPES
    {
        DEFAULT,
        MOUNTAIN
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
            MountainBiome.CreateBiome(orderedNodes[Random.Range(0, orderedNodes.Count)], Random.Range(4, 6));
        }
        nodeMesh.LoadMaterial(orderedNodes.Select(node=>node.biome).ToList());
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