using Spire.Xls;

namespace VTuber.ScheduleSystem.Events.DialogueEvent
{
    public class VStreamEventHeaderIndex
    {
        public const int DialogueNode = 10;
    }
    public class VDialogueEventConfiguration : VScheduleEventConfiguration
    {
        public string dialogueNode;

        public VDialogueEventConfiguration(CellRange row) : base(row)
        {
            dialogueNode = row.Columns[VStreamEventHeaderIndex.DialogueNode].Value;
        }

        public override VScheduleEvent CreateEvent()
        {
            return new VDialogueEvent(this);
        }
    }
}