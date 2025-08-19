using Spire.Xls;
using VTuber.Core.Managers;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingUpgradeRandomCardEffectConfiguration : VRaisingCardEffectConfiguration
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