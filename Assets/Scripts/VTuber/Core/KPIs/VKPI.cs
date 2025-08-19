using System;
using System.Collections.Generic;
using VTuber.BattleSystem.Core.KPIs.UI;
using VTuber.Core.KPIs.UI;
using VTuber.ScheduleSystem.Core;

namespace VTuber.BattleSystem.Core.KPIs
{
    [Serializable]
    public struct VKPIInfo
    {
        public VEventType EventType;
        public int RequiredAmount;
    }
    
    public class VKPI
    {
        public uint ID { get; private set; }
        public VEventType EventType { get; private set; }
        public int RequiredAmount { get; private set; }
        
        public bool IsPermanent { get; private set; }
        
        public string EventName { get; private set; }
        

        public VKPI(VEventType eventType, int requiredAmount, bool isPermanent = false)
        {
            RequiredAmount = requiredAmount;
            EventType = eventType;
            IsPermanent = isPermanent;
            
            switch (EventType)
            {
                case VEventType.Stream:
                    EventName = "直播";
                    break;
                case VEventType.Practice:
                    EventName = "练习";
                    break;
                case VEventType.Coop:
                    EventName = "协助";
                    break;
                case VEventType.Outside:
                    EventName = "外出";
                    break;
                case VEventType.Work:
                    EventName = "工作";
                    break;
                case VEventType.Rest:
                    EventName = "休息";
                    break;
                case VEventType.Other:
                    EventName = "其他";
                    break;
            }
        }

        public void OnAdded(uint id)
        {
            ID = id;
            VKPIUIManager.Instance.AddKPIUI(this);
        }
        
        public void OnRemoved()
        {
            VKPIUIManager.Instance.RemoveKPIUI(this);
        }
        
        public bool Check(Dictionary<VEventType, int> events)
        {
            bool satisfied = false;
            if (events.TryGetValue(EventType, out var count))
                satisfied = count >= RequiredAmount;
            
            VKPIUIManager.Instance.UpdateKPIUI(ID, count, satisfied);
            
            return satisfied;
        }
    }
}