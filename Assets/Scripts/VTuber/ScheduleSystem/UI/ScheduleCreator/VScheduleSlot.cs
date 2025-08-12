using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VTuber.CoopSystem;
using VTuber.Core.Foundation;
using VTuber.Core.RaisingEffect;
using VTuber.ScheduleSystem.Core;


namespace VTuber.ScheduleSystem.UI
{
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
        private List<VEventType> _coopEventTypes;

        [SerializeField] private GameObject coopEventGameObject;
        [SerializeField] private Image checkmark;
        [SerializeField] private Image pfp;
        [SerializeField] private List<Image> eventIcons;
        
        public void Initialize(Vector2Int coordination, VScheduleUI scheduleUI)
        {
            _coordination = coordination;
            _scheduleUI = scheduleUI;
        }

        public void SetCoopEvent(VCoopEventItem eventItem)
        {
            coopEventGameObject.SetActive(true);
            IsCoopEventSlot = true;
            pfp.sprite = eventItem.pfp;
            _coopEventEffects = eventItem.e.effects;
            _coopEventTypes = eventItem.e.eventTypes;
            for (int i = 0; i < eventItem.e.eventTypes.Count; i++)
            {
                eventIcons[i].gameObject.SetActive(true);
                string x = eventItem.e.eventTypes[i].ToString();
                eventIcons[i].sprite = VRaisingUI.Instance.GetIcon(x);
            }
        }
        
        public void RemoveCoopEvent()
        {
            coopEventGameObject.SetActive(false);
            IsCoopEventSlot = false;
            foreach (var icon in eventIcons)
            {
                icon.gameObject.SetActive(false);
            }
        }

        public void SetItem(VEventUI item)
        {
            _item = item;
            if (IsCoopEventSlot && _coopEventTypes.Contains(item.Event.Type))
            {
                checkmark.gameObject.SetActive(true);
                item.Event.SetCoopEffects(this, _coopEventEffects);
            }
        }

        public void RemoveItem()
        {
            checkmark.gameObject.SetActive(false);
            
            if (_item is not null && _item.Event is not null)
                _item.Event.RemoveCoopEffects(this);
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

        public void SetIndicator(int height, float offsetY)
        {
            if (height == 1)
            {
                _scheduleUI.ChangeIndicatorScale(1.0f);
                _scheduleUI.ChangeIndicatorPosition(transform.position);
                if (_item is not null)
                {
                    _scheduleUI.ChangeIndicatorColor(Color.red);
                    return;
                }

                _scheduleUI.ChangeIndicatorColor(Color.green);
                return;
            }

            if (height == 2)
            {
                _scheduleUI.ChangeIndicatorScale(2.0f);
                if (_coordination.y == 0)
                {
                    var position = (_scheduleUI.Slots[0, _coordination.x].transform.position +
                                    _scheduleUI.Slots[1, _coordination.x].transform.position) / 2f;
                    _scheduleUI.ChangeIndicatorPosition(position);
                    if (_scheduleUI.Slots[1, _coordination.x].Item is null
                        && _item is null)
                    {
                        _scheduleUI.ChangeIndicatorColor(Color.green);
                        return;
                    }

                    _scheduleUI.ChangeIndicatorColor(Color.red);
                    return;
                }

                if (_coordination.y == 1)
                {
                    if (offsetY > 0.0f)
                    {
                        if (_scheduleUI.Slots[0, _coordination.x].Item is null && _item is null)
                        {
                            var position = (_scheduleUI.Slots[0, _coordination.x].transform.position +
                                            _scheduleUI.Slots[1, _coordination.x].transform.position) / 2f;
                            _scheduleUI.ChangeIndicatorColor(Color.green);
                            _scheduleUI.ChangeIndicatorPosition(position);
                            return;
                        }
                        if (_scheduleUI.Slots[2, _coordination.x].Item is null && _item is null)
                        {
                            var position = (_scheduleUI.Slots[1, _coordination.x].transform.position +
                                            _scheduleUI.Slots[2, _coordination.x].transform.position) / 2f;
                            _scheduleUI.ChangeIndicatorColor(Color.green);
                            _scheduleUI.ChangeIndicatorPosition(position);
                            return;
                        }
                    }
                    else
                    {
                        if (_scheduleUI.Slots[2, _coordination.x].Item is null && _item is null)
                        {
                            var position = (_scheduleUI.Slots[1, _coordination.x].transform.position +
                                            _scheduleUI.Slots[2, _coordination.x].transform.position) / 2f;
                            _scheduleUI.ChangeIndicatorColor(Color.green);
                            _scheduleUI.ChangeIndicatorPosition(position);
                            return;
                        }

                        if (_scheduleUI.Slots[0, _coordination.x].Item is null && _item is null)
                        {
                            var position = (_scheduleUI.Slots[0, _coordination.x].transform.position +
                                            _scheduleUI.Slots[1, _coordination.x].transform.position) / 2f;
                            _scheduleUI.ChangeIndicatorColor(Color.green);
                            _scheduleUI.ChangeIndicatorPosition(position);
                            return;
                        }
                    }

                    _scheduleUI.ChangeIndicatorColor(Color.red);
                    _scheduleUI.ChangeIndicatorPosition(transform.position);
                    _scheduleUI.ChangeIndicatorScale(2.0f);
                }

                if (_coordination.y == 2)
                {
                    var position = (_scheduleUI.Slots[1, _coordination.x].transform.position +
                                    _scheduleUI.Slots[2, _coordination.x].transform.position) / 2f;
                    _scheduleUI.ChangeIndicatorPosition(position);
                    if (_scheduleUI.Slots[1, _coordination.x].Item is null && _item is null)
                    {
                        _scheduleUI.ChangeIndicatorColor(Color.green);
                        return;
                    }

                    _scheduleUI.ChangeIndicatorColor(Color.red);
                    return;
                }
            }

            if (height == 3)
            {
                _scheduleUI.ChangeIndicatorScale(3.0f);
                var position = _scheduleUI.Slots[1, _coordination.x].transform.position;
                _scheduleUI.ChangeIndicatorPosition(position);
                for (int y = 0; y < 3; y++)
                {
                    if (_scheduleUI.Slots[y, _coordination.x]._item is not null)
                    {
                        _scheduleUI.ChangeIndicatorColor(Color.red);
                        return;
                    }
                }

                _scheduleUI.ChangeIndicatorColor(Color.green);
            }
        }


        public bool FindPosition(int height, float offsetY, out List<VScheduleSlot> parents,
            out Transform transformParent, out Vector3 position)
        {
            parents = null;
            transformParent = null;
            position = Vector3.zero;

            if (_item is not null)
            {
                return false;
            }

            if (height == 1)
            {
                parents = new List<VScheduleSlot>()
                {
                    this
                };
                transformParent = transform;
                position = transform.position;
                return true;
            }

            if (height == 2)
            {
                if (_coordination.y == 0)
                {
                    if (_scheduleUI.Slots[1, _coordination.x].Item is null)
                    {
                        parents = new List<VScheduleSlot>()
                        {
                            _scheduleUI.Slots[0, _coordination.x],
                            _scheduleUI.Slots[1, _coordination.x]
                        };
                        transformParent = _scheduleUI.Slots[1, _coordination.x].transform;
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
                        if (_scheduleUI.Slots[0, _coordination.x].Item is null)
                        {
                            parents = new List<VScheduleSlot>()
                            {
                                _scheduleUI.Slots[0, _coordination.x],
                                _scheduleUI.Slots[1, _coordination.x]
                            };
                            transformParent = _scheduleUI.Slots[1, _coordination.x].transform;
                            position = (_scheduleUI.Slots[0, _coordination.x].transform.position +
                                        _scheduleUI.Slots[1, _coordination.x].transform.position) / 2f;
                            return true;
                        }

                        if (_scheduleUI.Slots[2, _coordination.x].Item is null)
                        {
                            parents = new List<VScheduleSlot>()
                            {
                                _scheduleUI.Slots[1, _coordination.x],
                                _scheduleUI.Slots[2, _coordination.x]
                            };
                            transformParent = _scheduleUI.Slots[2, _coordination.x].transform;
                            position = (_scheduleUI.Slots[1, _coordination.x].transform.position +
                                        _scheduleUI.Slots[2, _coordination.x].transform.position) / 2f;
                            return true;
                        }
                    }
                    else
                    {
                        if (_scheduleUI.Slots[2, _coordination.x].Item is null)
                        {
                            parents = new List<VScheduleSlot>()
                            {
                                _scheduleUI.Slots[1, _coordination.x],
                                _scheduleUI.Slots[2, _coordination.x]
                            };
                            transformParent = _scheduleUI.Slots[2, _coordination.x].transform;
                            position = (_scheduleUI.Slots[1, _coordination.x].transform.position +
                                        _scheduleUI.Slots[2, _coordination.x].transform.position) / 2f;
                            return true;
                        }
                        if (_scheduleUI.Slots[0, _coordination.x].Item is null)
                        {
                            parents = new List<VScheduleSlot>()
                            {
                                _scheduleUI.Slots[0, _coordination.x],
                                _scheduleUI.Slots[1, _coordination.x]
                            };
                            transformParent = _scheduleUI.Slots[1, _coordination.x].transform;
                            position = (_scheduleUI.Slots[0, _coordination.x].transform.position +
                                        _scheduleUI.Slots[1, _coordination.x].transform.position) / 2f;
                            return true;
                        }
                    }
                }

                if (_coordination.y == 2)
                {
                    if (_scheduleUI.Slots[1, _coordination.x].Item is null)
                    {
                        parents = new List<VScheduleSlot>()
                        {
                            _scheduleUI.Slots[1, _coordination.x],
                            _scheduleUI.Slots[2, _coordination.x]
                        };
                        transformParent = _scheduleUI.Slots[2, _coordination.x].transform;
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
                    if (_scheduleUI.Slots[y, _coordination.x]._item is not null)
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
                transformParent = _scheduleUI.Slots[2, _coordination.x].transform;
                position = _scheduleUI.Slots[1, _coordination.x].transform.position;
                return true;
            }

            return false;
        }
    }
}

