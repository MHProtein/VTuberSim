using UnityEngine;
using UnityEngine.EventSystems;
using VTuber.BattleSystem.Card;
using VTuber.BattleSystem.UI;
using VTuber.Core.Foundation;

namespace VTuber.ScheduleSystem.UI
{
    public class VSelectCardCardUI : VUIBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        private ISelectableCardMenu _cardLibraryUI;
        private VCardUI _cardUI;
        private bool _selectable = true;
        public VCard Card => _cardUI.Card;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.dragging)
                return;
            if (!_selectable)
                return;
            _cardUI.background.color = Color.grey;
            _cardLibraryUI.Select(this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
        }

        public void OnPointerExit(PointerEventData eventData)
        {
        }

        public void Initialize(VCardUI cardUI, ISelectableCardMenu menu, bool selectable)
        {
            _cardUI = cardUI;
            _cardLibraryUI = menu;
            _selectable = selectable;
        }

        public void SetSelectable(bool selectable)
        {
            _selectable = selectable;
        }

        public void UnSelect()
        {
            _cardUI.background.color = Color.white;
        }
    }
}