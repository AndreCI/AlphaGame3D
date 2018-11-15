using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShortkeyButton : MonoBehaviour
{
    public string key;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(key))
        {
            GetComponent<Button>().onClick.Invoke();
        }
    }
}
