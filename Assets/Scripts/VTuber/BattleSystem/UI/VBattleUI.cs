using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.UI
{
    public class VBattleUI : VUIBehaviour
    {
        public float smoothTime = 0.2f;
        [FormerlySerializedAs("cardSlots")] [SerializeField] private RectTransform handSlotsContent;
        private GameObject cardUIPrefab;
        private bool arrangingHandSlots = false;

        private List<VCardUI> _handSlotsCards;
        
        [Range(-1, 1)]public float overlap = 0.2f;
        public float curve = 0.0025f;
        
        public Vector2 cardSize;
        private Vector2 _scaledCardSize;
        private Vector2 _handSlotsSize;
        
        protected override void Awake()
        {
            base.Awake();
            cardUIPrefab = Resources.Load<GameObject>("Card/Prefabs/card");
            List<VCardUI> cards = new List<VCardUI>();
            _handSlotsCards = new List<VCardUI>();
            _handSlotsSize = handSlotsContent.rect.size;
            for (int i = 0; i < 3; i++)
            {
                var cardUI = Instantiate(cardUIPrefab, handSlotsContent);
                cards.Add(cardUI.GetComponent<VCardUI>());   
            }

            StartCoroutine(DrawCard(cards));
        } 
        
        private IEnumerator DrawCard(IEnumerable<VCardUI> cards)
        {
            arrangingHandSlots = true;
            foreach (var cardUI in cards)
            {
                (Vector3 position, Vector3 rotation, Vector3 scale) = ReserveSpaceForNewCard();
                cardUI.handCardUI = cardUI.AddComponent<VHandCardUI>();
                cardUI.handCardUI.index = _handSlotsCards.Count;
                cardUI.handCardUI.battleUI = this;
                cardUI.handCardUI.cardUI = cardUI;
                //cardUI.handCardUI.card = cardUI.Card;
                cardUI.handCardUI.SetPosition(position, smoothTime, true);
                cardUI.handCardUI.SetRotation(rotation, smoothTime, true);
                SetHandCardPositionRotation(cardUI, position.x);
                cardUI.handCardUI.SetScale(scale, smoothTime, true);
                _handSlotsCards.Add(cardUI);
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
                    card.handCardUI.SetScale(target, smoothTime, true);
                }
            }
            else
            {
                scale = 1.0f;
                foreach (var card in _handSlotsCards)
                {
                    card.handCardUI.SetScale(Vector3.one, smoothTime, true);
                }
            }
            
            _scaledCardSize.x = cardSize.x * (scale * (1.0f - overlap));
            _scaledCardSize.y = cardSize.y * scale;
            
            if (_handSlotsCards.Count == 1)
            {
                _handSlotsCards[0].handCardUI.SetScale(Vector3.one, smoothTime, true);
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
        
        private void SetHandCardPositionRotation(VCardUI ui, float offset)
        {
            var angle = 0;
            float x = 0.0f; 
            float y = 0.0f;  
            
            ui.handCardUI.
                SetPosition(new Vector3(x + offset, y, 0.0f), smoothTime, true);
        }

        
    }
}