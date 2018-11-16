using UnityEngine;

public class WizardAnimation : MonoBehaviour
{
    Wizard body;
    void Start()
    {
        body = GetComponentInParent<Wizard>();
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

