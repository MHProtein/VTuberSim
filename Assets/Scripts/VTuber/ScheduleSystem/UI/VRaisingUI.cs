using System;
using System.Collections.Generic;
using PrimeTween;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using VTuber.BattleSystem.Card;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;
using Yarn.Unity;

namespace VTuber.ScheduleSystem.UI
{
    public class VRaisingUI : VSingletonMonobehaviour<VRaisingUI>
    {
        [SerializeField] private TMP_Text weekCountText;
        
        [Header("Schedule")] [SerializeField] private Transform _scheduleUI;
        [SerializeField] private SerializableDictionary<string, Sprite> _icons;
        [SerializeField] private GameObject eventUIPrefab;
        
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
        [SerializeField] private TMP_Text pauseText;

        [SerializeField] private Transform executionSchedulePosition;

        
        [Space(3)] 
        [Header("PauseUI")] 
        [SerializeField] private GameObject pauseUI;

        [SerializeField] private Transform pauseSchedulePosition;
        
        [Space(3)] 
        [Header("CardLibraryUI")] 
        [SerializeField] private GameObject cardLibraryUIObject;

        [SerializeField] private VCardLibraryUI cardLibraryUI;

        protected override void OnEnable()
        {
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnNotifyEventDescriptionChange, OnNotifyEventDescriptionChange);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnNotifyEventDescriptionChange, OnNotifyEventDescriptionChange);
        }

        public void InitializeCardLibraryUI(List<VCard> cards)
        {
            cardLibraryUIObject.SetActive(true);
            cardLibraryUI.Initialize(cards, false, null);
        }
        
        public void CloseCardLibraryUI()
        {
            cardLibraryUI.Close();
            cardLibraryUIObject.SetActive(false);
        }
        
        public void SetPauseText(bool shouldPause)
        {
            pauseText.text = shouldPause ? "Pause After This" : "Pause Schedule";
        }
        
        private void OnNotifyEventDescriptionChange(Dictionary<string, object> messagedict)
        {
            eventNameUI.text = messagedict["Name"] as string;
            eventDescriptionUI.text = messagedict["Description"] as string;
        }

        public void SetExecutionUIActive(bool active)
        {
            executionUI.SetActive(active);
        }
        
        public void SetCreationUIActive(bool active)
        {
            scheduleCreationUI.SetActive(active);
        }
        
        public void SetPauseUIActive(bool active)
        {
            pauseUI.SetActive(active);
        }
        
        public Tween SetScheduleUIPositionToCreation()
        {
            return Tween.Position(_scheduleUI, creationSchedulePosition.position, 0.3f);
        }
        
        public Tween SetScheduleUIPositionToExecution()
        {
            return Tween.Position(_scheduleUI, executionSchedulePosition.position, 0.3f);
        }
        
        public Tween SetScheduleUIPositionToPause()
        {
            return Tween.Position(_scheduleUI, pauseSchedulePosition.position, 0.3f);
        }
        
        public Tween UpdateWeekCount(int weekCount)
        {
            weekCountText.text = $"周数：{weekCount}";
            return Tween.PunchScale(weekCountText.transform, Vector3.one * 1.3f, 0.3f);
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
            executionUI.SetActive(true);
            Tween.Position(_scheduleUI, executionSchedulePosition.position, 0.3f).OnComplete(()=>
            {
                onComplete?.Invoke();
            });
        }
        
        public VEventUI CreateEventUI(Transform parent)
        {
            var eventUI = Instantiate(eventUIPrefab, parent);
            var eventUIComponent = eventUI.GetComponent<VEventUI>();
            return eventUIComponent;
        }

        public Sprite GetIcon(string iconName)
        {
            if (_icons.TryGetValue(iconName, out var icon))
            {
                return icon;
            }

            Debug.LogWarning($"Icon with name {iconName} not found.");
            return null;
        }
    }
}