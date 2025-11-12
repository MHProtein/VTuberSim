using Spire.Xls;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingUpgradeCoopLevelConfiguration : VRaisingEffectConfiguration
    {
        public uint cooperatorID;

        public VRaisingUpgradeCoopLevelConfiguration(CellRange row) : base(row)
        {
        }

        public override VRaisingEffect CreateEffect(string parameter, string upgradedParameter)
        {
            return new VRaisingUpgradeCoopLevel(this, parameter);
        }
    }
}