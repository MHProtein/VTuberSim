using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using VTuber.CoopSystem;
using VTuber.Core.Foundation;
using VTuber.Core.Managers;
using VTuber.Core.RaisingEffect;
using VTuber.Core.UI;
using VTuber.ScheduleSystem.Core;
using VTuber.ScheduleSystem.Events;




namespace VTuber.ScheduleSystem.UI
{
    
    public class VScheduleSlotSaveData
    {
        public int coopEventID;
        public string coopEventIconName;
    }
    
    public class VScheduleSlot : VUIBehaviour
    {
        public VEventUI Item => _item;
        public bool IsCoopEventSlot {get; private set;}
        /// <summary>
        /// X=Day, Y=TimeOfDay
        /// </summary>
        public Vector2Int Coordination => _coordination;
        private VScheduleUI _scheduleUI;
        private Vector2Int _coordination;
        private VEventUI _item;
        private List<VRaisingEffect> _coopEventEffects;
        private List<VCoopEvent.VCoopEventType> _coopEventTypes;
        private int _coopEventID;
        private Sprite _coopEventIcon;

        [SerializeField] private GameObject coopEventGameObject;
        [SerializeField] private GameObject highlightFrame;
        [SerializeField] private Image checkmark;
        [SerializeField] private Image redCross;
        [SerializeField] private Image pfp;
        [SerializeField] private List<Image> eventIcons;
        [SerializeField] private TMP_Text effectText;
        [SerializeField] private Image effectImage;
        
        // Add this new field at the top with your other [SerializeField] variables
        [Header("Scheduling Condition UI")]
        [Tooltip("The UI element to show when a scheduling condition is met during drag")]
        [SerializeField] private GameObject conditionHighlight;
        
        
        
        public bool Available
        {
            get
            {
                if (_placeable)
                    return _item is null;
                if (_allowedEventID == -1)
                    return false;
                return _item is null && _allowedEventID == _eventID;
            }
        }

        private bool _placeable = true;
        private int _allowedEventID;
        private int _eventID;
        private bool _useThisTransformAsParent;

        public VScheduleSlotSaveData Save()
        {
            return new VScheduleSlotSaveData()
            {
                coopEventID = _coopEventID,
                coopEventIconName = _coopEventIcon?.name ?? ""
            };
        }

        public void Load(VScheduleSlotSaveData saveData)
        {
            _coopEventID = saveData.coopEventID;
            if (saveData.coopEventID == -1)
                return;
            _coopEventIcon = VResourcesManager.Instance.TryGetSprite(saveData.coopEventIconName);
            SetCoopEvent(new VCoopEventItem()
            {
                e = VDataManager.Instance.GetCoopEventByID((uint)saveData.coopEventID),
                pfp = _coopEventIcon
            });
        }
        
        public void Initialize(Vector2Int coordination, VScheduleUI scheduleUI, bool useThisTransformAsParent)
        {
            _coordination = coordination;
            _scheduleUI = scheduleUI;
            _useThisTransformAsParent = useThisTransformAsParent;
            _coopEventID = -1;
        }
        
        public List<VScheduleSlot> GetUDSlots() => _scheduleUI.GetUDSlots(this);
        
        public List<VScheduleSlot> GetLRSlots() => _scheduleUI.GetLRSlots(this);
        
        public List<VScheduleSlot> GetUDLRSlots() => _scheduleUI.GetUDLRSlots(this);
        
        public List<VScheduleSlot> GetSurroundingSlots() => _scheduleUI.GetSurroundingSlots(this);

        public void SetPlaceable(bool isPlaceable, bool showFrame, int allowedEventID)
        {
            _placeable = isPlaceable;
            highlightFrame.SetActive(showFrame);
            _allowedEventID = allowedEventID;
        }

        public void SetCoopEvent(VCoopEventItem eventItem)
        {
            _coopEventID = (int)eventItem.e.id;
            _coopEventIcon = eventItem.pfp;
            coopEventGameObject.SetActive(true);
            IsCoopEventSlot = true;
            pfp.sprite = eventItem.pfp;
            _coopEventEffects = eventItem.e.effects;
            _coopEventTypes = eventItem.e.eventTypes;
            for (int i = 0; i < eventItem.e.eventTypes.Count; i++)
            {
                eventIcons[i].gameObject.SetActive(true);
                string x = eventItem.e.eventTypes[i].eventType.ToString();
                eventIcons[i].sprite = VResourcesManager.Instance.TryGetSprite(x);
                if (eventItem.e.eventTypes[i].eventType == VEventType.Stream &&
                    eventItem.e.eventTypes[i].abilityIndex != -1)
                {
                    eventIcons[i].color = VRaisingUI.Instance.abilityColors[eventItem.e.eventTypes[i].abilityIndex];
                }
                
                foreach (var effect in _coopEventEffects)
                {
                    if (effect is IAttributeEffect attributeEffect)
                    {
                        effectText.text = "+" + effect.GetParameter();
                        effectImage.GetComponentInChildren<Image>().sprite =
                            VUIUtils.Instance.GetAttributeIcon(attributeEffect.AttributeName);
                    }
                }
            }
        }
        
        public void RemoveCoopEvent()
        {
            coopEventGameObject.SetActive(false);
            checkmark.gameObject.SetActive(false);
            redCross.gameObject.SetActive(false);
            IsCoopEventSlot = false;
            foreach (var icon in eventIcons)
            {
                icon.gameObject.SetActive(false);
            }
        }

        public bool IsInCoopEventTypes(VEventUI item)
        {
            if (IsCoopEventSlot)
            {
                foreach (var coopEventType in _coopEventTypes)
                {
                    if (coopEventType.eventType == VEventType.Stream && coopEventType.abilityIndex != -1)
                    {
                        if (item.Event is VStreamEvent streamEvent)
                        {
                            if (streamEvent.MainAttributeIndex == coopEventType.abilityIndex)
                            {
                                return true;
                            }
                        }
                    }
                    else if (item.Event.Type == coopEventType.eventType)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void SetItem(VEventUI item)
        {
            _item = item;
            if(_scheduleUI is not null)
                _scheduleUI.RecordEvent(item.Event);
            
            if (IsCoopEventSlot && (_coopEventTypes.Count == 0 || IsInCoopEventTypes(item)))
            {
                checkmark.gameObject.SetActive(true);
                checkmark.transform.SetParent(VScheduleUIHelper.Instance.CheckMarkParent);
                item.Event.SetCoopEffects(this, _coopEventEffects);
            }
            else if(IsCoopEventSlot)
            {
                redCross.gameObject.SetActive(true);
                redCross.transform.SetParent(VScheduleUIHelper.Instance.CheckMarkParent);
            }
        }

        public void RemoveItem()
        {
            checkmark.transform.SetParent(transform);
            checkmark.gameObject.SetActive(false);
            
            redCross.transform.SetParent(transform);
            redCross.gameObject.SetActive(false);

            if (_item is not null && _item.Event is not null)
            {
                if(_scheduleUI is not null)
                    _scheduleUI.UnrecordEvent(_item.Event);
                _item.Event.RemoveCoopEffects(this);
                _item.Event.SetSchedulingConditionMet(false);
            }
            _item = null;
        }

        public void DespawnItem()
        {
            if (_item is null)
                return;
            
            _item.Despawn();
        }
        
        public void DestroyItem()
        {
            if (_item is null)
                return;
            Destroy(_item.gameObject);
            _item = null;
        }
        
        private void ChangeIndicatorScale(float scale) => _scheduleUI?.ChangeIndicatorScale(scale);
        private void ChangeIndicatorPosition(Vector3 position) => _scheduleUI?.ChangeIndicatorPosition(position);
        private void ChangeIndicatorColor(Color color) => _scheduleUI?.ChangeIndicatorColor(color);
        

        public void SetIndicator(int height, float offsetY)
        {
            if (height == 1)
            {
                ChangeIndicatorScale(1.0f);
                ChangeIndicatorPosition(transform.position);
                if (!Available)
                {
                    ChangeIndicatorColor(Color.red);
                    return;
                }

                ChangeIndicatorColor(Color.green);
                return;
            }

            if (height == 2)
            {
                ChangeIndicatorScale(2.0f);
                if (_coordination.y == 0)
                {
                    var position = (_scheduleUI.Slots[0, _coordination.x].transform.position +
                                    _scheduleUI.Slots[1, _coordination.x].transform.position) / 2f;
                    ChangeIndicatorPosition(position);
                    if (Available && _scheduleUI.Slots[1, _coordination.x].Available)
                    {
                        ChangeIndicatorColor(Color.green);
                        return;
                    }

                    ChangeIndicatorColor(Color.red);
                    return;
                }

                if (_coordination.y == 1)
                {
                    if (offsetY > 0.0f)
                    {
                        if (_scheduleUI.Slots[0, _coordination.x].Available && Available)
                        {
                            var position = (_scheduleUI.Slots[0, _coordination.x].transform.position +
                                            _scheduleUI.Slots[1, _coordination.x].transform.position) / 2f;
                            ChangeIndicatorColor(Color.green);
                            ChangeIndicatorPosition(position);
                            return;
                        }
                        if (_scheduleUI.Slots[2, _coordination.x].Available && Available)
                        {
                            var position = (_scheduleUI.Slots[1, _coordination.x].transform.position +
                                            _scheduleUI.Slots[2, _coordination.x].transform.position) / 2f;
                            ChangeIndicatorColor(Color.green);
                            ChangeIndicatorPosition(position);
                            return;
                        }
                    }
                    else
                    {
                        if (_scheduleUI.Slots[2, _coordination.x].Available && Available)
                        {
                            var position = (_scheduleUI.Slots[1, _coordination.x].transform.position +
                                            _scheduleUI.Slots[2, _coordination.x].transform.position) / 2f;
                            ChangeIndicatorColor(Color.green);
                            ChangeIndicatorPosition(position);
                            return;
                        }

                        if (_scheduleUI.Slots[0, _coordination.x].Available && Available)
                        {
                            var position = (_scheduleUI.Slots[0, _coordination.x].transform.position +
                                            _scheduleUI.Slots[1, _coordination.x].transform.position) / 2f;
                            ChangeIndicatorColor(Color.green);
                            ChangeIndicatorPosition(position);
                            return;
                        }
                    }

                    ChangeIndicatorColor(Color.red);
                    ChangeIndicatorPosition(transform.position);
                    ChangeIndicatorScale(2.0f);
                }

                if (_coordination.y == 2)
                {
                    var position = (_scheduleUI.Slots[1, _coordination.x].transform.position +
                                    _scheduleUI.Slots[2, _coordination.x].transform.position) / 2f;
                    ChangeIndicatorPosition(position);
                    if (_scheduleUI.Slots[1, _coordination.x].Available && Available)
                    {
                        ChangeIndicatorColor(Color.green);
                        return;
                    }

                    ChangeIndicatorColor(Color.red);
                    return;
                }
            }

            if (height == 3)
            {
                ChangeIndicatorScale(3.0f);
                var position = _scheduleUI.Slots[1, _coordination.x].transform.position;
                ChangeIndicatorPosition(position);
                for (int y = 0; y < 3; y++)
                {
                    if (!_scheduleUI.Slots[y, _coordination.x].Available)
                    {
                        ChangeIndicatorColor(Color.red);
                        return;
                    }
                }

                ChangeIndicatorColor(Color.green);
            }
        }


        public bool FindPosition(int eventID, int height, float offsetY, out List<VScheduleSlot> parents,
            out Transform transformParent, out Vector3 position)
        {
            _eventID = eventID;
            parents = null;
            transformParent = null;
            position = Vector3.zero;

            if (_useThisTransformAsParent)
            {
                transformParent = transform;
            }
            else
            {
                transformParent = VSingletonMonobehaviour<VScheduleUIHelper>.Instance.EventParent;
            }

            if (!Available)
            {
                return false;
            }

            if (height == 1)
            {
                parents = new List<VScheduleSlot>()
                {
                    this
                };
                position = transform.position;
                return true;
            }

            if (height == 2)
            {
                if (_coordination.y == 0)
                {
                    if (_scheduleUI.Slots[1, _coordination.x].Available)
                    {
                        parents = new List<VScheduleSlot>()
                        {
                            _scheduleUI.Slots[0, _coordination.x],
                            _scheduleUI.Slots[1, _coordination.x]
                        };
                        position = (_scheduleUI.Slots[0, _coordination.x].transform.position +
                                    _scheduleUI.Slots[1, _coordination.x].transform.position) / 2f;
                        return true;
                    }

                    return false;
                }

                if (_coordination.y == 1)
                {
                    if (offsetY > 0.0f)
                    {
                        if (_scheduleUI.Slots[0, _coordination.x].Available)
                        {
                            parents = new List<VScheduleSlot>()
                            {
                                _scheduleUI.Slots[0, _coordination.x],
                                _scheduleUI.Slots[1, _coordination.x]
                            };
                            position = (_scheduleUI.Slots[0, _coordination.x].transform.position +
                                        _scheduleUI.Slots[1, _coordination.x].transform.position) / 2f;
                            return true;
                        }

                        if (_scheduleUI.Slots[2, _coordination.x].Available)
                        {
                            parents = new List<VScheduleSlot>()
                            {
                                _scheduleUI.Slots[1, _coordination.x],
                                _scheduleUI.Slots[2, _coordination.x]
                            };
                            position = (_scheduleUI.Slots[1, _coordination.x].transform.position +
                                        _scheduleUI.Slots[2, _coordination.x].transform.position) / 2f;
                            return true;
                        }
                    }
                    else
                    {
                        if (_scheduleUI.Slots[2, _coordination.x].Available)
                        {
                            parents = new List<VScheduleSlot>()
                            {
                                _scheduleUI.Slots[1, _coordination.x],
                                _scheduleUI.Slots[2, _coordination.x]
                            };
                            position = (_scheduleUI.Slots[1, _coordination.x].transform.position +
                                        _scheduleUI.Slots[2, _coordination.x].transform.position) / 2f;
                            return true;
                        }
                        if (_scheduleUI.Slots[0, _coordination.x].Available)
                        {
                            parents = new List<VScheduleSlot>()
                            {
                                _scheduleUI.Slots[0, _coordination.x],
                                _scheduleUI.Slots[1, _coordination.x]
                            };
                            position = (_scheduleUI.Slots[0, _coordination.x].transform.position +
                                        _scheduleUI.Slots[1, _coordination.x].transform.position) / 2f;
                            return true;
                        }
                    }
                }

                if (_coordination.y == 2)
                {
                    if (_scheduleUI.Slots[1, _coordination.x].Available)
                    {
                        parents = new List<VScheduleSlot>()
                        {
                            _scheduleUI.Slots[1, _coordination.x],
                            _scheduleUI.Slots[2, _coordination.x]
                        };
                        position = (_scheduleUI.Slots[1, _coordination.x].transform.position +
                                    _scheduleUI.Slots[2, _coordination.x].transform.position) / 2f;
                        return true;
                    }
                }

                return false;
            }

            if (height == 3)
            {
                for (int y = 0; y < 3; y++)
                {
                    if (!_scheduleUI.Slots[y, _coordination.x].Available)
                    {
                        return false;
                    }
                }

                parents = new List<VScheduleSlot>()
                {
                    _scheduleUI.Slots[0, _coordination.x],
                    _scheduleUI.Slots[1, _coordination.x],
                    _scheduleUI.Slots[2, _coordination.x]
                };
                position = _scheduleUI.Slots[1, _coordination.x].transform.position;
                return true;
            }

            return false;
        }

        public void SetUseThisTransformAsParent(bool b)
        {
            _useThisTransformAsParent = b;
        }

        public bool TestSchedulingCondition(bool appendEffects)
        {
            if (Item is not null && Item.Event.SchedulingCondition is not null)
            {
                bool isConditionMet = _item.Event.SchedulingCondition.IsTrue(_scheduleUI.Character, this);

                if (appendEffects)
                {
                    _item.Event.SetSchedulingConditionMet(isConditionMet);
                }
                return isConditionMet;
            }

            return false;
        }
        
        // Add these new public methods to the class
        #region Scheduling Condition Highlighting

        protected override void Awake()
        {
            // Make sure the highlight is off at the start
            if (conditionHighlight != null)
            {
                conditionHighlight.SetActive(false);
            }
        }

        /// <summary>
        /// Checks if the dragged event meets this slot's scheduling condition and shows/hides the highlight accordingly.
        /// </summary>
        /// <param name="eventBeingDragged">The event currently being dragged over this slot.</param>
        public void CheckAndHighlight(VScheduleEvent eventBeingDragged)
        {
            if (conditionHighlight == null || eventBeingDragged?.SchedulingCondition == null)
            {
                return;
            }

            // Use the character reference from the VScheduleUI, as seen in your TestSchedulingCondition method
            bool isConditionMet = eventBeingDragged.SchedulingCondition.IsTrue(_scheduleUI.Character, this);

            conditionHighlight.SetActive(isConditionMet);
        }

        /// <summary>
        /// Forces the condition highlight to be hidden.
        /// </summary>
        public void HideHighlight()
        {
            if (conditionHighlight != null)
            {
                conditionHighlight.SetActive(false);
            }
        }

        #endregion
    }
}

