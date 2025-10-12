using System.Collections.Generic;
using VTuber.Character;
using VTuber.Core.EventCenter;
using VTuber.Core.RaisingEffect;

namespace VTuber.ScheduleSystem.Events.DialogueEvent
{
    public class VDialogueEvent : VScheduleEvent
    {
        public string dialogueNode;
        public List<VRaisingEffect> effects;

        public VDialogueEvent(VDialogueEventConfiguration config) : base(config)
        {
            this.dialogueNode = config.dialogueNode;
            this.effects = config.effects;
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