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
        [SerializeField] public List<VCard> _deck = new List<VCard>();

        public List<VCard> DrawPile => _drawPile;
        [SerializeField] public List<VCard> _drawPile = new List<VCard>();

        public List<VCard> DiscardPile => _discardPile;
        public List<VCard> _discardPile = new List<VCard>();

        public List<VCard> HandPile => _handPile;
        [SerializeField] public List<VCard> _handPile = new List<VCard>();
        
        public List<VCard> ExaustPile => _exaustPile;
        [SerializeField] public List<VCard> _exaustPile = new List<VCard>();
        
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
            VRootEventCenter.Instance.RegisterListener(VRootEventKey.OnTurnBegin, OnTurnBegin);
            VRootEventCenter.Instance.RegisterListener(VRootEventKey.OnTurnEnd, OnTurnEnd);
        }
        
        
        public void OnDisable()
        {
            VRootEventCenter.Instance.RegisterListener(VRootEventKey.OnTurnBegin, OnTurnBegin);
            VRootEventCenter.Instance.RegisterListener(VRootEventKey.OnTurnEnd, OnTurnEnd);
        }

        private void OnTurnEnd(Dictionary<string, object> messagedict)
        {
            for (int i = 0; i < _handPile.Count; i++)
            {
                _discardPile.Add(_handPile[i]);
            }
            
            Dictionary<string, object> message = new Dictionary<string, object>();
            message.Add("Cards", _handPile);
            VRootEventCenter.Instance.Raise(VRootEventKey.OnCardsAddedToDiscardPile, message);
            
            _handPile.Clear();
            
            if (_drawPile.Count < _handSize)
            {
                List<VCard> cards = new List<VCard>();
                foreach (var card in _drawPile)
                {
                    cards.Add(card);
                    _handPile.Add(card);
                }
                _drawPile.Clear();
                    
                message.Clear();
                message.Add("Cards", cards);
                VRootEventCenter.Instance.Raise(VRootEventKey.OnDrawCards, message);
            }
        }


        public void DrawCards(int drawCount)
        {      
            if (drawCount <= 0)
                return;
            if (drawCount + _handPile.Count > _maxHandSize)
            {
                drawCount = _maxHandSize - _handPile.Count;
            }
            
            if (_drawPile.Count >= drawCount)
            {
                DrawFromDrawPile(drawCount);
            }
            else
            {
                DiscardToDraw();
                DrawFromDrawPile(drawCount);
            } 
        }

        
        public void Clear()
        {
            _deck.Clear();
            _drawPile.Clear();
            _discardPile.Clear();
            _handPile.Clear();
            _exaustPile.Clear();
        }
        
        private void OnCardPlayed(Dictionary<string, object> args)
        {
            VCard card = args["Card"] as VCard;
            if(card is null)
                return;
            for (int i = 0; i < _handPile.Count; i++)
            {
                if (card == _handPile[i])
                {
                    if(card.IsExaust)
                        _exaustPile.Add(_handPile[i]);
                    else
                        _discardPile.Add(_handPile[i]);
                    _handPile.RemoveAt(i);
                }
            }
        }
        
        private void OnTurnBegin(Dictionary<string, object> messagedict)
        {
            DrawCards(_handSize);
        }
        
        private void DiscardToDraw()
        {
            _drawPile.AddRange(_discardPile);
            _discardPile.Clear();
            
            VRootEventCenter.Instance.Raise(VRootEventKey.OnDiscardToDraw, null);
        }
        
        private void DrawFromDrawPile(int n)
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

            VDebug.Log("Drawn Cards: " + cards.Count);
            Dictionary<string, object> message = new Dictionary<string, object>();
            message.Add("Cards", cards);
            VRootEventCenter.Instance.Raise(VRootEventKey.OnDrawCards, message);
        }

    }
}