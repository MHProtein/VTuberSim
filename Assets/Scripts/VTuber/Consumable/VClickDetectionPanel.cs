using System;
using UnityEngine.EventSystems;
using VTuber.Core.Foundation;

namespace VTuber.Consumable
{
    public class VClickDetectionPanel : VUIBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        public Action onClick;
        public void OnPointerClick(PointerEventData eventData)
        {
            eventData.Use();
            onClick?.Invoke();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            eventData.Use();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            eventData.Use();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            eventData.Use();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            eventData.Use();
        }
    }
}