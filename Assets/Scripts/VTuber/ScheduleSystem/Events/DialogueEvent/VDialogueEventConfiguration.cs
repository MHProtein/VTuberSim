namespace VTuber.ScheduleSystem.Events.DialogueEvent
{
    public class VDialogueEventConfiguration : VScheduleEventConfiguration
    {
        public string dialogueNode;
        public override VScheduleEvent CreateEvent()
        {
            return new VDialogueEvent(this);
        }
    }
}