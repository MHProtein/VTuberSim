using System.Collections.Generic;
using UnityEngine;
using VTuber.BattleSystem.Card;
using VTuber.Core.EventCenter;

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

        public VCardPilesManager()
        {
            _deck = new List<VCard>();
            _drawPile = new List<VCard>();
            _discardPile = new List<VCard>();
            _handPile = new List<VCard>();
            _exaustPile = new List<VCard>();
        }
        
        public void OnEnable()
        {
            VRootEventCenter.Instance.RegisterListener(VRootEventKeys.OnTurnBegin, OnTurnBegin);
            VRootEventCenter.Instance.RegisterListener(VRootEventKeys.OnTurnEnd, OnTurnEnd);
        }
        
        
        public void OnDisable()
        {
            VRootEventCenter.Instance.RegisterListener(VRootEventKeys.OnTurnBegin, OnTurnBegin);
            VRootEventCenter.Instance.RegisterListener(VRootEventKeys.OnTurnEnd, OnTurnEnd);
        }

        private void OnTurnEnd(Dictionary<string, object> messagedict)
        {
            
            for (int i = 0; i < _handPile.Count; i++)
            {
                _discardPile.Add(_handPile[i]);
            }
            
            Dictionary<string, object> message = new Dictionary<string, object>();
            message.Add("cards", _handPile);
            VRootEventCenter.Instance.Raise(VRootEventKeys.OnCardsAddedToDiscardPile, message);
            
            _handPile.Clear();

            if (_drawPile.Count < (int)messagedict["HandSize"])
            {
                List<VCard> cards = new List<VCard>();
                foreach (var card in _drawPile)
                {
                    cards.Add(card);
                    _handPile.Add(card);
                }
                _drawPile.Clear();
                    
                message.Clear();
                message.Add("cards", cards);
                VRootEventCenter.Instance.Raise("OnDrawCards", message);
            }
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

            Dictionary<string, object> message = new Dictionary<string, object>();
            message.Add("cards", cards);
            VRootEventCenter.Instance.Raise(VRootEventKeys.OnDrawCards, message);
        }
        
        public void Clear()
        {
            _deck.Clear();
            _drawPile.Clear();
            _discardPile.Clear();
            _handPile.Clear();
            _exaustPile.Clear();
        }
        
        private void OnTurnBegin(Dictionary<string, object> messagedict)
        {
            
        }
        
        private void DiscardToDraw()
        {
            _drawPile.AddRange(_discardPile);
            _discardPile.Clear();
            
            VRootEventCenter.Instance.Raise(VRootEventKeys.OnDiscardToDraw, null);
        }


        
        
        
    }
}