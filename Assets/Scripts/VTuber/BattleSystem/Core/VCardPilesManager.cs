using System.Collections.Generic;
using UnityEngine;
using VTuber.BattleSystem.Card;
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
        
        public List<VCard> ExaustPile => _exaustPile;
        private List<VCard> _exaustPile = new List<VCard>();
        
        private int _handSize;
        private int _maxHandSize;

        public VCardPilesManager(int handSize, int maxHandSize, VCardLibrary cardLibrary)
        {
            _deck = new List<VCard>();
            _drawPile = new List<VCard>();
            _discardPile = new List<VCard>();
            _handPile = new List<VCard>();
            _exaustPile = new List<VCard>();
            _handSize = handSize;
            _maxHandSize = maxHandSize;
            
            _deck.AddRange(cardLibrary.GetCards());
            _drawPile.AddRange(_deck);
        }
        
        public void OnEnable()
        {
            VBattleRootEventCenter.Instance.RegisterListener(VRootEventKey.OnTurnBegin, OnTurnBegin);
            VBattleRootEventCenter.Instance.RegisterListener(VRootEventKey.OnRequestDrawCards, OnRequestDrawCards);
            VBattleRootEventCenter.Instance.RegisterListener(VRootEventKey.OnCardDisposed, OnCardDisposed);
            VBattleRootEventCenter.Instance.RegisterListener(VRootEventKey.OnCardPlayed, OnRemoveCardFromHandPile);
            VBattleRootEventCenter.Instance.RegisterListener(VRootEventKey.OnCardBeginDisposal, OnRemoveCardFromHandPile);
        }

        public void OnDisable()
        {
            VBattleRootEventCenter.Instance.RemoveListener(VRootEventKey.OnTurnBegin, OnTurnBegin);
            VBattleRootEventCenter.Instance.RemoveListener(VRootEventKey.OnRequestDrawCards, OnRequestDrawCards);
            VBattleRootEventCenter.Instance.RemoveListener(VRootEventKey.OnCardDisposed, OnCardDisposed);
            VBattleRootEventCenter.Instance.RemoveListener(VRootEventKey.OnCardPlayed, OnRemoveCardFromHandPile);
            VBattleRootEventCenter.Instance.RemoveListener(VRootEventKey.OnCardBeginDisposal, OnRemoveCardFromHandPile);
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
            VBattleRootEventCenter.Instance.Raise(VRootEventKey.OnDrawCards, message);
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
            _exaustPile.Clear();
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
                    break;
                }
            }
        }
        
        private void DisposeCard(VCard card)
        {
            if(card is null)
                return;
            
            if(card.IsExaust)
                _exaustPile.Add(card);
            else
                _discardPile.Add(card);
        }
        
        private void OnTurnBegin(Dictionary<string, object> messagedict)
        {
            DrawCards(_handSize);
        }
        
        private void DiscardToDraw()
        {
            _drawPile.AddRange(_discardPile);
            _discardPile.Clear();
            
            VBattleRootEventCenter.Instance.Raise(VRootEventKey.OnDiscardToDraw, null);
        }
    }
}