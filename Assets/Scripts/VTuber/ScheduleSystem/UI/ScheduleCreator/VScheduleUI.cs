
using System;
using System.Collections.Generic;
using System.Linq;
using PrimeTween;
using SlayTheSpire.System.SavingSystem;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using VTuber.BattleSystem.Core.KPIs;
using VTuber.BattleSystem.Core.ScriptSystem;
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
    }
    
    public class VScheduleUI : VUIBehaviour, IDataPersistence
    {
        public Vector2Int slotSize;
        [SerializeField] protected GameObject itemPrefab;
        [SerializeField] protected Transform indicator;
        [SerializeField] protected Image indicatorLeft;
        [SerializeField] protected Image indicatorRight;
        [SerializeField] protected Button continueButton; 
        private Vector2Int _currentIndicatorCoord = Vector2Int.zero;
        
        public VCharacter Character => _character;
        private VCharacter _character;
        
        public VScheduleSlot[,] Slots => slots;
        protected VScheduleSlot[,] slots;

        protected VAnimationQueue animationQueue;

        private List<VScheduleEvent> _events = new List<VScheduleEvent>();
        private Dictionary<VEventType, int> _eventCount = new Dictionary<VEventType, int>();
        private List<int> _streamCount = new();
        private VKPIManager _kpiManager;
        private VScript _script;
        
        protected override void Awake()
        {
            _kpiManager = new VKPIManager();
            PrimeTweenConfig.warnEndValueEqualsCurrent = false;
            slots = new VScheduleSlot[slotSize.y, slotSize.x];
            var slotList = GetComponentsInChildren<VScheduleSlot>();
            animationQueue = new VAnimationQueue();
            int i = 0; 
            for (int y = 0; y < slotSize.y; y++)
            {
                for (int x = 0; x < slotSize.x; x++)
                {    
                    slots[y, x] = slotList[i++];
                    slots[y, x].Initialize(new Vector2Int(x, y), this, false);
                }
            }
            _eventCount = new Dictionary<VEventType, int>();
            foreach (VEventType eventType in Enum.GetValues(typeof(VEventType)))
            {
                _eventCount.Add(eventType, 0);
            }
            _streamCount = new List<int>()
            {
                0, 0, 0
            };
        }
        
        public void Load(GameData data)
        {
            _currentIndicatorCoord = data.scheduleUISaveData.currentIndicatorCoord;
            _kpiManager.Load(data.scheduleUISaveData.kpiManagerSaveData);
        }

        public void Save(GameData data)
        {
            data.scheduleUISaveData = new VScheduleUISaveData()
            {
                currentIndicatorCoord = _currentIndicatorCoord,
                kpiManagerSaveData = _kpiManager.Save()
            };
        }

        public void LoadEvents(VWeeklySchedule schedule)
        {
            foreach (var daySchedule in schedule.GetAllDays())
            {
                foreach (var evt in daySchedule.GetAllEvents())
                {
                    //slots[evt.Coordinate.y, evt.Coordinate.x].
                }
            }
        }
        
        protected override void OnEnable()
        {
            base.OnEnable();
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnEventBeginExecute, OnEventExecuted);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnEventUISelected, OnEventUISelected);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnEventUIPlaced, OnEventUIPlaced);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnPhaseEndingSelected, OnPhaseEndingSelected);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnEndRun, OnEndRun);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnEventBeginExecute, OnEventExecuted);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnEventUISelected, OnEventUISelected);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnEventUIPlaced, OnEventUIPlaced);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnPhaseEndingSelected, OnPhaseEndingSelected);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnEndRun, OnEndRun);
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
            foreach (var slot in slots)
            {
                slot.SetPlaceable(false, false, -1);
            }
        }

        private void OnEventUISelected(Dictionary<string, object> messagedict)
        {
            var e = messagedict["Event"] as VScheduleEvent;
            List<VPlacingCondition> attributeConditions = e.PlacingConditions.FindAll(c => c is VAttributePlacingCondition);
            foreach (var condition in attributeConditions)
            {
                if (!condition.IsTrue(_character, null))
                {
                    return;
                }
            }
            foreach (var slot in slots)
            {
                bool isPlaceable = true;
                foreach (var condition in e.PlacingConditions)
                {
                    if (condition is not VAttributePlacingCondition)
                    {
                        if (!condition.IsTrue(_character, slot))
                        {
                            isPlaceable = false;
                            break;
                        }
                    }
                }
                slot.SetPlaceable(isPlaceable, isPlaceable, -1);
            }
        }

        public void SwitchToCreation(VCharacter character, VScript script, int weekIndex)
        {
            _events.Clear();
            _eventCount = new Dictionary<VEventType, int>();
            foreach (VEventType eventType in Enum.GetValues(typeof(VEventType)))
            {
                _eventCount.Add(eventType, 0);
            }     
            _streamCount = new List<int>()
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
                {
                    e = VDataManager.Instance.CreateStreamEventByID(specialEvent.eventID);
                }
                else
                {
                    e = VDataManager.Instance.CreateDialogueEventByID(specialEvent.eventID);
                }
                e.Phase = specialEvent.phase;
                e.IsPhaseStart = specialEvent.isPhaseStart;
                e.IsSpecialEvent = true;
                var ui = VRaisingUI.Instance.CreateEventUI(VScheduleUIHelper.Instance.CanvasRect);
                ui.Initialize(e, slots[(int)specialEvent.timeOfDay, specialEvent.DayIndex], true);
                ui.SetFixed(true);
            }
            foreach (var slot in slots)
            {
                slot.SetPlaceable(false, false, -1);
            }
            
            List<Vector2Int> occupiedPositions = new List<Vector2Int>();
            foreach (var slot in slots)
            {
                if (slot.Item != null)
                {
                    occupiedPositions.Add(slot.Coordination);
                }
            }
            
            var coopEvents = character.CooperatorManager.GetCoopEvents(occupiedPositions);
            foreach (var coopEvent in coopEvents)
            {
                slots[coopEvent.position.y, coopEvent.position.x].SetCoopEvent(coopEvent);
            }
        }
        
        public void SwitchToModify()
        {
            for (int y = 0; y < slotSize.y; y++)
            {
                for (int x = 0; x < slotSize.x; x++)
                {    
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
            }
        }
        
        public void SwitchToExecution()
        {
            for (int y = 0; y < slotSize.y; y++)
            {
                for (int x = 0; x < slotSize.x; x++)
                {    
                    if (slots[y, x].Item != null)
                    {
                        slots[y, x].Item.SetInteractive(false);
                        slots[y, x].Item.SetColorOriginal();
                    }
                }
            }
            ChangeIndicatorColor(Color.yellow);
            MoveIndicator(_currentIndicatorCoord);
        }

        public void Initialize(VCharacter character, VScript script)
        {
            _character = character;
            _script = script;
            _kpiManager = new VKPIManager();
            _kpiManager.AddPermanentKPI(script.kpis);
        }

        private void OnEventExecuted(Dictionary<string, object> messagedict)
        {
            Vector2Int coordinate = (Vector2Int)messagedict["Coordinate"];
            if (coordinate.x == -1)
                return;
            ChangeIndicatorPosition(slots[coordinate.y, coordinate.x].Item.transform.position);
            ChangeIndicatorScale(slots[coordinate.y, coordinate.x].Item.Event.Duration);
        }

        public Tween MoveIndicator(Vector2Int coordinate)
        {
            if (coordinate.x == -1)
                return ChangeIndicatorPosition(slots[_currentIndicatorCoord.y, _currentIndicatorCoord.x].Item.transform.position);
            ChangeIndicatorPosition(slots[coordinate.y, coordinate.x].Item.transform.position);
            _currentIndicatorCoord = coordinate;
            return ChangeIndicatorScale(slots[coordinate.y, coordinate.x].Item.Event.Duration);
        }

        public void ResetSchedule()
        {
            for (int x = 0; x < slotSize.x; x++)
            {
                for (int y = 0; y < slotSize.y; y++)
                {
                    if (slots[y, x].Item is null)
                        continue;
                    if(!slots[y, x].Item.Event.IsExecuted && !slots[y, x].Item.IsFixed)
                        slots[y, x].DespawnItem();
                }
            }
        }
        
        public void DestroyAllItems()
        {
            for (int x = 0; x < slotSize.x; x++)
            {
                for (int y = 0; y < slotSize.y; y++)
                {
                    slots[y, x].DestroyItem();
                }
            }
        }
        
        public Tween ChangeIndicatorPosition(Vector2 position)
        {
            return Tween.Position(indicator, position, 0.2f);
        }
        
        public Tween ChangeIndicatorScale(float scale)
        {
            return Tween.ScaleY(indicator,scale, 0.2f);
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
            int down = slot.Coordination.y - 1;
            int up = slot.Coordination.y + 1;
            List<VScheduleSlot> ret = new List<VScheduleSlot>();
            if (down >= 0)
                ret.Add(slots[down, slot.Coordination.x]);
            if (up < slotSize.y)
                ret.Add(slots[up, slot.Coordination.x]);
            return ret;
        }
        
        public List<VScheduleSlot> GetLRSlots(VScheduleSlot slot)
        {
            int left = slot.Coordination.x - 1;
            int right = slot.Coordination.x + 1;
            List<VScheduleSlot> ret = new List<VScheduleSlot>();
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
            int down = slot.Coordination.y - 1;
            int up = slot.Coordination.y + 1;
            int left = slot.Coordination.x - 1;
            int right = slot.Coordination.x + 1;
            List<VScheduleSlot> ret = new List<VScheduleSlot>();

            for (int i = down; i <= up; i++)
            {
                if(i < 0 || i >= slotSize.y)
                    continue;
                for (int j = left; j <= right; j++)
                {
                    if(j < 0 || j >= slotSize.x)
                        continue;
                    if (slots[i, j] != slot)
                    {
                        ret.Add(slots[i, j]);
                    }
                }
            }
            return ret;
        }
        
        public void CompleteSchedule(uint size1Id, uint size2Id, uint size3Id)
        {
            foreach (var slot in slots)
            {
                slot.SetPlaceable(true, false, -1);
            }
            for (int x = 0; x < slotSize.x; x++)
            {
                int emptyCount = 0;
                for (int y = 0; y < slotSize.y; y++)
                {
                    if(slots[y, x].Item == null)
                    {
                        emptyCount++;
                    }
                    else
                    {
                        if (emptyCount > 0)
                        {
                            var yy = y - emptyCount;
                            VEventUI eventUIObject = VRaisingUI.Instance.CreateEventUI(VScheduleUIHelper.Instance.CanvasRect);
                            uint eventId = 0; 
                            if(emptyCount == 1)
                                eventId = size1Id;
                            else if(emptyCount == 2)
                                eventId = size2Id;
                            var e = VDataManager.Instance.CreateDialogueEventByID(eventId);
                            e.IsSpecialEvent = true;
                            eventUIObject.Initialize(e, slots[yy, x], true);
                            
                            emptyCount = 0;
                        }
                    }
                }
                if (emptyCount > 0)
                {
                    var yy = 3 - emptyCount;
                    VEventUI eventUIObject = VRaisingUI.Instance.CreateEventUI(VScheduleUIHelper.Instance.CanvasRect);
                    uint eventId = 0; 
                    if(emptyCount == 1)
                        eventId = size1Id;
                    else if(emptyCount == 2)
                        eventId = size2Id;
                    else if(emptyCount == 3)
                        eventId = size3Id;
                    var e = VDataManager.Instance.CreateDialogueEventByID(eventId);
                    e.IsSpecialEvent = true;
                    eventUIObject.Initialize(e, slots[yy, x], true);
                }
            }
            foreach (var slot in slots)
            {
                slot.SetPlaceable(false, false, -1);
            }
        }
        public void RecordEvent(VScheduleEvent e)
        {
            if (_events.Contains(e))
                return;
            _events.Add(e);
            e.Phase = _script.CurrentPhase;
            _eventCount[e.Type]++;
            if (e is VStreamEvent streamEvent)
            {
                _streamCount[streamEvent.MainAttributeIndex]++;
            }
            continueButton.interactable = _kpiManager.CheckKPIs(_eventCount, _streamCount);
        }
        
        public void UnrecordEvent(VScheduleEvent e)
        {
            if (!_events.Contains(e))
                return;
            _events.Remove(e);
            _eventCount[e.Type]--;
            if (e is VStreamEvent streamEvent)
            {
                _streamCount[streamEvent.MainAttributeIndex]--;
            }
            continueButton.interactable = _kpiManager.CheckKPIs(_eventCount, _streamCount);
        }
    }
}


