using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BackButton : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("b"))
        {
            GetComponent<Button>().onClick.Invoke();
        }
    }
}
