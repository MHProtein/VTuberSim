using UnityEngine;
using UnityEngine.EventSystems;
using VTuber.BattleSystem.Card;
using VTuber.BattleSystem.UI;
using VTuber.Core.Foundation;

namespace VTuber.ScheduleSystem.UI
{
    public class VSelectCardCardUI : VUIBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        public VCard Card => _cardUI.Card;
        private VCardUI _cardUI;
        private VCardLibraryUI _cardLibraryUI;
        private bool _selectable = true;
        
        public void Initialize(VCardUI cardUI, VCardLibraryUI cardLibraryUI, bool selectable)
        {
            _cardUI = cardUI;
            _cardLibraryUI = cardLibraryUI;
            _selectable = selectable;
        }

        public void UnSelect()
        {
            _cardUI.background.color = Color.white;
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
            if (!_selectable)
                return;
            _cardUI.background.color = Color.grey;
            _cardLibraryUI.Select(this);
        }
    }
}