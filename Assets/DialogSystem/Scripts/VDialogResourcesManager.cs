using System.Collections.Generic;
using UnityEngine;
using VTuber.Core.Foundation;

public class VDialogResourcesManager : VSingleton<VDialogResourcesManager>
{
    private Dictionary<string, Dialog> dialogDic = new Dictionary<string, Dialog>();
    
    public void LoadDialogs()
    {
        var assets = Resources.LoadAll<TextAsset>("Dialogs");
        foreach (var asset in assets)
        {
            dialogDic.Add(asset.name, new Dialog(asset));
        }
    }
    
    public Dialog TryGetDialog(string name)
    {
        if(dialogDic.TryGetValue(name, out var dialog)) 
            return dialog;
        
        Debug.LogError($"没有找到对应的对话 {name}");
        return null;
    }
}
