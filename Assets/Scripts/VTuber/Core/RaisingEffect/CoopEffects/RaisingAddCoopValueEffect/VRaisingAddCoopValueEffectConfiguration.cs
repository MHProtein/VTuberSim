using Spire.Xls;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingAddCoopValueEffectConfiguration : VRaisingEffectConfiguration
    {
        public uint cooperatorID;

        public VRaisingAddCoopValueEffectConfiguration(CellRange row) : base(row)
        {
            cooperatorID = uint.Parse(row.Columns[VRaisingEffectHeaderIndex.Param].Value);
        }

        public override VRaisingEffect CreateEffect(string parameter, string upgradedParameter)
        {
            return new VRaisingAddCoopValueEffect(this, parameter, upgradedParameter);
        }
    }
}