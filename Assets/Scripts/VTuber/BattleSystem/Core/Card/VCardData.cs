using UnityEngine;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.Core.Data
{
    public class VCardData : VScriptableObject
    {
        public string cardName;
        public string description;
        public int cost;
        public Sprite icon;
    }
}