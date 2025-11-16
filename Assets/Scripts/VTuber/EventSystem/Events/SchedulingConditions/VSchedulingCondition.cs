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
        ExcludeID
    }

    public enum VSchedulingConditionPositionPatterns
    {
        None,
        UD,
        LR,
        UDLR,
        All
    }

    public class VSchedulingConditionHeaderIndex
    {
        public const int Id = 0;
        public const int Name = 1;
        public const int Description = 2;
        public const int PlacingCondition = 3;
        public const int ExecuteBeforeEvent = 4; // 在事件前执行
        public const int Pattern = 5;
        public const int TargetType = 6;
        public const int TargetValue = 7;
        public const int Effect1 = 8;
        public const int E1Param = 9;
        public const int Effect2 = 10;
        public const int E2Param = 11;
        public const int Effect3 = 12;
        public const int E3Param = 13;
    }

    public class VSchedulingCondition
    {
        public uint Id => _id;
        private uint _id;
        
        
        // --- 新增：公开属性供 UI 读取 ---
        public VSchedulingConditionType Type => _type;
        public uint TargetID => _targetID;
        public VEventType TargetType => _targetType;
    
        // 辅助属性：判断目标是否为直播流事件 (用于 VEventUI 查询)
        // 注意：这里假设如果 TargetType 是 Stream 类，或者是 ID 且该 ID 对应的是 Stream
        // 具体判断逻辑取决于你的策划配置表是否在 ID 模式下也指定了类型。
        // 如果配置表中 ID 模式下没有指定类型，你可能需要尝试获取或在 VSchedulingCondition 解析时存储 IsStream。
        public bool IsTargetStream => _targetType == VEventType.Stream;
        
        
        
        public List<VRaisingEffect> Effects => _effects;
        private List<VRaisingEffect> _effects;
        
        // Add this line to expose the Position Pattern for UI purposes
        public VSchedulingConditionPositionPatterns PositionPattern => _positionPattern;
        
        private readonly VPlacingCondition _placingCondition;
        private readonly VSchedulingConditionPositionPatterns _positionPattern;

        private readonly bool _isStream;
        private readonly uint _targetID;
        private readonly VEventType _targetType;
        private readonly VSchedulingConditionType _type;
        private readonly bool _shouldExecuteBeforeEvent;
        
        public bool ShouldExecuteBeforeEvent => _shouldExecuteBeforeEvent;
        
        public VSchedulingCondition(CellRange row)
        {
            _id = uint.Parse(row.Columns[VSchedulingConditionHeaderIndex.Id].Value);

            var placingConditionStr = row.Columns[VSchedulingConditionHeaderIndex.PlacingCondition].Value;
            if (!placingConditionStr.IsNullOrWhitespace())
                _placingCondition = VDataManager.Instance.GetPlacingCondtionByID(uint.Parse(placingConditionStr));

            var typeStr = row.Columns[VSchedulingConditionHeaderIndex.TargetType].Value;
            
            _shouldExecuteBeforeEvent = int.Parse(row.Columns[VSchedulingConditionHeaderIndex.ExecuteBeforeEvent].Value) == 1;
            
            if (!typeStr.IsNullOrWhitespace())
            {
                _positionPattern =
                    Enum.Parse<VSchedulingConditionPositionPatterns>(row
                        .Columns[VSchedulingConditionHeaderIndex.Pattern].Value);
                _type = Enum.Parse<VSchedulingConditionType>(typeStr);

                switch (_type)
                {
                    case VSchedulingConditionType.ID:
                    {
                        var targetStr = row.Columns[VSchedulingConditionHeaderIndex.TargetValue].Value;
                        if (targetStr.Contains("S"))
                        {
                            _isStream = true;
                            targetStr = targetStr.Substring(1);
                        }
                        _targetID = uint.Parse(targetStr);
                        break;
                    }
                    case VSchedulingConditionType.Type:
                        _targetType =
                            Enum.Parse<VEventType>(row.Columns[VSchedulingConditionHeaderIndex.TargetValue].Value);
                        break;
                    case VSchedulingConditionType.SameType:
                        break;
                    case VSchedulingConditionType.ExcludeType:
                        _targetType =
                            Enum.Parse<VEventType>(row.Columns[VSchedulingConditionHeaderIndex.TargetValue].Value);
                        break;
                    case VSchedulingConditionType.ExcludeID:
                    {
                        var targetStr = row.Columns[VSchedulingConditionHeaderIndex.TargetValue].Value;
                        if (targetStr.Contains("S"))
                        {
                            _isStream = true;
                            targetStr = targetStr.Substring(1);
                        }
                        _targetID = uint.Parse(targetStr);
                        break;
                    }
                }
            }

            _effects = new List<VRaisingEffect>();
            for (var i = VSchedulingConditionHeaderIndex.Effect1; i <= VSchedulingConditionHeaderIndex.E3Param; i += 2)
            {
                var effectIDStr = row.Columns[i].Value;
                if (effectIDStr.IsNullOrWhitespace())
                    continue;
                Effects.Add(VDataManager.Instance.CreateRaisingEffectByID(Convert.ToUInt32(effectIDStr.Trim()),
                    row.Columns[i + 1].Value.Trim(), row.Columns[i + 1].Value.Trim()));
            }
        }

        public bool IsTrue(VCharacter character, VScheduleSlot slot)
        {
            if (_placingCondition is not null) return _placingCondition.IsTrue(character, slot);

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
                    if (s.Item is not null)
                        if (s.Item.Event.Type == _targetType)
                            return false;

                return true;
            }

            if (_type == VSchedulingConditionType.ExcludeID)
            {
                foreach (var s in slots)
                    if (s.Item is not null)
                    {
                        if (_isStream && s.Item.Event is VStreamEvent && s.Item.Event.EventID == _targetID)
                            return false;
                    }

                return true;
            }

            foreach (var s in slots)
                if (s.Item is not null)
                    switch (_type)
                    {
                        case VSchedulingConditionType.ID:
                        {
                            if (_isStream && s.Item.Event is not VStreamEvent)
                                return false;
                            return s.Item.Event.EventID == _targetID;
                        }
                        case VSchedulingConditionType.Type:
                            return s.Item.Event.Type == _targetType;
                        case VSchedulingConditionType.SameType:
                            return s.Item.Event.Type == slot.Item.Event.Type;
                    }

            return false;
        }
    }
}