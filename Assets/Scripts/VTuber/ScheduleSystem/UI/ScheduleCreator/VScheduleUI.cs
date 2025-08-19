
using System;
using System.Collections.Generic;
using PrimeTween;
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

namespace VTuber.ScheduleSystem.UI
{
    
    public class VScheduleUI : VUIBehaviour
    {
        public Vector2Int slotSize;
        [SerializeField] protected GameObject itemPrefab;
        [SerializeField] protected Transform indicator;
        [SerializeField] protected Image indicatorLeft;
        [SerializeField] protected Image indicatorRight;
        [SerializeField] protected Button continueButton; 
        private Vector2Int _currentIndicatorCoord = Vector2Int.zero;
        private VCharacter _character;
        
        public VScheduleSlot[,] Slots => slots;
        protected VScheduleSlot[,] slots;

        protected VAnimationQueue animationQueue;
        
        private Dictionary<VEventType, int> eventCount = new Dictionary<VEventType, int>();
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
                    slots[y, x].Initialize(new Vector2Int(x, y), this);
                }
            }
            eventCount = new Dictionary<VEventType, int>();
            foreach (VEventType eventType in Enum.GetValues(typeof(VEventType)))
            {
                eventCount.Add(eventType, 0);
            }
            _streamCount = new List<int>()
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
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnPhaseEndingSelected, OnPhaseEndingSelected);
        }



        protected override void OnDisable()
        {
            base.OnDisable();
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnEventBeginExecute, OnEventExecuted);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnEventUISelected, OnEventUISelected);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnEventUIPlaced, OnEventUIPlaced);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnPhaseEndingSelected, OnPhaseEndingSelected);
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
                slot.SetPlaceable(false, false);
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
                slot.SetPlaceable(isPlaceable, isPlaceable);
            }
        }

        public void SwitchToCreation(VCharacter character, VScript script, int weekIndex)
        {
            eventCount = new Dictionary<VEventType, int>();
            foreach (VEventType eventType in Enum.GetValues(typeof(VEventType)))
            {
                eventCount.Add(eventType, 0);
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
                slot.SetPlaceable(true, false);
            }
            DestroyAllItems();
            var specialEvents = script.GetSpecialEvents(weekIndex);
            foreach (var specialEvent in specialEvents)
            {
                VScheduleEvent e;
                if (specialEvent.eventType == VEventType.Stream)
                {
                    e = VResourcesManager.Instance.CreateStreamEventByID(specialEvent.eventID);
                }
                else
                {
                    e = VResourcesManager.Instance.CreateDialogueEventByID(specialEvent.eventID);
                }
                e.Phase = specialEvent.phase;
                e.IsPhaseStart = specialEvent.isPhaseStart;
                e.IsSpecialEvent = true;
                var ui = VRaisingUI.Instance.CreateEventUI(VScheduleUIHelper.Instance.CanvasRect);
                ui.Initialize(e, slots[(int)specialEvent.timeOfDay, specialEvent.DayIndex]);
                ui.SetFixed(true);
            }
            foreach (var slot in slots)
            {
                slot.SetPlaceable(false, false);
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

        public void CompleteSchedule(uint size1Id, uint size2Id, uint size3Id)
        {
            foreach (var slot in slots)
            {
                slot.SetPlaceable(true, false);
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
                            var e = VResourcesManager.Instance.CreateDialogueEventByID(eventId);
                            e.IsSpecialEvent = true;
                            eventUIObject.Initialize(e, slots[yy, x]);
                            
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
                    var e = VResourcesManager.Instance.CreateDialogueEventByID(eventId);
                    e.IsSpecialEvent = true;
                    eventUIObject.Initialize(e, slots[yy, x]);
                }
            }
            foreach (var slot in slots)
            {
                slot.SetPlaceable(false, false);
            }
        }
        public void RecordEvent(VScheduleEvent e)
        {
            eventCount[e.Type]++;
            if (e is VStreamEvent streamEvent)
            {
                _streamCount[streamEvent.MainAttributeIndex]++;
            }
            continueButton.interactable = _kpiManager.CheckKPIs(eventCount, _streamCount);
        }
        public void UnrecordEvent(VScheduleEvent e)
        {
            eventCount[e.Type]--;
            if (e is VStreamEvent streamEvent)
            {
                _streamCount[streamEvent.MainAttributeIndex]--;
            }
            continueButton.interactable = _kpiManager.CheckKPIs(eventCount, _streamCount);
        }
    }
}


