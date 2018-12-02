using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class Forest
{
    private List<Tree> trees;

    public Forest()
    {
        trees = new List<Tree>();
    }

    public void AddTree(GameObject prefab, Vector3 position)
    {
        trees.Add(new Tree()
        {
            prefab = prefab,
            position = position,
        });
    }

    public void SetVisible(bool v)
    {
        foreach(Tree t in trees)
        {
            t.SetVisible(v);
        }
    }

    private class Tree
    {
        public GameObject prefab;
        public Vector3 position;
        private bool instanciated;
        public Tree()
        {
            instanciated = false;
        }

        public void SetVisible(bool v)
        {
            if(v && !instanciated)
            {
                MonoBehaviour.Instantiate(prefab, position, new Quaternion(0f,0f,0f,0f));
                instanciated = true;
            }
            prefab.SetActive(v);
        }
    }
}