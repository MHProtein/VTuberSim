using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextChangeLine : MonoBehaviour
{
    public int numToChangeLine;

    private Text text;
    private string oldText;
    private bool once = true;

    private void Awake()
    {
        text = GetComponent<Text>();
        oldText = text.text;
    }

    // Update is called once per frame
    void Update()
    {
        if (oldText != text.text)
        {
            once = true;
        }
        if (once)
        {
            string str=text.text;
            string result = "";
            int point = 0;
            for (int i = 0; i < str.Length; i++)
            {
                if (point == numToChangeLine)
                {              
                    result += '\n';
                    point = 0;
                }
                result += str[i];
                point++;
            }     
            text.text = result;
            oldText = text.text;
            once = false;
        }

        

    }
}
