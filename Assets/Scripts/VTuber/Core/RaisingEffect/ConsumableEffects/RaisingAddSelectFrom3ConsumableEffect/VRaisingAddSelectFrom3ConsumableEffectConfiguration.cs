using Spire.Xls;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingAddSelectFrom3ConsumableEffectConfiguration : VRaisingConsumableEffectConfiguration
    {
        public VRaisingAddSelectFrom3ConsumableEffectConfiguration(CellRange row) : base(row)
        {
        }

        public override VRaisingEffect CreateEffect(string parameter, string upgradedParameter)
        {
            return new VRaisingAddSelectFrom3ConsumableEffect(this);
        }
    }
}