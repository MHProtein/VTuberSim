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
        public int WeekIndex => weekIndex - 1;
        protected int weekIndex;
        
        public int DayIndex => dayIndex - 1;
        protected int dayIndex;
        
        public TimeOfDay timeOfDay;
        public VEventType eventType;
        public uint eventID;
        public VPhase phase;
        public bool isPhaseStart;
        
        public void SetWeekDay(int weekIndex, int dayIndex)
        {
            this.weekIndex = weekIndex;
            this.dayIndex = dayIndex;
        }
        
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
        [LabelText("开始事件类型")]
        [SerializeField] public VEventType startEventType;
        [LabelText("开始事件ID")]
        [SerializeField] public uint startEventID;
        
        [HorizontalGroup("结束事件", Gap = 10)]
        [Header("结束事件")]
        [LabelText("周")]
        public int endEventWeekIndex;
        
        [LabelText("结束事件ID")][SerializeField] List<uint> endEventIDs = new List<uint>();
        
        [LabelText("特殊事件")]
        [SerializeField] private List<VSpecialEventData> specialEventData;

        private uint _endEventID;

        public VPhase nextPhase;
        
        public List<VSpecialEventData> GetSpecialEventData(int weekIndex)
        {
            var list = new List<VSpecialEventData>();

            foreach (var e in specialEventData)
            {
                if (e.WeekIndex == weekIndex)
                {
                    list.Add(e);
                }
            }

            if(weekIndex != endEventWeekIndex - 1) return list;
            
            var endingEvent = new VSpecialEventData
            {
                timeOfDay = TimeOfDay.Morning,
                eventType = VEventType.Stream,
                eventID = _endEventID,
                phase = this,
            };
            endingEvent.SetWeekDay(endEventWeekIndex - 1, 7);
            list.Add(endingEvent);
            
            return list;
        }

        public bool IsInPhase(int weekIndex)
        {
            if (weekIndex >= startEventWeekIndex - 1 && weekIndex <= endEventWeekIndex - 1)
            {
                return true;
            }

            return false;
        }

        public VScheduleEvent GetStartEvent()
        {
            var e = VResourcesManager.Instance.CreateDialogueEventByID(startEventID);
            e.Phase = this;
            return e;
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
                e.Phase = this;
                events.Add(new KeyValuePair<VStreamEvent, List<bool>>(e, e.CanExecuteAsPhaseEnding(character)));
            }

            return events;
        }
    }
}