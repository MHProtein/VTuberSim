using System;
using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;
using VTuber.ScheduleSystem.Events;

namespace VTuber.ScheduleSystem.UI
{
    public class VEventDataUI : VUIBehaviour, IPointerEnterHandler,
        IPointerDownHandler, IPointerUpHandler, IPointerClickHandler,
        IPointerExitHandler, IBeginDragHandler, IDragHandler
    {
        [SerializeField] private Image icon;
        [SerializeField] private Image background;
        [SerializeField] private Image nameIcon;
        [SerializeField] private GameObject eventUIPrefab;
        [SerializeField] private TMP_Text duration;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text costText;
        [SerializeField] private Image costIcon;
        
        private VScheduleEventConfiguration _data;
        
        private bool spawnable;
        
        private VScheduleCreatorSlot slot;
        
        public void Initialize(VScheduleEventConfiguration data)
        {
            _data = data;
            icon.sprite = VResourcesManager.Instance.TryGetSprite(data.icon);;
            background.color = data.backgroundColor;
            duration.text = data.Duration.ToString();
            nameText.text = data.eventName;
            nameIcon.color = data.backgroundColor;

            switch (data.costType)
            {
                case VEventCostType.Stamina:
                    costIcon.sprite = VResourcesManager.Instance.TryGetSprite("Icon_Stamina");
                    break;
                case VEventCostType.Money:
                    costIcon.sprite = VResourcesManager.Instance.TryGetSprite("Icon_Money");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            costText.text = data.cost.ToString();
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
            background.color = Color.cyan;
            nameIcon.color = Color.cyan;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            spawnable = false;
            background.color = _data.backgroundColor;
            nameIcon.color = _data.backgroundColor;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            VDebug.Log("Begin Drag");
            if (!spawnable)
                return;
            VEventUI eventUIObject = VRaisingUI.Instance.CreateEventUI(VScheduleUIHelper.Instance.CanvasRect);
            eventUIObject.InitializeDrag(_data.CreateEvent(), transform.position);
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
            eventUIObject.GetComponent<VEventUI>().InitializeDrag(_data.CreateEvent(), transform.position);
        }

        public void OnDrag(PointerEventData eventData)
        {
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            eventData.Use();
        }
    }
}


