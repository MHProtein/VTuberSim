using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using VTuber.Core.Foundation;
using VTuber.Core.Managers;
using VTuber.ScheduleSystem.Core;
using VTuber.ScheduleSystem.Events;

namespace VTuber.BattleSystem.Core.ScriptSystem
{
    public class VScriptConfiguration : VScriptableObject
    {
        [SerializeField] public List<VPhase> phases;
        

    }
}
