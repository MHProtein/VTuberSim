using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.Utilities;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UI;
using VTuber.Core.Foundation;


public class DialogContent
{
    public int id;
    public string iconId;
    public string imageId;
    public string speakerName;
    public bool ifOption;
    public bool ifPlayer;
    public int effectID;
    public string effectParameter;
    public string context;
    public string optionDescription;
    public int nextId;
    public bool ifImage;
}

public class Dialog
{
    //对话csv文件
    public TextAsset csvFile;
    
    //当前对话id
    public int index;

    [HideInInspector] public bool loaded = false;
    
    //对话内容表
    public Dictionary<int,DialogContent> contentDic = new Dictionary<int, DialogContent>();
    
    public Dialog(TextAsset csvFile)
    {
        loaded = false;
        this.csvFile = csvFile;
        InitDialog();
    }

    public void InitDialog()
    {
        loaded = true;
        string[] data = csvFile.text.Split(new char[] { '\n' });
        
        for (int i = 1; i < data.Length-1; i++)
        {
            string[] row = data[i].Split(new char[] { ',' });
            DialogContent dc = new DialogContent();
            // 处理每一行数据
            for (int j = 0; j < row.Length; j++)
            { 
                switch (j)
                { 
                    case 0:
                        dc.id = int.Parse(row[j]);
                        break;
                    case 1:
                        if (row[j].Equals("1"))
                        {
                            dc.ifOption = true;
                        }
                        break;
                    case 2:
                        dc.optionDescription = row[j];
                        break;
                    case 3:
                        string parameterStr = row[j];
                        string[] parameters = parameterStr.Split('\\');
                        if (parameters[0].IsNullOrWhitespace())
                        {
                            dc.effectID = -1;
                            break;
                        }
                        dc.effectID = int.Parse(parameters[0].Trim());
                        if (parameters.Length == 2)
                            dc.effectParameter = parameters[1];
                        break;
                    case 4:
                        if (row[j].Equals("1"))
                        {
                            dc.ifPlayer = true;
                        }
                        break;
                    case 5:
                        dc.iconId = row[j];
                        break;
                    case 6:
                        if (row[j].Equals("1"))
                        {
                            dc.ifImage = true;
                        }
                        break;
                    case 7:
                        dc.imageId = row[j];
                        break;
                    case 8:
                        dc.context = row[j];
                        break;
                    case 9:
                        dc.speakerName = row[j];
                        break;
                    case 10:
                        dc.nextId = int.Parse(row[j]);
                        break;
                }
            }
            
            contentDic.Add(dc.id, dc);
        }
    }
}
