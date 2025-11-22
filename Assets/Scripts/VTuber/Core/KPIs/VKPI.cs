using System;
using System.Collections.Generic;
using VTuber.Core.KPIs.UI;
using VTuber.Core.UI;
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
        public VEventType EventType { get; }
        public int RequiredAmount { get; }

        public bool IsPermanent { get; private set; }

        public string EventName { get; private set; }
        public int AbilityIndex { get; }
        public string AbilityName { get; private set; }

        public VKPI(VEventType eventType, int requiredAmount, int abilityIndex, bool isPermanent)
        {
            RequiredAmount = requiredAmount;
            EventType = eventType;
            IsPermanent = isPermanent;
            AbilityIndex = abilityIndex;

            if (eventType == VEventType.Stream)
            {
                if (abilityIndex == 0) AbilityName = "歌回";
                if (abilityIndex == 1) AbilityName = "游戏";
                if (abilityIndex == 2) AbilityName = "杂谈";
            }

            EventName = VUIUtils.Instance.GetEventName(eventType);
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
            var satisfied = false;
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