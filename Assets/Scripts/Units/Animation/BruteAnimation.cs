using UnityEngine;

public class BruteAnimation : MonoBehaviour
{
    Brute body;
    // Use this for initialization
    void Start()
    {
        body = GetComponentInParent<Brute>();
    }

    void FootR()
    {
        body.FootR();
    }
    void FootL()
    {
        body.FootL();
    }
    void Hit()
    {
        body.Hit();
    }
}
