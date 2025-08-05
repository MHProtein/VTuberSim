using Spire.Xls;
using VTuber.Character;
using VTuber.Core.Managers;
using VTuber.ScheduleSystem.Core;

namespace VTuber.ScheduleSystem.Events
{
    public class VPhaseEndingConditionHeaderIndex
    {
        public const int Id = 0;
        public const int Name = 1;
        public const int Description = 2;
        public const int Type = 3;
        public const int NameOrID = 4;
        public const int Parameter = 5;
    }
    
    public class VPhaseEndingAttributeCondition : VPhaseEndingCondition
    {
        public string attributeName;
        public int requiredValue;

        public VPhaseEndingAttributeCondition(CellRange row) : base(row)
        {
            attributeName = row.Columns[VPhaseEndingConditionHeaderIndex.NameOrID].Value;
            requiredValue = int.Parse(row.Columns[VPhaseEndingConditionHeaderIndex.Parameter].Value);
        }

        public override bool IsConditionMet(VCharacter character)
        {
            if (character.AttributeManager.TryGetAttribute(attributeName, out var attribute))
            {
                return attribute.Value >= requiredValue;
            }
            return false;
        }

        public override string GetDescription()
        {
            return $" {attributeName} 大于 {requiredValue}";
        }
    }
    
    public  class VPhaseEndingEventCondition : VPhaseEndingCondition
    {
        public VScheduleEventType eventType;
        public uint eventId;
        public VPhaseEndingEventCondition(CellRange row) : base(row)
        {
            eventId =  uint.Parse(row.Columns[VPhaseEndingConditionHeaderIndex.NameOrID].Value);
        }
        
        public override bool IsConditionMet(VCharacter character)
        {
            return character.HasCompletedEvent(eventId);
        }

        public override string GetDescription()
        {
            VScheduleEventConfiguration eventConfiguration;
            if(eventType == VScheduleEventType.Stream)
            {
                eventConfiguration = VResourcesManager.Instance.GetStreamEventConfigurationByID(eventId);
            }
            else
            {
                eventConfiguration = VResourcesManager.Instance.GetDialogueEventConfigurationByID(eventId);
            }

            return $"完成 {eventConfiguration.eventName} 事件";
        }
    }
    
    public abstract class VPhaseEndingCondition
    {
        public uint id;

        public VPhaseEndingCondition(CellRange row)
        {
            id = uint.Parse(row.Columns[VPhaseEndingConditionHeaderIndex.Id].Value);
        }
        public virtual bool IsConditionMet(VCharacter character)
        {
            // Default implementation, can be overridden by derived classes
            return false;
        }

        public abstract string GetDescription();

    }
}