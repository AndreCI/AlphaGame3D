using System.Collections.Generic;
using UnityEngine;

public class NodeMesh : MonoBehaviour
{
    public int sizePerNode;
    public int NodeNumberX;
    public int NodeNumberY;
    public float NodeSize;
    private Vector3[] vertices;
    private Vector3[] normals;
    private Vector2[] uv;
    private List<int> trianges;
    private float tileSize;
    public int tileResolution;
    public Texture2D texture;
    public Material material;
    // Use this for initialization
    void Start()
    {
        GenerateMesh();
        LoadMaterial();
        //GetNodeHigh(4, 4);
    }

    private void GetNodeHigh(int x_pos, int z_pos)
    {
        int sizex = sizePerNode * NodeNumberX;
        int sizez = sizePerNode * NodeNumberY;
        int vsizex = sizex + 1;
        int vsizez = sizez + 1;
        int x, z;
        int z_start = z_pos * sizePerNode;
        int x_start = x_pos * sizePerNode;
        for (z = z_start; z < z_start + sizePerNode; z++)
        {
            for (x = x_start; x < x_start + sizePerNode; x++)
            {
                float high =new FractalBrownianMotion().GetHeight(x, z) * 4;
                vertices[z * vsizex + x] = new Vector3(x * tileSize, high, z * tileSize);
            }
        }
                UpdateMesh();
    }
    private void LoadMaterial()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = material;
    }

    private void LoadTexture()
    {
        int sizex = sizePerNode * NodeNumberX;
        int sizez = sizePerNode * NodeNumberY;
        int texWidth = sizePerNode * NodeNumberX * tileResolution;
        int texHeight = sizePerNode * NodeNumberY * tileResolution;
        Texture2D newTexture = new Texture2D(sizex, sizez);
        int x, z;
        for (z = 0; z < sizez; z+=tileResolution)
        {
            for (x = 0; x < sizex; x+=tileResolution)
            {
                Color[] colors = texture.GetPixels(0, 0, tileResolution, tileResolution);
                newTexture.SetPixels(x * tileResolution, z * tileResolution, tileResolution, tileResolution, colors);
            }
        }
        newTexture.filterMode = FilterMode.Point;
        newTexture.wrapMode = TextureWrapMode.Clamp;
        newTexture.Apply();
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.sharedMaterials[0].mainTexture = newTexture;
    }
    private void BuildTexture()
    {
        int sizex = sizePerNode * NodeNumberX;
        int sizez = sizePerNode * NodeNumberY;
        int texWidth = sizePerNode * NodeNumberX * tileResolution;
        int texHeight = sizePerNode * NodeNumberY * tileResolution;
        Texture2D texture = new Texture2D(texWidth, texHeight);
        int x, z;
        Color[] color = { Color.blue, Color.blue, Color.blue, Color.blue };
        for (z = 0; z < sizez; z++)
        {
            for (x = 0; x < sizex; x++)
            {
                if ((x+z) % 2 == 0)
                {
                    color[0] =  Color.blue;
                }
                else
                {
                    color[0] = Color.red;
                }
                texture.SetPixels(x * tileResolution, z * tileResolution, tileResolution, tileResolution, color);
            }
        }
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.Apply();
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.sharedMaterials[0].mainTexture = texture;
    }

    public void GenerateMesh()
    {
        int sizex = sizePerNode * NodeNumberX;
        int sizez = sizePerNode * NodeNumberY;
        int vsizex = sizex + 1;
        int vsizez = sizez + 1;
        int numTiles = sizez * sizex;
        tileSize = NodeSize / (float)sizePerNode;
        int numTris = numTiles * 2;
        int numVer = vsizex * vsizez;
        vertices = new Vector3[numVer];
        normals = new Vector3[numVer];
        uv = new Vector2[numVer];
        trianges = new List<int>(numTris * 3);
        for (int i = 0; i < numTris * 3; i++) { trianges.Add(0); }
        int x, z;
        for (z = 0; z < vsizez; z++)
        {
            for (x = 0; x < vsizex; x++)
            {
                float high = 0;// new FractalBrownianMotion().GetHeight(x, z)*10;
                vertices[z * vsizex + x] = new Vector3(x * tileSize, high, z * tileSize);
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
                trianges[triangeOffset + 0] = z * vsizex + x + 0; //0 ; 0,0
                trianges[triangeOffset + 1] = z * vsizex + x + vsizex + 0; //2 ; 1,0
                trianges[triangeOffset + 2] = z * vsizex + x + vsizex + 1; //3 ; 1,1

                trianges[triangeOffset + 3] = z * vsizex + x + 0; //0 ; 0,0
                trianges[triangeOffset + 4] = z * vsizex + x + vsizex + 1; //3 ; 1,1
                trianges[triangeOffset + 5] = z * vsizex + x +  1; //1 ; 0,1
            }
        }
        UpdateMesh();
    }

    private void UpdateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = trianges.ToArray();
        mesh.uv = uv;
        mesh.normals = normals;


        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;
    }
}
