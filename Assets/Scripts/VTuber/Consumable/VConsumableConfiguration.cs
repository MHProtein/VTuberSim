using System.Collections.Generic;
using VTuber.BattleSystem.Core;
using VTuber.BattleSystem.Effect;
using VTuber.Core.RaisingEffect;

namespace VTuber.Consumable
{
    public class VRaisingConsumableConfiguration : VConsumableConfiguration
    {
        public List<VRaisingEffect> effects;
    }
    public class VBattleConsumableConfiguration
    {
        public List<VEffect> effects;

        public void ApplyEffects(VBattle battle)
        {
        }
    }

    public class VConsumableConfiguration
    {
        public uint id;
        public string name;
        public string description;
        public VConsumableRarity rarity;
    }

    public enum VConsumableRarity
    {
        Common,
        Rare, 
        Epic,
    }

    public enum VConsumableType
    {
        Stream,
        Raising
    }
}