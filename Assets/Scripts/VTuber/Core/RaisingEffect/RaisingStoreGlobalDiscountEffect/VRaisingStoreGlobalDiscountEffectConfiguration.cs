using Spire.Xls;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingStoreGlobalDiscountEffectConfiguration : VRaisingEffectConfiguration
    {
        public VRaisingStoreGlobalDiscountEffectConfiguration(CellRange row) : base(row)
        {
        }

        public override VRaisingEffect CreateEffect(string parameter, string upgradedParameter)
        {
            return new VRaisingStoreGlobalDiscountEffect(this, parameter, upgradedParameter);
        }
    }
}