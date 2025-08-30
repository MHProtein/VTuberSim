using System;
using UnityEngine.EventSystems;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.UI
{
    public class VEllipsisUI : VUIBehaviour, IPointerClickHandler
    {
        public Action onClick;
        public void OnPointerClick(PointerEventData eventData)
        {
            onClick?.Invoke();
        }
    }
}