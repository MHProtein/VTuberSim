
using System.Collections.Generic;
using PrimeTween;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using VTuber.BattleSystem.UI;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;
using VTuber.Core.Managers;

namespace VTuber.ScheduleSystem.UI
{
    public class VScheduleUI : VUIBehaviour
    {
        public Vector2Int slotSize;
        [SerializeField] protected GameObject itemPrefab;
        [SerializeField] protected Transform indicator;
        [SerializeField] protected Image indicatorLeft;
        [SerializeField] protected Image indicatorRight;
        
        public VScheduleSlot[,] Slots => slots;
        protected VScheduleSlot[,] slots;

        protected VAnimationQueue _animationQueue;
        
        protected override void Awake()
        {
            PrimeTweenConfig.warnEndValueEqualsCurrent = false;
            slots = new VScheduleSlot[slotSize.y, slotSize.x];
            var slotList = GetComponentsInChildren<VScheduleSlot>();
            _animationQueue = new VAnimationQueue();
            int i = 0; 
            for (int y = 0; y < slotSize.y; y++)
            {
                for (int x = 0; x < slotSize.x; x++)
                {    
                    slots[y, x] = slotList[i++];
                    slots[y, x].Initialize(new Vector2Int(x, y), this);
                }
            }
        }

        public void SwitchToCreation()
        {
            for (int y = 0; y < slotSize.y; y++)
            {
                for (int x = 0; x < slotSize.x; x++)
                {
                    if (slots[y, x].Item != null)
                    {
                        
                        Destroy(slots[y, x].Item.gameObject);
                    }
                }
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
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnEventExecuted, OnEventExecuted);
        }
        
        protected override void OnDisable()
        {
            base.OnDisable();
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnEventExecuted, OnEventExecuted);
        }
        
        private void OnEventExecuted(Dictionary<string, object> messagedict)
        {
            Vector2Int coordinate = (Vector2Int)messagedict["Coordinate"];
            ChangeIndicatorPosition(slots[coordinate.y, coordinate.x].Item.transform.position);
            ChangeIndicatorScale(slots[coordinate.y, coordinate.x].Item.Event.Duration);
        }

        public Tween MoveIndicator(Vector2Int coordinate)
        {
            ChangeIndicatorPosition(slots[coordinate.y, coordinate.x].Item.transform.position);
            return ChangeIndicatorScale(slots[coordinate.y, coordinate.x].Item.Event.Duration);
        }

        public void ResetSchedule()
        {            
            for (int x = 0; x < slotSize.x; x++)
            {
                for (int y = 0; y < slotSize.y; y++)
                {
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
        
        public void ChangeIndicatorPosition(Vector2 position)
        {
            Tween.Position(indicator, position, 0.2f);
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
                            var eventData = VResourcesManager.Instance.GetDialogueEventConfigurationByID(eventId);
                            eventUIObject.Initialize(eventData, slots[yy, x]);
                            
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
                    var eventData = VResourcesManager.Instance.GetDialogueEventConfigurationByID(eventId);
                    eventUIObject.Initialize(eventData, slots[yy, x]);
                }
            }
        }
    }
}


