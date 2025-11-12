using System;
using Spire.Xls;

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

    public abstract class VRaisingEffectConfiguration
    {
        public string description;
        public string effectName;
        public uint id;

        public VRaisingEffectConfiguration(CellRange row)
        {
            id = Convert.ToUInt32(row.Columns[VRaisingEffectHeaderIndex.Id].Value);
            effectName = row.Columns[VRaisingEffectHeaderIndex.Name].Value;
            description = row.Columns[VRaisingEffectHeaderIndex.Description].Value;
        }

        public abstract VRaisingEffect CreateEffect(string parameter, string upgradedParameter);
    }
}