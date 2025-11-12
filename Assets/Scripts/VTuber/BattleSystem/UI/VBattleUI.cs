using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PrimeTween;
using Tutorial.Script;
using Tutorial.UI;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VTuber.BattleSystem.Card;
using VTuber.BattleSystem.Core;
using VTuber.BattleSystem.Effect;
using VTuber.Core.Foundation;
using VTuber.Core.Managers;
using VTuber.Core.SE;
using VTuber.Dialogue.UI;

namespace VTuber.BattleSystem.UI
{
    public class VBattleUI : VUIBehaviour
    {
        [SerializeField] private GameObject battleRoot;
        [SerializeField] private Transform cardPileContent;

        [FormerlySerializedAs("scrollView")] [SerializeField]
        private GameObject cardPileScrollView;

        [SerializeField] private Transform discardPileTransform;
        [SerializeField] private Transform drawPileTransform;

        [FormerlySerializedAs("cardSlots")] [SerializeField]
        private RectTransform handSlotsContent;

        [Space(3)] [Header("PickCard Menu")] [SerializeField]
        private GameObject pickCardMenuScroll;

        [SerializeField] private Transform pickCardContent;

        [Space(3)] [Header("Animations")] [SerializeField]
        private float cardToDisposeTime = 0.2f;

        [FormerlySerializedAs("drawCardTime")] [SerializeField]
        private float drawCardToSlotTime = 0.2f;

        [SerializeField] private float cardMoveAfterPlayingTime = 0.2f;
        [SerializeField] private float cardApplyTime = 0.2f;
        [SerializeField] [Range(-1, 1)] private float overlap = 0.2f;

        [Space(3)] [Header("CardPileButtons")] [SerializeField]
        private Button drawCardPileButton;

        [SerializeField] private Button discardCardPileButton;
        [SerializeField] private Button exaustCardPileButton;
        [SerializeField] private Button deckCardPileButton;
        [SerializeField] private Button pickCardButton;

        [SerializeField] private RectTransform battleUIWrapper;
        [SerializeField] private GameObject battlePausePanel;

        [Space(3)] [SerializeField] private Button skipTurnButton;

        [SerializeField] private GameObject cardUIPrefab;
        [SerializeField] private VTips tipUI;

        public Vector2 cardSize;

        private readonly float curve = 0.0f;

        private List<VCardUI> _displayingCards = new();

        private Coroutine _drawCardCoroutine;
        private List<VHandCardUI> _handSlotsCards;
        private Vector2 _handSlotsSize;
        private VPickCardMenu _pickCardMenu;
        private Vector2 _scaledCardSize;

        private bool arrangingHandSlots;

        private VHandCardUI cardToDispose;
        private bool disposingAll;
        public bool IsCardApplying { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            _handSlotsCards = new List<VHandCardUI>();
            _handSlotsSize = handSlotsContent.rect.size;
            cardToDispose = null;
            _pickCardMenu = pickCardMenuScroll.GetComponent<VPickCardMenu>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnBattleBegin, OnBattleBegin);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnBattlePause, OnBattlePause);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnBattleEnd, OnBattleEnd);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnDrawCards, OnDrawCards);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnTurnEnd, OnTurnEnd);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnCardPlayed, OnCardPlayed);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnRedrawCards, OnRedrawCards);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnBattleUIInitialize, OnBattleUIInitialize);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnNotifyBeginDisposeCard,
                OnCardBeginDispose);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnCardsPickedFromPile,
                OnCardsPickedFromPile);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnBeginPickCardsFromPile,
                OnBeginPickCardsFromPile);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnBattleBegin, OnBattleBegin);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnBattlePause, OnBattlePause);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnBattleEnd, OnBattleEnd);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnDrawCards, OnDrawCards);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnTurnEnd, OnTurnEnd);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnCardPlayed, OnCardPlayed);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnRedrawCards, OnRedrawCards);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnBattleUIInitialize, OnBattleUIInitialize);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnNotifyBeginDisposeCard,
                OnCardBeginDispose);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnCardsPickedFromPile,
                OnCardsPickedFromPile);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnBeginPickCardsFromPile,
                OnBeginPickCardsFromPile);
        }

        private void OnBattleUIInitialize(Dictionary<string, object> messagedict)
        {
            SetTips(messagedict["TipConfig"] as VTipConfig);
        }

        public void SetTips(VTipConfig tipConfig)
        {
            if (tipConfig is null)
            {
                tipUI.gameObject.SetActive(false);
                return;
            }
            
            tipUI.gameObject.SetActive(true);
            tipUI.SetTips(tipConfig.title, tipConfig.description, tipConfig.image);
        }

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
            foreach (var ui in _handSlotsCards) ui.selected = value;
        }

        public void PickCardDebug()
        {
            ShowPickCardMenu(VCardPileType.ALL, 3, false, false);
        }

        public void ShowPickCardMenu(VCardPileType cardPileType, int count, bool isFromCard, bool shouldPlayTwice)
        {
            List<VCard> cards;

            switch (cardPileType)
            {
                case VCardPileType.DrawPile:
                    cards = VBattle.Instance.CardPilesManager.DrawPile.ToList();
                    break;
                case VCardPileType.Discard:
                    cards = VBattle.Instance.CardPilesManager.DiscardPile.ToList();
                    break;
                case VCardPileType.Exhaust:
                    cards = VBattle.Instance.CardPilesManager.ExhaustPile.ToList();
                    break;
                case VCardPileType.Deck:
                    cards = VBattle.Instance.CardPilesManager.Deck.ToList();
                    break;
                case VCardPileType.ALL:
                    cards = VDataManager.Instance.GetAllCardConfigurations().Select(card => card.CreateCard())
                        .ToList();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(cardPileType), cardPileType, null);
            }

            var cardUIs = ShowCardScroll(cards, pickCardContent);
            _displayingCards = cardUIs;
            pickCardMenuScroll.SetActive(true);
            _pickCardMenu.BeginPickCard(cardUIs, count, cardPileType, isFromCard, shouldPlayTwice);
        }

        private List<VCardUI> ShowCardScroll(IEnumerable<VCard> cards, Transform content)
        {
            var cardUIs = new List<VCardUI>();
            foreach (var card in cards) cardUIs.Add(SpawnCardUI(card, content));

            return cardUIs;
        }

        public void ShowDrawPile()
        {
            cardPileScrollView.SetActive(true);
            _displayingCards = ShowCardScroll(VBattle.Instance.CardPilesManager.DrawPile, cardPileContent);
        }

        public void ShowDiscard()
        {
            cardPileScrollView.SetActive(true);
            _displayingCards = ShowCardScroll(VBattle.Instance.CardPilesManager.DiscardPile, cardPileContent);
        }

        public void ShowDeck()
        {
            cardPileScrollView.SetActive(true);
            _displayingCards = ShowCardScroll(VBattle.Instance.CardPilesManager.Deck, cardPileContent);
        }

        public void ShowExhaustPile()
        {
            cardPileScrollView.SetActive(true);
            _displayingCards = ShowCardScroll(VBattle.Instance.CardPilesManager.ExhaustPile, cardPileContent);
        }

        public void ShowExit()
        {
            cardPileScrollView.SetActive(false);
            foreach (var card in _displayingCards)
                if (card)
                    Destroy(card.gameObject);
            _displayingCards.Clear();
        }

        public void SkipTurn()
        {
            VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnSkipTurnClicked, new Dictionary<string, object>());
            skipTurnButton.interactable = false;
        }

        public Tween SetBattlePause(bool paused)
        {
            return VEventSystemUI.Instance.SetFullScreen();
        }

        private void OnBattleEnd(Dictionary<string, object> messagedict)
        {
            //SetFullScreen(0.75f).OnComplete(() => battleRoot.SetActive(false));
        }

        private void OnBattlePause(Dictionary<string, object> messagedict)
        {
            SetBattlePause((bool)messagedict["Paused"]);
        }

        private void OnBattleBegin(Dictionary<string, object> messagedict)
        {
            disposingAll = false;
            foreach (var card in _handSlotsCards) Destroy(card.gameObject);
            _handSlotsCards.Clear();
        }

        private void OnBeginPickCardsFromPile(Dictionary<string, object> messagedict)
        {
            ShowPickCardMenu((VCardPileType)messagedict["CardPileType"], (int)messagedict["CardCount"],
                (bool)messagedict["IsFromCard"], (bool)messagedict["ShouldPlayTwice"]);
        }

        private void OnCardsPickedFromPile(Dictionary<string, object> messagedict)
        {
            pickCardMenuScroll.SetActive(false);
            var cards = messagedict["PickedCards"] as List<VCard>;
            _drawCardCoroutine = StartCoroutine(DrawCard(cards,
                (bool)messagedict["IsFromCard"],
                (bool)messagedict["ShouldPlayTwice"]));
        }

        private void OnCardBeginDispose(Dictionary<string, object> messagedict)
        {
            if (cardToDispose is not null)
            {
                IsCardApplying = false;
                DisposeCard(cardToDispose);
                cardToDispose = null;

                foreach (var card in _handSlotsCards) card.OnCardStopApplying();
            }
        }

        private void OnCardPlayed(Dictionary<string, object> messagedict)
        {
            SetSkipTurnButtonInteractable(false);
            IsCardApplying = true;
            foreach (var handCardUI in _handSlotsCards) handCardUI.SetInteractive(false);
            var card = messagedict["Card"] as VCard;
            var cardUI = GetCardById(card.Id);

            if (cardUI is null)
                return;

            cardUI.SetPosition(cardUI.transform.localPosition + Vector3.up * 500.0f, cardMoveAfterPlayingTime, false);
            cardToDispose = cardUI;

            var index = cardUI.index;
            Rearrange(index);

            StartCoroutine(DelayNotifyCardMovedToPlayPosition(cardMoveAfterPlayingTime + cardApplyTime, cardUI));
        }

        private IEnumerator DelayNotifyCardMovedToPlayPosition(float delayTime, VHandCardUI cardUI)
        {
            yield return new WaitForSeconds(delayTime);

            VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnCardMovedToPlayPosition,
                new Dictionary<string, object>
                {
                    { "Card", cardUI.card }
                });
        }

        public void SetSkipTurnButtonInteractable(bool interactable)
        {
            skipTurnButton.interactable = interactable;
        }

        private VHandCardUI GetCardById(uint cardId)
        {
            return _handSlotsCards.FirstOrDefault(ui => ui.card.Id == cardId);
        }

        private void OnRedrawCards(Dictionary<string, object> messagedict)
        {
            RedrawCards(messagedict["ShouldPlayTwice"] as bool? ?? false);
        }

        private void RedrawCards(bool shouldPlayTwice)
        {
            var redrawCount = _handSlotsCards.Count;
            DisposeAllCards();
            StartCoroutine(DelayDrawCards(cardToDisposeTime, redrawCount, shouldPlayTwice));
        }

        private IEnumerator DelayDrawCards(float delayTime, int drawCount, bool shouldPlayTwice)
        {
            yield return new WaitForSeconds(delayTime);

            VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnRequestDrawCards, new Dictionary<string, object>
            {
                { "DrawCount", drawCount },
                { "IsFromCard", false },
                { "ShouldPlayTwice", shouldPlayTwice }
            });
            if (shouldPlayTwice)
            {
                StartCoroutine(DelayPlayTwiceDrawCards(drawCardToSlotTime));
            }
            else
            {
                DisposeCard(cardToDispose);
                IsCardApplying = false;
                cardToDispose = null;
            }
        }

        private IEnumerator DelayPlayTwiceDrawCards(float delayTime)
        {
            yield return new WaitForSeconds(delayTime);

            RedrawCards(false);
        }

        public void DisposeAllCards()
        {
            disposingAll = true;
            SetSkipTurnButtonInteractable(false);
            for (var i = _handSlotsCards.Count - 1; i >= 0; i--) DisposeCard(_handSlotsCards[i], false);
            _handSlotsCards.Clear();
        }

        private void DisposeCard(VHandCardUI cardUI, bool isUsed = true)
        {
            if (cardUI is null)
                return;

            VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnCardBeginDisposal, new Dictionary<string, object>
            {
                { "Card", cardUI.card }
            });

            cardUI.MoveToDiscardPile(discardPileTransform.position, cardToDisposeTime);

            StartCoroutine(DelayNotifyCardDisposed(cardToDisposeTime, cardUI, isUsed));
        }

        private IEnumerator DelayNotifyCardDisposed(float delayTime, VHandCardUI cardUI, bool isUsed)
        {
            if (isUsed)
                SetSkipTurnButtonInteractable(false);
            yield return new WaitForSeconds(delayTime);

            foreach (var handSlotsCard in _handSlotsCards) handSlotsCard.SetInteractive(true);

            VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnCardDisposed, new Dictionary<string, object>
            {
                { "Card", cardUI.card },
                { "IsUsed", isUsed }
            });

            if (isUsed)
            {
                VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnCardUsed, new Dictionary<string, object>
                {
                    { "Card", cardUI.card }
                });
                if (!disposingAll)
                    SetSkipTurnButtonInteractable(true);
            }
        }

        private void OnTurnEnd(Dictionary<string, object> messagedict)
        {
            DisposeAllCards();

            _handSlotsCards.Clear();

            VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnNotifyTurnBeginDelay, new Dictionary<string, object>
            {
                { "DelaySeconds", cardToDisposeTime }
            });
        }

        private void OnDrawCards(Dictionary<string, object> messageDict)
        {
            var cards = messageDict["Cards"] as List<VCard>;
            if (cards == null)
                return;
            _drawCardCoroutine = StartCoroutine
            (DrawCard(cards, (bool)messageDict["IsFromCard"],
                (bool)messageDict["ShouldPlayTwice"]));
        }

        private IEnumerator DrawCard(IEnumerable<VCard> cards, bool isFromCard, bool shouldPlayTwice)
        {
            disposingAll = false;
            SetSkipTurnButtonInteractable(false);
            arrangingHandSlots = true;
            foreach (var card in cards)
            {
                var cardUI = Instantiate(cardUIPrefab, drawPileTransform.position, Quaternion.identity, null)
                    .GetComponent<VCardUI>();
                cardUI.SetCard(card);
                cardUI.transform.localScale = Vector3.zero;
                cardUI.transform.SetParent(handSlotsContent);
                
                var handCardUI = cardUI.gameObject.AddComponent<VHandCardUI>();
                var (position, rotation, scale) = ReserveSpaceForNewCard();
                handCardUI.index = _handSlotsCards.Count;
                handCardUI.battleUI = this;
                handCardUI.card = card;
                card.setPlayable = handCardUI.SetCardPlayable;
                card.popularityPreviewAction = handCardUI.SetPopularityPreview;
                card.shieldPreviewAction = handCardUI.SetShieldPreview;
                handCardUI.cardUI = cardUI;
                handCardUI.ToHandSlot(position, rotation, Vector3.one, drawCardToSlotTime);
                SetHandCardPositionRotation(handCardUI, position.x);
                handCardUI.SetScale(scale, drawCardToSlotTime, true);
                _handSlotsCards.Add(handCardUI);

                yield return new WaitForSeconds(drawCardToSlotTime);

                VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnCardMovedToHandSlot,
                    new Dictionary<string, object>
                    {
                        { "Card", card }
                    });
            }

            arrangingHandSlots = false;

            if (shouldPlayTwice)
                VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnPlayTheSecondTime,
                    new Dictionary<string, object>());
            else if (isFromCard)
                VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnNotifyBeginDisposeCard,
                    new Dictionary<string, object>());

            SetSkipTurnButtonInteractable(true);
        }

        private (Vector3 position, Vector3 rotation, Vector3 scale) ReserveSpaceForNewCard()
        {
            if (_handSlotsCards.Count == 0)
                return (Vector3.zero, Vector3.zero, Vector3.one);

            Vector3 position;
            Vector3 rotation;

            var scale = _handSlotsSize.x / (_handSlotsCards.Count + 1) / cardSize.x;
            if (scale < 1)
            {
                foreach (var card in _handSlotsCards)
                {
                    var target = new Vector3(scale, scale, 1.0f);
                    card.SetScale(target, drawCardToSlotTime, true);
                }
            }
            else
            {
                scale = 1.0f;
                foreach (var card in _handSlotsCards) card.SetScale(Vector3.one, drawCardToSlotTime, true);
            }

            _scaledCardSize.x = cardSize.x * (scale * (1.0f - overlap));
            _scaledCardSize.y = cardSize.y * scale;

            if (_handSlotsCards.Count == 1)
            {
                _handSlotsCards[0].SetScale(Vector3.one, drawCardToSlotTime, true);
                SetHandCardPositionRotation(_handSlotsCards[0], -(cardSize.x / 2.0f));
                return (new Vector3(_scaledCardSize.x / 2.0f, 0.0f, 0.0f),
                    new Vector3(0.0f, 0.0f, curve / 2.0f), Vector3.one);
            }

            if ((_handSlotsCards.Count + 1) % 2 == 0) //even with the next card
            {
                var medium2 = (_handSlotsCards.Count + 1) / 2;
                var medium1 = medium2 - 1;

                var medium1X = -(_scaledCardSize.x / 2.0f);
                var medium2X = -medium1X;

                for (var before = 0; before != medium1; ++before)
                    SetHandCardPositionRotation(_handSlotsCards[before],
                        medium1X - _scaledCardSize.x * (medium1 - before));

                SetHandCardPositionRotation(_handSlotsCards[medium1], medium1X);
                SetHandCardPositionRotation(_handSlotsCards[medium2], medium2X);

                int after;
                for (after = medium2 + 1; after != _handSlotsCards.Count; ++after)
                    SetHandCardPositionRotation(_handSlotsCards[after],
                        medium2X + _scaledCardSize.x * (after - medium2));

                position = new Vector3(medium2X + _scaledCardSize.x * (after - medium2), 0.0f, 0.0f);
                rotation = new Vector3(0.0f, 0.0f, -curve * (after - medium2));
            }
            else //odd with the next card
            {
                var medium = (_handSlotsCards.Count + 1) / 2;

                for (var before = 0; before != medium; ++before)
                    SetHandCardPositionRotation(_handSlotsCards[before], -_scaledCardSize.x * (medium - before));

                SetHandCardPositionRotation(_handSlotsCards[medium], 0.0f);

                int after;
                for (after = medium + 1; after != _handSlotsCards.Count; ++after)
                    SetHandCardPositionRotation(_handSlotsCards[after], _scaledCardSize.x * (after - medium));

                position = new Vector3(_scaledCardSize.x * (after - medium), 0.0f, 0.0f);
                rotation = new Vector3(0.0f, 0.0f, -curve * (after - medium));
            }

            return (position, rotation, new Vector3(scale, scale, 1.0f));
        }

        private void Rearrange()
        {
            if (_handSlotsCards.Count == 0)
                return;

            arrangingHandSlots = true;

            for (var i = 0; i != _handSlotsCards.Count; ++i) _handSlotsCards[i].index = i;
            var scale = _handSlotsSize.x / (_handSlotsCards.Count * cardSize.x);
            if (scale < 1.0f)
            {
                foreach (var card in _handSlotsCards)
                {
                    var target = new Vector3(scale, scale, 1.0f);
                    card.SetScale(target, drawCardToSlotTime, true);
                }
            }
            else
            {
                scale = 1.0f;
                foreach (var card in _handSlotsCards) card.SetScale(Vector3.one, drawCardToSlotTime, true);
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
                var medium2 = _handSlotsCards.Count / 2;
                var medium1 = medium2 - 1;
                var medium1X = -(_scaledCardSize.x / 2.0f);
                var medium2X = -medium1X;

                for (var before = 0; before != medium1; ++before)
                    SetHandCardPositionRotation(_handSlotsCards[before],
                        medium1X - _scaledCardSize.x * (medium1 - before));

                SetHandCardPositionRotation(_handSlotsCards[medium1], medium1X);
                SetHandCardPositionRotation(_handSlotsCards[medium2], medium2X);

                for (var after = medium2 + 1; after != _handSlotsCards.Count; ++after)
                    SetHandCardPositionRotation(_handSlotsCards[after],
                        medium2X + _scaledCardSize.x * (after - medium2));
            }
            else
            {
                var medium = _handSlotsCards.Count / 2;

                for (var before = 0; before != medium; ++before)
                    SetHandCardPositionRotation(_handSlotsCards[before], -_scaledCardSize.x * (medium - before));

                SetHandCardPositionRotation(_handSlotsCards[medium], 0.0f);

                int after;
                for (after = medium + 1; after != _handSlotsCards.Count; ++after)
                    SetHandCardPositionRotation(_handSlotsCards[after], _scaledCardSize.x * (after - medium));
            }

            arrangingHandSlots = false;
        }

        private void SetHandCardPositionRotation(VHandCardUI ui, float offset)
        {
            ui.SetInteractive(false);
            ui.SetPosition(new Vector3(offset, 0.0f, 0.0f), drawCardToSlotTime, true,
                () => ui.SetInteractive(!IsCardApplying));
        }

        public void UnselectCurrent()
        {
            foreach (var cardUI in _handSlotsCards)
                if (cardUI.selfSelected)
                    cardUI.Deselect();
        }

        public void PlayCardSelectedSFX()
        {
            VAudioPlayer.Instance.PlayStaticSFX(VSFXType.Selection);
        }

        public void PlayCardPlayedSFX()
        {
            VAudioPlayer.Instance.PlayStaticSFX(VSFXType.Battle_CardPlayed);
        }
    }
}