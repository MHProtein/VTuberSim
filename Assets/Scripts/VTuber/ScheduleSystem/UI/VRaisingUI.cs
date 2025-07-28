using System;
using System.Collections.Generic;
using PrimeTween;
using TMPro;
using UnityEngine;
using VTuber.Core.EventCenter;
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

        protected override void OnEnable()
        {
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnNotifyEventDescriptionChange, OnNotifyEventDescriptionChange);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnNotifyEventDescriptionChange, OnNotifyEventDescriptionChange);
        }
        
        private void OnNotifyEventDescriptionChange(Dictionary<string, object> messagedict)
        {
            eventNameUI.text = messagedict["Name"] as string;
            eventDescriptionUI.text = messagedict["Description"] as string;
        }

        public void SwitchToScheduleCreation(Action onComplete = null)
        {
            executionUI.SetActive(false);
            scheduleCreationUI.SetActive(true);
            Tween.Position(_scheduleUI, creationSchedulePosition.position, 0.3f).OnComplete(()=>
            {
                onComplete?.Invoke();
            });
        }

        public void SwitchToExecution(Action onComplete = null)
        {
            scheduleCreationUI.SetActive(false);
            executionUI.SetActive(true);
            Tween.Position(_scheduleUI, executionSchedulePosition.position, 0.3f).OnComplete(()=>
            {
                onComplete?.Invoke();
            });
        }
        
        public void SwitchToPause(Action onComplete = null)
        {
            Tween.Scale(battleUI, Vector3.one * 0.75f, 0.3f).OnComplete(() => onComplete?.Invoke());
        }

        public void SwitchToBattle()
        {
            Tween.Scale(battleUI, Vector3.one * 1.0f, 0.3f);
        }
    }
}