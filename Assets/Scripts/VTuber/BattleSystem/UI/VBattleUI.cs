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
        private List<VCardUI> _displayingCards = new List<VCardUI>();
        [SerializeField] private GameObject scrollView;
        
        public float smoothTime = 0.2f;
        [FormerlySerializedAs("cardSlots")] [SerializeField] private RectTransform handSlotsContent;
        private bool arrangingHandSlots = false;

        private List<VHandCardUI> _handSlotsCards;
        private GameObject cardUIPrefab;
        
        [Range(-1, 1)]public float overlap = 0.2f;
        public float curve = 0.0025f;
        
        public Vector2 cardSize;
        private Vector2 _scaledCardSize;
        private Vector2 _handSlotsSize;

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
        
        protected override void Awake()
        {
            base.Awake();
            
            cardUIPrefab = Resources.Load<GameObject>("Card/Prefabs/card");
            _handSlotsCards = new List<VHandCardUI>();
            _handSlotsSize = handSlotsContent.rect.size;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            VRootEventCenter.Instance.RegisterListener(VRootEventKey.OnDrawCards, OnDrawCards);
            VRootEventCenter.Instance.RegisterListener(VRootEventKey.OnTurnEnd, OnTurnEnd);
        }

        private void OnTurnEnd(Dictionary<string, object> messageDict)
        {
            foreach (var cardUI in _handSlotsCards)
            {
                if(cardUI)
                    Destroy(cardUI.gameObject);
            }
            _handSlotsCards.Clear();
        }

        private void OnDrawCards(Dictionary<string, object> messageDict)
        {
            List<VCard> cards = messageDict["Cards"] as List<VCard>;
            if (cards == null)
            {
                VDebug.LogError("OnDrawCards: Cards is null");
                return;
            }
            
            StartCoroutine(DrawCard(cards));
        }

        private IEnumerator DrawCard(IEnumerable<VCard> cards)
        {
            arrangingHandSlots = true;
            foreach (var card in cards)
            {
                var cardUI = Instantiate(cardUIPrefab, handSlotsContent).GetComponent<VCardUI>();
                var handCardUI = cardUI.AddComponent<VHandCardUI>();
                (Vector3 position, Vector3 rotation, Vector3 scale) = ReserveSpaceForNewCard();
                handCardUI.index = _handSlotsCards.Count;
                handCardUI.battleUI = this;
                handCardUI.card = card;
                handCardUI.cardUI = cardUI;
                handCardUI.SetPosition(position, smoothTime, true);
                handCardUI.SetRotation(rotation, smoothTime, true);
                SetHandCardPositionRotation(handCardUI, position.x);
                handCardUI.SetScale(scale, smoothTime, true);
                _handSlotsCards.Add(handCardUI);
                //cardUI.Card.isHandCard = true;
                //cardUI.Card.Validate();
                yield return new WaitForSeconds(smoothTime);
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
                    card.SetScale(target, smoothTime, true);
                }
            }
            else
            {
                scale = 1.0f;
                foreach (var card in _handSlotsCards)
                {
                    card.SetScale(Vector3.one, smoothTime, true);
                }
            }
            
            _scaledCardSize.x = cardSize.x * (scale * (1.0f - overlap));
            _scaledCardSize.y = cardSize.y * scale;
            
            if (_handSlotsCards.Count == 1)
            {
                _handSlotsCards[0].SetScale(Vector3.one, smoothTime, true);
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
                    card.SetScale(target, smoothTime, true);
                }
            }
            else
            {
                scale = 1.0f;
                foreach (var card in _handSlotsCards)
                {
                    card.SetScale(Vector3.one, smoothTime, true);
                }
            }
            
            _scaledCardSize.x = cardSize.x * (scale * (1.0f - overlap));
            _scaledCardSize.y = cardSize.y * scale;


            if (_handSlotsCards.Count == 1)
            {
                _handSlotsCards[0].SetScale(Vector3.one, smoothTime, true);
                SetHandCardPositionRotation(_handSlotsCards[0], 0.0f);
            }
            
            if (_handSlotsCards.Count == 2)
            {
                _handSlotsCards[0].SetScale(Vector3.one, smoothTime, true);
                SetHandCardPositionRotation(_handSlotsCards[0], -(cardSize.x / 2.0f));
                
                _handSlotsCards[1].SetScale(Vector3.one, smoothTime, true);
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
            ui.SetPosition(new Vector3(offset, 0.0f, 0.0f), smoothTime, true);
        }
        
    }
}