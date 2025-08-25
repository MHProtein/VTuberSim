using System.Collections.Generic;
using VTuber.Character;
using VTuber.Core.EventCenter;
using VTuber.Core.Managers;
using VTuber.ScheduleSystem.Core;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingAddEventAfterCurrentEffect : VRaisingEffect
    {
        private VEventType _eventType;
        private uint eventId;
        public VRaisingAddEventAfterCurrentEffect(VRaisingAddEventAfterCurrentEffectConfiguration configuration, string parameter) : base(configuration)
        {
            _eventType = configuration.eventType;
            eventId = uint.Parse(parameter.Trim());
        }

        public override void ApplyEffect(VCharacter character, Dictionary<string, object> messagedict)
        {
            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnAddFollowUpEvent, new Dictionary<string, object>
            {
                { "EventType", _eventType },
                { "EventId", eventId },
            });
        }

        public override void Upgrade()
        {
            
        }

        public override void DownGrade()
        {
            
        }
        
        public override string GetParameter()
        {
            if (_eventType == VEventType.Stream)
            {
                return VDataManager.Instance.GetStreamEventConfigurationByID(eventId).eventName;
            }
            return VDataManager.Instance.GetDialogueEventConfigurationByID(eventId).eventName;
        }
    }
}