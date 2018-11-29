using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public abstract class Biome
{
    List<Node> biomeNodes;

    public Biome(List<Node> nodes)
    {
        biomeNodes = nodes;
    }

}