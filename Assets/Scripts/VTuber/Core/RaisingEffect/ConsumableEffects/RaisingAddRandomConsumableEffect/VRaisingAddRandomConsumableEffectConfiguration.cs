using Spire.Xls;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingAddRandomConsumableEffectConfiguration : VRaisingConsumableEffectConfiguration
    {
        public VRaisingAddRandomConsumableEffectConfiguration(CellRange row) : base(row)
        {
        }

        public override VRaisingEffect CreateEffect(string parameter, string upgradedParameter)
        {
            throw new System.NotImplementedException();
        }
    }
}