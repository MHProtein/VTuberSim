using System;
using UnityEngine;
using UnityEngine.EventSystems;
using VTuber.BattleSystem.Card;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.UI
{
    public class VPickCardUI : VUIBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        private VCard _card;
        private VCardUI _cardUI;
        private VPickCardMenu _pickCardMenu;
        private bool _isSelected;
        
        public void Initialize(VCardUI cardUI, VPickCardMenu pickCardMenu)
        {
            _card = cardUI.Card;
            _cardUI = cardUI;
            _pickCardMenu = pickCardMenu;
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.dragging)
                return;
            if (_isSelected)
            {
                _pickCardMenu.RemoveCard(_card);
                _cardUI.SetBackgroundColor(Color.white);
                _isSelected = false;
                return;
            }

            if (_pickCardMenu.SelectCard(_card))
            {
                _isSelected = true;
                _cardUI.SetBackgroundColor(Color.grey);
            }
        }
    }
}