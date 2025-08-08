using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using VTuber.BattleSystem.BattleAttribute;
using VTuber.BattleSystem.Buff;
using VTuber.BattleSystem.Card;
using VTuber.BattleSystem.Effect;
using VTuber.Character;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;
using VTuber.Relic;

namespace VTuber.BattleSystem.Core
{
    public class VBattle : VSingletonMonobehaviour<VBattle>
    {
        [FormerlySerializedAs("_configuration")] [SerializeField] protected VBattleConfiguration configuration;

        #region Managers

        public VBattleAttributeManager BattleAttributeManager => _battleAttributeManager;
        protected VBattleAttributeManager _battleAttributeManager;
        
        public VCardPilesManager CardPilesManager => _cardPilesManager;
        protected VCardPilesManager _cardPilesManager;
        
        public VBuffManager BuffManager => _buffManager;
        protected VBuffManager _buffManager;

        public VBattleRelicManager BattleRelicManager => _battleRelicManager;
        private VBattleRelicManager _battleRelicManager;

        #endregion
        
        #region Attributes

        protected VBattleTurnAttribute _turnAttribute;

        protected VBattlePlayLeftAttribute _playLeftAttribute;
        // private VBattlePopularityAttribute _popularityAttribute;
        // private VBattleParameterAttribute _parameterAttribute;
        // private VBattleAttribute _shieldAttribute;

        #endregion

        protected int _currentPlayCountLeft = 0;
        
        public int TurnLeft => _turnAttribute.Value;
        public int PlayLeft => _playLeftAttribute.Value;
        
        protected int MaxTurnCount => configuration.maxTurnCount;

        protected bool _shouldNextCardPlayTwice = false;
        protected bool _shouldRedraw = false;
        
        protected List<VEffect> _playTwiceEffects;
        protected Dictionary<string, object> _playTwiceMessageDict;
        protected VCharacterAttributeManager _characterAttributeManager;
        protected bool paused = false;
        protected int _targetPopularity;
        
        public void NextCardPlayTwice()
        {
            _shouldNextCardPlayTwice = true;
            
            VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnNotifyBeginDisposeCard, new Dictionary<string ,object>() { });
        }
        
        public void RedrawRest()
        {
            _shouldRedraw = true;
        }

        public void Pause()
        {
            paused = !paused;

            VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnBattlePause, new Dictionary<string, object>
            {
                {"Paused", paused}
            });
            
        }
        
        public virtual void InitializeBattle(VCharacterAttributeManager characterAttributeManager,
            VCardLibrary cardLibrary, int initialTurnCount, int targetPopularity, int initialViewers, List<VBattleRelic> relics)
        {
            _targetPopularity = targetPopularity;
            _characterAttributeManager = characterAttributeManager;
            _battleAttributeManager = new VBattleAttributeManager();
            _cardPilesManager = new VCardPilesManager(configuration.handSize, configuration.maxHandSize, cardLibrary); 
            _buffManager = new VBuffManager(this);
            _battleRelicManager = new VBattleRelicManager(this, relics);
            
            _battleAttributeManager.OnEnable();
            _cardPilesManager.OnEnable();
            _buffManager.OnEnable();
            
            _battleAttributeManager.AttributesConversion(_characterAttributeManager);
            _turnAttribute = new VBattleTurnAttribute(initialTurnCount);
            _playLeftAttribute = new VBattlePlayLeftAttribute(configuration.defaultPlayPerTurn);
            
            _battleAttributeManager.AddAttribute("BATurn", _turnAttribute);
            _battleAttributeManager.AddAttribute("BAPlayLeft", _playLeftAttribute);
            
            _battleAttributeManager.AddAttribute("BAPopularity", new VBattlePopularityAttribute(0));
            _battleAttributeManager.AddAttribute("BAParameter", new VBattleParameterAttribute(0));
            
            _battleAttributeManager.AddAttribute("BAShield", new VBattleStaminaAttribute(0, VBattleEventKey.OnShieldChange));
            _battleAttributeManager.AddAttribute("BARevenue", new VBattleStaminaAttribute(0, VBattleEventKey.OnRevenueChange));

            _battleAttributeManager.InitializeInternalManagers();
            
            if(_battleAttributeManager.TryGetAttribute("BAViewerCount", out var viewerCountAttribute))
            {
                viewerCountAttribute.AddTo(initialViewers, false);
            }
            
            VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnBattleBegin, new Dictionary<string, object>
            {
                {"TurnLeft", TurnLeft},
                {"TargetPopularity", targetPopularity},
            });
            
            InitializeTurn();
        }
        
        protected override void OnEnable()
        {
            base.OnEnable();
            
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
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnAttributeValueChange, OnAttributeValueChange);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnParameterPopularityModifierChanged, OnParameterPopularityModifierChanged);
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
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnAttributeValueChange, OnAttributeValueChange);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnParameterPopularityModifierChanged, OnParameterPopularityModifierChanged);
        }
        
        
        private void OnParameterPopularityModifierChanged(Dictionary<string, object> messagedict)
        {
            foreach (var card in _cardPilesManager.HandPile)
            {
                card.PreviewPopularity(this, false);
            }
        }
        
        private void OnAttributeValueChange(Dictionary<string, object> messagedict)
        {
            foreach (var card in _cardPilesManager.HandPile)
            {
                card.TestCondition(this);
            }
        }
        
        
        private void OnRequestPickCardsFromPile(Dictionary<string, object> messagedict)
        {
            int cardCount = (int)messagedict["CardCount"];
            if(_cardPilesManager.HandPile.Count + cardCount > configuration.maxHandSize)
            {
                cardCount = configuration.maxHandSize - _cardPilesManager.HandPile.Count;
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
                    card.setPlayable?.Invoke(_battleAttributeManager.StaminaManager.TestCost(card.Cost));
                    break;
                case CostType.TrueStamina:
                    card.setPlayable?.Invoke(_battleAttributeManager.StaminaManager.TestCost(card.Cost, true));
                    break;
                case CostType.Buff:
                    card.setPlayable?.Invoke(_buffManager.TestCost(card.CostBuffId, card.Cost));
                    break;
            }
            card.TestCondition(this);
            card.PreviewPopularity(this, true);
        }
        
        private void OnBuffValueUpdated(Dictionary<string, object> messagedict)
        {
            foreach (var card in _cardPilesManager.HandPile)
            {
                if(card.CostType == CostType.Buff)
                    card.setPlayable?.Invoke(_buffManager.TestCost(card.CostBuffId, card.Cost));
                card.TestCondition(this);
                card.PreviewPopularity(this, false);
            }
        }

        private void OnBuffAdded(Dictionary<string, object> messagedict)
        {
            foreach (var card in _cardPilesManager.HandPile)
            {
                if(card.CostType == CostType.Buff)
                    card.setPlayable?.Invoke(_buffManager.TestCost(card.CostBuffId, card.Cost));
                card.TestCondition(this);
                card.PreviewPopularity(this, false);
            } 
        }
        
        private void OnStaminaChange(Dictionary<string, object> messagedict)
        {
            foreach (var card in _cardPilesManager.HandPile)
            {
                if(card.CostType == CostType.Stamina)
                    card.setPlayable?.Invoke(_battleAttributeManager.StaminaManager.TestCost(card.Cost));
                if(card.CostType == CostType.TrueStamina)
                    card.setPlayable?.Invoke(_battleAttributeManager.StaminaManager.TestCost(card.Cost, true));
            }
        }
        
        private void OnSkipTurnClicked(Dictionary<string, object> messagedict)
        {
            EndTurn();
        }
        
        private void OnPlayTheSecondTime(Dictionary<string, object> messagedict)
        {
            _shouldNextCardPlayTwice = false;
            
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
            VDebug.Log("剩余可行动次数: " + PlayLeft);
            if (PlayLeft <= 0)
            {
                EndTurn();
                if (_shouldRedraw) _shouldRedraw = false;
            }
        }
        
        private void OnNotifyTurnBeginDelay(Dictionary<string, object> messagedict)
        {
            if(TurnLeft <= 0)
                return;
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
                {"HandSize", configuration.maxHandSize}
                
            });
            VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnTurnBeginBuffApply, new Dictionary<string, object>
            {
            });
        }

        public void EndTurn()
        {
            Debug.Log("回合结束: " + TurnLeft);
            _turnAttribute.AddTo(-1, false);
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
            
            if (TurnLeft <= 0)
            {
                bool isTargetMet = _battleAttributeManager.TryGetAttribute("BAPopularity", out var popularityAttribute) && 
                                   popularityAttribute.Value >= _targetPopularity;
                
                _characterAttributeManager.ConvertToCharacterAttributes(_battleAttributeManager.BattleAttributes);
                
                _buffManager.Clear();
                _battleAttributeManager.Clear();
                _cardPilesManager.DiscardPile.Clear();
                _cardPilesManager.DrawPile.Clear();
                _cardPilesManager.HandPile.Clear();
                _cardPilesManager.Deck.Clear();
                
                
                VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnBattleEnd, new Dictionary<string, object>
                {
                    {"TurnLeft", TurnLeft}
                });              
                
                VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnBattleEndNotify, new Dictionary<string, object>
                {
                    {"TurnLeft", TurnLeft},
                    {"IsTargetMet", isTargetMet}
                });
            }
        }
        
        private void OnCardPlayed(Dictionary<string, object> messagedict)
        {
            VDebug.Log(messagedict is null ? "卡牌消息为空" : "卡牌消息有效");

            VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnPreCardApply, messagedict);
            switch ((CostType)messagedict["CostType"])
            {
                case CostType.Stamina:
                    _battleAttributeManager.StaminaManager.ApplyCost((int)messagedict["Cost"]);
                    break;
                case CostType.TrueStamina:
                    _battleAttributeManager.StaminaManager.ApplyCost((int)messagedict["Cost"], true);
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
                {"ShouldPlayTwice", _shouldNextCardPlayTwice},
            });
            
            if (_shouldNextCardPlayTwice)
                _shouldNextCardPlayTwice = false;
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
            
            if (_shouldNextCardPlayTwice)
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
                effect.ApplyEffect(this, 1, true, _shouldNextCardPlayTwice);
            }

            if (!effectApplied)
            {
                VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnNotifyBeginDisposeCard,
                    new Dictionary<string, object>()
                    {

                    });
                return;
            }
            
            if (_shouldRedraw)
            {
                _shouldRedraw = false;
                if (PlayLeft == 0)
                    return;
                Redraw();
            }
        }
    }
}