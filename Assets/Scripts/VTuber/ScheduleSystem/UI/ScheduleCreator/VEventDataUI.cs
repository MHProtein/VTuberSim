// using System;
// using System.Collections;
// using System.Collections.Generic;
// using PrimeTween;
// using UnityEngine;
// using UnityEngine.EventSystems;
// using UnityEngine.UI;
// using VTuber.Core.Foundation;
//
// namespace VTuber.ScheduleSystem.UI
// {
//     public class VEventDataUI : VUIBehaviour, IPointerEnterHandler,
//         IPointerDownHandler, IPointerUpHandler,
//         IPointerExitHandler, IBeginDragHandler, IDragHandler
//     {
//         [SerializeField] private Image icon;
//         [SerializeField] private Image background;
//         [SerializeField] private GameObject eventUIPrefab;
//         
//         private EventData _data;
//         
//         private bool spawnable;
//         
//         private VScheduleCreatorSlot slot;
//         
//         public void Initialize(EventData data)
//         {
//             _data = data;
//             icon.sprite = data.icon;
//             background.color = data.backgroundColor;
//         }
//         
//         public void OnPointerEnter(PointerEventData eventData)
//         {
//             spawnable = true;
//         }
//
//         public void OnPointerDown(PointerEventData eventData)
//         {
//             
//         }
//
//         public void OnPointerExit(PointerEventData eventData)
//         {
//             spawnable = false;
//         }
//
//         public void OnBeginDrag(PointerEventData eventData)
//         {
//             VDebug.Log("Begin Drag");
//             if (!spawnable)
//                 return;
//             
//             GameObject eventUIObject = Instantiate(eventUIPrefab, VSingletonMonobehaviour<VScheduleUIHelper>.Instance.CanvasRect);
//             eventUIObject.GetComponent<VEventUI>().InitializeDrag(_data, transform.position);
//         }
//
//         public void OnPointerUp(PointerEventData eventData)
//         {
//             if (!spawnable)
//                 return;
//             VDebug.Log("Pointer Up");
//         //     var results = VSingletonMonobehaviour<VScheduleUIHelper>.Instance.RaycastFromMouse();
//         //     foreach (var result in results)
//         //     {
//         //         var ui = result.gameObject.GetComponent<VEventDataUI>();
//         //         if(ui == null)
//         //             continue;
//         //         if (ui && (ui == this))
//         //         {
//         //            
//         //         }
//         //     }
//             GameObject eventUIObject = Instantiate(eventUIPrefab, VSingletonMonobehaviour<VScheduleUIHelper>.Instance.CanvasRect);
//             eventUIObject.GetComponent<VEventUI>().InitializeDrag(_data, transform.position);
//         }
//
//         public void OnDrag(PointerEventData eventData)
//         {
//         }
//     }
// }
//
//
