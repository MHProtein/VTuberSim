using UnityEngine;
using VTuber.ScheduleSystem.Core;

namespace VTuber.ScheduleSystem.Phase
{
    [CreateAssetMenu(fileName = "PhaseConfiguration", menuName = "VTuber/Schedule/PhaseConfiguration")]
    public class PhaseConfiguration : ScriptableObject
    {
        public string phaseName;

        public TimeOfDay timeOfDay;

        public bool autoExecute = false;

        [TextArea]
        public string description;
    }
}