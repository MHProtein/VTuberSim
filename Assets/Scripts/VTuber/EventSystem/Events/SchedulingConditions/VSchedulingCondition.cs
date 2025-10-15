using System;
using System.Collections.Generic;
using Sirenix.Utilities;
using Spire.Xls;
using VTuber.Character;
using VTuber.Core.Managers;
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
        ExcludeID,
    }

    public enum VSchedulingConditionPositionPatterns
    {
        None,
        UD,
        LR,
        UDLR,
        All,
    }
    
    public class VSchedulingConditionHeaderIndex
    {
        public const int Id = 0;
        public const int Name = 1;
        public const int Description = 2;
        public const int PlacingCondition = 3;
        public const int Pattern = 4;
        public const int TargetType = 5;
        public const int TargetValue = 6;
        public const int Effect1 = 7;
        public const int E1Param = 8;
        public const int Effect2 = 9;
        public const int E2Param = 10;
        public const int Effect3 = 11;
        public const int E3Param = 12;
    }
    
    public class VSchedulingCondition
    {
        public uint Id => _id;
        private uint _id;
        
        private VPlacingCondition _placingCondition;
        private VSchedulingConditionPositionPatterns _positionPattern;
        private VSchedulingConditionType _type;
        
        public List<VRaisingEffect> Effects => _effects;
        private List<VRaisingEffect> _effects;
        
        private uint _targetID;
        private VEventType _targetType;

        public VSchedulingCondition(CellRange row)
        {
            _id = uint.Parse(row.Columns[VSchedulingConditionHeaderIndex.Id].Value);
            
            string placingConditionStr = row.Columns[VSchedulingConditionHeaderIndex.PlacingCondition].Value;
            if (!placingConditionStr.IsNullOrWhitespace())
                _placingCondition = VDataManager.Instance.GetPlacingCondtionByID(uint.Parse(placingConditionStr));

            string typeStr = row.Columns[VSchedulingConditionHeaderIndex.TargetType].Value;
            if (!typeStr.IsNullOrWhitespace())
            {
                _positionPattern = Enum.Parse<VSchedulingConditionPositionPatterns>(row.Columns[VSchedulingConditionHeaderIndex.Pattern].Value);
                _type = Enum.Parse<VSchedulingConditionType>(typeStr);

                switch (_type)
                {
                    case VSchedulingConditionType.ID:
                        _targetID = uint.Parse(row.Columns[VSchedulingConditionHeaderIndex.TargetValue].Value);
                        break;
                    case VSchedulingConditionType.Type:
                        _targetType = Enum.Parse<VEventType>(row.Columns[VSchedulingConditionHeaderIndex.TargetValue].Value);
                        break;
                    case VSchedulingConditionType.SameType:
                        break;
                    case VSchedulingConditionType.ExcludeType:
                        _targetType = Enum.Parse<VEventType>(row.Columns[VSchedulingConditionHeaderIndex.TargetValue].Value);
                        break;
                    case VSchedulingConditionType.ExcludeID:
                        _targetID = uint.Parse(row.Columns[VSchedulingConditionHeaderIndex.TargetValue].Value);
                        break;
                }
            }
            
            _effects = new List<VRaisingEffect>();
            for (int i = VSchedulingConditionHeaderIndex.Effect1; i <= VSchedulingConditionHeaderIndex.E3Param; i += 2)
            {
                var effectIDStr = row.Columns[i].Value;
                if (effectIDStr.IsNullOrWhitespace())
                    continue;
                _effects.Add(VDataManager.Instance.CreateRaisingEffectByID(Convert.ToUInt32(effectIDStr.Trim()),
                    row.Columns[i + 1].Value.Trim(), row.Columns[i + 1].Value.Trim()));
            }
        }

        public bool IsTrue(VCharacter character, VScheduleSlot slot)
        {
            if (_placingCondition is not null)
            {
                return _placingCondition.IsTrue(character, slot);
            }
            
            List<VScheduleSlot> slots = null;

            switch (_positionPattern)
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
            
            if (_type == VSchedulingConditionType.ExcludeID)
            {
                foreach (var s in slots)
                {
                    if (s.Item is not null)
                    {
                        if (s.Item.Event.EventID == _targetID)
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