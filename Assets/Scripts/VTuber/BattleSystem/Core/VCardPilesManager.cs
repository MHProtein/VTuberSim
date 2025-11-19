using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VTuber.BattleSystem.Card;
using VTuber.BattleSystem.Effect;
using VTuber.Character;
using VTuber.Core.Foundation;
using VTuber.Core.Managers;
using Random = UnityEngine.Random;

namespace VTuber.BattleSystem.Core
{
    public class VCardPilesManagerSaveData
    {
        public List<uint> Deck;
        public List<uint> DiscardPile;
        public List<uint> DrawPile;
        public List<uint> ExhaustPile;
        public List<uint> HandPile;
        public bool isTutorial;
        public Dictionary<int, List<uint>> tutorialTurnHandCards;
        public bool isFirstTurn;
    }

    public class VCardPilesManager
    {
        private readonly int _handSize;
        private readonly int _maxHandSize;

        private bool _isFirstTurn;
        private bool _isLoad;
        private bool _isTutorial;
        private Dictionary<int, List<uint>> _tutorialTurnHandCards;

        public VCardPilesManager(int handSize, int maxHandSize, VCardLibrary cardLibrary, List<uint> tutorialDeck,
            Dictionary<int, List<uint>> tutorialTurnHandCards, VCardPilesManagerSaveData saveData)
        {
            _handSize = handSize;
            _maxHandSize = maxHandSize;

            Deck = new List<VCard>();
            DrawPile = new List<VCard>();
            DiscardPile = new List<VCard>();
            HandPile = new List<VCard>();
            ExhaustPile = new List<VCard>();

            if (tutorialDeck is not null)
            {
                Deck.AddRange(tutorialDeck.Select(VDataManager.Instance.CreateCardByID));
                DrawPile.AddRange(Deck);
                _isTutorial = true;
                _tutorialTurnHandCards = tutorialTurnHandCards;
                return;
            }

            if (saveData is not null)
            {
                Load(cardLibrary, saveData);
                return;
            }

            _isTutorial = false;

            Deck.AddRange(cardLibrary.GetCards());
            DrawPile.AddRange(Deck);
            _isFirstTurn = true;
        }

        public List<VCard> Deck { get; } = new();

        public List<VCard> DrawPile { get; } = new();

        public List<VCard> DiscardPile { get; } = new();

        public List<VCard> HandPile { get; } = new();

        public List<VCard> ExhaustPile { get; } = new();

        public VCardPilesManagerSaveData Save()
        {
            if (_isTutorial)
                return new VCardPilesManagerSaveData
                {
                    isTutorial = _isTutorial,
                    isFirstTurn = _isFirstTurn,
                    Deck = new List<uint>(Deck.Select(card => card.configID).ToList()) ,
                    DrawPile = new List<uint>(DrawPile.Select(card => card.configID).ToList()),
                    DiscardPile = new List<uint>(DiscardPile.Select(card => card.configID).ToList()),
                    HandPile = new List<uint>(HandPile.Select(card => card.configID).ToList()),
                    ExhaustPile = new List<uint>(ExhaustPile.Select(card => card.configID).ToList()),
                    tutorialTurnHandCards = _tutorialTurnHandCards
                };
            return new VCardPilesManagerSaveData
            {
                isTutorial = _isTutorial,
                isFirstTurn = _isFirstTurn,
                Deck = new List<uint>(Deck.Select(card => card.Id).ToList()) ,
                DrawPile = new List<uint>(DrawPile.Select(card => card.Id).ToList()),
                DiscardPile = new List<uint>(DiscardPile.Select(card => card.Id).ToList()),
                HandPile = new List<uint>(HandPile.Select(card => card.Id).ToList()),
                ExhaustPile = new List<uint>(ExhaustPile.Select(card => card.Id).ToList()),
            };
        }

        private void Load(VCardLibrary cardLibrary, VCardPilesManagerSaveData saveData)
        {
            Clear();
            _isLoad = true;
            _isTutorial = saveData.isTutorial;
            _isFirstTurn = saveData.isFirstTurn;
            if (_isTutorial)
            {
                Deck.AddRange(saveData.Deck.Select(VDataManager.Instance.CreateCardByID));
                DrawPile.AddRange(saveData.DrawPile.Select(VDataManager.Instance.CreateCardByID));
                DiscardPile.AddRange(saveData.DiscardPile.Select(VDataManager.Instance.CreateCardByID));
                HandPile.AddRange(saveData.HandPile.Select(VDataManager.Instance.CreateCardByID));
                ExhaustPile.AddRange(saveData.ExhaustPile.Select(VDataManager.Instance.CreateCardByID));
                _tutorialTurnHandCards = saveData.tutorialTurnHandCards;
                return;
            }

            Deck.AddRange(saveData.Deck.Select(cardLibrary.GetCardByID));
            DrawPile.AddRange(saveData.DrawPile.Select(cardLibrary.GetCardByID));
            DiscardPile.AddRange(saveData.DiscardPile.Select(cardLibrary.GetCardByID));
            HandPile.AddRange(saveData.HandPile.Select(cardLibrary.GetCardByID));
            ExhaustPile.AddRange(saveData.ExhaustPile.Select(cardLibrary.GetCardByID));
        }

        public VCard GetCardByIDFromDeck(uint id)
        {
            return Deck.Find(card => card.configID == id);
        }

        public void OnEnable()
        {
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnTurnBegin, OnTurnBegin);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnRequestDrawCards, OnRequestDrawCards);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnCardDisposed, OnCardDisposed);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnCardPlayed, OnRemoveCardFromHandPile);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnCardBeginDisposal,
                OnRemoveCardFromHandPile);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnCardsPickedFromPile,
                OnCardsPickedFromPile);
        }

        public void OnDisable()
        {
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnTurnBegin, OnTurnBegin);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnRequestDrawCards, OnRequestDrawCards);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnCardDisposed, OnCardDisposed);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnCardPlayed, OnRemoveCardFromHandPile);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnCardBeginDisposal,
                OnRemoveCardFromHandPile);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnCardsPickedFromPile,
                OnCardsPickedFromPile);
        }

        private void OnCardsPickedFromPile(Dictionary<string, object> messagedict)
        {
            var cardPileType = (VCardPileType)messagedict["CardPileType"];
            var pickedCards = messagedict["PickedCards"] as List<VCard>;

            if (pickedCards == null || pickedCards.Count == 0)
                return;

            List<VCard> pile = null;

            switch (cardPileType)
            {
                case VCardPileType.DrawPile:
                    pile = DrawPile;
                    break;
                case VCardPileType.Discard:
                    pile = DiscardPile;
                    break;
                case VCardPileType.Exhaust:
                    pile = ExhaustPile;
                    break;
                case VCardPileType.Deck:
                    HandPile.AddRange(pickedCards);
                    return;
                case VCardPileType.ALL:
                    return;
            }

            RemoveCardsFromPile(pile, pickedCards);
            HandPile.AddRange(pickedCards);
        }

        private void RemoveCardsFromPile(List<VCard> pile, List<VCard> cardsToRemove)
        {
            foreach (var card in cardsToRemove) RemoveCardFrom(pile, card);
        }

        private void RemoveCardFrom(List<VCard> pile, VCard card)
        {
            for (var i = pile.Count - 1; i >= 0; i--)
                if (pile[i] == card)
                {
                    pile.RemoveAt(i);
                    break;
                }
        }

        private void OnRemoveCardFromHandPile(Dictionary<string, object> messagedict)
        {
            RemoveFromHandPile(messagedict["Card"] as VCard);
        }

        private void OnCardDisposed(Dictionary<string, object> args)
        {
            var card = args["Card"] as VCard;
            DisposeCard(card, (bool)args["IsUsed"]);
        }

        private void OnRequestDrawCards(Dictionary<string, object> messagedict)
        {
            DrawCards((int)messagedict["DrawCount"], false, -1, (bool)messagedict["IsFromCard"],
                (bool)messagedict["ShouldPlayTwice"]);
        }

        public void DrawCards(int drawCount, bool isTurnBegin, int currentTurnIndex, bool isFromCard = false,
            bool shouldPlayTwice = false)
        {
            var cards = new List<VCard>();
            if (_isFirstTurn && !_isTutorial)
            {
                _isFirstTurn = false;
                var priorityCards = DrawPile.TakeWhile(card => card.IsPrioritized).ToList();
                if (priorityCards.Count > _maxHandSize)
                    cards = priorityCards.OrderBy(card => Random.Range(0f, 1f)).Take(_maxHandSize).ToList();
                else
                    cards = priorityCards;
                drawCount = Mathf.Max(0, drawCount - cards.Count);
            }
            else if (drawCount <= 0)
            {
                return;
            }

            if (drawCount > 0)
            {
                if (drawCount + HandPile.Count > _maxHandSize) drawCount = _maxHandSize - HandPile.Count;

                if (DrawPile.Count >= drawCount)
                {
                    cards.AddRange(DrawFromDrawPile(drawCount, isTurnBegin, currentTurnIndex));
                }
                else
                {
                    DiscardToDraw();
                    if (drawCount > DrawPile.Count)
                        drawCount = DrawPile.Count;
                    cards.AddRange(DrawFromDrawPile(drawCount, isTurnBegin, currentTurnIndex));
                }
            }

            VDebug.Log("Drawn Cards: " + cards.Count);
            var message = new Dictionary<string, object>();
            message.Add("Cards", cards);
            message.Add("IsFromCard", isFromCard);
            message.Add("ShouldPlayTwice", shouldPlayTwice);
            VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnDrawCards, message);
        }

        private List<VCard> DrawFromDrawPile(int n, bool isTurnBegin, int currentTurnIndex)
        {
            var cards = new List<VCard>();
            if (_isLoad)
            {
                _isLoad = false;
                cards.AddRange(HandPile);
                return cards;
            }

            if (_isTutorial && currentTurnIndex != -1 && _tutorialTurnHandCards.Count > currentTurnIndex)
            {
                foreach (var cardIndex in _tutorialTurnHandCards[currentTurnIndex])
                {
                    var card = DrawPile.Find(card => card.configID == cardIndex);
                    if (card is not null)
                        DrawPile.Remove(card);

                    if (card is null)
                    {
                        card = DiscardPile.Find(card => card.configID == cardIndex);
                        if (card is not null)
                            DiscardPile.Remove(card);
                    }

                    if (card is not null)
                    {
                        HandPile.Add(card);
                        cards.Add(card);
                    }
                }

                return cards;
            }

            var RGNs = new HashSet<int>();
            while (RGNs.Count < n)
            {
                var num = Random.Range(0, DrawPile.Count);
                if (RGNs.Contains(num))
                    continue;
                RGNs.Add(num);
                HandPile.Add(DrawPile[num]);
                cards.Add(DrawPile[num]);
            }

            foreach (var card in HandPile) DrawPile.Remove(card);

            return cards;
        }

        public void Clear()
        {
            Deck.Clear();
            DrawPile.Clear();
            DiscardPile.Clear();
            HandPile.Clear();
            ExhaustPile.Clear();
        }

        private void RemoveFromHandPile(VCard card)
        {
            if (card is null)
                return;

            for (var i = 0; i < HandPile.Count; i++)
                if (card == HandPile[i])
                {
                    HandPile.RemoveAt(i);
                    VDebug.Log($"已从手牌移除卡牌：{card.CardName}");
                    break;
                }
        }

        private void DisposeCard(VCard card, bool isUsed)
        {
            if (card is null)
                return;

            if (card.IsExhaust && isUsed)
            {
                ExhaustPile.Add(card);
                VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnCardEnterExaustPile,
                    new Dictionary<string, object>
                    {
                        { "Card", card }
                    });
                VDebug.Log($"卡牌已移入消耗堆：{card.CardName}");
            }
            else
            {
                DiscardPile.Add(card);
                VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnCardEnterDiscardPile,
                    new Dictionary<string, object>
                    {
                        { "Card", card }
                    });
                VDebug.Log($"卡牌已移入弃牌堆：{card.CardName}");
            }
        }

        private void OnTurnBegin(Dictionary<string, object> messagedict)
        {
            DrawCards(_handSize, true, (int)messagedict["TurnIndex"]);
            VDebug.Log($"回合开始，抽取 {_handSize} 张卡牌。");
        }

        private void DiscardToDraw()
        {
            DrawPile.AddRange(DiscardPile);
            DiscardPile.Clear();
            VDebug.Log("弃牌堆已洗入抽牌堆。");
            VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnDiscardToDraw, null);
        }

        public VCard GetCardById(uint id)
        {
            return Deck.Find(card => card.Id == id);
        }
    }
}