using System;
using Spire.Xls;
using VTuber.Core.Managers;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingDeleteSelectedConfiguration : VRaisingEffectConfiguration
    {
        public VCardCondition Condition => _condition;
        private VCardCondition _condition;
        
        public VRaisingDeleteSelectedConfiguration(CellRange row) : base(row)
        {
        }

        public override VRaisingEffect CreateEffect(string parameter, string upgradedParameter)
        {
            return new VRaisingDeleteSelected(this);
        }
    }
}