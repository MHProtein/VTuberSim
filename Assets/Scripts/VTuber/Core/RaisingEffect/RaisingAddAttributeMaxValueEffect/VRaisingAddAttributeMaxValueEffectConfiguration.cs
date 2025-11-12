using Spire.Xls;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingAddAttributeMaxValueEffectConfiguration : VRaisingEffectConfiguration
    {
        public string attributeName;

        public VRaisingAddAttributeMaxValueEffectConfiguration(CellRange row) : base(row)
        {
            attributeName = row.Columns[VRaisingEffectHeaderIndex.Param].Value;
        }

        public override VRaisingEffect CreateEffect(string parameter, string upgradedParameter)
        {
            return new VRaisingAddAttributeMaxValueEffect(this, parameter, upgradedParameter);
        }
    }
}