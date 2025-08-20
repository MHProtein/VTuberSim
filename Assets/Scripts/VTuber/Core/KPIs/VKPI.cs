using System;
using System.Collections.Generic;
using System.IO;
using VTuber.BattleSystem.Core.KPIs.UI;
using VTuber.Core.KPIs.UI;
using VTuber.ScheduleSystem.Core;

namespace VTuber.BattleSystem.Core.KPIs
{
    [Serializable]
    public class VKPIInfo
    {
        public VEventType EventType;
        public int RequiredAmount;
        public int AbilityIndex;
        
        public VKPIInfo()
        {
            AbilityIndex = -1;
        }
    }
    
    public class VKPI
    {
        public uint ID { get; private set; }
        public VEventType EventType { get; private set; }
        public int RequiredAmount { get; private set; }
        
        public bool IsPermanent { get; private set; }
        
        public string EventName { get; private set; }
        public int AbilityIndex { get; private set; }
        public string AbilityName { get; private set; }
        

        public VKPI(VEventType eventType, int requiredAmount, int abilityIndex, bool isPermanent)
        {
            RequiredAmount = requiredAmount;
            EventType = eventType;
            IsPermanent = isPermanent;

            if (eventType == VEventType.Stream)
            {
                if (abilityIndex == 0) AbilityName = "歌力";
                if (abilityIndex == 1) AbilityName = "游戏力";
                if (abilityIndex == 2) AbilityName = "杂谈力";
            }
            
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
        
        public bool Check(Dictionary<VEventType, int> events, List<int> streamEvents)
        {
            bool satisfied = false;
            var count = 0;
            if (EventType == VEventType.Stream && AbilityIndex != -1)
            {
                count = streamEvents[AbilityIndex];
                satisfied = streamEvents[AbilityIndex] >= RequiredAmount;
            }
            else
            {
                if (events.TryGetValue(EventType, out count))
                    satisfied = count >= RequiredAmount;
            }
            
            VKPIUIManager.Instance.UpdateKPIUI(ID, count, satisfied);
            
            return satisfied;
        }
    }
}