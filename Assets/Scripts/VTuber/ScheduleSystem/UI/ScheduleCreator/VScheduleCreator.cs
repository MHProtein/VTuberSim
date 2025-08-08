using System.Collections.Generic;
using UnityEngine;
using VTuber.Core.Managers;
using VTuber.ScheduleSystem.Events;
using Yarn.Unity;

namespace VTuber.ScheduleSystem.UI
{
    public class VScheduleCreator : VScheduleTable
    {
        public Vector2Int slotSize;
        [SerializeField] protected GameObject itemPrefab;
        
        protected VScheduleCreatorSlot[,] slots;
        
        [SerializeField] private List<VScheduleEventConfiguration> eventDatas;
        public GameObject eventUIPrefab;
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
        
        protected override void Start()
        {
            base.Start(); 
            foreach (var eventData in eventDatas)
            {
                var slot = GetAvailableSlot();
                var eventObj = Instantiate(itemPrefab, slot.transform);
                var eventUI = eventObj.GetComponent<VEventDataUI>();
                eventUI.Initialize(eventData);
                slot.SetItem(eventUI);
            }
        }

        public void InitializeCreator(List<VScheduleEventConfiguration> configurations)
        {
            eventDatas = configurations;
        }
        
    }
}