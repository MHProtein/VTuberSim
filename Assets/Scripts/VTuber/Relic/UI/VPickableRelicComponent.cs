using UnityEngine;
using UnityEngine.EventSystems;
using VTuber.BattleSystem.UI;
using VTuber.Core.Foundation;

namespace VTuber.Relic.UI
{
    public class VPickableRelicComponent : VMonoBehaviour, IPointerDownHandler
    {
        private VRelic _relic;
        private VRelicSlotUI _relicUI;
        private bool _isSelected;
        private VPickRelicMenu _pickRelicMenu;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.dragging)
                return;
            if (_isSelected)
            {
                _pickRelicMenu.RemoveCard(_relic);
                _relicUI.SetBackgroundColor(Color.white);
                _isSelected = false;
                return;
            }

            if (_pickRelicMenu.SelectCard(_relic))
            {
                _isSelected = true;
                _relicUI.SetBackgroundColor(Color.cyan);
            }
        }

        public void Initialize(VRelicSlotUI cardUI, VPickRelicMenu pickRelicMenu)
        {
            _relic = cardUI.Relic;
            _relicUI = cardUI;
            _pickRelicMenu = pickRelicMenu;
        }
    }
}