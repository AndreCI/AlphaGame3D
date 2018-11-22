using UnityEngine;

public class WarriorAnimation : MonoBehaviour
{
    Warrior body;
    // Use this for initialization
    void Start()
    {
        body = GetComponentInParent<Warrior>();
    }

    void FootR()
    {
        //body.FootR();
    }
    void FootL()
    {
        //body.FootL();
    }
    void Hit()
    {
        //body.Hit();
    }
}
