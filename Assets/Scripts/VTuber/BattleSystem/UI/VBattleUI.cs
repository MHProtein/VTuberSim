using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using VTuber.BattleSystem.Card;
using VTuber.BattleSystem.Core;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.UI
{
    public class VBattleUI : VUIBehaviour
    {
        [SerializeField] private Transform content;
        [SerializeField] private GameObject scrollView;
        [SerializeField] private Transform discardPileTransform;
        [SerializeField] private Transform drawPileTransform;
        
        [FormerlySerializedAs("cardSlots")] [SerializeField] private RectTransform handSlotsContent;
        
        [Space(3)]
        [Header("Animations")]
        [SerializeField] private float cardToDisposeTime = 0.2f;
        [FormerlySerializedAs("drawCardTime")] [SerializeField] private float drawCardToSlotTime = 0.2f;
        [SerializeField] private float cardMoveAfterPlayingTime = 0.2f;
        [SerializeField] private float cardApplyTime = 0.2f;
        [SerializeField] [Range(-1, 1)] private float overlap = 0.2f;
        
        private float curve = 0.0f;
        
        private bool arrangingHandSlots = false;

        private List<VCardUI> _displayingCards = new List<VCardUI>();
        private List<VHandCardUI> _handSlotsCards;
        private GameObject cardUIPrefab;
        
        public Vector2 cardSize;
        private Vector2 _scaledCardSize;
        private Vector2 _handSlotsSize;

        private VHandCardUI cardToDispose;

        public void Rearrange(int index)
        {
            if (_handSlotsCards.Count == 0)
                return;
            _handSlotsCards.RemoveAt(index);
            Rearrange();
        }
        
        public VCardUI SpawnCardUI(VCard card, Transform parent)
        {
            if (card == null)
            {
                VDebug.LogError("SpawnCardUI: Card is null");
                return null;
            }
            
            var cardUI = Instantiate(cardUIPrefab, parent).GetComponent<VCardUI>();
            cardUI.SetCard(card);
            
            return cardUI;
        }
        
        public void Selected(bool value)
        {
            foreach (var ui in _handSlotsCards)
            {
                ui.selected = value;
            }
        }
        
        private void ShowCardScroll(IEnumerable<VCard> cards)
        {
            scrollView.SetActive(true);
            foreach (var card in cards)
            {
                _displayingCards.Add(SpawnCardUI(card, content));
            }
        }
        
        public void ShowDrawPile()
        {
            ShowCardScroll(VBattle.Instance.CardPilesManager.DrawPile);
        }        
        public void ShowDiscard()
        {
            ShowCardScroll(VBattle.Instance.CardPilesManager.DiscardPile);
        }
        
        public void ShowDeck()
        {
            ShowCardScroll(VBattle.Instance.CardPilesManager.Deck);
        }

        public void ShowExaustPile()
        {
            ShowCardScroll(VBattle.Instance.CardPilesManager.ExaustPile);
        }
        public void ShowExit()
        {
            scrollView.SetActive(false);
            foreach (var card in _displayingCards)
            {
                if (card) Destroy(card.gameObject);
            }
            _displayingCards.Clear();
        }
        
        public void SkipTurn()
        {
            VBattleRootEventCenter.Instance.Raise(VRootEventKey.OnSkipTurnClicked, new Dictionary<string, object>());
        }
        
        protected override void Awake()
        {
            base.Awake();
            
            cardUIPrefab = Resources.Load<GameObject>("Prefabs/UI/Card");
            _handSlotsCards = new List<VHandCardUI>();
            _handSlotsSize = handSlotsContent.rect.size;
            cardToDispose = null;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            VBattleRootEventCenter.Instance.RegisterListener(VRootEventKey.OnDrawCards, OnDrawCards);
            VBattleRootEventCenter.Instance.RegisterListener(VRootEventKey.OnTurnEnd, OnTurnEnd);
            VBattleRootEventCenter.Instance.RegisterListener(VRootEventKey.OnCardPlayed, OnCardPlayed);
            VBattleRootEventCenter.Instance.RegisterListener(VRootEventKey.OnRedrawCards, OnRedrawCards);
            VBattleRootEventCenter.Instance.RegisterListener(VRootEventKey.OnNotifyBeginDisposeCard, OnEffectAnimationFinished);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            VBattleRootEventCenter.Instance.RemoveListener(VRootEventKey.OnDrawCards, OnDrawCards);
            VBattleRootEventCenter.Instance.RemoveListener(VRootEventKey.OnTurnEnd, OnTurnEnd);
            VBattleRootEventCenter.Instance.RemoveListener(VRootEventKey.OnCardPlayed, OnCardPlayed);
            VBattleRootEventCenter.Instance.RemoveListener(VRootEventKey.OnNotifyBeginDisposeCard, OnEffectAnimationFinished);
        }
        
        private void OnEffectAnimationFinished(Dictionary<string, object> messagedict)
        {
            if (cardToDispose is not null)
            {
                DisposeCard(cardToDispose);
                cardToDispose = null;
            }
        }

        private void OnCardPlayed(Dictionary<string, object> messagedict)
        {
            var card = messagedict["Card"] as VCard;
            var cardUI = GetCardById(card.Id);
            
            if(cardUI is null)
                return;
            
            cardUI.SetPosition(cardUI.transform.localPosition + Vector3.up * 500.0f, cardMoveAfterPlayingTime, false);
            cardToDispose = cardUI;
            
            int index = cardUI.index;
            Rearrange(index);

            foreach (var handSlotsCard in _handSlotsCards)
            {
                handSlotsCard.SetInteractive(false);
            }
            
            StartCoroutine(DelayNotifyCardMovedToPlayPosition(cardMoveAfterPlayingTime + cardApplyTime, cardUI));
        }
        
        IEnumerator DelayNotifyCardMovedToPlayPosition(float delayTime, VHandCardUI cardUI)
        {
            yield return new WaitForSeconds(delayTime);
            
            VBattleRootEventCenter.Instance.Raise(VRootEventKey.OnCardMovedToPlayPosition, new Dictionary<string, object>
            {
                {"Card", cardUI.card},
            });
        }
        
        private VHandCardUI GetCardById(int cardId)
        {
            return _handSlotsCards.FirstOrDefault(ui => ui.card.Id == cardId);
        }

        private void OnRedrawCards(Dictionary<string, object> messagedict)
        {
            RedrawCards(messagedict["ShouldPlayTwice"] as bool? ?? false);
        }

        private void RedrawCards(bool shouldPlayTwice)
        {
            
            int redrawCount = _handSlotsCards.Count;
            DisposeAllCards();
            StartCoroutine(DelayDrawCards(cardToDisposeTime, redrawCount, shouldPlayTwice));
        }
        
        IEnumerator DelayDrawCards(float delayTime, int drawCount, bool shouldPlayTwice)
        {
            yield return new WaitForSeconds(delayTime);
            
            VBattleRootEventCenter.Instance.Raise(VRootEventKey.OnRequestDrawCards, new Dictionary<string, object>
            {
                {"DrawCount", drawCount}
            });
            if (shouldPlayTwice)
            {
                StartCoroutine(DelayPlayTwiceDrawCards(drawCardToSlotTime));
            }
            else
            {
                DisposeCard(cardToDispose);
                cardToDispose = null;
            }
        }

        IEnumerator DelayPlayTwiceDrawCards(float delayTime)
        {
            yield return new WaitForSeconds(delayTime);
            
            RedrawCards(false);
        }

        public void DisposeAllCards()
        {
            for (int i = _handSlotsCards.Count - 1; i >= 0; i--)
            {
                DisposeCard(_handSlotsCards[i], false);
            }
            _handSlotsCards.Clear();
        }
        
        private void DisposeCard(VHandCardUI cardUI, bool isUsed = true)
        {
            if(cardUI is null)
                return;
            
            VBattleRootEventCenter.Instance.Raise(VRootEventKey.OnCardBeginDisposal, new Dictionary<string, object>
            {
                {"Card", cardUI.card}
            });
            
            cardUI.MoveToDiscardPile(discardPileTransform.position, cardToDisposeTime);
            
            StartCoroutine(DelayNotifyCardDisposed(cardToDisposeTime, cardUI, isUsed));
        }
        
        IEnumerator DelayNotifyCardDisposed(float delayTime, VHandCardUI cardUI, bool isUsed)
        {
            yield return new WaitForSeconds(delayTime);
            
            foreach (var handSlotsCard in _handSlotsCards)
            {
                handSlotsCard.SetInteractive(true);
            }
            
            VBattleRootEventCenter.Instance.Raise(VRootEventKey.OnCardDisposed, new Dictionary<string, object>
            {
                {"Card", cardUI.card}
            });

            if (isUsed)
            {
                VBattleRootEventCenter.Instance.Raise(VRootEventKey.OnCardUsed, new Dictionary<string, object>
                {
                    {"Card", cardUI.card}
                });
            }
        }
        
        private void OnTurnEnd(Dictionary<string, object> messagedict)
        {
            DisposeAllCards();
            
            _handSlotsCards.Clear();
            
            VBattleRootEventCenter.Instance.Raise(VRootEventKey.OnNotifyTurnBeginDelay, new Dictionary<string, object>
            {
                {"DelaySeconds", cardToDisposeTime},
            });
            
        }
        
        private void OnDrawCards(Dictionary<string, object> messageDict)
        {
            List<VCard> cards = messageDict["Cards"] as List<VCard>;
            if (cards == null)
                return;
            
            StartCoroutine(DrawCard(cards));
        }

        private IEnumerator DrawCard(IEnumerable<VCard> cards)
        {
            arrangingHandSlots = true;
            foreach (var card in cards)
            {
                var cardUI = Instantiate(cardUIPrefab, drawPileTransform.position, Quaternion.identity, null).GetComponent<VCardUI>();
                cardUI.SetCard(card);
                cardUI.transform.localScale = Vector3.zero;
                cardUI.transform.SetParent(handSlotsContent);
                
                var handCardUI = cardUI.AddComponent<VHandCardUI>();
                (Vector3 position, Vector3 rotation, Vector3 scale) = ReserveSpaceForNewCard();
                handCardUI.index = _handSlotsCards.Count;
                handCardUI.battleUI = this;
                handCardUI.card = card;
                card.SetPlayable = handCardUI.SetCardPlayble;
                handCardUI.cardUI = cardUI;
                handCardUI.ToHandSlot(position, rotation, Vector3.one, drawCardToSlotTime);
                SetHandCardPositionRotation(handCardUI, position.x);
                handCardUI.SetScale(scale, drawCardToSlotTime, true);
                _handSlotsCards.Add(handCardUI);
                
                yield return new WaitForSeconds(drawCardToSlotTime);
                
                VBattleRootEventCenter.Instance.Raise(VRootEventKey.OnCardMovedToHandSlot,
                    new Dictionary<string, object>()
                    {
                        { "Card", card }
                    });
            }
            arrangingHandSlots = false;
        }
        
        private (Vector3 position, Vector3 rotation, Vector3 scale) ReserveSpaceForNewCard()
        {
            if (_handSlotsCards.Count == 0)
                return (Vector3.zero, Vector3.zero, Vector3.one);

            Vector3 position;
            Vector3 rotation;

            float scale = _handSlotsSize.x / (_handSlotsCards.Count + 1) / cardSize.x;
            if (scale < 1)
            {
                foreach (var card in _handSlotsCards)
                {
                    Vector3 target = new Vector3(scale, scale, 1.0f);
                    card.SetScale(target, drawCardToSlotTime, true);
                }
            }
            else
            {
                scale = 1.0f;
                foreach (var card in _handSlotsCards)
                {
                    card.SetScale(Vector3.one, drawCardToSlotTime, true);
                }
            }
            
            _scaledCardSize.x = cardSize.x * (scale * (1.0f - overlap));
            _scaledCardSize.y = cardSize.y * scale;
            
            if (_handSlotsCards.Count == 1)
            {
                _handSlotsCards[0].SetScale(Vector3.one, drawCardToSlotTime, true);
                SetHandCardPositionRotation(_handSlotsCards[0], -(cardSize.x / 2.0f));
                return (new Vector3(_scaledCardSize.x / 2.0f, 0.0f, 0.0f),
                    new Vector3(0.0f, 0.0f, (curve / 2.0f)),  Vector3.one);
            }
            
            if ((_handSlotsCards.Count + 1) % 2 == 0) //even with the next card
            {
                int medium2 = (_handSlotsCards.Count + 1) / 2;
                int medium1= medium2 - 1;

                float medium1X = -(_scaledCardSize.x / 2.0f);
                float medium2X = -medium1X;
                
                for(int before = 0; before != medium1; ++before)
                {
                    SetHandCardPositionRotation(_handSlotsCards[before], medium1X - _scaledCardSize.x * (medium1 - before));
                }
                
                SetHandCardPositionRotation(_handSlotsCards[medium1], medium1X);
                SetHandCardPositionRotation(_handSlotsCards[medium2], medium2X);
                
                int after;
                for(after = medium2 + 1; after != _handSlotsCards.Count; ++after)
                {
                    SetHandCardPositionRotation(_handSlotsCards[after], medium2X + _scaledCardSize.x * (after - medium2));
                }

                position = new Vector3(medium2X + _scaledCardSize.x * (after - medium2), 0.0f, 0.0f);
                rotation = new Vector3(0.0f, 0.0f, -curve * (after - medium2));
            }
            else //odd with the next card
            {
                int medium = (_handSlotsCards.Count + 1) / 2;
                
                for(int before = 0; before != medium; ++before)
                {
                    SetHandCardPositionRotation(_handSlotsCards[before], -_scaledCardSize.x * (medium - before));
                }
                
                SetHandCardPositionRotation(_handSlotsCards[medium], 0.0f);
                
                int after;
                for(after = medium + 1; after != _handSlotsCards.Count; ++after)
                {
                    SetHandCardPositionRotation(_handSlotsCards[after], _scaledCardSize.x * (after - medium));
                }

                position = new Vector3(_scaledCardSize.x * (after - medium), 0.0f, 0.0f);
                rotation = new Vector3(0.0f, 0.0f, -curve * (after - medium));
            }
            
            return (position, rotation, new Vector3(scale, scale, 1.0f));
        }
        
        private void Rearrange()
        {
            if(_handSlotsCards.Count == 0 )
                return;
            
            arrangingHandSlots = true;
            
            for (int i = 0; i != _handSlotsCards.Count; ++i)
            {
                _handSlotsCards[i].index = i;
            }
            float scale = _handSlotsSize.x / (_handSlotsCards.Count * cardSize.x);
            if (scale < 1.0f)
            {
                foreach (var card in _handSlotsCards)
                {
                    Vector3 target = new Vector3(scale, scale, 1.0f);
                    card.SetScale(target, drawCardToSlotTime, true);
                }
            }
            else
            {
                scale = 1.0f;
                foreach (var card in _handSlotsCards)
                {
                    card.SetScale(Vector3.one, drawCardToSlotTime, true);
                }
            }
            
            _scaledCardSize.x = cardSize.x * (scale * (1.0f - overlap));
            _scaledCardSize.y = cardSize.y * scale;


            if (_handSlotsCards.Count == 1)
            {
                _handSlotsCards[0].SetScale(Vector3.one, drawCardToSlotTime, true);
                SetHandCardPositionRotation(_handSlotsCards[0], 0.0f);
            }
            
            if (_handSlotsCards.Count == 2)
            {
                _handSlotsCards[0].SetScale(Vector3.one, drawCardToSlotTime, true);
                SetHandCardPositionRotation(_handSlotsCards[0], -(cardSize.x / 2.0f));
                
                _handSlotsCards[1].SetScale(Vector3.one, drawCardToSlotTime, true);
                SetHandCardPositionRotation(_handSlotsCards[1], cardSize.x / 2.0f);
            }
            
            if (_handSlotsCards.Count % 2 == 0)
            {
                int medium2 = _handSlotsCards.Count / 2;
                int medium1= medium2 - 1;
                float medium1X = -(_scaledCardSize.x / 2.0f);
                float medium2X = -medium1X;
                
                for(int before = 0; before != medium1; ++before)
                {
                    SetHandCardPositionRotation(_handSlotsCards[before], medium1X - _scaledCardSize.x * (medium1 - before));
                }
                
                SetHandCardPositionRotation(_handSlotsCards[medium1], medium1X);
                SetHandCardPositionRotation(_handSlotsCards[medium2], medium2X);
                
                for(int after = medium2 + 1; after != _handSlotsCards.Count; ++after)
                {
                    SetHandCardPositionRotation(_handSlotsCards[after], medium2X + _scaledCardSize.x * (after - medium2));
                }
            }
            else
            {
                int medium = _handSlotsCards.Count / 2;
                
                for(int before = 0; before != medium; ++before)
                {
                    SetHandCardPositionRotation(_handSlotsCards[before], -_scaledCardSize.x * (medium - before));
                }
                
                SetHandCardPositionRotation(_handSlotsCards[medium], 0.0f);
                
                int after;
                for(after = medium + 1; after != _handSlotsCards.Count; ++after)
                {
                    SetHandCardPositionRotation(_handSlotsCards[after], _scaledCardSize.x * (after - medium));
                }
            }
            arrangingHandSlots = false;
        }
        
        private void SetHandCardPositionRotation(VHandCardUI ui, float offset)
        {
            ui.SetPosition(new Vector3(offset, 0.0f, 0.0f), drawCardToSlotTime, true);
        }
        
        public void UnselectCurrent()
        {
            foreach (var cardUI in _handSlotsCards)
            {
                if (cardUI.selfSelected)
                {
                    cardUI.Deselect();
                }
            }
        }
    }
}