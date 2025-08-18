using Spire.Xls;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingEnterStoreEffectConfiguration : VRaisingEffectConfiguration
    {
        public VRaisingEnterStoreEffectConfiguration(CellRange row) : base(row)
        {
        }

        public override VRaisingEffect CreateEffect(string parameter, string upgradedParameter)
        {
            return new VRaisingEnterStoreEffect(this);
        }
    }
}