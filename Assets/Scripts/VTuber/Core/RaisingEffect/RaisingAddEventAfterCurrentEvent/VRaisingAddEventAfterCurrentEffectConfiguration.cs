using System;
using System.Collections.Generic;
using Spire.Xls;
using VTuber.Core.EventCenter;
using VTuber.ScheduleSystem.Core;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingAddEventAfterCurrentEffectConfiguration : VRaisingEffectConfiguration
    {
        public VScheduleEventType eventType;
        public VRaisingAddEventAfterCurrentEffectConfiguration(CellRange row) : base(row)
        {
            eventType = Enum.Parse<VScheduleEventType>(row.Columns[VRaisingEffectHeaderIndex.Param].Value.Trim());
        }

        public override VRaisingEffect CreateEffect(string parameter, string upgradedParameter)
        {
            return new VRaisingAddEventAfterCurrentEffect(this, parameter);
        }
    }
}