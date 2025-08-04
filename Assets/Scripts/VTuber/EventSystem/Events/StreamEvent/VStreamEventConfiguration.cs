using System.Collections.Generic;
using Sirenix.Utilities;
using Spire.Xls;
using UnityEngine.Serialization;

namespace VTuber.ScheduleSystem.Events
{
    public class VStreamEventHeaderIndex
    {
        public const int TurnCount = 9;
        public const int Target = 10;
        public const int InitialViewers = 11;
        public const uint SuccessEvent = 12;
        public const uint FailEvent = 13;
        public const int PhaseEndingConditions = 14;
    }
    public class VStreamEventConfiguration : VScheduleEventConfiguration
    {
        public int initialTurnCount;
        public int targetPopularity;
        public int initialViewers;
        public uint successEvent;
        public uint failureEvent;
        public bool isPhaseEndingEvent = false;
        public List<VPhaseEndingCondition> phaseEndingConditions;

        public VStreamEventConfiguration(CellRange row) : base(row)
        {
            initialTurnCount = int.Parse(row.Columns[VStreamEventHeaderIndex.TurnCount].Value);
            targetPopularity = int.Parse(row.Columns[VStreamEventHeaderIndex.Target].Value);
            initialViewers = int.Parse(row.Columns[VStreamEventHeaderIndex.InitialViewers].Value);
            successEvent = uint.Parse(row.Columns[VStreamEventHeaderIndex.SuccessEvent].Value);
            failureEvent = uint.Parse(row.Columns[VStreamEventHeaderIndex.FailEvent].Value);
            phaseEndingConditions = new List<VPhaseEndingCondition>();
            string phaseEndingConditionsStr = row.Columns[VStreamEventHeaderIndex.PhaseEndingConditions].Value;
            if (!phaseEndingConditionsStr.IsNullOrWhitespace())
            {
                string[] conditions = phaseEndingConditionsStr.Split(',');
                isPhaseEndingEvent = true;
                foreach (var condition in conditions)
                {
                    if (int.TryParse(condition, out int conditionID))
                    {
                        if (conditionID == -1)
                        {
                            phaseEndingConditions.Clear();
                            break;
                        }
                    }
                }
            }
        }

        public override VScheduleEvent CreateEvent()
        {
            return new VStreamEvent(this);
        }
    }
}