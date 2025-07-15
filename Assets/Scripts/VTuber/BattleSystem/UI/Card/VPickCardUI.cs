using System;
using UnityEngine.EventSystems;
using VTuber.BattleSystem.Card;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.UI
{
    public class VPickCardUI : VUIBehaviour , IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        private VCard card;
        private Action<VCard> _onCardSelected;
        
        public void Initialize(VCard card, Action<VCard> onCardSelected)
        {
            card = card;
            _onCardSelected = onCardSelected;
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            
        }
    }
}