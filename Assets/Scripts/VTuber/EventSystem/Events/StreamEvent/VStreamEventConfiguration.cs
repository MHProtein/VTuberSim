using System.Collections.Generic;
using Sirenix.Utilities;
using Spire.Xls;
using UnityEngine.Serialization;
using VTuber.Core.Managers;

namespace VTuber.ScheduleSystem.Events
{
    public class VStreamEventHeaderIndex
    {
        public const int TurnCount = 10;
        public const int Target = 11;
        public const int InitialViewers = 12;
        public const uint SuccessEvent = 13;
        public const uint FailEvent = 14;
        public const int PhaseEndingConditions = 15;
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
                        else
                        {
                            phaseEndingConditions.Add(VResourcesManager.Instance.GetPhaseEndingConditionByID((uint)conditionID));
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