using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Tutorial.Script;
using UnityEngine;
using VTuber.Core.Managers;
using VTuber.Core.ScriptSystem;
using VTuber.ScheduleSystem.Events;

namespace VTuber.ScheduleSystem.UI
{
    public class VScheduleCreator : VScheduleTable
    {
        public Vector2Int slotSize;
        [SerializeField] protected GameObject itemPrefab;
        
        protected VScheduleCreatorSlot[,] slots;
        
        private List<VScheduleEventConfiguration> _eventDatas;
        private List<GameObject> _eventObjects = new List<GameObject>();
        protected override void Awake()
        {
            slots = new VScheduleCreatorSlot[slotSize.y, slotSize.x];
            var slotList = GetComponentsInChildren<VScheduleCreatorSlot>();
            
            int i = 0; 
            for (int y = 0; y < slotSize.y; y++)
            {
                for (int x = 0; x < slotSize.x; x++)
                {    
                    slots[y, x] = slotList[i++];
                }
            }
        }
        
        public void InitializeTutorialCreator(VTutorialScript script)
        {
            script.AddOnWeekAdvancedCallback((weekIndex) =>
            {
                _eventDatas.Clear();
                var eventConfigs = 
                    script.CurrentWeekEventList.Select(e => (VScheduleEventConfiguration)VDataManager.Instance.GetDialogueEventConfigurationByID(e)).ToList();
                eventConfigs.AddRange(script.StreamEventList.Select(e => VDataManager.Instance.GetStreamEventConfigurationByID(e)).ToList());
                _eventDatas = eventConfigs;

                foreach (var eventObject in _eventObjects)
                {
                    Destroy(eventObject);
                }
                _eventObjects.Clear();
                
                CreateEventObjects();
            });
            
            var eventConfigs = 
                script.CurrentWeekEventList.Select(e => (VScheduleEventConfiguration)VDataManager.Instance.GetDialogueEventConfigurationByID(e)).ToList();
            eventConfigs.AddRange(script.StreamEventList.Select(e => VDataManager.Instance.GetStreamEventConfigurationByID(e)).ToList());
            _eventDatas = eventConfigs;
            CreateEventObjects();
        }
        
        public void InitializeCreator(VScript script)
        {
            var events = script.EventList.Select(e => (VScheduleEventConfiguration)VDataManager.Instance.GetDialogueEventConfigurationByID(e)).ToList();
            events.AddRange(script.StreamEventList.Select(e => VDataManager.Instance.GetStreamEventConfigurationByID(e)).ToList());
            _eventDatas = events;
            CreateEventObjects();
        }
        
        VScheduleCreatorSlot GetAvailableSlot()
        {
            for (int x = 0; x < slotSize.x; x++)
            {
                for (int y = 0; y < slotSize.y; y++)
                {
                    if (slots[y, x].Item is null)
                        return slots[y, x];
                }
            }

            return null;
        }

        private void CreateEventObjects()
        {
            foreach (var eventData in _eventDatas)
            {
                var slot = GetAvailableSlot();
                var eventObj = Instantiate(itemPrefab, slot.transform);
                eventObj.transform.localPosition = Vector3.zero;
                var eventUI = eventObj.GetComponent<VEventDataUI>();
                eventUI.Initialize(eventData);
                slot.SetItem(eventUI);
                _eventObjects.Add(eventObj);
            }
        }
        
    }
}