using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "SO/ItemData", fileName = "ItemData")]
public class EventData : ScriptableObject
{
    public string name;
    [TextArea] public string description;
    public Sprite icon;
    public Color backgroundColor = Color.white;
    public int height;
}
