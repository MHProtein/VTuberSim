using System;
using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;
using VTuber.ScheduleSystem.Events;

namespace VTuber.ScheduleSystem.UI
{
    public class VEventDataUI : VUIBehaviour, IPointerEnterHandler,
        IPointerDownHandler, IPointerUpHandler,
        IPointerExitHandler, IBeginDragHandler, IDragHandler
    {
        [SerializeField] private Image icon;
        [SerializeField] private Image background;
        [SerializeField] private GameObject eventUIPrefab;
        
        private VScheduleEventConfiguration _data;
        
        private bool spawnable;
        
        private VScheduleCreatorSlot slot;
        
        public void Initialize(VScheduleEventConfiguration data)
        {
            _data = data;
            icon.sprite = data.icon;
            background.color = data.backgroundColor;
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            spawnable = true;
            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnNotifyEventDescriptionChange,
                new Dictionary<string, object>()
                {
                    {"Name", _data.eventName},
                    {"Description", _data.description}
                });
            background.color = Color.white;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            spawnable = false;
            background.color = _data.backgroundColor;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            VDebug.Log("Begin Drag");
            if (!spawnable)
                return;
            
            GameObject eventUIObject = Instantiate(eventUIPrefab, VSingletonMonobehaviour<VScheduleUIHelper>.Instance.CanvasRect);
            eventUIObject.GetComponent<VEventUI>().InitializeDrag(_data, transform.position);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!spawnable)
                return;
            VDebug.Log("Pointer Up");
        //     var results = VSingletonMonobehaviour<VScheduleUIHelper>.Instance.RaycastFromMouse();
        //     foreach (var result in results)
        //     {
        //         var ui = result.gameObject.GetComponent<VEventDataUI>();
        //         if(ui == null)
        //             continue;
        //         if (ui && (ui == this))
        //         {
        //            
        //         }
        //     }
            GameObject eventUIObject = Instantiate(eventUIPrefab, VSingletonMonobehaviour<VScheduleUIHelper>.Instance.CanvasRect);
            eventUIObject.GetComponent<VEventUI>().InitializeDrag(_data, transform.position);
        }

        public void OnDrag(PointerEventData eventData)
        {
        }
    }
}


