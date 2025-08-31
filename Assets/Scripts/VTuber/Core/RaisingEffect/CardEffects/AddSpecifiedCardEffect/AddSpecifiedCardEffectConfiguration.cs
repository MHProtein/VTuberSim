using System.Collections.Generic;
using Spire.Xls;
using VTuber.Character;
using VTuber.Core.Managers;

namespace VTuber.Core.RaisingEffect
{
    public class VAddSpecifiedCardEffectConfiguration : VRaisingEffectConfiguration
    {
        public VAddSpecifiedCardEffectConfiguration(CellRange row) : base(row)
        {
            
        }

        public override VRaisingEffect CreateEffect(string parameter, string upgradedParameter)
        {
            return new VAddSpecifiedCardEffect(this, parameter);
        }
    }
}