using System.Collections.Generic;
using UnityEngine;
using VTuber.BattleSystem.Card;
using VTuber.BattleSystem.Effect;
using VTuber.BattleSystem.UI;
using VTuber.Character;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.Core
{
    public class VCardPilesManager
    {
        public List<VCard> Deck => _deck;
        private List<VCard> _deck = new List<VCard>();

        public List<VCard> DrawPile => _drawPile;
        private List<VCard> _drawPile = new List<VCard>();

        public List<VCard> DiscardPile => _discardPile;
        private List<VCard> _discardPile = new List<VCard>();

        public List<VCard> HandPile => _handPile;
        private List<VCard> _handPile = new List<VCard>();
        
        public List<VCard> ExhaustPile => _exhaustPile;
        private List<VCard> _exhaustPile = new List<VCard>();
        
        private int _handSize;
        private int _maxHandSize;

        public VCardPilesManager(int handSize, int maxHandSize, VCardLibrary cardLibrary)
        {
            _deck = new List<VCard>();
            _drawPile = new List<VCard>();
            _discardPile = new List<VCard>();
            _handPile = new List<VCard>();
            _exhaustPile = new List<VCard>();
            _handSize = handSize;
            _maxHandSize = maxHandSize;
            
            _deck.AddRange(cardLibrary.GetCards());
            _drawPile.AddRange(_deck);
        }
        
        public void OnEnable()
        {
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnTurnBegin, OnTurnBegin);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnRequestDrawCards, OnRequestDrawCards);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnCardDisposed, OnCardDisposed);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnCardPlayed, OnRemoveCardFromHandPile);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnCardBeginDisposal, OnRemoveCardFromHandPile);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnCardsPickedFromPile, OnCardsPickedFromPile);
        }

        public void OnDisable()
        {
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnTurnBegin, OnTurnBegin);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnRequestDrawCards, OnRequestDrawCards);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnCardDisposed, OnCardDisposed);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnCardPlayed, OnRemoveCardFromHandPile);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnCardBeginDisposal, OnRemoveCardFromHandPile);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnCardsPickedFromPile, OnCardsPickedFromPile);
        }

        private void OnCardsPickedFromPile(Dictionary<string, object> messagedict)
        {
            VCardPileType cardPileType = (VCardPileType)messagedict["CardPileType"];
            List<VCard> pickedCards = messagedict["PickedCards"] as List<VCard>;
            
            if (pickedCards == null || pickedCards.Count == 0)
                return;

            List<VCard> pile = null;
            
            switch (cardPileType)
            {
                case VCardPileType.DrawPile:
                    pile = _drawPile;
                    break;
                case VCardPileType.Discard:
                    pile = _discardPile;
                    break;
                case VCardPileType.Exhaust:
                    pile = _exhaustPile;
                    break;
                case VCardPileType.Deck:
                    _handPile.AddRange(pickedCards);
                    return;
            }
            
            RemoveCardsFromPile(pile, pickedCards);
            _handPile.AddRange(pickedCards);
        }

        private void RemoveCardsFromPile(List<VCard> pile, List<VCard> cardsToRemove)
        {
            foreach (var card in cardsToRemove)
            {
                RemoveCardFrom(pile, card);
            }
        }
        
        private void RemoveCardFrom(List<VCard> pile, VCard card)
        {
            for (int i = pile.Count - 1; i >= 0; i--)
            {
                if(pile[i] == card)
                {
                    pile.RemoveAt(i);
                    break;
                }
            }
        }
        
        private void OnRemoveCardFromHandPile(Dictionary<string, object> messagedict)
        {
            RemoveFromHandPile(messagedict["Card"] as VCard);
        }       
        
        private void OnCardDisposed(Dictionary<string, object> args)
        {
            VCard card = args["Card"] as VCard;
            DisposeCard(card);
        }

        private void OnRequestDrawCards(Dictionary<string, object> messagedict)
        {
            DrawCards((int)messagedict["DrawCount"], (bool)messagedict["IsFromCard"], (bool)messagedict["ShouldPlayTwice"]);
        }

        public void DrawCards(int drawCount, bool isFromCard = false, bool shouldPlayTwice = false)
        {      
            if (drawCount <= 0)
                return;
            if (drawCount + _handPile.Count > _maxHandSize)
            {
                drawCount = _maxHandSize - _handPile.Count;
            }

            List<VCard> cards;
            if (_drawPile.Count >= drawCount)
            {
                cards = DrawFromDrawPile(drawCount);
            }
            else
            {
                DiscardToDraw();
                cards = DrawFromDrawPile(drawCount);
            }
                
            
            VDebug.Log("Drawn Cards: " + cards.Count);
            Dictionary<string, object> message = new Dictionary<string, object>();
            message.Add("Cards", cards);
            message.Add("IsFromCard", isFromCard);
            message.Add("ShouldPlayTwice", shouldPlayTwice);
            VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnDrawCards, message);
        }
        
        private List<VCard> DrawFromDrawPile(int n)
        {
            List<VCard> cards = new List<VCard>();
            HashSet<int> RGNs = new HashSet<int>();
            while (RGNs.Count < n)
            {
                int num = Random.Range(0, _drawPile.Count);
                if (RGNs.Contains(num))
                    continue;
                RGNs.Add(num);
                _handPile.Add(_drawPile[num]);
                cards.Add(_drawPile[num]);
            }

            foreach (var card in _handPile)
            {
                _drawPile.Remove(card);
            }

            return cards;
        }
        
        public void Clear()
        {
            _deck.Clear();
            _drawPile.Clear();
            _discardPile.Clear();
            _handPile.Clear();
            _exhaustPile.Clear();
        }
        
        private void RemoveFromHandPile(VCard card)
        {
            if(card is null)
                return;
            
            for (int i = 0; i < _handPile.Count; i++)
            {
                if (card == _handPile[i])
                {
                    _handPile.RemoveAt(i);
                    VDebug.Log($"已从手牌移除卡牌：{card.CardName}");
                    break;
                }
            }
        }
        
        private void DisposeCard(VCard card)
        {
            if(card is null)
                return;
            
            if(card.IsExhaust)
            {
                _exhaustPile.Add(card);
                VDebug.Log($"卡牌已移入消耗堆：{card.CardName}");
            }
            else
            {
                _discardPile.Add(card);
                VDebug.Log($"卡牌已移入弃牌堆：{card.CardName}");
            }
        }
        
        private void OnTurnBegin(Dictionary<string, object> messagedict)
        {
            DrawCards(_handSize);
            VDebug.Log($"回合开始，抽取 {_handSize} 张卡牌。");
        }
        
        private void DiscardToDraw()
        {
            _drawPile.AddRange(_discardPile);
            _discardPile.Clear();
            VDebug.Log($"弃牌堆已洗入抽牌堆。");
            VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnDiscardToDraw, null);
        }
    }
}