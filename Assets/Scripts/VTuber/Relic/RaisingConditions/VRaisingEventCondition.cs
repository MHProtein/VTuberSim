using System;
using System.Collections.Generic;
using Spire.Xls;
using VTuber.Character;
using VTuber.ScheduleSystem.Core;
using VTuber.ScheduleSystem.Events;

namespace VTuber.Relic
{
    public class VRaisingEventCondition : VRaisingRelicCondition
    {
        public enum VEventConditionType
        {
            ID,
            Type
        }

        public VEventConditionType conditionType;
        public string targetValue;
        
        public VRaisingEventCondition(CellRange row) : base(row)
        {
            conditionType = Enum.Parse<VEventConditionType>(row.Columns[VRaisingRelicConditionHeaderIndex.ConditionType].Value.Trim());
            targetValue = row.Columns[VRaisingRelicConditionHeaderIndex.Value].Value.Trim();
        }

        public override bool IsTrue(VCharacter character, Dictionary<string, object> message)
        {
            switch (conditionType)
            {
                case VEventConditionType.ID:
                    return (message["Event"] as VScheduleEvent).EventID == uint.Parse(targetValue.Trim());
                case VEventConditionType.Type:
                    return (message["Event"] as VScheduleEvent).Type == Enum.Parse<VScheduleEventType>(targetValue.Trim());
            }

            return false;
        }
    }
}