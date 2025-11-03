using System;
using System.Collections.Generic;
using System.Linq;
using PrimeTween;
using SlayTheSpire.System.SavingSystem;
using UnityEngine;
using UnityEngine.UI;
using VTuber.BattleSystem.Core.KPIs;
using VTuber.BattleSystem.UI;
using VTuber.Character;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;
using VTuber.Core.Managers;
using VTuber.Core.ScriptSystem;
using VTuber.EventSystem.Events;
using VTuber.ScheduleSystem.Core;
using VTuber.ScheduleSystem.Events;
using VTuber.ScheduleSystem.Schedule;

namespace VTuber.ScheduleSystem.UI
{
    public class VScheduleUISaveData
    {
        public Vector2Int currentIndicatorCoord;
        public VKPIManagerSaveData kpiManagerSaveData;
        public VScheduleSlotSaveData[,] slots;
    }

    public class VScheduleUI : VUIBehaviour
    {
        public Vector2Int slotSize;
        [SerializeField] protected GameObject eventUIPrefab;
        [SerializeField] protected Transform indicator;
        [SerializeField] protected Image indicatorLeft;
        [SerializeField] protected Image indicatorRight;
        [SerializeField] protected Button continueButton;

        private readonly List<VScheduleEvent> _events = new();
        private Vector2Int _currentIndicatorCoord = Vector2Int.zero;
        private Dictionary<VEventType, int> _eventCount = new();
        private VKPIManager _kpiManager;
        private bool _loadingEvents;
        private VScript _script;
        private List<int> _streamCount = new();

        protected VAnimationQueue animationQueue;
        protected VScheduleSlot[,] slots;

        public VCharacter Character { get; private set; }

        public VScheduleSlot[,] Slots => slots;

        protected override void Awake()
        {
            _kpiManager = new VKPIManager();
            PrimeTweenConfig.warnEndValueEqualsCurrent = false;
            slots = new VScheduleSlot[slotSize.y, slotSize.x];
            var slotList = GetComponentsInChildren<VScheduleSlot>();
            animationQueue = new VAnimationQueue();
            var i = 0;
            for (var y = 0; y < slotSize.y; y++)
            for (var x = 0; x < slotSize.x; x++)
            {
                slots[y, x] = slotList[i++];
                slots[y, x].Initialize(new Vector2Int(x, y), this, false);
            }

            _eventCount = new Dictionary<VEventType, int>();
            foreach (VEventType eventType in Enum.GetValues(typeof(VEventType))) _eventCount.Add(eventType, 0);
            _streamCount = new List<int>
            {
                0, 0, 0
            };
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnEventBeginExecute, OnEventExecuted);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnEventUISelected, OnEventUISelected);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnEventUIPlaced, OnEventUIPlaced);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnPhaseEndingSelected,
                OnPhaseEndingSelected);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnEndRun, OnEndRun);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnEventBeginExecute, OnEventExecuted);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnEventUISelected, OnEventUISelected);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnEventUIPlaced, OnEventUIPlaced);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnPhaseEndingSelected,
                OnPhaseEndingSelected);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnEndRun, OnEndRun);
        }

        public void Load(SaveData data)
        {
            _currentIndicatorCoord = data.scheduleUISaveData.currentIndicatorCoord;
            _kpiManager.Load(data.scheduleUISaveData.kpiManagerSaveData);

            for (var y = 0; y < slotSize.y; y++)
            for (var x = 0; x < slotSize.x; x++)
                slots[y, x].Load(data.scheduleUISaveData.slots[y, x]);
        }

        public void Save(SaveData data)
        {
            data.scheduleUISaveData = new VScheduleUISaveData
            {
                currentIndicatorCoord = _currentIndicatorCoord,
                kpiManagerSaveData = _kpiManager.Save()
            };
            data.scheduleUISaveData.slots = new VScheduleSlotSaveData[slotSize.y, slotSize.x];
            for (var y = 0; y < slotSize.y; y++)
            for (var x = 0; x < slotSize.x; x++)
                data.scheduleUISaveData.slots[y, x] = slots[y, x].Save();
        }

        public void LoadEvents(VWeeklySchedule schedule)
        {
            _loadingEvents = true;
            foreach (var daySchedule in schedule.GetAllDays())
            foreach (var evt in daySchedule.GetAllEvents())
            {
                var eventUI = Instantiate(eventUIPrefab, VScheduleUIHelper.Instance.EventParent)
                    .GetComponent<VEventUI>();
                eventUI.Initialize(evt, slots[evt.Coordinate.y, evt.Coordinate.x], true);
                if (eventUI.Event.IsSpecialEvent || eventUI.Event.IsPhaseEndingEvent) eventUI.SetFixed(true);
            }

            foreach (var slot in slots) slot.SetPlaceable(false, false, -1);
            _loadingEvents = false;
        }

        private void OnEndRun(Dictionary<string, object> messagedict)
        {
            _kpiManager.ClearKPIs();
        }

        private void OnPhaseEndingSelected(Dictionary<string, object> messagedict)
        {
            _kpiManager.ClearPhaseKPIs();
            var kpis = messagedict["KPIs"] as List<VKPI>;
            _kpiManager.AddPhaseKPI(kpis);
        }

        private void OnEventUIPlaced(Dictionary<string, object> messagedict)
        {
            if (_loadingEvents)
                return;
            foreach (var slot in slots) slot.SetPlaceable(false, false, -1);
        }

        private void OnEventUISelected(Dictionary<string, object> messagedict)
        {
            var e = messagedict["Event"] as VScheduleEvent;
            var attributeConditions = e.PlacingConditions.FindAll(c => c is VAttributePlacingCondition);
            foreach (var condition in attributeConditions)
                if (!condition.IsTrue(Character, null))
                    return;

            foreach (var slot in slots)
            {
                var isPlaceable = true;
                foreach (var condition in e.PlacingConditions)
                    if (condition is not VAttributePlacingCondition)
                        if (!condition.IsTrue(Character, slot))
                        {
                            isPlaceable = false;
                            break;
                        }

                slot.SetPlaceable(isPlaceable, isPlaceable, -1);
            }
        }

        public void SwitchToCreation(VCharacter character, VScript script, int weekIndex)
        {
            _events.Clear();
            _eventCount = new Dictionary<VEventType, int>();
            foreach (VEventType eventType in Enum.GetValues(typeof(VEventType))) _eventCount.Add(eventType, 0);
            _streamCount = new List<int>
            {
                0, 0, 0
            };

            continueButton.interactable = !_kpiManager.HasKPIs();
            _kpiManager.ResetKPIUIs();
            foreach (var slot in slots)
            {
                slot.RemoveCoopEvent();
                slot.SetPlaceable(true, false, -1);
            }

            DestroyAllItems();
            var specialEvents = script.GetSpecialEvents(weekIndex);
            foreach (var specialEvent in specialEvents)
            {
                VScheduleEvent e;
                if (specialEvent.eventType == VEventType.Stream)
                    e = VDataManager.Instance.CreateStreamEventByID(specialEvent.eventID);
                else if (specialEvent.eventType == VEventType.TutorialStream)
                    e = new VStreamEvent(specialEvent.tutorialStreamConfig);
                else
                    e = VDataManager.Instance.CreateDialogueEventByID(specialEvent.eventID);
                e.Phase = specialEvent.phase;
                e.IsPhaseStart = specialEvent.isPhaseStart;
                e.IsSpecialEvent = true;
                var ui = VRaisingUI.Instance.CreateEventUI(VScheduleUIHelper.Instance.CanvasRect);
                ui.Initialize(e, slots[(int)specialEvent.timeOfDay, specialEvent.DayIndex], false);
                ui.SetFixed(true);
            }

            foreach (var slot in slots) slot.SetPlaceable(false, false, -1);

            var occupiedPositions = new List<Vector2Int>();
            foreach (var slot in slots)
                if (slot.Item != null)
                    occupiedPositions.Add(slot.Coordination);

            var coopEvents = character.CooperatorManager.GetCoopEvents(occupiedPositions);
            foreach (var coopEvent in coopEvents)
                slots[coopEvent.position.y, coopEvent.position.x].SetCoopEvent(coopEvent);
        }

        public void SwitchToModify()
        {
            for (var y = 0; y < slotSize.y; y++)
            for (var x = 0; x < slotSize.x; x++)
                if (slots[y, x].Item != null)
                {
                    if (slots[y, x].Item.Event.IsExecuted)
                    {
                        slots[y, x].Item.SetInteractive(false);
                        slots[y, x].Item.SetColorGrey();
                    }
                    else
                    {
                        slots[y, x].Item.SetInteractive(true);
                    }
                }
        }

        public void SwitchToExecution()
        {
            for (var y = 0; y < slotSize.y; y++)
            for (var x = 0; x < slotSize.x; x++)
                if (slots[y, x].Item != null)
                {
                    slots[y, x].Item.SetInteractive(false);
                    slots[y, x].Item.SetColorOriginal();
                }

            ChangeIndicatorColor(Color.yellow);
            MoveIndicator(_currentIndicatorCoord);
        }

        public void Initialize(VCharacter character, VScript script)
        {
            Character = character;
            _script = script;
            _kpiManager = new VKPIManager();
            _kpiManager.AddPermanentKPI(script.kpis);
        }

        private void OnEventExecuted(Dictionary<string, object> messagedict)
        {
            var coordinate = (Vector2Int)messagedict["Coordinate"];
            if (coordinate.x == -1)
                return;
            ChangeIndicatorPosition(slots[coordinate.y, coordinate.x].Item.transform.position);
            ChangeIndicatorScale(slots[coordinate.y, coordinate.x].Item.Event.Duration);
        }

        public Tween MoveIndicator(Vector2Int coordinate)
        {
            if (coordinate.x == -1)
                return ChangeIndicatorPosition(slots[_currentIndicatorCoord.y, _currentIndicatorCoord.x].Item.transform
                    .position);
            ChangeIndicatorPosition(slots[coordinate.y, coordinate.x].Item.transform.position);
            _currentIndicatorCoord = coordinate;
            return ChangeIndicatorScale(slots[coordinate.y, coordinate.x].Item.Event.Duration);
        }

        public void ResetSchedule()
        {
            for (var x = 0; x < slotSize.x; x++)
            for (var y = 0; y < slotSize.y; y++)
            {
                if (slots[y, x].Item is null)
                    continue;
                if (!slots[y, x].Item.Event.IsExecuted && !slots[y, x].Item.IsFixed)
                    slots[y, x].DespawnItem();
            }
        }

        public void DestroyAllItems()
        {
            for (var x = 0; x < slotSize.x; x++)
            for (var y = 0; y < slotSize.y; y++)
                slots[y, x].DestroyItem();
        }

        public Tween ChangeIndicatorPosition(Vector2 position)
        {
            return Tween.Position(indicator, position, 0.2f);
        }

        public Tween ChangeIndicatorScale(float scale)
        {
            return Tween.ScaleY(indicator, scale, 0.2f);
        }

        public void ChangeIndicatorColor(Color color)
        {
            indicatorLeft.color = color;
            indicatorRight.color = color;
        }

        public Tween ResetIndicatorPosition()
        {
            return Tween.Position(indicator, slots[0, 0].Item.transform.position, 0.2f);
        }

        public List<VScheduleSlot> GetUDSlots(VScheduleSlot slot)
        {
            var down = slot.Coordination.y - 1;
            var up = slot.Coordination.y + 1;
            var ret = new List<VScheduleSlot>();
            if (down >= 0)
                ret.Add(slots[down, slot.Coordination.x]);
            if (up < slotSize.y)
                ret.Add(slots[up, slot.Coordination.x]);
            return ret;
        }

        public List<VScheduleSlot> GetLRSlots(VScheduleSlot slot)
        {
            var left = slot.Coordination.x - 1;
            var right = slot.Coordination.x + 1;
            var ret = new List<VScheduleSlot>();
            if (left >= 0)
                ret.Add(slots[slot.Coordination.y, left]);
            if (right < slotSize.x)
                ret.Add(slots[slot.Coordination.y, right]);
            return ret;
        }

        public List<VScheduleSlot> GetUDLRSlots(VScheduleSlot slot)
        {
            return GetUDSlots(slot).Concat(GetLRSlots(slot)).ToList();
        }

        public List<VScheduleSlot> GetSurroundingSlots(VScheduleSlot slot)
        {
            var down = slot.Coordination.y - 1;
            var up = slot.Coordination.y + 1;
            var left = slot.Coordination.x - 1;
            var right = slot.Coordination.x + 1;
            var ret = new List<VScheduleSlot>();

            for (var i = down; i <= up; i++)
            {
                if (i < 0 || i >= slotSize.y)
                    continue;
                for (var j = left; j <= right; j++)
                {
                    if (j < 0 || j >= slotSize.x)
                        continue;
                    if (slots[i, j] != slot) ret.Add(slots[i, j]);
                }
            }

            return ret;
        }

        public void CompleteSchedule(uint size1Id, uint size2Id, uint size3Id)
        {
            foreach (var slot in slots) slot.SetPlaceable(true, false, -1);
            for (var x = 0; x < slotSize.x; x++)
            {
                var emptyCount = 0;
                for (var y = 0; y < slotSize.y; y++)
                    if (slots[y, x].Item == null)
                    {
                        emptyCount++;
                    }
                    else
                    {
                        if (emptyCount > 0)
                        {
                            var yy = y - emptyCount;
                            var eventUIObject =
                                VRaisingUI.Instance.CreateEventUI(VScheduleUIHelper.Instance.CanvasRect);
                            uint eventId = 0;
                            if (emptyCount == 1)
                                eventId = size1Id;
                            else if (emptyCount == 2)
                                eventId = size2Id;
                            var e = VDataManager.Instance.CreateDialogueEventByID(eventId);
                            e.IsSpecialEvent = true;
                            eventUIObject.Initialize(e, slots[yy, x], true);
                            slots[yy, x].Item.SetInteractive(false);
                            e.IsSpecialEvent = false;
                            emptyCount = 0;
                        }
                    }

                if (emptyCount > 0)
                {
                    var yy = 3 - emptyCount;
                    var eventUIObject = VRaisingUI.Instance.CreateEventUI(VScheduleUIHelper.Instance.CanvasRect);
                    uint eventId = 0;
                    if (emptyCount == 1)
                        eventId = size1Id;
                    else if (emptyCount == 2)
                        eventId = size2Id;
                    else if (emptyCount == 3)
                        eventId = size3Id;
                    var e = VDataManager.Instance.CreateDialogueEventByID(eventId);
                    e.IsSpecialEvent = true;
                    eventUIObject.Initialize(e, slots[yy, x], true);
                    slots[yy, x].Item.SetInteractive(false);
                    e.IsSpecialEvent = false;
                }
            }

            foreach (var slot in slots) slot.SetPlaceable(false, false, -1);
        }

        public void RecordEvent(VScheduleEvent e)
        {
            if (_events.Contains(e))
                return;
            _events.Add(e);
            e.Phase = _script.CurrentPhase;
            _eventCount[e.Type]++;
            if (e is VStreamEvent streamEvent) _streamCount[streamEvent.MainAttributeIndex]++;
            continueButton.interactable = _kpiManager.CheckKPIs(_eventCount, _streamCount);
        }

        public void UnrecordEvent(VScheduleEvent e)
        {
            if (!_events.Contains(e))
                return;
            _events.Remove(e);
            _eventCount[e.Type]--;
            if (e is VStreamEvent streamEvent) _streamCount[streamEvent.MainAttributeIndex]--;
            continueButton.interactable = _kpiManager.CheckKPIs(_eventCount, _streamCount);
        }

        public void Clear()
        {
            _events.Clear();
            _eventCount = new Dictionary<VEventType, int>();
            foreach (VEventType eventType in Enum.GetValues(typeof(VEventType))) _eventCount.Add(eventType, 0);
            _streamCount = new List<int>
            {
                0, 0, 0
            };
            continueButton.interactable = !_kpiManager.HasKPIs();
            _kpiManager.ResetKPIUIs();
            foreach (var slot in slots)
            {
                slot.RemoveCoopEvent();
                slot.SetPlaceable(true, false, -1);
            }

            DestroyAllItems();
        }
    }
}