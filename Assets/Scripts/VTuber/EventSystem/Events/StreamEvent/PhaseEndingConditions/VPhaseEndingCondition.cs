using Spire.Xls;
using VTuber.Character;

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
    }
    
    public class VPhaseEndingEventCondition : VPhaseEndingCondition
    {
        public uint eventId;
        public VPhaseEndingEventCondition(CellRange row) : base(row)
        {
            eventId =  uint.Parse(row.Columns[VPhaseEndingConditionHeaderIndex.NameOrID].Value);
        }
        
        public override bool IsConditionMet(VCharacter character)
        {
            return character.HasCompletedEvent(eventId);
        }
    }
    
    public class VPhaseEndingCondition
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
    }
}