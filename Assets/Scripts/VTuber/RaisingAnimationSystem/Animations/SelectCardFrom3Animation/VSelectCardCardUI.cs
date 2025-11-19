using PrimeTween;
using UnityEngine;
using UnityEngine.EventSystems;
using VTuber.BattleSystem.Card;
using VTuber.BattleSystem.UI;
using VTuber.Core.Foundation;
using VTuber.RaisingAnimationSystem.Animations.SelectCardMenuAnimation;
using VTuber.ScheduleSystem.UI;

namespace VTuber.RaisingAnimationSystem.Animations.SelectCardFrom3Animation
{
    public class VSelectCardCardUI : VUIBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        private ISelectableCardMenu _cardLibraryUI;
        private VCardUI _cardUI;
        private bool _selectable = true;
        public VCard Card => _cardUI.Card;
        private Vector3 _cardPosition;

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

        public void SetCard(VCard card, bool selectable)
        {
            _cardUI.SetCard(card);
            _selectable = selectable;
        }
        
        public void Popup()
        {
            var selectable = _selectable;
            SetSelectable(false);
            _cardUI.transform.localScale = Vector3.zero;
            Tween.Scale(_cardUI.transform, Vector3.one * 1.1f, 0.5f, Ease.OutCubic).OnComplete(() =>
            {
                Tween.Scale(_cardUI.transform, Vector3.one, 0.5f, Ease.OutBack).OnComplete(() =>
                {
                    SetSelectable(selectable);
                });
            });
        }
    }
}