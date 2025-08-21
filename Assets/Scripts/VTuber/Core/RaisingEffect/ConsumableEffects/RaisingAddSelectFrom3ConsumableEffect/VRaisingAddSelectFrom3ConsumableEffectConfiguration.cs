using Spire.Xls;

namespace VTuber.Core.RaisingEffect.ConsumableEffects
{
    public class VRaisingAddSelectFrom3ConsumableEffectConfiguration : VRaisingEffectConfiguration
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