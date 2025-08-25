using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using VTuber.BattleSystem.Core.KPIs;
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
        
        [HorizontalGroup("curves")]
        [Header("衰退线1")]
        [SerializeField] AnimationCurve decayCurve1;
        [HorizontalGroup("curves")]
        [Header("衰退线2")]
        [SerializeField] AnimationCurve decayCurve2;
        [HorizontalGroup("curves")]
        [Header("衰退线3")]
        [SerializeField] AnimationCurve decayCurve3;

        public List<VKPI> KPIs { get; private set; }

        public List<AnimationCurve> DecayCurves
        {
            get
            {
                var _decayCurves = new List<AnimationCurve>();
                _decayCurves.Add(decayCurve1);
                _decayCurves.Add(decayCurve2);
                _decayCurves.Add(decayCurve3);
                return _decayCurves;
            }
        }

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
            var e = VDataManager.Instance.CreateDialogueEventByID(startEventID);
            e.Phase = this;
            return e;
        }
        
        public void SetEndingEventID(uint id)
        {
            _endEventID = id;
        }
        
        public List<VStreamEvent> GetPhaseEndingEvents(VCharacter character)
        {
            List<VStreamEvent> events = new List<VStreamEvent>();
            foreach (var id in endEventIDs)
            {
                var e = VDataManager.Instance.CreateStreamEventByID(id);
                e.Phase = this;
                events.Add(e);
            }

            return events;
        }
    }
}