using UnityEngine;

public class BruteAnimation : MonoBehaviour
{
    Brute body;
    // Use this for initialization
    void Start()
    {
        body = GetComponentInParent<Brute>();
    }
}
