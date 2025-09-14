using System;
using System.Collections.Generic;
using Spire.Xls;
using VTuber.Character;
using VTuber.Core.RaisingEffect;
using VTuber.EventSystem.Events;
using VTuber.ScheduleSystem.Core;
using VTuber.ScheduleSystem.UI;

namespace VTuber.ScheduleSystem.Events
{
    public enum VSchedulingConditionType
    {
        ID,
        Type,
        SameType,
        ExcludeType,
    }

    public enum VSchedulingConditionPositionPatterns
    {
        None,
        UD,
        LR,
        UDLR,
        All,
    }
    
    public class VSchedulingCondition
    {
        VPlacingCondition _placingCondition;
        private VSchedulingConditionPositionPatterns _positionPatternType;
        VSchedulingConditionType _type;
        List<VRaisingEffect> _effects;
        
        uint _targetID;
        VEventType _targetType;

        public VSchedulingCondition(CellRange row)
        {
            
        }

        public bool IsTrue(VCharacter character, VScheduleSlot slot)
        {
            if (_placingCondition is not null)
            {
                return _placingCondition.IsTrue(character, slot);
            }
            
            List<VScheduleSlot> slots = null;

            switch (_positionPatternType)
            {
                case VSchedulingConditionPositionPatterns.None:
                    return false;
                case VSchedulingConditionPositionPatterns.UD:
                    slots = slot.GetUDSlots();
                    break;
                case VSchedulingConditionPositionPatterns.LR:
                    slots = slot.GetLRSlots();
                    break;
                case VSchedulingConditionPositionPatterns.UDLR:
                    slots = slot.GetUDLRSlots();
                    break;
                case VSchedulingConditionPositionPatterns.All:
                    slots = slot.GetSurroundingSlots();
                    break;
            }

            if (slots is null)
                return false;

            if (_type == VSchedulingConditionType.ExcludeType)
            {
                foreach (var s in slots)
                {
                    if (s.Item is not null)
                    {
                        if (s.Item.Event.Type == _targetType)
                            return false;
                    }
                }
                return true;
            }
            foreach (var s in slots)
            {
                if (s.Item is not null)
                {
                    switch (_type)
                    {
                        case VSchedulingConditionType.ID:
                            return s.Item.Event.EventID == _targetID;
                        case VSchedulingConditionType.Type:
                            return s.Item.Event.Type == _targetType;
                        case VSchedulingConditionType.SameType:
                            return s.Item.Event.Type == slot.Item.Event.Type;
                    }
                }
            }
            
            return false;
        }
    }
}