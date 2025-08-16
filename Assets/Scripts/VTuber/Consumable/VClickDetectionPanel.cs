using System;
using UnityEngine.EventSystems;
using VTuber.Core.Foundation;

namespace VTuber.Consumable
{
    public class VClickDetectionPanel : VUIBehaviour, IPointerClickHandler
    {
        public Action onClick;
        public void OnPointerClick(PointerEventData eventData)
        {
            eventData.Use();
            onClick?.Invoke();
        }
    }
}