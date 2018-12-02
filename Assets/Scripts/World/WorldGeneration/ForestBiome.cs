using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System;

public class ForestBiome
{
    private FractalBrownianMotion fbm;
    private List<GameObject> trees;
    public ForestBiome(FractalBrownianMotion fbm_, List<GameObject> trees_)
    {
        fbm = fbm_;
        trees = trees_;
    }

    public void CreateBiome(Node source, int number)
    {
        Node current = source;
        List<Node> forestNodes = new List<Node>(){source};
        
        source.biome = WorldGeneration.BIOME_TYPES.FOREST_NOTREE;
        while (number > 0)
        {
            foreach(Node n in current.adjacentNodes)
            {
                if (n.biome != WorldGeneration.BIOME_TYPES.FOREST_NOTREE && n.biome!=WorldGeneration.BIOME_TYPES.MOUNTAIN && n.biome != WorldGeneration.BIOME_TYPES.WATER)
                {
                    if (n.adjacentNodes.Select(adj => adj.biome == WorldGeneration.BIOME_TYPES.FOREST_NOTREE).ToList().Count > 3)
                    {
                        
                        n.biome = WorldGeneration.BIOME_TYPES.FOREST_NOTREE;
                        forestNodes.Add(n);
                        number -= 1;
                    }
                    else
                    {
                        if (UnityEngine.Random.Range(0f, 1f) > 0.8f)
                        {
                            n.biome = WorldGeneration.BIOME_TYPES.FOREST_NOTREE;
                            number -= 1;
                            forestNodes.Add(n);
                        }
                        
                    }
                }
            }
            current = current.adjacentNodes[UnityEngine.Random.Range(0, current.adjacentNodes.Count)];
        }

        List<Node> withTree = forestNodes.OrderBy(x => Guid.NewGuid()).Take(Mathf.FloorToInt(1)).ToList(); //TODO: change
        int withTreeNbr = UnityEngine.Random.Range(2, 4);
        int nbr = 0;
        while (nbr<withTreeNbr && withTree.Count != 0)
        {
            withTree[0].biome = WorldGeneration.BIOME_TYPES.FOREST_TREE;
            SpawnTrees(withTree[0]);
            withTree = forestNodes.Where(x => x.adjacentNodes.Where(intern=>intern.biome==WorldGeneration.BIOME_TYPES.FOREST_TREE).ToList().Count<1).ToList(); //TODO: change
            withTree.OrderBy(x => Guid.NewGuid());
            nbr += 1;
        }
    }

    private void SpawnTrees(Node n)
    {
        fbm.Update();
        int neighborRadius = 6;// NodeMesh.Instance.nodeResolution;
        Dictionary<Vector2, float> noise = new Dictionary<Vector2, float>();
        for (int x = 0; x < NodeMesh.Instance.nodeResolution; x++)
        {
            for (int z = 0; z < NodeMesh.Instance.nodeResolution; z++)
            {

                noise.Add(new Vector2(x, z), fbm.GetHeight((float)x / (float)NodeMesh.Instance.nodeResolution, (float)z / (float)NodeMesh.Instance.nodeResolution));
               
            }
        }

        for (int x = 0; x < NodeMesh.Instance.nodeResolution; x++)
        {
            for (int z = 0; z < NodeMesh.Instance.nodeResolution; z++)
            {
                float currentValue = noise[new Vector2(x, z)];
                int neighborZBegin = (int)Mathf.Max(0, z - neighborRadius);
                int neighborZEnd = (int)Mathf.Min(NodeMesh.Instance.nodeResolution - 1, z + neighborRadius);
                int neighborXBegin = (int)Mathf.Max(0, x - neighborRadius);
                int neighborXEnd = (int)Mathf.Min(NodeMesh.Instance.nodeResolution - 1, x + neighborRadius);
                float maxValue = currentValue;
                for (int neighborZ = neighborZBegin; neighborZ <= neighborZEnd; neighborZ++)
                {
                    for (int neighborX = neighborXBegin; neighborX <= neighborXEnd; neighborX++)
                    {
                        float neighborValue = noise[new Vector2(neighborX, neighborZ)];
                        // saves the maximum tree noise value in the radius
                        if (neighborValue >= maxValue)
                        {
                            maxValue = neighborValue;
                        }
                    }
                }
                if (currentValue == maxValue)
                {
                    Debug.Log((float)x / (float)NodeMesh.Instance.nodeResolution);
                    Debug.Log((float)z / (float)NodeMesh.Instance.nodeResolution);
                    Debug.Log(x);
                    Debug.Log(z);
                    Debug.Log(currentValue);
                    Vector3 treePosition = new Vector3(n.position.x + (float)x / (float)NodeMesh.Instance.nodeResolution, 
                        n.position.y, 
                        n.position.z + (float)z / (float)NodeMesh.Instance.nodeResolution);
                    SpawnSingleTree(n, treePosition);
                }
              //  Debug.Log(currentValue + "::" + maxValue);
            }
        }
    }
    private void SpawnSingleTree(Node n, Vector3 treePosition)
    {
        GameObject prefab = trees.OrderBy(t => Guid.NewGuid()).Take(1).ToList()[0];
        if (n.forest == null)
        {
            n.forest = new Forest();
        }
        n.forest.AddTree(prefab, treePosition);
        Debug.Log("tree spawn at position " + treePosition);
    }
}