using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using VTuber.Core.Foundation;

[System.Serializable]
public class TreeDisplayData
{
    public string name;
    public string path;
    public ScriptableObject so;
}

[System.Serializable]
public class KeyValueData<Ikey, T>
{
    public Ikey key;
    public T value;

    public KeyValueData()
    {

    }

    public KeyValueData(Ikey key, T value)
    {
        this.key = key;
        this.value = value;
    }
}

public class SO_PreviewCfg: VScriptableObject
{      
    public static SO_PreviewCfg Instance => Resources.Load<SO_PreviewCfg>("so_preview cfg");

    public List<KeyValueData<string, List<TreeDisplayData>>> displayDatas;

    public void AddMark(string key, string name, ScriptableObject so)
    {
        int index = displayDatas.FindIndex((i) => i.key == key);
        if (index == -1)
        {
            displayDatas.Add(new KeyValueData<string, List<TreeDisplayData>>(key, new List<TreeDisplayData>()));
            index = displayDatas.Count - 1;
        }

        displayDatas[index].value.Add(new TreeDisplayData()
        {
            name = name,
            so = so,
        });
    }

}
