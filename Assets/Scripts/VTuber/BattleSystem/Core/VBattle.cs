using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VTuber.BattleSystem.BattleAttribute;
using VTuber.BattleSystem.Buff;
using VTuber.BattleSystem.Card;
using VTuber.BattleSystem.Effect;
using VTuber.Character;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.Core
{
    public class VBattle : VSingletonMonobehaviour<VBattle>
    {
        private VBattleConfiguration _configuration;

        #region Managers

        public VBattleAttributeManager BattleAttributeManager => _battleAttributeManager;
        private VBattleAttributeManager _battleAttributeManager;
        
        public VCardPilesManager CardPilesManager => _cardPilesManager;
        private VCardPilesManager _cardPilesManager;
        
        public VBuffManager BuffManager => _buffManager;
        private VBuffManager _buffManager;

        #endregion
        
        #region Attributes

        private VBattleTurnAttribute _turnAttribute;

        private VBattlePlayLeftAttribute _playLeftAttribute;
        // private VBattlePopularityAttribute _popularityAttribute;
        // private VBattleParameterAttribute _parameterAttribute;
        // private VBattleAttribute _shieldAttribute;

        #endregion

        private int _currentPlayCountLeft = 0;
        
        public int TurnLeft => _turnAttribute.Value;
        public int PlayLeft => _playLeftAttribute.Value;
        
        private int MaxTurnCount => _configuration.maxTurnCount;

        private bool shouldNextCardPlayTwice = false;
        private bool shouldRedraw = false;
        
        private List<VEffect> _playTwiceEffects;
        private Dictionary<string, object> _playTwiceMessageDict;

        public void NextCardPlayTwice()
        {
            shouldNextCardPlayTwice = true;
            
            VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnNotifyBeginDisposeCard, new Dictionary<string ,object>() { });
        }
        
        public void RedrawRest()
        {
            shouldRedraw = true;
        }
        
        public void InitializeBattle(VCharacterAttributeManager characterAttributeManager, VBattleConfiguration configuration, VCardLibrary cardLibrary)
        {
            _configuration = configuration;

            _battleAttributeManager = new VBattleAttributeManager(characterAttributeManager);
            _cardPilesManager = new VCardPilesManager(_configuration.handSize, _configuration.maxHandSize, cardLibrary); 
            _buffManager = new VBuffManager(this);
            //_battleAttributeManager.AddAttribute("BAShield", new VBattleAttribute(0, false));
        }

        protected override void Awake()
        {
            base.Awake();
            
        }
        
        
        protected override void Start()
        {
            base.Start();
            
            _turnAttribute = new VBattleTurnAttribute(_configuration.maxTurnCount);
            _playLeftAttribute = new VBattlePlayLeftAttribute(_configuration.defaultPlayPerTurn);
            
            _battleAttributeManager.AddAttribute("BATurn", _turnAttribute);
            _battleAttributeManager.AddAttribute("BAPlayLeft", _playLeftAttribute);
            
            _battleAttributeManager.AddAttribute("BAPopularity", new VBattlePopularityAttribute(0));
            _battleAttributeManager.AddAttribute("BAParameter", new VBattleParameterAttribute(0));
            _battleAttributeManager.AddAttribute("BASingingMultiplier", new VBattleMultiplierAttribute(500));
            
            _battleAttributeManager.AddAttribute("BAStamina", new VBattleStaminaAttribute(100, VBattleEventKey.OnStaminaChange, 100));
            _battleAttributeManager.AddAttribute("BAShield", new VBattleStaminaAttribute(0, VBattleEventKey.OnShieldChange));
            
            VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnBattleBegin, new Dictionary<string, object>
            {
                {"TurnLeft", TurnLeft},
            });
            
            InitializeTurn();
        }
        
        protected override void OnEnable()
        {
            base.OnEnable();
            _battleAttributeManager.OnEnable();
            _cardPilesManager.OnEnable();
            _buffManager.OnEnable();
            
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnBuffAdded, OnBuffAdded);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnBuffValueUpdated, OnBuffValueUpdated);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnCardPlayed, OnCardPlayed);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnStaminaChange, OnStaminaChange);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnNotifyTurnBeginDelay, OnNotifyTurnBeginDelay);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnCardUsed, OnCardUsed);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnCardMovedToPlayPosition, OnCardMovedToPlayPosition);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnPlayTheSecondTime, OnPlayTheSecondTime);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnSkipTurnClicked, OnSkipTurnClicked);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnCardMovedToHandSlot, OnCardMovedToHandSlot);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnRequestPickCardsFromPile, OnRequestPickCardsFromPile);
            
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _battleAttributeManager.OnDisable();
            _cardPilesManager.OnDisable();
            _buffManager.OnDisable();
            
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnBuffAdded, OnBuffAdded);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnBuffValueUpdated, OnBuffValueUpdated);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnCardPlayed, OnCardPlayed);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnStaminaChange, OnStaminaChange);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnNotifyTurnBeginDelay, OnNotifyTurnBeginDelay);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnCardUsed, OnCardUsed);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnCardMovedToPlayPosition, OnCardMovedToPlayPosition);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnPlayTheSecondTime, OnPlayTheSecondTime);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnSkipTurnClicked, OnSkipTurnClicked);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnCardMovedToHandSlot, OnCardMovedToHandSlot);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnRequestPickCardsFromPile, OnRequestPickCardsFromPile);
        }
        
        private void OnRequestPickCardsFromPile(Dictionary<string, object> messagedict)
        {
            int cardCount = (int)messagedict["CardCount"];
            if(_cardPilesManager.HandPile.Count + cardCount > _configuration.maxHandSize)
            {
                cardCount = _configuration.maxHandSize - _cardPilesManager.HandPile.Count;
            }

            if (cardCount <= 0)
                return;

            messagedict["CardCount"] = cardCount;
            VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnBeginPickCardsFromPile, messagedict);
        }

        
        private void OnCardMovedToHandSlot(Dictionary<string, object> messagedict)
        {
            VCard card = messagedict["Card"] as VCard;
            
            switch (card.CostType)
            {
                case CostType.Stamina:
                    card.SetPlayable(_battleAttributeManager.TestCost(card.Cost));
                    break;
                case CostType.Buff:
                    card.SetPlayable(_buffManager.TestCost(card.CostBuffId, card.Cost));
                    break;
            }
        }
        
        private void OnBuffValueUpdated(Dictionary<string, object> messagedict)
        {
            foreach (var card in _cardPilesManager.HandPile)
            {
                if(card.CostType == CostType.Buff)
                    card.SetPlayable(_buffManager.TestCost(card.CostBuffId, card.Cost));
            }
        }

        private void OnBuffAdded(Dictionary<string, object> messagedict)
        {
            foreach (var card in _cardPilesManager.HandPile)
            {
                if(card.CostType == CostType.Buff)
                    card.SetPlayable(_buffManager.TestCost(card.CostBuffId, card.Cost));
            } 
        }
        
        private void OnStaminaChange(Dictionary<string, object> messagedict)
        {
            foreach (var card in _cardPilesManager.HandPile)
            {
                if(card.CostType == CostType.Stamina)
                    card.SetPlayable(_battleAttributeManager.TestCost(card.Cost));
            }
        }
        
        private void OnSkipTurnClicked(Dictionary<string, object> messagedict)
        {
            EndTurn();
        }
        
        private void OnPlayTheSecondTime(Dictionary<string, object> messagedict)
        {
            shouldNextCardPlayTwice = false;
            
            if(_playTwiceEffects is not null && _playTwiceMessageDict is not null)
                ApplyCardEffects( _playTwiceEffects, _playTwiceMessageDict);
            
            _playTwiceEffects = null;
            _playTwiceMessageDict = null;
        }
        
        private void OnCardMovedToPlayPosition(Dictionary<string, object> messagedict)
        {
            var card = messagedict["Card"] as VCard;
            if (card is null)
                return;
            var effects = card.Effects;
            
            ApplyCardEffects(effects, messagedict);
        }
        
        private void OnCardUsed(Dictionary<string, object> messagedict)
        {
            _playLeftAttribute.AddTo(-1, false);
            VDebug.Log("Play Left: " + PlayLeft);
            if (PlayLeft <= 0)
            {
                EndTurn();
                if (shouldRedraw) shouldRedraw = false;
            }
        }
        
        private void OnNotifyTurnBeginDelay(Dictionary<string, object> messagedict)
        {
            StartCoroutine(DelayInitializeTurn((float)messagedict["DelaySeconds"]));
        }
        
        IEnumerator DelayInitializeTurn(float delayTime)
        {
            yield return new WaitForSeconds(delayTime);

            InitializeTurn();
        }

        public void InitializeTurn()
        {
            VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnTurnBegin, new Dictionary<string, object>
            {
                {"TurnLeft", TurnLeft},
                {"HandSize", _configuration.maxHandSize}
            });
        }

        public void EndTurn()
        {
            Debug.Log("End Turn: " + TurnLeft);
            _turnAttribute.AddTo(-1, false);
            if (TurnLeft <= 0)
            {
                // End battle
            }
            else
            {
                VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnTurnEndBuffApply, new Dictionary<string, object>
                {
                    {"TurnLeft", TurnLeft}
                });
                
                VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnTurnResolution, new Dictionary<string, object>
                {
                    {"TurnLeft", TurnLeft}
                });
                
                VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnTurnEnd, new Dictionary<string, object>
                {
                    {"TurnLeft", TurnLeft}
                });
            }
        }
        
        private void OnCardPlayed(Dictionary<string, object> messagedict)
        {
            VDebug.Log(messagedict is null);

            VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnPreCardApply, messagedict);

            switch ((CostType)messagedict["CostType"])
            {
                case CostType.Stamina:
                    _battleAttributeManager.ApplyCost((int)messagedict["Cost"]);
                    break;
                case CostType.Buff:
                    _buffManager.ApplyCost((uint)messagedict["CostBuffId"], (int)messagedict["Cost"]);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Redraw()
        {
            VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnRedrawCards, new Dictionary<string, object>
            {
                {"ShouldPlayTwice", shouldNextCardPlayTwice},
            });
            
            if (shouldNextCardPlayTwice)
                shouldNextCardPlayTwice = false;
        }

        private void ApplyCardEffects(List<VEffect> effects, Dictionary<string, object> messagedict)
        {
            if (effects is null || effects.Count == 0)
            {
                VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnNotifyBeginDisposeCard,
                    new Dictionary<string, object>()
                    {

                    });
                return;
            }
            
            if (shouldNextCardPlayTwice)
            {
                _playTwiceEffects = effects;
                _playTwiceMessageDict = messagedict;
            }
            bool effectApplied = false;
            foreach (var effect in effects)
            {
                if (!effect.CanApply(this, messagedict))
                    continue;
                effectApplied = true;
                effect.ApplyEffect(this, 1, true, shouldNextCardPlayTwice);
            }

            if (!effectApplied)
            {
                VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnNotifyBeginDisposeCard,
                    new Dictionary<string, object>()
                    {

                    });
                return;
            }
            
            if (shouldRedraw)
            {
                shouldRedraw = false;
                if (PlayLeft == 0)
                    return;
                Redraw();
            }
        }
    }
}