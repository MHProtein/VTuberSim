using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using Spire.Xls;
using UnityEngine;
using VTuber.Core.Foundation;
using VTuber.Core.Managers;
using VTuber.EventSystem.Events;
using VTuber.ScheduleSystem.Core;

namespace VTuber.ScheduleSystem.Events
{
    public enum VEventCostType
    {
        Stamina = 0,
        Money = 1,
    }
    
    public class VEventHeaderIndex
    {
        public const int Id = 0;
        public const int Name = 1;
        public const int Description = 2;
        public const int Type = 3;
        public const int Duration = 4;
        public const int CostType = 5;
        public const int Cost = 6;
        public const int Icon = 7;
        public const int BackGroundColor = 8;
        public const int PlacingCondition = 9;
        public const int SchedulingCondition = 10;
    }
    
    public class VScheduleEventConfiguration
    {
        public string eventName;
        public uint id;
        public int Duration => _duration;
        private int _duration = 1;
        
        public string description;

        public string icon;
        
        public Color backgroundColor = Color.white;

        public VEventType type;

        public VEventCostType costType;
        public int cost;
        public List<uint> placingConditions;
        public VSchedulingCondition schedulingCondition;
        
        public VScheduleEventConfiguration(CellRange row)
        {
            id = uint.Parse(row.Columns[VEventHeaderIndex.Id].Value);
            eventName = row.Columns[VEventHeaderIndex.Name].Value;
            description = row.Columns[VEventHeaderIndex.Description].Value;
            type = Enum.Parse<VEventType>(row.Columns[VEventHeaderIndex.Type].Value);
            _duration = int.Parse(row.Columns[VEventHeaderIndex.Duration].Value);
            costType = Enum.Parse<VEventCostType>(row.Columns[VEventHeaderIndex.CostType].Value);
            cost = int.Parse(row.Columns[VEventHeaderIndex.Cost].Value);
            icon = row.Columns[VEventHeaderIndex.Icon].Value;
            
            string placingConditionsStr = row.Columns[VEventHeaderIndex.PlacingCondition].Value;
            if (!placingConditionsStr.IsNullOrWhitespace())
                placingConditions = placingConditionsStr.Split(',').Select(x => uint.Parse(x.Trim())).ToList();
            else
            {
                placingConditions = new List<uint>();
            }
            
            string schedulingConditionStr = row.Columns[VEventHeaderIndex.SchedulingCondition].Value;
            if (!schedulingConditionStr.IsNullOrWhitespace())
                schedulingCondition = VDataManager.Instance.GetSchedulingConditionByID(uint.Parse(schedulingConditionStr));
            
            ColorUtility.TryParseHtmlString(row.Columns[VEventHeaderIndex.BackGroundColor].Value, 
                out backgroundColor);
        }
        
        public virtual VScheduleEvent CreateEvent()
        {
            return new VScheduleEvent(this);
        }

        public void SetDuration(int duration)
        {
            this._duration = duration;
        }
    }
}