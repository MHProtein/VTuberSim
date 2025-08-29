using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class DialogObj : MonoBehaviour
{
    public Image speakerIcon;
    public TMP_Text nameText;
    public RectTransform contentTransform;
    public RectTransform contentDMTransform;
    public RectTransform contentGroupTransform;
    public RectTransform WordBoxTransform;
    public Text words;
    public RectTransform rect;
    private Vector2 originSize;
    public Image wordsImage;
    
    public int numToChangeLine;
    public float lineHeight=20f;
    private string oldText;
    private bool once = true;
    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        originSize = rect.sizeDelta;
         // if (words)
         // {
         //     oldText = words.text;
         // }
    }

    public void ShowDialog(DialogContent dc)
    {
        if (dc.isDM)
        {
            nameText.gameObject.SetActive(false);
            if(contentDMTransform != null)
                contentTransform.anchoredPosition = contentDMTransform.anchoredPosition;
        }
        else
        {
            nameText.gameObject.SetActive(true);
            nameText.text = dc.speakerName;
            if(contentGroupTransform != null)
                contentTransform.anchoredPosition = contentGroupTransform.anchoredPosition;
        }
        speakerIcon.sprite = Resources.Load<Sprite>($"Sprites/SpeakerIcons/{dc.iconId}");
        if (dc.ifImage)
        {
            wordsImage.sprite = Resources.Load<Sprite>($"Sprites/SpeakerImages/{dc.imageId}");
            WordBoxTransform.localScale *= 1.2f;
        }
        else
        {
            words.text = ProcessWords(dc.context);
            int lines=Mathf.FloorToInt(dc.context.Length/numToChangeLine);
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, originSize.y+lines*lineHeight);
        }
        Canvas.ForceUpdateCanvases();
    }

    private void Update()
    {
        // if (oldText != words.text)
        // {
        //     once = true;
        // }
        // if (once)
        // {
        //     string str=words.text;
        //     string result = "";
        //     int point = 0;
        //     for (int i = 0; i < str.Length; i++)
        //     {
        //         if (point == numToChangeLine)
        //         {              
        //             result += '\n';
        //             point = 0;
        //         }
        //         result += str[i];
        //         point++;
        //     }     
        //     words.text = result;
        //     oldText = words.text;
        //     int lines=Mathf.FloorToInt(words.text.Length/numToChangeLine);
        //     rect.sizeDelta = new Vector2(rect.sizeDelta.x, originSize.y+lines*lineHeight);
        //     once = false;
        // }
    }

    private string ProcessWords(string text)
    {
        string str=text;
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
        return result;
    }
}
