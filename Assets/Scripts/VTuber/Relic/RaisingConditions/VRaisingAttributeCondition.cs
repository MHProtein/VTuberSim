using System;
using System.Collections.Generic;
using Spire.Xls;
using VTuber.Character;

namespace VTuber.Relic
{
    public class VRaisingRelicAttributeCondition : VRaisingRelicCondition
    {
        public enum VRaisingAttributeConditionType
        {
            Value,
            Delta
        }
        
        public VRaisingAttributeConditionType conditionType;
        public int targetValue;

        public VRaisingRelicAttributeCondition(CellRange row) : base(row)
        {
            conditionType = Enum.Parse<VRaisingAttributeConditionType>(row.Columns[VRaisingRelicConditionHeaderIndex.ConditionType].Value.Trim());
            targetValue = int.Parse(row.Columns[VRaisingRelicConditionHeaderIndex.Value].Value.Trim());
        }

        public override bool IsTrue(VCharacter character, Dictionary<string, object> message)
        {
            switch (conditionType)
            {
                case VRaisingAttributeConditionType.Value:
                    return Compare((int)message["NewValue"], targetValue);
                case VRaisingAttributeConditionType.Delta:
                    return Compare((int)message["Delta"], targetValue);
            }

            return false;
        }
        
        
    }
}