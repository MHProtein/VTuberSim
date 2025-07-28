using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VTuber.Core.Foundation;

namespace VTuber.ScheduleSystem.UI
{
    public class VRaisingUI : VSingletonMonobehaviour<VRaisingUI>
    {
        [Header("Schedule")] [SerializeField] private Transform _scheduleUI;
        
        [Space(3)]
        [Header("ScheduleCreation")] 
        [SerializeField]
        private GameObject scheduleCreationUI;

        [SerializeField] private TMP_Text eventNameUI;
        [SerializeField] private TMP_Text eventDescriptionUI;
        
        [SerializeField] 
        private Transform creationSchedulePosition;

        [SerializeField] private VScheduleCreator scheduleCreatorUI;

        [Space(3)] 
        [Header("ExecutionUI")] 
        [SerializeField] private GameObject executionUI;

        [SerializeField] private Transform executionSchedulePosition;

        [Space(3)] 
        [Header("EventUI")] 
        [SerializeField]
        private Transform battleUI;

        public void SwitchToScheduleCreation()
        {
            executionUI.SetActive(false);
            scheduleCreationUI.SetActive(true);
            Tween.Position(_scheduleUI, creationSchedulePosition.position, 0.3f);
        }

        public void SwitchToExecution()
        {
            scheduleCreationUI.SetActive(false);
            executionUI.SetActive(true);
            Tween.Position(_scheduleUI, executionSchedulePosition.position, 0.3f);
        }
        
        public void SwitchToPause()
        {
            Tween.Scale(battleUI, Vector3.one * 0.75f, 0.3f);
        }

        public void SwitchToBattle()
        {
            Tween.Scale(battleUI, Vector3.one * 1.0f, 0.3f);
        }
    }
}