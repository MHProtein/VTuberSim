using Spire.Xls;
using UnityEngine.Serialization;

namespace VTuber.ScheduleSystem.Events
{
    public class VStreamEventHeaderIndex
    {
        public const int DialogueNode = 9;
    }
    public class VStreamEventConfiguration : VScheduleEventConfiguration
    {
        public int initialTurnCount;

        public VStreamEventConfiguration(CellRange row) : base(row)
        {
            initialTurnCount = int.Parse(row.Columns[VStreamEventHeaderIndex.DialogueNode].Value);
        }

        public override VScheduleEvent CreateEvent()
        {
            return new VStreamEvent(this);
        }
    }
}