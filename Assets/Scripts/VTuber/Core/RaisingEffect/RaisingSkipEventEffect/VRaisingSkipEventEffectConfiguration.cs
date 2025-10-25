using Spire.Xls;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingSkipEventEffectConfiguration : VRaisingEffectConfiguration
    {
        public VRaisingSkipEventEffectConfiguration(CellRange row) : base(row)
        {
        }

        public override VRaisingEffect CreateEffect(string parameter, string upgradedParameter)
        {
            return new VRaisingSkipEventEffect(this);
        }
    }
}