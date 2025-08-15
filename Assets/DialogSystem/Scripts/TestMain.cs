using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMain : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            DialogSystem.Instance.LoadDialog(0);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
        }
    }
}
