using System.Collections.Generic;
using System.Linq;
using Spire.Xls;
using VTuber.BattleSystem.Effect;

namespace VTuber.Core.RaisingEffect
{
    public abstract class VRaisingCardEffectConfiguration : VRaisingEffectConfiguration
    {
        public readonly List<float> rarityProbabilities;
        public readonly List<float> upgradeProbabilities;
        public VRaisingCardEffectConfiguration(CellRange row) : base(row)
        {
            rarityProbabilities = new List<float>();
            upgradeProbabilities = new List<float>();
            
            string parameterStr = row.Columns[VRaisingEffectHeaderIndex.Param].Value;

            string[] parameters = parameterStr.Split(',');
            for (int i = 0; i < 6; i++)
            {
                if (i < 3)
                    rarityProbabilities.Add(float.Parse(parameters[i].Trim()));
                else
                    upgradeProbabilities.Add(float.Parse(parameters[i].Trim()));
            }
        }

        public override VRaisingEffect CreateEffect(string parameter, string upgradedParameter)
        {
            return new VRaisingCardEffect(this);
        }
    }
}