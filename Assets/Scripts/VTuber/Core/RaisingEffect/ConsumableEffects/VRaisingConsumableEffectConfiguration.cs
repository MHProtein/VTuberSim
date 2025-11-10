using System.Collections.Generic;
using Spire.Xls;

namespace VTuber.Core.RaisingEffect
{
    public abstract class VRaisingConsumableEffectConfiguration : VRaisingEffectConfiguration
    {
        public readonly List<float> rarityProbabilities;

        public VRaisingConsumableEffectConfiguration(CellRange row) : base(row)
        {
            rarityProbabilities = new List<float>();
            var parameterStr = row.Columns[VRaisingEffectHeaderIndex.Param].Value;

            var parameters = parameterStr.Split(',');
            for (var i = 0; i < 3; i++) rarityProbabilities.Add(float.Parse(parameters[i].Trim()));
        }
    }
}