using Spire.Xls;
using VTuber.Character;
using VTuber.ScheduleSystem.UI;

namespace VTuber.EventSystem.Events
{
    public class VPlacingConditionHeaderIndex
    {
        public const int Id = 0;
        public const int Name = 1;
        public const int Description = 2;
        public const int TargetValue = 3;
    }
    public abstract class VPlacingCondition
    {
        public uint Id { get; private set; }
        public abstract bool IsTrue(VCharacter character, VScheduleSlot slot);
        
        public VPlacingCondition(CellRange row)
        {
            Id = uint.Parse(row.Columns[VPlacingConditionHeaderIndex.Id].Value);
        }
        
    }
}