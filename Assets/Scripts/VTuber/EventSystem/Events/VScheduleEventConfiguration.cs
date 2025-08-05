using System;
using Spire.Xls;
using UnityEngine;
using VTuber.Core.Foundation;
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

        public VScheduleEventType type;

        public VEventCostType costType;
        public int cost;

        public VScheduleEventConfiguration(CellRange row)
        {
            id = uint.Parse(row.Columns[VEventHeaderIndex.Id].Value);
            eventName = row.Columns[VEventHeaderIndex.Name].Value;
            description = row.Columns[VEventHeaderIndex.Description].Value;
            type = Enum.Parse<VScheduleEventType>(row.Columns[VEventHeaderIndex.Type].Value);
            _duration = int.Parse(row.Columns[VEventHeaderIndex.Duration].Value);
            costType = Enum.Parse<VEventCostType>(row.Columns[VEventHeaderIndex.CostType].Value);
            cost = int.Parse(row.Columns[VEventHeaderIndex.Cost].Value);
            icon = row.Columns[VEventHeaderIndex.Icon].Value;
            
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