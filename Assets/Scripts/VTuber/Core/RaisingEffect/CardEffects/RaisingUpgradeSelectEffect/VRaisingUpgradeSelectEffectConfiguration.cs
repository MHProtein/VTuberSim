using Spire.Xls;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingUpgradeSelectEffectConfiguration : VRaisingEffectConfiguration
    {
        public VRaisingUpgradeSelectEffectConfiguration(CellRange row) : base(row)
        {
        }

        public override VRaisingEffect CreateEffect(string parameter, string upgradedParameter)
        {
            return new VRaisingUpgradeSelectEffect(this);
        }
    }
}