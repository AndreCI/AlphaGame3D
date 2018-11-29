﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NodeMesh : MonoBehaviour
{
    private static NodeMesh instance;
    public static NodeMesh Instance
    {
        get
        {
            return instance;
        }
    }
    public int nodeResolution; //Number of point per node
    public int nodeNumberX;
    public int nodeNumberY;
    public float nodeSize; //size of the node in game
    public int offset;
    private Vector3[] vertices;
    private Vector3[] normals;
    private Vector2[] uv;
    private List<int> triangles;
    private float tileSize; //Number of squares per Node
    public Material defaultMaterial;
    public Material mountainMaterial;
    public List<List<int>> submeshesTriangles;
    private List<Material> materialForNodes;
    private Mesh mesh;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    private Dictionary<WorldGeneration.BIOME_TYPES, Material> biomesToMaterial;
    public int sizex; //Total size
    public int sizez;//8 nodes which have 16 vectrices each
    public int vsizex;//Total number of vectrices in the x directions
    public int vsizez;

    // Use this for initialization
    void Awake()
    {
        submeshesTriangles = new List<List<int>>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshFilter = gameObject.AddComponent<MeshFilter>();
        mesh = meshFilter.mesh;
        biomesToMaterial = new Dictionary<WorldGeneration.BIOME_TYPES, Material>()
        {
            { WorldGeneration.BIOME_TYPES.DEFAULT, defaultMaterial },
            {WorldGeneration.BIOME_TYPES.MOUNTAIN,  mountainMaterial}
        };
        sizex = nodeResolution * nodeNumberX; //Total size
        sizez = nodeResolution * nodeNumberY; //8 nodes which have 16 vectrices each
        vsizex = sizex + 1; //Total number of vectrices in the x directions
        vsizez = sizez + 1;
        instance = this;
        //LoadTexture();
        //BuildTexture();
        // GetNodeHigh(4, 4);
    }
    private void GetNodeHigh(int x_pos, int z_pos)
    {
        
        int x, z;
        int z_start = z_pos * nodeResolution + offset;
        int x_start = x_pos * nodeResolution + offset;
        int z_end = z_start + nodeResolution - offset;
        int x_end = x_start + nodeResolution - offset;
        for (z = z_start; z < z_end; z++)
        {
            for (x = x_start; x < x_end; x++)
            {
                float high = (x - x_start) * (x_end - x) * (z - z_start) * (z_end - z)/(nodeResolution*2);//new FractalBrownianMotion().GetHeight(x, z) * 8;
                vertices[z * vsizex + x] = new Vector3(x * tileSize, high, z * tileSize);
            }
        }
                UpdateMesh();
    }

    public void LoadMaterialForNode(Node node)
    {
        int x = (int)(node.position.x / nodeSize);
        Debug.Log("------");
        Debug.Log(node.biome.ToString());
        int z = (int)(node.position.z / nodeSize);
        Debug.Log(x + "::" + z);
        materialForNodes[z * nodeNumberX + x] = biomesToMaterial[node.biome];
        mesh.SetTriangles(submeshesTriangles[z * nodeNumberX + x], z * nodeNumberX + x);
    }

    public void ReLoadMaterials()
    {
        meshRenderer.materials = materialForNodes.ToArray();

        for (int i = 0; i < submeshesTriangles.Count; i++)
        {
            mesh.SetTriangles(submeshesTriangles[i], i);
        }

        //mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
    }

    public void LoadMaterial(List<WorldGeneration.BIOME_TYPES> biomes)
    {
        if (biomes.Count != submeshesTriangles.Count)
        {
            throw new System.ArgumentOutOfRangeException();
        }
        materialForNodes = new List<Material>();
        for (int i=0; i < submeshesTriangles.Count; i++)
        {
            materialForNodes.Add(biomesToMaterial[biomes[i]]);
        }

        ReLoadMaterials();
    }

    private void BuildTexture()
    {
        int tileResolution = nodeResolution;
        int sizex = nodeResolution * nodeNumberX;
        int sizez = nodeResolution * nodeNumberY;
        int texWidth = nodeResolution * nodeNumberX * tileResolution;
        int texHeight = nodeResolution * nodeNumberY * tileResolution;
        Texture2D texture = new Texture2D(texWidth, texHeight);
        int x, z;
        Color[] red = new Color[tileResolution*tileResolution];
        for(int i=0; i < tileResolution * tileResolution; i++)
        {
            red[i] = Color.red;
        }
        Color[] blue = new Color[tileResolution * tileResolution];
        for (int i = 0; i < tileResolution * tileResolution; i++)
        {
            blue[i] = Color.blue;
        }

        for (z = 0; z < sizez; z++)
        {
            for (x = 0; x < sizex; x++)
            {
                if ((x+z) % 2 == 0)
                {
                    texture.SetPixels(x * tileResolution, z * tileResolution, tileResolution, tileResolution, red);
                }
                else
                {
                    texture.SetPixels(x * tileResolution, z * tileResolution, tileResolution, tileResolution, blue);
                }
                
            }
        }
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.Apply();
        meshRenderer.sharedMaterials[0].mainTexture = texture;
    }

    public void GenerateMesh()
    {
        mesh.Clear();
        mesh.subMeshCount = nodeNumberX * nodeNumberY;
        //There is nodes and tiles: nodes are ingame representation of a place, while tiles agglomerate to create
        //the node. There is multiple tiles in nodes.
        for (int i =0; i<nodeNumberX; i++)
        {
            for(int j = 0; j < nodeNumberY; j++)
            {
                submeshesTriangles.Add(new List<int>());
            }
        }
        
        int numTiles = sizez * sizex; //Number of tiles created. Maybe use vsize?
        tileSize = (float)nodeSize / (float)nodeResolution;
        int numTris = numTiles * 2;
        int numVer = vsizex * vsizez; //Number of total vecttrices
        vertices = new Vector3[numVer];
        normals = new Vector3[numVer];
        uv = new Vector2[numVer];
        triangles = new List<int>(numTris * 3);
        for (int i = 0; i < numTris * 3; i++) { triangles.Add(0); }
        int x, z;
        for (z = 0; z < vsizez; z++)
        {
            for (x = 0; x < vsizex; x++)
            {
                float high = 0;// new FractalBrownianMotion().GetHeight(x, z)*10;
                vertices[z * vsizex + x] = new Vector3(x * tileSize , high, z*tileSize );
                normals[z * vsizex + x] = Vector3.up;
                uv[z * vsizex + x] = new Vector2((float)x / sizex, (float)z / sizez);
            }
        }
        for (z = 0; z < sizez; z++)
        {
            for (x = 0; x < sizex; x++)
            {
                int squareIdx = z * sizex + x;
                int triangeOffset = squareIdx * 6;
                triangles[triangeOffset + 0] = z * vsizex + x + 0; //0 ; 0,0
                triangles[triangeOffset + 1] = z * vsizex + x + vsizex + 0; //2 ; 1,0
                triangles[triangeOffset + 2] = z * vsizex + x + vsizex + 1; //3 ; 1,1

                triangles[triangeOffset + 3] = z * vsizex + x + 0; //0 ; 0,0
                triangles[triangeOffset + 4] = z * vsizex + x + vsizex + 1; //3 ; 1,1
                triangles[triangeOffset + 5] = z * vsizex + x +  1; //1 ; 0,1

                GetTrianglesSubmesh(x, z).Add(z * vsizex + x + 0); //0 ; 0,0
                GetTrianglesSubmesh(x, z).Add(z * vsizex + x + vsizex + 0); //2 ; 1,0
                GetTrianglesSubmesh(x, z).Add(z * vsizex + x + vsizex + 1); //3 ; 1,1

                GetTrianglesSubmesh(x, z).Add(z * vsizex + x + 0); //0 ; 0,0
                GetTrianglesSubmesh(x, z).Add(z * vsizex + x + vsizex + 1); //3 ; 1,1
                GetTrianglesSubmesh(x, z).Add(z * vsizex + x + 1); //1 ; 0,1

            }

        }
        UpdateMesh();
    }

    private List<int> GetTrianglesSubmesh(int x, int z)
    {
        x = Mathf.FloorToInt((float)x / (float)nodeResolution);
        z = Mathf.FloorToInt((float)z / (float)nodeResolution);
        return submeshesTriangles[z * nodeNumberX+x];
    }

    private void UpdateMesh()
    {
        //mesh = new Mesh();
       // mesh.Clear();
        mesh.vertices = vertices;
        //mesh.triangles = triangles.ToArray();
        mesh.uv = uv;
        mesh.normals = normals;

      //  meshFilter.mesh = mesh;
    }
}
