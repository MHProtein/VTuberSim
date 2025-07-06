using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using VTuber.BattleSystem.Core.Buff.Actions;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.Core.Buff
{
    public class VBuffConfiguration : VScriptableObject
    {
        public string actionName;
        public string description;
        public Sprite sprite;
        public int duration;
        public int maxStacks;

        public List<VActionConfiguration> actionConfigurations;
    }
}