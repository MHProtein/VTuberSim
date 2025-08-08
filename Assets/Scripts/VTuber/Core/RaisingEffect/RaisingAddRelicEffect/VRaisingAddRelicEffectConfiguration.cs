using Spire.Xls;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingAddRelicEffectConfiguration : VRaisingEffectConfiguration
    {
        public uint relicId;
        public VRaisingAddRelicEffectConfiguration(CellRange row) : base(row)
        {
            relicId = uint.Parse(row.Columns[VRaisingEffectHeaderIndex.Param].Value);
        }

        public override VRaisingEffect CreateEffect(string parameter, string upgradedParameter)
        {
            return new VRaisingAddRelicEffect(this);
        }
    }
}