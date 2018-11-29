using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class MountainBiome : Biome
{
    public MountainBiome(List<Node> nodes) : base(nodes)
    {
    }

    public static void CreateBiome(Node source, int number)
    {
        Node current = source;
        source.biome = WorldGeneration.BIOME_TYPES.MOUNTAIN;
        while (number > 0)
        {
            foreach(Node n in current.adjacentNodes)
            {
                if (n.biome != WorldGeneration.BIOME_TYPES.MOUNTAIN)
                {
                    if (n.adjacentNodes.Select(adj => adj.biome == WorldGeneration.BIOME_TYPES.MOUNTAIN).ToList().Count > 3)
                    {
                        
                        n.biome = WorldGeneration.BIOME_TYPES.MOUNTAIN;
                        number -= 1;
                    }
                    else
                    {
                        if (Random.Range(0f, 1f) > 0.8f)
                        {
                            n.biome = WorldGeneration.BIOME_TYPES.MOUNTAIN;
                            number -= 1;
                        }
                        
                    }
                }
            }
            current = current.adjacentNodes[Random.Range(0, current.adjacentNodes.Count)];
        }
    }
}