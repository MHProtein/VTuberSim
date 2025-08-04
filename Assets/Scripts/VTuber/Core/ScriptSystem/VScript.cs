using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using VTuber.Core.Foundation;
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
        public ScheduleEventType eventType;
        public uint eventID;
    }
    [Serializable]
    public class VPhase
    {
        [SerializeField] public string phaseName;
        [SerializeField] public string description;
        [SerializeField] public List<VSpecialEventData> specialEventData;

    }
    
    public class VScript : VScriptableObject
    {
        public List<VPhase> _phases;
        
        public List<VSpecialEventData> GetSpecialEvents(int weekIndex)
        {
            List<VSpecialEventData> events = new List<VSpecialEventData>();
            foreach (var phase in _phases)
            {
                if (phase.specialEventData != null)
                {
                    foreach (var eventData in phase.specialEventData)
                    {
                        if (eventData.weekIndex == weekIndex)
                        {
                            events.Add(eventData);
                        }
                    }
                }
            }
            return events;
        }
    }
}