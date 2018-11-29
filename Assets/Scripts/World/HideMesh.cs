using UnityEngine;
using System.Collections;

public class HideMesh : MonoBehaviour
{
    public Node node;

    public void OnMouseDown()
    {
        node.OnMouseDown();
    }
    public void OnMouseEnter()
    {
        node.OnMouseOver();
    }
    public void OnMouseExit()
    {
        node.OnMouseExit();
    }
}
