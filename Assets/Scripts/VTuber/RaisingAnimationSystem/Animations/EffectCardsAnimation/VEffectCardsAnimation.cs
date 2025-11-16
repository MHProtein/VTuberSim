using System;
using System.Collections.Generic;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using VTuber.ScheduleSystem.UI.RaisingAnimationSystem;

// Make sure Odin is used

namespace VTuber.RaisingAnimationSystem.Animations.EffectCardsAnimation
{
    public class VEffectCardsAnimationView : VRaisingAnimation
    {
        [SerializeField, LabelText("卡片预制体")]
        private GameObject effectCardPrefab;

        [SerializeField, LabelText("起始位置")]
        private Transform startPosition;

        [SerializeField, LabelText("结束位置")]
        private Transform endPosition;

        [SerializeField, LabelText("卡片数量")]
        private int cardCount = 3;

        [SerializeField, LabelText("卡片间距（Y方向）")]
        private float space = 50.0f;

        [SerializeField, LabelText("卡片宽度")]
        private float _cardWidth = 1400;

        [SerializeField, LabelText("卡片高度")]
        private float _cardHeight = 250;

        // Animation parameters
        [SerializeField, LabelText("移除顶部卡片时间")]
        private float removeDuration = 0.4f;

        [SerializeField, LabelText("移除顶部卡片后，移动剩余卡片间隔")]
        private float removeDelay = 0.2f;

        [SerializeField, LabelText("卡片移动时间")]
        private float moveDuration = 0.5f;

        [SerializeField, LabelText("卡片移动Ease")]
        private Ease moveEase = Ease.OutSine;

        [SerializeField, LabelText("卡片停留时间（新卡片入场后）")]
        private float cardHoldDelay = 1.0f;

        [SerializeField, LabelText("最后全部卡片移除时间")]
        private float finalRemoveDuration = 0.25f;


        private List<VEffectCard> _effectCards = new();
        private Queue<VEffectCard> _cards = new();
        private List<Vector3> _cardPositions = new();

        private int _currentCards = 0;


        protected override void Awake()
        {
            base.Awake();

            for (int i = 0; i < cardCount; i++)
            {
                float offsetIndex = i - (cardCount - 1) / 2f;
                offsetIndex = -offsetIndex;

                float y = offsetIndex * (_cardHeight + space);
                _cardPositions.Add(new Vector3(0, y, 0));

                _effectCards.Add(Instantiate(effectCardPrefab, ui.transform).GetComponent<VEffectCard>());
            }
        }


        public override void BeginAnimation(VAnimationRequest request, Action onCompleted, bool isLastSameType)
        {
            base.BeginAnimation(request, onCompleted, isLastSameType);
            
            var availableCard = GetAvailableCard();
            if (availableCard == null)
            {
                var topCard = _cards.Dequeue();

                var sequence = Sequence.Create();
                sequence
                    .Chain(Tween.LocalPosition(topCard.transform, endPosition.localPosition, removeDuration))
                    .ChainDelay(removeDelay);

                _currentCards--;

                foreach (var card in _cards)
                {
                    card.index--;
                    sequence.Chain(Tween.LocalPosition(card.transform, _cardPositions[card.index], moveDuration));
                }

                sequence.ChainCallback(() =>
                {
                    topCard.transform.position = startPosition.position;
                    topCard.SetEffect(request, debug);
                    AddNewCard(topCard, onCompleted, isLastSameType);
                });
            }
            else
            {
                availableCard.SetEffect(request, debug);
                AddNewCard(availableCard, onCompleted, isLastSameType);
            }
        }


        private void AddNewCard(VEffectCard card, Action onCompleted, bool isLastSameType)
        {
            card.isAvailable = false;
            card.index = _currentCards;
            _currentCards++;
            _cards.Enqueue(card);

            var sequence = Sequence.Create();
            sequence
                .Chain(Tween.LocalPosition(card.transform, _cardPositions[_currentCards - 1], moveDuration, moveEase))
                .ChainDelay(cardHoldDelay);

            if (isLastSameType)
            {
                foreach (var c in _cards)
                {
                    sequence.Chain(Tween.LocalPosition(c.transform, endPosition.localPosition, finalRemoveDuration));
                }
            }

            sequence.ChainCallback(()=>
            {
                onCompleted?.Invoke();
                if(isLastSameType)
                    ResetAnimation();
            });
        }


        private VEffectCard GetAvailableCard()
        {
            foreach (var effectCard in _effectCards)
            {
                if (effectCard.isAvailable)
                    return effectCard;
            }
            return null;
        }


        public override void ResetAnimation()
        {
            base.ResetAnimation();
            foreach (var card in _effectCards)
            {
                card.transform.localPosition = startPosition.localPosition;
                card.isAvailable = true;
            }
            _cards.Clear();
            _currentCards = 0;
        }
    }
}
