using Spire.Xls;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingUpgradeRandomCardEffectConfiguration : VRaisingEffectConfiguration
    {
        public VRaisingUpgradeRandomCardEffectConfiguration(CellRange row) : base(row)
        {
        }

        public override VRaisingEffect CreateEffect(string parameter, string upgradedParameter)
        {
            return new VRaisingUpgradeRandomCardEffect(this);
        }
    }
}