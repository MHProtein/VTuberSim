using System.Collections.Generic;
using VTuber.Character;
using VTuber.Core.EventCenter;

namespace VTuber.ScheduleSystem.Events.DialogueEvent
{
    public class VDialogueEvent : VScheduleEvent
    {
        public string dialogueNode;

        public VDialogueEvent(VDialogueEventConfiguration config) : base(config)
        {
            this.dialogueNode = config.dialogueNode;
        }
        
        public override bool Execute(VCharacter player)
        {
            if (!CanExecute(player))
                return false;
            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnEventStart, new Dictionary<string, object>()
            {
                {"Event", this},
                {"DialogueNode", dialogueNode}
            });
            IsExecuted = true;
            return true;
        }
        
    }
}