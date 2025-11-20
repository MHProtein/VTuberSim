using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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
        public string coopEventIconName;
        public int coopEventID;
    }

    public class VScheduleSlot : VUIBehaviour, IPointerEnterHandler
    {
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
        
        private int _allowedEventID;
        private List<VRaisingEffect> _coopEventEffects;
        private Sprite _coopEventIcon;
        private int _coopEventID;
        private List<VCoopEvent.VCoopEventType> _coopEventTypes;
        private string _coopEventDescription;
        private Vector2Int _coordination;
        private int _eventID;

        private bool _placeable = true;

        private VScheduleUI _scheduleUI;
        private bool _useThisTransformAsParent;
        public VEventUI Item { get; private set; }

        public bool IsCoopEventSlot { get; private set; }

        /// <summary>
        ///     X=Day, Y=TimeOfDay
        /// </summary>
        public Vector2Int Coordination => _coordination;

        public bool Available
        {
            get
            {
                if (_placeable)
                    return Item is null;
                if (_allowedEventID == -1)
                    return false;
                return Item is null && _allowedEventID == _eventID;
            }
        }

        public VScheduleSlotSaveData Save()
        {
            return new VScheduleSlotSaveData
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
            var coopEvent = VDataManager.Instance.GetCoopEventByID((uint)saveData.coopEventID);
            SetCoopEvent(new VCoopEventItem
            {
                e = coopEvent,
                pfp = _coopEventIcon,
                description = coopEvent.description,
            });
        }

        public void Initialize(Vector2Int coordination, VScheduleUI scheduleUI, bool useThisTransformAsParent)
        {
            _coordination = coordination;
            _scheduleUI = scheduleUI;
            _useThisTransformAsParent = useThisTransformAsParent;
            _coopEventID = -1;
        }

        public List<VScheduleSlot> GetUDSlots()
        {
            return _scheduleUI.GetUDSlots(this);
        }

        public List<VScheduleSlot> GetLRSlots()
        {
            return _scheduleUI.GetLRSlots(this);
        }

        public List<VScheduleSlot> GetUDLRSlots()
        {
            return _scheduleUI.GetUDLRSlots(this);
        }

        public List<VScheduleSlot> GetSurroundingSlots()
        {
            return _scheduleUI.GetSurroundingSlots(this);
        }

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
            _coopEventDescription = eventItem.description;
            for (var i = 0; i < eventItem.e.eventTypes.Count; i++)
            {
                eventIcons[i].gameObject.SetActive(true);
                var x = eventItem.e.eventTypes[i].eventType.ToString();
                eventIcons[i].sprite = VResourcesManager.Instance.TryGetSprite(x);
                if (eventItem.e.eventTypes[i].eventType == VEventType.Stream &&
                    eventItem.e.eventTypes[i].abilityIndex != -1)
                    eventIcons[i].color = VRaisingUI.Instance.abilityColors[eventItem.e.eventTypes[i].abilityIndex];

                foreach (var effect in _coopEventEffects)
                    if (effect is IAttributeEffect attributeEffect)
                    {
                        effectText.text = "+" + effect.GetParameter();
                        effectImage.GetComponentInChildren<Image>().sprite =
                            VUIUtils.Instance.GetAttributeIcon(attributeEffect.AttributeName);
                    }
            }
        }

        public void RemoveCoopEvent()
        {
            coopEventGameObject.SetActive(false);
            checkmark.gameObject.SetActive(false);
            redCross.gameObject.SetActive(false);
            IsCoopEventSlot = false;
            foreach (var icon in eventIcons) icon.gameObject.SetActive(false);
        }

        public bool IsInCoopEventTypes(VEventUI item)
        {
            if (IsCoopEventSlot)
                foreach (var coopEventType in _coopEventTypes)
                    if (coopEventType.eventType == VEventType.Stream && coopEventType.abilityIndex != -1)
                    {
                        if (item.Event is VStreamEvent streamEvent)
                            if (streamEvent.MainAttributeIndex == coopEventType.abilityIndex)
                                return true;
                    }
                    else if (item.Event.Type == coopEventType.eventType)
                    {
                        return true;
                    }

            return false;
        }

        public void SetItem(VEventUI item)
        {
            Item = item;
            if (_scheduleUI is not null)
                _scheduleUI.RecordEvent(item.Event);

            if (IsCoopEventSlot && (_coopEventTypes.Count == 0 || IsInCoopEventTypes(item)))
            {
                checkmark.gameObject.SetActive(true);
                checkmark.transform.SetParent(VScheduleUIHelper.Instance.CheckMarkParent);
                item.Event.SetCoopEffects(this, _coopEventEffects, _coopEventIcon, _coopEventDescription);
            }
            else if (IsCoopEventSlot)
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

            if (Item is not null && Item.Event is not null)
            {
                if (_scheduleUI is not null)
                    _scheduleUI.UnrecordEvent(Item.Event);
                Item.Event.RemoveCoopEffects(this);
                Item.Event.SetSchedulingConditionMet(false);
            }

            Item = null;
        }

        public void DespawnItem()
        {
            if (Item is null)
                return;

            Item.Despawn();
        }

        public void DestroyItem()
        {
            if (Item is null)
                return;
            Destroy(Item.gameObject);
            Item = null;
        }

        private void ChangeIndicatorScale(float scale)
        {
            _scheduleUI?.ChangeIndicatorScale(scale);
        }

        private void ChangeIndicatorPosition(Vector3 position)
        {
            _scheduleUI?.ChangeIndicatorPosition(position, false);
        }

        private void ChangeIndicatorColor(Color color)
        {
            _scheduleUI?.ChangeIndicatorColor(color);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_scheduleUI is null || !_scheduleUI.Editing)
                return;
            _scheduleUI.MoveIndicator(Coordination);
        }

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
                for (var y = 0; y < 3; y++)
                    if (!_scheduleUI.Slots[y, _coordination.x].Available)
                    {
                        ChangeIndicatorColor(Color.red);
                        return;
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
                transformParent = transform;
            else
                transformParent = VSingletonMonobehaviour<VScheduleUIHelper>.Instance.EventParent;

            if (!Available) return false;

            if (height == 1)
            {
                parents = new List<VScheduleSlot>
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
                        parents = new List<VScheduleSlot>
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
                            parents = new List<VScheduleSlot>
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
                            parents = new List<VScheduleSlot>
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
                            parents = new List<VScheduleSlot>
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
                            parents = new List<VScheduleSlot>
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
                    if (_scheduleUI.Slots[1, _coordination.x].Available)
                    {
                        parents = new List<VScheduleSlot>
                        {
                            _scheduleUI.Slots[1, _coordination.x],
                            _scheduleUI.Slots[2, _coordination.x]
                        };
                        position = (_scheduleUI.Slots[1, _coordination.x].transform.position +
                                    _scheduleUI.Slots[2, _coordination.x].transform.position) / 2f;
                        return true;
                    }

                return false;
            }

            if (height == 3)
            {
                for (var y = 0; y < 3; y++)
                    if (!_scheduleUI.Slots[y, _coordination.x].Available)
                        return false;

                parents = new List<VScheduleSlot>
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
                var isConditionMet = Item.Event.SchedulingCondition.IsTrue(_scheduleUI.Character, this);

                if (appendEffects) Item.Event.SetSchedulingConditionMet(isConditionMet);
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