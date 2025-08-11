using System.Collections.Generic;
using VTuber.Character;
using VTuber.Core.EventCenter;
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

        public override void ApplyEffect(VCharacter character)
        {
            base.ApplyEffect(character);

            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnAddFollowUpEvent, new Dictionary<string, object>
            {
                { "EventType", _eventType },
                { "EventId", eventId },
            });
        }
    }
}