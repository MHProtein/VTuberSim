using Spire.Xls;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingEndingEffectConfiguration : VRaisingEffectConfiguration
    {
        public VRaisingEndingEffectConfiguration(CellRange row) : base(row)
        {
        }

        public override VRaisingEffect CreateEffect(string parameter, string upgradedParameter)
        {
            return new VRaisingEndingEffect(this);
        }
    }
}