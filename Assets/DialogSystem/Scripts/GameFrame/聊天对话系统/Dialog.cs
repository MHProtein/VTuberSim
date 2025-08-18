using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.Utilities;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UI;
using VTuber.Character;
using VTuber.Core.Foundation;
using VTuber.Core.Managers;
using VTuber.Core.RaisingEffect;


public class DialogContent
{
    public int id;
    public string iconId;
    public string imageId;
    public string speakerName;
    public bool ifOption;
    public bool ifPlayer;
    public List<VRaisingEffect> effects = new();
    public string context;
    public string optionDescription;
    public int nextId;
    public bool ifImage;

    public void AppleEffects(VCharacter character)
    {
        foreach (var effect in effects)
        {
            effect.ApplyEffect(character, null);
        }
    }
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

    public VRaisingEffect GetEffect(string parameterStr)
    {
        
        string[] parameters = parameterStr.Split('\\');
        if (parameters[0].IsNullOrWhitespace())
        {
            return null;
        }
        var effectID = uint.Parse(parameters[0].Trim());
        string effectParameter = "";
        if (parameters.Length == 2)
            effectParameter = parameters[1];
        return VResourcesManager.Instance.CreateRaisingEffectByID(effectID, effectParameter, effectParameter);
    }

    public void InitDialog()
    {
        loaded = true;
        string[] data = csvFile.text.Split(new char[] { '\n' });
        Debug.Log(csvFile.name);
        for (int i = 1; i < data.Length-1; i++)
        {
            string[] row = data[i].Split(new char[] { ',' });
            if(row[0].IsNullOrWhitespace())
                break;
            DialogContent dc = new DialogContent();
            // 处理每一行数据
            for (int j = 0; j < row.Length; j++)
            { 
                switch (j)
                { 
                    case 0:
                        try
                        {
                            dc.id = int.Parse(row[j]);
                        }
                        catch (Exception e)
                        {
                            Debug.Log(e.Message);
                        }

                     
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
                    case 4:
                    case 5:
                    case 6:
                        string parameterStr = row[j];
                        var effect = GetEffect(parameterStr);
                        if (effect is not null)
                        {
                            dc.effects.Add(effect);
                        }
                        break;
                    case 7:
                        if (row[j].Equals("1"))
                        {
                            dc.ifPlayer = true;
                        }
                        break;
                    case 8:
                        dc.iconId = row[j];
                        break;
                    case 9:
                        if (row[j].Equals("1"))
                        {
                            dc.ifImage = true;
                        }
                        break;
                    case 10:
                        dc.imageId = row[j];
                        break;
                    case 11:
                        dc.context = row[j];
                        break;
                    case 12:
                        dc.speakerName = row[j];
                        break;
                    case 13:
                        dc.nextId = int.Parse(row[j]);
                        break;
                }
            }
            
            contentDic.Add(dc.id, dc);
        }
    }
}
