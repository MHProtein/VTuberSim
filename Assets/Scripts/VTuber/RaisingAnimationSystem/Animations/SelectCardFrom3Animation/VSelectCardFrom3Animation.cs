using System;
using System.Collections.Generic;
using System.Linq;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;
using VTuber.BattleSystem.Card;
using VTuber.BattleSystem.UI;
using VTuber.Core.Foundation;
using VTuber.Core.Managers;
using VTuber.Core.SE;
using VTuber.Dialogue.UI;
using VTuber.RaisingAnimationSystem;
using VTuber.ScheduleSystem.UI.RaisingAnimationSystem;

namespace VTuber.ScheduleSystem.UI
{
    
    public class VSelectCardFrom3Animation : VRaisingAnimation, ISelectableCardMenu
    {
        [SerializeField] private GameObject cardPrefab;

        [SerializeField] private Button confirmButton;

        public List<Transform> positions;
        public Transform spawnPosition;
        private List<VSelectCardCardUI> _cardUIs;
        private VSelectCardCardUI _selectedCardUI;
        
        private Action _onComplete;

        protected override void Awake()
        {
            base.Awake();
            confirmButton.onClick.AddListener(Confirm);
        }

        public void Select(VSelectCardCardUI cardUI)
        {
            VAudioPlayer.Instance.PlayStaticSFX(VSFXType.Selection);
            confirmButton.interactable = true;
            if (_selectedCardUI != null && _selectedCardUI == cardUI)
                return;

            if (_selectedCardUI is not null)
                _selectedCardUI.UnSelect();
            _selectedCardUI = cardUI;
        }

        public override void BeginAnimation(VAnimationRequest request, Action onComplete, bool isLastSameType)
        {
            base.BeginAnimation(request, onComplete, isLastSameType);

            _onComplete = onComplete;
            Initialize(request.cards);
        }

        public void Initialize(List<VCard> cards)
        {
            confirmButton.interactable = false;
            var i = 0;
            _cardUIs = new List<VSelectCardCardUI>();
            foreach (var card in cards)
            {
                var item = Instantiate(cardPrefab, transform);
                var cardItem = item.AddComponent<VSelectCardCardUI>();
                var cardUI = cardItem.GetComponent<VCardUI>();
                cardUI.SetCard(card);

                cardItem.Initialize(cardUI, this, false);
                _cardUIs.Add(cardItem);

                cardUI.transform.localScale = Vector3.zero;
                cardUI.transform.position = spawnPosition.position;
                Tween.Position(cardUI.transform, positions[i].position, 0.5f);
                Tween.Scale(cardUI.transform, Vector3.one * 1.5f, 0.5f, Ease.OutBounce).OnComplete(() =>
                {
                    cardItem.SetSelectable(true);
                });
                i++;
            }
        }

        public void Confirm()
        {
            var sequence = Sequence.Create();
            foreach (var cardUI in _cardUIs)
            {
                cardUI.SetSelectable(false);
                if (cardUI != _selectedCardUI)
                {
                    sequence.Group(Tween.Scale(cardUI.GetComponent<VCardUI>().transform, Vector3.zero, 0.3f,
                        Ease.InBack));
                }
            }
            
            sequence.Chain(Tween.Position(_selectedCardUI.transform, positions[2].position, 0.25f, Ease.InOutCubic))
                .Chain(Tween.Scale(_selectedCardUI.GetComponent<VCardUI>().transform, Vector3.one * 1.3f, 0.25f,
                    Ease.InOutCubic))
                .Chain(Tween.Scale(_selectedCardUI.GetComponent<VCardUI>().transform, Vector3.zero, 0.1f,
                    Ease.InOutCubic))
                .OnComplete(() =>
                {
                    VRaisingAnimationSystem.Instance.EnqueueAnimationRequest(
                        VAnimationRequestFactory.CreateAddCardRequest(_selectedCardUI.Card), true);
                    _onComplete?.Invoke();
                    foreach (var cardUI in _cardUIs) Destroy(cardUI.gameObject);
                    _cardUIs.Clear();
                    _selectedCardUI = null;
                    VEventSystemUI.Instance.CloseSelectFrom3Menu();
                });
        }
    }
}