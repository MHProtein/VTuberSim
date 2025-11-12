using Spire.Xls;
using VTuber.Core.Managers;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingAddSelectedFrom3EffectConfiguration : VRaisingCardEffectConfiguration
    {
        public VRaisingAddSelectedFrom3EffectConfiguration(CellRange row) : base(row)
        {
            var conditionStr = row.Columns[VRaisingEffectHeaderIndex.Condition].Value;
            if (string.IsNullOrEmpty(conditionStr))
                Condition = null;
            else
                Condition = VDataManager.Instance.GetCardConditionByID(uint.Parse(conditionStr.Trim()));
        }

        public VCardCondition Condition { get; }


        public override VRaisingEffect CreateEffect(string parameter, string upgradedParameter)
        {
            return new VRaisingAddSelectedFrom3Effect(this);
        }
    }
}