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
            string parameterStr = row.Columns[VRaisingEffectHeaderIndex.Param].Value;

            string[] parameters = parameterStr.Split(',');
            for (int i = 0; i < 3; i++)
            {
                rarityProbabilities.Add(float.Parse(parameters[i].Trim()));
            }
        }
    }
}