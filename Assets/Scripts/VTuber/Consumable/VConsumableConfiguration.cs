using System;
using System.Collections.Generic;
using Sirenix.Utilities;
using Spire.Xls;
using VTuber.BattleSystem.Core;
using VTuber.BattleSystem.Effect;
using VTuber.Core.Managers;
using VTuber.Core.RaisingEffect;

namespace VTuber.Consumable
{
    public class VConsumableHeaderIndex
    {    
        public const int Id = 0;
        public const int Name = 1;
        public const int Description = 2;
        public const int Type = 3;
        public const int Rarity = 4;
        public const int Effect1 = 5;
        public const int E1Param = 6;
        public const int Effect2 = 7;
        public const int E2Param = 8;
        public const int Effect3 = 9;
        public const int E3Param = 10;
    }
    
    public class VRaisingConsumableConfiguration : VConsumableConfiguration
    {
        public List<VRaisingEffect> effects;

        public VRaisingConsumableConfiguration(CellRange row) : base(row)
        {
            effects = new List<VRaisingEffect>();
            for (int i = VConsumableHeaderIndex.Effect1; i <= VConsumableHeaderIndex.E3Param; i += 2)
            {
                var effectIDStr = row.Columns[i].Value;
                if (effectIDStr.IsNullOrWhitespace())
                    continue;
                effects.Add(VResourcesManager.Instance.CreateRaisingEffectByID(Convert.ToUInt32(effectIDStr.Trim()),
                    row.Columns[i + 1].Value.Trim(), row.Columns[i + 1].Value.Trim()));
            }
        }

        public override VConsumable CreateConsumable()
        {
            return new VRaisingConsumable(idDistributor++, this);
        }
    }
    public class VBattleConsumableConfiguration : VConsumableConfiguration
    {
        public List<VEffect> effects;

        public VBattleConsumableConfiguration(CellRange row) : base(row)
        {
            effects = new List<VEffect>();
            for (int i = VConsumableHeaderIndex.Effect1; i <= VConsumableHeaderIndex.E3Param; i += 2)
            {
                var effectIDStr = row.Columns[i].Value;
                if (effectIDStr.IsNullOrWhitespace())
                    continue;
                effects.Add(VResourcesManager.Instance.CreateEffectByID(Convert.ToUInt32(effectIDStr.Trim()),
                    row.Columns[i + 1].Value.Trim(), row.Columns[i + 1].Value.Trim()));
            }
        }

        public override VConsumable CreateConsumable()
        {
            return new VBattleConsumable(idDistributor++, this);
        }
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
    
    public class VConsumableConfiguration
    {
        public uint id;
        public string name;
        public string description;
        public VConsumableRarity rarity;
        protected uint idDistributor = 0;

        public VConsumableConfiguration(CellRange row)
        {
            id = Convert.ToUInt32(row.Columns[VConsumableHeaderIndex.Id].Value.Trim());
            name = row.Columns[VConsumableHeaderIndex.Name].Value.Trim();
            description = row.Columns[VConsumableHeaderIndex.Description].Value.Trim();
            rarity = Enum.Parse<VConsumableRarity>(row.Columns[VConsumableHeaderIndex.Rarity].Value.Trim());
        }
        
        public virtual VConsumable CreateConsumable()
        {
            return new VConsumable(0, this);
        }
        
    }


}