using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using VTuber.Core.Foundation;
using VTuber.ScheduleSystem.Core;

namespace VTuber.BattleSystem.Core.ScriptSystem
{
    [Serializable]
    public class VSpecialEventData
    {
        public int weekIndex;
        public int dayIndex;
        public TimeOfDay timeOfDay;
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
    }
}