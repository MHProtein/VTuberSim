using System.Collections.Generic;
using UnityEngine;
using VTuber.Core.Foundation;

public class VResourcesManager : VSingleton<VResourcesManager>
{
    private readonly Dictionary<string, Dialog> dialogDic = new();
    private readonly Dictionary<string, Sprite> spriteDic = new();

    public void Load()
    {
        LoadDialogs();
        LoadSprites();
    }

    public void LoadDialogs()
    {
        var assets = Resources.LoadAll<TextAsset>("Dialogs");
        foreach (var asset in assets) dialogDic.Add(asset.name, new Dialog(asset));
    }

    public void LoadSprites()
    {
        var assets = Resources.LoadAll<Sprite>("UI/Sprites");
        foreach (var asset in assets) spriteDic.Add(asset.name, asset);
    }

    public Dialog TryGetDialog(string name)
    {
        if (dialogDic.TryGetValue(name, out var dialog))
            return dialog;

        Debug.LogError($"没有找到对应的对话 {name}");
        return null;
    }

    public Sprite TryGetSprite(string name)
    {
        if (spriteDic.TryGetValue(name, out var sprite))
            return sprite;

        Debug.LogError($"没有找到对应的Sprite {name}");
        return null;
    }
}