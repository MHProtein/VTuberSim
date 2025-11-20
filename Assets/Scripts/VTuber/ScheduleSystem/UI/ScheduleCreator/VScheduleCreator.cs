using System.Collections.Generic;
using System.Linq;
using Tutorial.Script;
using UnityEngine;
using VTuber.Character;
using VTuber.Core.Managers;
using VTuber.Core.ScriptSystem;
using VTuber.ScheduleSystem.Events;

namespace VTuber.ScheduleSystem.UI
{
    public class VScheduleCreator : VScheduleTable
    {
        public Vector2Int slotSize;
        [SerializeField] protected GameObject itemPrefab;
        private readonly List<GameObject> _eventObjects = new();

        private List<VScheduleEventConfiguration> _eventDatas;
        private bool _isFirstTime = true;

        protected VScheduleCreatorSlot[,] slots;

        protected override void Awake()
        {
            slots = new VScheduleCreatorSlot[slotSize.y, slotSize.x];
            var slotList = GetComponentsInChildren<VScheduleCreatorSlot>();

            var i = 0;
            for (var y = 0; y < slotSize.y; y++)
            for (var x = 0; x < slotSize.x; x++)
                slots[y, x] = slotList[i++];
        }

        protected override void Start()
        {
            base.Start();
            _isFirstTime = false;
            CreateEventObjects();
        }

        private void Clear()
        {
            for (var y = 0; y < slotSize.y; y++)
            for (var x = 0; x < slotSize.x; x++)
                slots[y, x].RemoveItem();

            foreach (var eventObject in _eventObjects) Destroy(eventObject);
            _eventObjects.Clear();
        }

        public void InitializeTutorialCreator(VTutorialScript script, VCharacter character)
        {
            script.AddOnWeekAdvancedCallback(weekIndex =>
            {
                _eventDatas.Clear();
                var eventConfigs =
                    script.CurrentWeekDialogEventList.Select(e =>
                            (VScheduleEventConfiguration)VDataManager.Instance.GetDialogueEventConfigurationByID(e))
                        .ToList();
                eventConfigs.AddRange(script.CurrentWeekStreamEventList
                    .Select(e => VDataManager.Instance.GetStreamEventConfigurationByID(e)).ToList());
                _eventDatas = eventConfigs;
                
                if (character.IsCharacterEventStream)
                {
                    _eventDatas.Add(VDataManager.Instance.GetStreamEventConfigurationByID(character.CharacterEventID));
                }
                else
                {
                    _eventDatas.Add(VDataManager.Instance.GetDialogueEventConfigurationByID(character.CharacterEventID));
                }

                CreateEventObjects();
            });

            var eventConfigs =
                script.CurrentWeekDialogEventList.Select(e =>
                    (VScheduleEventConfiguration)VDataManager.Instance.GetDialogueEventConfigurationByID(e)).ToList();
            eventConfigs.AddRange(script.CurrentWeekStreamEventList
                .Select(e => VDataManager.Instance.GetStreamEventConfigurationByID(e)).ToList());
            _eventDatas = eventConfigs;
            if (!_isFirstTime) CreateEventObjects();
        }

        public void InitializeCreator(VScript script, VCharacter character)
        {
            var events = script.EventList.Select(e =>
                (VScheduleEventConfiguration)VDataManager.Instance.GetDialogueEventConfigurationByID(e)).ToList();
            events.AddRange(script.StreamEventList.Select(e => VDataManager.Instance.GetStreamEventConfigurationByID(e))
                .ToList());
            _eventDatas = events;
            
            if (character.IsCharacterEventStream)
            {
                _eventDatas.Add(VDataManager.Instance.GetStreamEventConfigurationByID(character.CharacterEventID));
            }
            else
            {
                _eventDatas.Add(VDataManager.Instance.GetDialogueEventConfigurationByID(character.CharacterEventID));
            }
            
            if (!_isFirstTime) CreateEventObjects();
        }

        private VScheduleCreatorSlot GetAvailableSlot()
        {
            for (var x = 0; x < slotSize.x; x++)
            for (var y = 0; y < slotSize.y; y++)
                if (slots[y, x].Item is null)
                    return slots[y, x];

            return null;
        }

        private void CreateEventObjects()
        {
            Clear();
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