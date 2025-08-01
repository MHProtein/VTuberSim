
using System;
using Spire.Xls;
using VTuber.BattleSystem.Effect;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingEffectHeaderIndex
    {
        public const int Id = 0;
        public const int Name = 1;
        public const int Description = 2;
        public const int Type = 3;
        public const int Condition = 4;
        public const int Param = 5;
    }
    
    public class VRaisingEffectConfiguration
    {
        public uint id;
        public string effectName;
        public string description;

        public VRaisingEffectConfiguration(CellRange row)
        {
            id = Convert.ToUInt32(row.Columns[VRaisingEffectHeaderIndex.Id].Value);
            effectName = row.Columns[VRaisingEffectHeaderIndex.Name].Value;
            description = row.Columns[VRaisingEffectHeaderIndex.Description].Value;
        }
        
        public virtual VRaisingEffect CreateEffect(string parameter)
        {
            return new VRaisingEffect(this);
        }
        
    }
}