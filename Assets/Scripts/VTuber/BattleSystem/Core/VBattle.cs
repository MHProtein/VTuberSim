using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using VTuber.BattleSystem.BattleAttribute;
using VTuber.BattleSystem.Buff;
using VTuber.BattleSystem.Card;
using VTuber.BattleSystem.Effect;
using VTuber.BattleSystem.UI;
using VTuber.Character;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;
using VTuber.Dialogue.UI;
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

        protected bool _shouldNextCardPlayTwice;
        protected bool _shouldRedraw = false;

        protected VCard _playTwiceCard;
        protected Dictionary<string, object> _playTwiceMessageDict;
        protected VCharacterAttributeManager _characterAttributeManager;
        protected bool paused = false;
        protected int _targetPopularity;
        protected int _extraTargetPopularity;
        protected int _abiliyBonus;
        protected bool _isPhaseEnding = false;
        private int _mainAttributeIndex;
        
        private List<AnimationCurve> _decayCurves;
        private List<int> _abilityTurnCounts;
        private bool _shouldEndBattle = false;

        public Dictionary<string, int> CardTypeHistory => cardTypeHistory;
        protected Dictionary<string, int> cardTypeHistory;
        
        public virtual void InitializeBattle(bool isPhaseEnding, VCharacterAttributeManager characterAttributeManager,
            VCardLibrary cardLibrary, int initialTurnCount, int mainAttributeIndex, List<int> abilityTurnCounts, List<AnimationCurve> decayCurves,
        int targetPopularity, int extraTargetPopularity, int abilityBonus, int initialViewers, List<VBattleRelic> relics)
        {
            _mainAttributeIndex = mainAttributeIndex;
            _isPhaseEnding = isPhaseEnding;
            cardTypeHistory = new Dictionary<string, int>();
            _targetPopularity = targetPopularity;
            _extraTargetPopularity = extraTargetPopularity;
            _decayCurves = decayCurves;
            _abiliyBonus = abilityBonus;
            _abilityTurnCounts = abilityTurnCounts;
            _characterAttributeManager = characterAttributeManager;
            _battleAttributeManager = new VBattleAttributeManager(isPhaseEnding);
            _cardPilesManager = new VCardPilesManager(configuration.handSize, configuration.maxHandSize, cardLibrary); 
            _buffManager = new VBuffManager(this);
            
            _battleAttributeManager.OnEnable();
            _cardPilesManager.OnEnable();
            _buffManager.OnEnable();
            
            VEventSystemUI.Instance.OpenBattleUI();
            
            VEventSystemUI.Instance.PlayVideo(() =>
            {
                _battleAttributeManager.AttributesConversion(_characterAttributeManager);
                _turnAttribute = new VBattleTurnAttribute(initialTurnCount);
                _playLeftAttribute = new VBattlePlayLeftAttribute(configuration.defaultPlayPerTurn);

                if (!isPhaseEnding)
                {
                    _battleAttributeManager.TryGetAttribute("BASingingMultiplier", out var attribute);
                    attribute.SetValue(100, false, false, false);
                    _battleAttributeManager.TryGetAttribute("BAGamingMultiplier", out attribute);
                    attribute.SetValue(100, false, false, false);
                    _battleAttributeManager.TryGetAttribute("BAChattingMultiplier", out attribute);
                    attribute.SetValue(100, false, false, false);
                }
            
                _battleAttributeManager.AddAttribute("BATurn", _turnAttribute);
                _battleAttributeManager.AddAttribute("BAPlayLeft", _playLeftAttribute);
            
                _battleAttributeManager.AddAttribute("BAShield", new VBattleStaminaAttribute(0, VBattleEventKey.OnShieldChange, true));
                _battleAttributeManager.AddAttribute("BARevenue", new VBattleStaminaAttribute(0, VBattleEventKey.OnRevenueChange));
            
                _battleAttributeManager.AddAttribute("BAPopularity", new VBattlePopularityAttribute(0));
                _battleAttributeManager.AddAttribute("BAParameter", new VBattleParameterAttribute(0));
                
                _battleRelicManager = new VBattleRelicManager(this, relics);
                if(_battleAttributeManager.TryGetAttribute("BAViewerCount", out var viewerCountAttribute))
                {
                    viewerCountAttribute.AddTo(initialViewers, false);
                }
                _battleAttributeManager.InitializeInternalManagers(mainAttributeIndex, abilityTurnCounts);
                VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnBattleBegin, new Dictionary<string, object>
                {
                    { "TurnLeft", TurnLeft },
                    { "TargetPopularity", targetPopularity },
                    { "ExtraTargetPopularity", extraTargetPopularity },
                    { "IsPhaseEnding", isPhaseEnding },
                    { "CharacterAttributeManager", characterAttributeManager },
                    { "BattleAttributeManager", _battleAttributeManager }
                });
            
                foreach (var buff in characterAttributeManager.GetBuffs())
                {
                    if(buff is not null)
                        _buffManager.AddBuff(buff, 1, false, false);
                }
            
                InitializeTurn();
            });
        }
        
        public void SetShouldNextCardPlayTwice(bool value)
        {
            if(value == false)
                VDebug.Log("");
            _shouldNextCardPlayTwice = value;
        }
        
        public void NextCardPlayTwice()
        {
            SetShouldNextCardPlayTwice(true);
            
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
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnPopularityChange, OnPopularityChange);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnShieldModifierChanged, OnShieldModifierChanged);
        }
        
        protected override void OnDisable()
        {
            base.OnDisable();
            
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
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnPopularityChange, OnPopularityChange);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnShieldModifierChanged, OnShieldModifierChanged);
        }
        
        private void OnShieldModifierChanged(Dictionary<string, object> messagedict)
        {
            if (_cardPilesManager is null)
                return;
            foreach (var card in _cardPilesManager.HandPile)
            {
                card.PreviewShield(this, false);
            }
        }
        
        private void OnPopularityChange(Dictionary<string, object> messagedict)
        {
            if (_cardPilesManager is null)
                return;
            int value = (int)messagedict["NewValue"];
            if (!_isPhaseEnding)
            {
                if (value >= _extraTargetPopularity)
                {
                    _shouldEndBattle = true;
                }
            }
        }
        
        private void OnParameterPopularityModifierChanged(Dictionary<string, object> messagedict)
        {
            if (_cardPilesManager is null)
                return;
            foreach (var card in _cardPilesManager.HandPile)
            {
                card.PreviewPopularity(this, false);
            }
        }
        
        private void OnAttributeValueChange(Dictionary<string, object> messagedict)
        {
            if (_cardPilesManager is null)
                return;
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
            
            card.TestCondition(this);
            card.PreviewPopularity(this, true);
            card.PreviewShield(this, true);
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
                card.TestCondition(this);
                card.PreviewPopularity(this, false);
            } 
        }
        
        private void OnStaminaChange(Dictionary<string, object> messagedict)
        {
            foreach (var card in _cardPilesManager.HandPile)
            {
                if(card.CostType == CostType.Stamina)
                    card.TestCondition(this);
                if(card.CostType == CostType.TrueStamina)
                    card.TestCondition(this);
            }
        }
        
        private void OnSkipTurnClicked(Dictionary<string, object> messagedict)
        {
            EndTurn();
            if(_battleAttributeManager is not null)
                _battleAttributeManager.SkipTurnRecoverStamina();
        }
        
        private void OnPlayTheSecondTime(Dictionary<string, object> messagedict)
        {
            SetShouldNextCardPlayTwice(false);
            VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnCardUsed,
                new Dictionary<string, object>()
                {
                    { "Card", _playTwiceCard },
                    { "IsPlayTwice", true }
                });
            if(_playTwiceCard is not null && _playTwiceMessageDict is not null)
                ApplyCardEffects( _playTwiceCard, _playTwiceMessageDict);
            
            _playTwiceCard = null;
            _playTwiceMessageDict = null;
        }
        
        private void OnCardMovedToPlayPosition(Dictionary<string, object> messagedict)
        {
            var card = messagedict["Card"] as VCard;
            if (card is null)
                return;
            
            ApplyCardEffects(card, messagedict);
        }
        
        private void OnCardUsed(Dictionary<string, object> messagedict)
        {
            if (messagedict.TryGetValue("IsPlayTwice", out object value))
            {
                if ((bool)value)
                {
                    return;
                }
            }
            _playLeftAttribute.AddTo(-1, false);
            VDebug.Log("剩余可行动次数: " + PlayLeft);
            if (PlayLeft <= 0)
            {
                EndTurn();
                if (_shouldRedraw) _shouldRedraw = false;
            }

            if (_shouldEndBattle)
            {
                _shouldEndBattle = false;
                EndBattle();
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
            if (TurnLeft <= 0)
            {
                EndBattle();
                return;
            }
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
            
        }
        
        public int CalculateAbilityGain(int popularity)
        {
            int attributeGain = 0;
            if (popularity >= _extraTargetPopularity)
            {
                attributeGain = _abiliyBonus;
                return attributeGain;
            }
            if (popularity >= _targetPopularity)
            {
                attributeGain = Mathf.CeilToInt((_abiliyBonus * 0.5f + _abiliyBonus * 0.5f * 
                    (popularity - _targetPopularity) / (_extraTargetPopularity - _targetPopularity)));
            }
            return attributeGain;
        }

        public string GetAbilityKey(int index)
        {
            
            if(index == 0) return "CASingingAbility";
            else if(index == 1) return "CAGamingAbility";
            else return "CAChattingAbility";
        }
        
        public string GetBattleAbilityKey(int index)
        {
            if(index == 0) return "BASingingMultiplier";
            else if(index == 1) return "BAGamingMultiplier";
            else return "BAChattingMultiplier";
        }
        
        private void EndBattle()
        {
            _battleAttributeManager.TryGetAttribute("BAPopularity", out var battleAttribute);
            var popularityAttribute = battleAttribute as VBattlePopularityAttribute;
            if (!_isPhaseEnding)
            {
                var attributeGain = CalculateAbilityGain(popularityAttribute.Value);
                
                string attributeKey = GetAbilityKey(_mainAttributeIndex);
                
                _characterAttributeManager.TryGetAttribute(attributeKey, out var attribute);
                attribute.AddTo(attributeGain);
            }
            else
            {
                string attributeKey = GetAbilityKey(_mainAttributeIndex);
                _characterAttributeManager.TryGetAttribute(attributeKey, out var ability);
                ability.AddTo((int)_decayCurves[0].Evaluate(popularityAttribute.ScoreForAbilities[GetBattleAbilityKey(_mainAttributeIndex)]));

                int index1, index2;
                if (_mainAttributeIndex == 0)
                {
                    index1 = 1;
                    index2 = 2;
                }
                else if (_mainAttributeIndex == 1)
                {
                    index1 = 0;
                    index2 = 2;
                }
                else
                {
                    index1 = 0;
                    index2 = 1;
                }
                
                _characterAttributeManager.TryGetAttribute(GetAbilityKey(index1), out var ability1);
                _characterAttributeManager.TryGetAttribute(GetAbilityKey(index2), out var ability2);
                if(_abilityTurnCounts[index1] <= _abilityTurnCounts[index2])
                {
                    ability1.AddTo((int)_decayCurves[2].Evaluate(popularityAttribute.ScoreForAbilities[GetBattleAbilityKey(index1)]));
                    ability2.AddTo((int)_decayCurves[1].Evaluate(popularityAttribute.ScoreForAbilities[GetBattleAbilityKey(index2)]));
                }
                else if(_abilityTurnCounts[index1] > _abilityTurnCounts[index2])
                {
                    ability1.AddTo((int)_decayCurves[1].Evaluate(popularityAttribute.ScoreForAbilities[GetBattleAbilityKey(index1)]));
                    ability2.AddTo((int)_decayCurves[2].Evaluate(popularityAttribute.ScoreForAbilities[GetBattleAbilityKey(index2)]));
                }

            }
                
            _characterAttributeManager.ConvertToCharacterAttributes(_battleAttributeManager.BattleAttributes);
                
            VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnBattleEnd, new Dictionary<string, object>
            {
                { "TurnLeft", TurnLeft },
                { "CharacterAttributeManager", _characterAttributeManager },
                { "BattleAttributeManager", _battleAttributeManager },
                { "ReachedTarget", popularityAttribute.Value >= _targetPopularity },
                { "ReachedExtraTarget", popularityAttribute.Value >= _extraTargetPopularity },
            });
            
            _buffManager.Clear();
            _battleAttributeManager.Clear();
            _cardPilesManager.Clear();

            _battleAttributeManager.OnDisable();
            _cardPilesManager.OnDisable();
            _buffManager.OnDisable();
            
            _cardPilesManager = null;
            _buffManager = null;
            _battleAttributeManager = null;
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
                SetShouldNextCardPlayTwice(false);
        }

        private void ApplyCardEffects(VCard card, Dictionary<string, object> messagedict)
        {
            if (!cardTypeHistory.TryAdd(card.CardType, 1))
            {
                cardTypeHistory[card.CardType]++;
            }

            List<VEffect> effects = card.Effects;
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
                _playTwiceCard = card;
                _playTwiceMessageDict = messagedict;
            }
            bool effectApplied = false;
            bool tempShouldPlayTwice = _shouldNextCardPlayTwice;
            foreach (var effect in effects)
            {
                if (!effect.CanApply(this, messagedict))
                    continue;
                effectApplied = true;
                effect.ApplyEffect(this, 1, true, tempShouldPlayTwice);
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