using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using VTuber.Character;
using VTuber.Core.Managers;
using VTuber.ScheduleSystem.Core;
using VTuber.ScheduleSystem.Events;

namespace VTuber.BattleSystem.Core.ScriptSystem
{
    [Serializable]
    public class VSpecialEventData
    {
        public int weekIndex;
        public int dayIndex;
        public TimeOfDay timeOfDay;
        public VScheduleEventType eventType;
        public uint eventID;
        public VPhase phase;
        public bool isPhaseStart;
    }
    [Serializable]
    public class VPhase
    {
        [SerializeField] public string phaseName;
        [SerializeField] public string description;
        
        [Header("开始事件")]
        [LabelText("周")]
        public int startEventWeekIndex;

        
        [Header("")]
        [LabelText("时间段")]
        [SerializeField] public VScheduleEventType startEventType;
        [SerializeField] public uint startEventID;
        
        [HorizontalGroup("结束事件", Gap = 10)]
        [Header("结束事件")]
        [LabelText("周")]
        public int endEventWeekIndex;
        
        [LabelText("结束事件ID")][SerializeField] List<uint> endEventIDs = new List<uint>();
        
        [LabelText("特殊事件")]
        [SerializeField] private List<VSpecialEventData> specialEventData;

        private uint _endEventID;
        
        public List<VSpecialEventData> GetSpecialEventData()
        {
            var list = new List<VSpecialEventData>();

            list.AddRange(specialEventData);
            
            var endingEvent = new VSpecialEventData
            {
                weekIndex = startEventWeekIndex,
                dayIndex = 6,
                timeOfDay = TimeOfDay.Morning,
                eventType = VScheduleEventType.Stream,
                eventID = _endEventID,
                phase = this,
            };
            list.Add(endingEvent);
            
            return list;
        }

        public bool IsInPhase(int weekIndex)
        {
            if (weekIndex >= startEventWeekIndex && weekIndex <= endEventWeekIndex)
            {
                return true;
            }

            return false;
        }

        public VScheduleEvent GetStartEvent()
        {
            return VResourcesManager.Instance.CreateDialogueEventByID(startEventID);
        }
        
        public void SetEndingEventID(uint id)
        {
            _endEventID = id;
        }
        
        public List<KeyValuePair<VStreamEvent, List<bool>>> GetPhaseEndingEvents(VCharacter character)
        {
            List<KeyValuePair<VStreamEvent, List<bool>>> events = new List<KeyValuePair<VStreamEvent, List<bool>>>();
            foreach (var id in endEventIDs)
            {
                var e = VResourcesManager.Instance.CreateStreamEventByID(id);
                events.Add(new KeyValuePair<VStreamEvent, List<bool>>(e, e.CanExecuteAsPhaseEnding(character)));
            }

            return events;
        }
    }
}