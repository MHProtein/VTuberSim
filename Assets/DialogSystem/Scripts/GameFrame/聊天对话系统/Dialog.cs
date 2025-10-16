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
using VTuber.Relic;


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
    public bool isDM;

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
    public struct CharacterInfo
    {
        public string name;
        public string icon;
    }
    //对话csv文件
    public TextAsset csvFile;
    
    //当前对话id
    public int index;
    public bool isDM;

    [HideInInspector] public bool loaded = false;
    public string dialogName;
    public List<CharacterInfo> characterInfos = new List<CharacterInfo>();
    
    //对话内容表
    public Dictionary<int,DialogContent> contentDic = new Dictionary<int, DialogContent>();
    
    public Dialog(TextAsset csvFile)
    {
        loaded = false;
        this.csvFile = csvFile;
        InitDialog();
    }

    private VRaisingEffect GetEffect(string parameterStr)
    {
        
        string[] parameters = parameterStr.Split('\\');
        if (parameters[0].IsNullOrWhitespace())
        {
            return null;
        }
        uint effectID = 0;
        try
        {
            effectID = uint.Parse(parameters[0].Trim());
        }
        catch(Exception e)
        {
            VDebug.Log(e.Message);
        }
        string effectParameter = "";
        if (parameters.Length == 2)
            effectParameter = parameters[1];
        return VDataManager.Instance.CreateRaisingEffectByID(effectID, effectParameter, effectParameter);
    }

    public List<VRelicConfiguration> GetRelics()
    {
        List<VRelicConfiguration> relics = new List<VRelicConfiguration>();
        foreach (var line in contentDic.Values)
        {
            foreach (var effect in line.effects)
            {
                if (effect is VRaisingAddRelicEffect relicEffect)
                {
                    relics.Add(VDataManager.Instance.Relics[relicEffect.RelicId]);
                }
            }
        }

        return relics;
    }
    
    public List<VRaisingEffect> GetEffects()
    {
        List<VRaisingEffect> effects = new List<VRaisingEffect>();
        
        foreach (var line in contentDic.Values)
        {
            effects.AddRange(line.effects);
        }

        return effects;
    }

    public void InitDialog()
    {
        loaded = true;
        string[] data = csvFile.text.Split(new char[] { '\n' });

        try
        {
            dialogName = data[0].Split(new char[] { ',' })[0];
        
            var rawCharacterInfos = data[1].Split(new char[] { ',' });
            
            
            for (int i = 0; i < rawCharacterInfos.Length; i++)
            {
                if (rawCharacterInfos[i].IsNullOrWhitespace())
                    break;
                CharacterInfo characterInfo = new CharacterInfo();
                var info = rawCharacterInfos[i].Split(new char[] { '\\' });
                characterInfo.name = info[0];
                characterInfo.icon = info[1];
                characterInfos.Add(characterInfo);
            }
        }
        catch (Exception e)
        {
            VDebug.LogError("对话 " + csvFile.name + "前两行有问题");
            throw;
        }
        

        isDM = characterInfos.Count == 2;

        for (int i = 3; i < data.Length - 1; i++) 
        {
            int j = 0;
            try
            {
                string[] row = data[i].Split(new char[] { ',' });
                if(row[0].IsNullOrWhitespace())
                    break;
                DialogContent dc = new DialogContent();
                dc.isDM = isDM;
                // 处理每一行数据
                for (j = 0; j < row.Length; j++)
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
                            if (row[j].Equals("1"))
                            {
                                dc.ifImage = true;
                            }
                            break;
                        case 9:
                            dc.imageId = row[j];
                            break;
                        case 10:
                            dc.context = row[j];
                            break;
                        case 11:
                            int index = int.Parse(row[j]) - 1;
                            dc.speakerName = characterInfos[index].name;
                            dc.iconId = characterInfos[index].icon;
                            break;
                        case 12:
                            dc.nextId = int.Parse(row[j]);
                            break;
                    }
                }
                contentDic.Add(dc.id, dc);
            }
            catch (Exception e)
            {
                VDebug.LogError("对话 " + csvFile.name + " " + i + "行" + j + "有问题 " + e.Message);
                throw;
            }
            
        }
    }
}
