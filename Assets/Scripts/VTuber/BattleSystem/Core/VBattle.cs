using System;
using System.Collections;
using System.Collections.Generic;
using SlayTheSpire.System.SavingSystem;
using UnityEngine;
using VTuber.BattleSystem.BattleAttribute;
using VTuber.BattleSystem.Buff;
using VTuber.BattleSystem.Card;
using VTuber.BattleSystem.Effect.Conditions;
using VTuber.Character;
using VTuber.Character.Attributes;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;
using VTuber.Dialogue.UI;
using VTuber.Relic;
using VTuber.ScheduleSystem.UI;

namespace VTuber.BattleSystem.Core
{
    public class VBattleSaveData
    {
        public int abilityBonus;
        public List<int> abilityTurnCounts;
        public VBattleAttributeManagerSaveData attributeManagerSaveData;
        public bool battleEnded;
        public int battleLookUpIDDistributor;
        public VBattleRelicManagerSaveData battleRelicManagerSaveData;
        public VBuffManagerSaveData buffManagerSaveData;
        public VCardPilesManagerSaveData cardPilesManagerSaveData;
        public Dictionary<string, int> cardTypeHistory;
        public int extraTargetPopularity;
        public bool isPhaseEnding;
        public int mainAttributeIndex;
        public uint playTwiceCard;
        public Dictionary<string, object> playTwiceMessageDict;
        public bool shouldEndBattle;

        public bool shouldPlayNextCardTwice;
        public int targetPopularity;
    }

    public class VBattle : VSingletonMonobehaviour<VBattle>
    {
        [SerializeField] protected VBattleConfiguration configuration;

        protected bool _shouldNextCardPlayTwice;
        protected bool _shouldRedraw;

        protected VCard _playTwiceCard;
        protected Dictionary<string, object> _playTwiceMessageDict;
        protected VCharacterAttributeManager _characterAttributeManager;
        protected bool paused;
        protected int _targetPopularity;
        protected int _extraTargetPopularity;
        protected int _abilityBonus;
        protected bool _isPhaseEnding;
        private int _mainAttributeIndex;

        private List<AnimationCurve> _decayCurves;
        private List<int> _abilityTurnCounts;
        private bool _shouldEndBattle;
        private bool _battleEnded;
        private bool _isDebugScene;
        private bool _initialized;
        protected Dictionary<string, int> cardTypeHistory;

        private List<VAttributeCondition> _tutorialConditions;

        public int TurnLeft => _turnAttribute.Value;
        public int PlayLeft => _playLeftAttribute.Value;

        public Dictionary<string, int> CardTypeHistory => cardTypeHistory;

        protected override void OnEnable()
        {
            base.OnEnable();

            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnBuffAdded, OnBuffAdded);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnBuffValueUpdated, OnBuffValueUpdated);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnCardPlayed, OnCardPlayed);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnStaminaChange, OnStaminaChange);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnNotifyTurnBeginDelay,
                OnNotifyTurnBeginDelay);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnCardUsed, OnCardUsed);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnCardMovedToPlayPosition,
                OnCardMovedToPlayPosition);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnPlayTheSecondTime, OnPlayTheSecondTime);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnSkipTurnClicked, OnSkipTurnClicked);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnCardMovedToHandSlot,
                OnCardMovedToHandSlot);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnRequestPickCardsFromPile,
                OnRequestPickCardsFromPile);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnAttributeValueChange,
                OnAttributeValueChange);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnParameterPopularityModifierChanged,
                OnParameterPopularityModifierChanged);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnPopularityChange, OnPopularityChange);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnShieldModifierChanged,
                OnShieldModifierChanged);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnRestartBattle, ReloadBattle);

            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnReset, OnSwitchToMainMenu);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnBuffAdded, OnBuffAdded);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnBuffValueUpdated, OnBuffValueUpdated);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnCardPlayed, OnCardPlayed);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnStaminaChange, OnStaminaChange);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnNotifyTurnBeginDelay,
                OnNotifyTurnBeginDelay);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnCardUsed, OnCardUsed);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnCardMovedToPlayPosition,
                OnCardMovedToPlayPosition);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnPlayTheSecondTime, OnPlayTheSecondTime);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnSkipTurnClicked, OnSkipTurnClicked);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnCardMovedToHandSlot,
                OnCardMovedToHandSlot);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnRequestPickCardsFromPile,
                OnRequestPickCardsFromPile);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnAttributeValueChange,
                OnAttributeValueChange);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnParameterPopularityModifierChanged,
                OnParameterPopularityModifierChanged);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnPopularityChange, OnPopularityChange);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnShieldModifierChanged,
                OnShieldModifierChanged);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnShieldModifierChanged,
                OnShieldModifierChanged);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnRestartBattle, ReloadBattle);

            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnReset, OnSwitchToMainMenu);
        }

        public VBattleSaveData Save()
        {
            if (!_initialized)
                return new VBattleSaveData();
            var saveData = new VBattleSaveData();
            saveData.battleLookUpIDDistributor = VBattleLookUpTables.Instance.IDDistributor;
            saveData.attributeManagerSaveData = _battleAttributeManager.Save();
            saveData.cardPilesManagerSaveData = _cardPilesManager.Save();
            saveData.buffManagerSaveData = _buffManager.Save();
            saveData.battleRelicManagerSaveData = BattleRelicManager.Save();

            saveData.shouldPlayNextCardTwice = _shouldNextCardPlayTwice;
            saveData.targetPopularity = _targetPopularity;
            saveData.extraTargetPopularity = _extraTargetPopularity;
            saveData.abilityBonus = _abilityBonus;
            saveData.isPhaseEnding = _isPhaseEnding;
            saveData.mainAttributeIndex = _mainAttributeIndex;
            saveData.abilityTurnCounts = _abilityTurnCounts;
            saveData.shouldEndBattle = _shouldEndBattle;
            saveData.battleEnded = _battleEnded;

            if (_playTwiceCard is not null)
                saveData.playTwiceCard = _playTwiceCard.Id;

            saveData.playTwiceMessageDict = _playTwiceMessageDict;
            saveData.cardTypeHistory = cardTypeHistory;
            return saveData;
        }

        public void InitializeBattle(VBattleSaveData saveData, List<AnimationCurve> decayCurves,
            VCharacterAttributeManager characterAttributeManager,
            VCardLibrary cardLibrary)
        {
            VRaisingUI.Instance.SwitchAttributesUIBattle(false);
            VEventSystemUI.Instance.OpenBattleUI();

            VBattleLookUpTables.Instance.Initialize(saveData);

            _mainAttributeIndex = saveData.mainAttributeIndex;
            _isPhaseEnding = saveData.isPhaseEnding;
            cardTypeHistory = saveData.cardTypeHistory;
            _targetPopularity = saveData.targetPopularity;
            _extraTargetPopularity = saveData.extraTargetPopularity;
            _decayCurves = decayCurves;
            _abilityBonus = saveData.abilityBonus;
            _abilityTurnCounts = saveData.abilityTurnCounts;
            _characterAttributeManager = characterAttributeManager;

            _shouldNextCardPlayTwice = saveData.shouldPlayNextCardTwice;
            _shouldEndBattle = saveData.shouldEndBattle;
            _battleEnded = saveData.battleEnded;

            VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnBattleUIInitialize, new Dictionary<string, object>
            {
                { "TargetPopularity", _targetPopularity },
                { "ExtraTargetPopularity", _extraTargetPopularity },
                { "IsPhaseEnding", _isPhaseEnding }
            });

            VEventSystemUI.Instance.PlayVideo(() =>
            {
                _battleAttributeManager =
                    new VBattleAttributeManager(_isPhaseEnding, saveData.attributeManagerSaveData);
                _buffManager = new VBuffManager(this, saveData.buffManagerSaveData);
                _cardPilesManager = new VCardPilesManager(configuration.handSize, configuration.maxHandSize,
                    cardLibrary,
                    null, null, saveData.cardPilesManagerSaveData);

                _turnAttribute = _battleAttributeManager.BattleAttributes["BATurn"] as VBattleTurnAttribute;
                _playLeftAttribute = _battleAttributeManager.BattleAttributes["BAPlayLeft"] as VBattlePlayLeftAttribute;
                BattleRelicManager = new VBattleRelicManager(this, saveData.battleRelicManagerSaveData);
                _initialized = true;

                if (saveData.playTwiceCard != 0)
                    _playTwiceCard = _cardPilesManager.GetCardById(saveData.playTwiceCard);
                _playTwiceMessageDict = saveData.playTwiceMessageDict;

                _battleAttributeManager.OnEnable();
                _cardPilesManager.OnEnable();
                _buffManager.OnEnable();

                VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnBattleBegin, new Dictionary<string, object>
                {
                    { "IsLoadGame", true },
                    { "TurnLeft", TurnLeft },
                    { "TargetPopularity", _targetPopularity },
                    { "ExtraTargetPopularity", _extraTargetPopularity },
                    { "IsPhaseEnding", _isPhaseEnding },
                    { "CharacterAttributeManager", characterAttributeManager },
                    { "BattleAttributeManager", _battleAttributeManager }
                });
                InitializeTurn();
            });
        }

        public virtual void InitializeBattle(bool isDebugScene, bool isPhaseEnding,
            VCharacterAttributeManager characterAttributeManager,
            VCardLibrary cardLibrary, int initialTurnCount, int mainAttributeIndex, List<int> abilityTurnCounts,
            List<AnimationCurve> decayCurves,
            int targetPopularity, int extraTargetPopularity, int abilityBonus, int initialViewers,
            List<VBattleRelic> relics,
            bool isTutorial = false, List<VAttributeCondition> tutorialConditions = null,
            List<uint> tutorialDeck = null, Dictionary<int, List<uint>> tutorialTurnHandCards = null)
        {
            _initialized = true;
            _isDebugScene = isDebugScene;
            _battleEnded = false;

            if (!isDebugScene)
            {
                VRaisingUI.Instance.SwitchAttributesUIBattle(false);
                VEventSystemUI.Instance.OpenBattleUI();
            }

            VBattleLookUpTables.Instance.Initialize(null);

            _mainAttributeIndex = mainAttributeIndex;
            _isPhaseEnding = isPhaseEnding;
            cardTypeHistory = new Dictionary<string, int>();
            _targetPopularity = targetPopularity;
            _extraTargetPopularity = extraTargetPopularity;
            _decayCurves = decayCurves;
            _abilityBonus = abilityBonus;
            _abilityTurnCounts = abilityTurnCounts;
            _characterAttributeManager = characterAttributeManager;
            _battleAttributeManager = new VBattleAttributeManager(isPhaseEnding, null);
            _cardPilesManager = new VCardPilesManager(configuration.handSize, configuration.maxHandSize, cardLibrary,
                tutorialDeck, tutorialTurnHandCards, null);
            _buffManager = new VBuffManager(this);
            _tutorialConditions = tutorialConditions;

            _battleAttributeManager.OnEnable();
            _cardPilesManager.OnEnable();
            _buffManager.OnEnable();


            if (isDebugScene)
            {
                InitializeLogic(isPhaseEnding, initialTurnCount, initialViewers, relics,
                    mainAttributeIndex, abilityTurnCounts, targetPopularity, extraTargetPopularity,
                    characterAttributeManager);
                return;
            }

            VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnBattleUIInitialize, new Dictionary<string, object>
            {
                { "TargetPopularity", _targetPopularity },
                { "ExtraTargetPopularity", _extraTargetPopularity },
                { "IsPhaseEnding", _isPhaseEnding }
            });

            VEventSystemUI.Instance.PlayVideo(() =>
            {
                _initialized = true;
                InitializeLogic(isPhaseEnding, initialTurnCount, initialViewers, relics,
                    mainAttributeIndex, abilityTurnCounts, targetPopularity, extraTargetPopularity,
                    characterAttributeManager);
                if (isTutorial) VDataPersistenceManager.Instance.SaveGameTutorialBattle();
            });
        }

        public void InitializeLogic(bool isPhaseEnding, int initialTurnCount, int initialViewers,
            List<VBattleRelic> relics, int mainAttributeIndex, List<int> abilityTurnCounts,
            int targetPopularity, int extraTargetPopularity, VCharacterAttributeManager characterAttributeManager)
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

            _battleAttributeManager.AddAttribute("BAShield",
                new VBattleStaminaAttribute(0, VBattleEventKey.OnShieldChange, true));
            _battleAttributeManager.AddAttribute("BARevenue",
                new VBattleStaminaAttribute(0, VBattleEventKey.OnRevenueChange));

            _battleAttributeManager.AddAttribute("BAPopularity", new VBattlePopularityAttribute(0));
            _battleAttributeManager.AddAttribute("BAParameter", new VBattleParameterAttribute(0));

            BattleRelicManager = new VBattleRelicManager(this, relics);
            if (_battleAttributeManager.TryGetAttribute("BAViewerCount", out var viewerCountAttribute))
                viewerCountAttribute.AddTo(initialViewers, false);
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
                if (buff is not null)
                    _buffManager.AddBuff(buff, 1, false, false);

            InitializeTurn();
        }

        public void SetShouldNextCardPlayTwice(bool value)
        {
            if (value == false)
                VDebug.Log("");
            _shouldNextCardPlayTwice = value;
        }

        public void NextCardPlayTwice()
        {
            SetShouldNextCardPlayTwice(true);

            VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnNotifyBeginDisposeCard,
                new Dictionary<string, object>());
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
                { "Paused", paused }
            });
        }

        private void OnSwitchToMainMenu(Dictionary<string, object> messagedict)
        {
            VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnBattleEnd, new Dictionary<string, object>
            {
                { "IsReturnToMainMenu", true }
            });

            _initialized = false;
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

        private void OnShieldModifierChanged(Dictionary<string, object> messagedict)
        {
            if (_cardPilesManager is null)
                return;
            foreach (var card in _cardPilesManager.HandPile)
            {
                if (card is null)
                    continue;
                card.PreviewShield(this, false);
            }
        }

        private void OnPopularityChange(Dictionary<string, object> messagedict)
        {
            if (_cardPilesManager is null)
                return;
            var value = (int)messagedict["NewValue"];
            if (!_isPhaseEnding)
                if (value >= _extraTargetPopularity)
                    _shouldEndBattle = true;
        }

        private void OnParameterPopularityModifierChanged(Dictionary<string, object> messagedict)
        {
            if (_cardPilesManager is null)
                return;
            foreach (var card in _cardPilesManager.HandPile) card.PreviewPopularity(this, false);
        }

        private void OnAttributeValueChange(Dictionary<string, object> messagedict)
        {
            if (_cardPilesManager is null)
                return;
            foreach (var card in _cardPilesManager.HandPile)
            {
                if (card is null)
                    continue;
                card.TestCondition(this);
            }
        }


        private void OnRequestPickCardsFromPile(Dictionary<string, object> messagedict)
        {
            var cardCount = (int)messagedict["CardCount"];
            if (_cardPilesManager.HandPile.Count + cardCount > configuration.maxHandSize)
                cardCount = configuration.maxHandSize - _cardPilesManager.HandPile.Count;

            if (cardCount <= 0)
                return;

            messagedict["CardCount"] = cardCount;
            VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnBeginPickCardsFromPile, messagedict);
        }


        private void OnCardMovedToHandSlot(Dictionary<string, object> messagedict)
        {
            var card = messagedict["Card"] as VCard;

            card.TestCondition(this);
            card.PreviewPopularity(this, true);
            card.PreviewShield(this, true);
        }

        private void OnBuffValueUpdated(Dictionary<string, object> messagedict)
        {
            if (_cardPilesManager is null)
                return;
            foreach (var card in _cardPilesManager.HandPile)
            {
                if (card is null)
                    continue;
                if (card.CostType == CostType.Buff)
                    card.setPlayable?.Invoke(_buffManager.TestCost(card.CostBuffId, card.Cost));
                card.TestCondition(this);
                card.PreviewPopularity(this, false);
            }
        }

        private void OnBuffAdded(Dictionary<string, object> messagedict)
        {
            if (_cardPilesManager is null)
                return;
            foreach (var card in _cardPilesManager.HandPile)
            {
                if (card is null)
                    continue;
                card.TestCondition(this);
                card.PreviewPopularity(this, false);
            }
        }

        private void OnStaminaChange(Dictionary<string, object> messagedict)
        {
            if (_cardPilesManager is null)
                return;
            foreach (var card in _cardPilesManager.HandPile)
            {
                if (card is null)
                    continue;
                if (card.CostType == CostType.Stamina)
                    card.TestCondition(this);
                if (card.CostType == CostType.TrueStamina)
                    card.TestCondition(this);
            }
        }

        private void OnSkipTurnClicked(Dictionary<string, object> messagedict)
        {
            EndTurn();
            if (_battleAttributeManager is not null)
                _battleAttributeManager.SkipTurnRecoverStamina();
        }

        private void OnPlayTheSecondTime(Dictionary<string, object> messagedict)
        {
            SetShouldNextCardPlayTwice(false);
            VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnCardUsed,
                new Dictionary<string, object>
                {
                    { "Card", _playTwiceCard },
                    { "IsPlayTwice", true }
                });
            if (_playTwiceCard is not null && _playTwiceMessageDict is not null)
                ApplyCardEffects(_playTwiceCard, _playTwiceMessageDict);

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
            if (messagedict.TryGetValue("IsPlayTwice", out var value))
                if ((bool)value)
                    return;

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

        private IEnumerator DelayInitializeTurn(float delayTime)
        {
            yield return new WaitForSeconds(delayTime);

            InitializeTurn();
        }

        private protected void InitializeTurn()
        {
            if (TurnLeft <= 0)
            {
                EndBattle();
                return;
            }

            if (!_isDebugScene)
                VDataPersistenceManager.Instance.SaveGame();
            VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnTurnBegin, new Dictionary<string, object>
            {
                { "TurnLeft", TurnLeft },
                { "TurnIndex", _turnAttribute.TurnIndex },
                { "HandSize", configuration.maxHandSize }
            });
            VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnTurnBeginBuffApply,
                new Dictionary<string, object>());
        }

        private void EndTurn()
        {
            VDebug.Log("回合结束: " + TurnLeft);
            _turnAttribute.AddTo(-1, false);
            VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnTurnEndBuffApply, new Dictionary<string, object>
            {
                { "TurnLeft", TurnLeft }
            });

            VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnTurnResolution, new Dictionary<string, object>
            {
                { "TurnLeft", TurnLeft }
            });

            VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnTurnEnd, new Dictionary<string, object>
            {
                { "TurnLeft", TurnLeft }
            });
        }

        private int CalculateAbilityGain(int popularity)
        {
            var attributeGain = 0;
            if (popularity >= _extraTargetPopularity)
            {
                attributeGain = _abilityBonus;
                return attributeGain;
            }

            if (popularity >= _targetPopularity)
                attributeGain = Mathf.CeilToInt(_abilityBonus * 0.5f + _abilityBonus * 0.5f *
                    (popularity - _targetPopularity) / (_extraTargetPopularity - _targetPopularity));
            return attributeGain;
        }

        private string GetAbilityKey(int index)
        {
            if (index == 0) return "CASingingAbility";
            if (index == 1) return "CAGamingAbility";
            return "CAChattingAbility";
        }

        private string GetBattleAbilityKey(int index)
        {
            if (index == 0) return "BASingingMultiplier";
            if (index == 1) return "BAGamingMultiplier";
            return "BAChattingMultiplier";
        }

        private void EndBattle()
        {
            if (_battleEnded)
                return;
            if (_isDebugScene)
            {
                VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnBattleEnd, new Dictionary<string, object>());
                _buffManager.Clear();
                _battleAttributeManager.Clear();
                _cardPilesManager.Clear();

                _battleAttributeManager.OnDisable();
                _cardPilesManager.OnDisable();
                _buffManager.OnDisable();

                _cardPilesManager = null;
                _buffManager = null;
                _battleAttributeManager = null;
                return;
            }


            var isTutorialConditionsSatisfied = TestTutorialConditions();

            _battleEnded = true;
            _battleAttributeManager.TryGetAttribute("BAPopularity", out var battleAttribute);
            var popularityAttribute = battleAttribute as VBattlePopularityAttribute;
            if (!_isPhaseEnding)
            {
                var attributeGain = CalculateAbilityGain(popularityAttribute.Value);

                var attributeKey = GetAbilityKey(_mainAttributeIndex);

                _characterAttributeManager.TryGetAttribute(attributeKey, out var attribute);
                if (attribute is VAbilityAttribute abilityAttribute) abilityAttribute.AddAbility(attributeGain, true);
            }
            else
            {
                var attributeKey = GetAbilityKey(_mainAttributeIndex);
                _characterAttributeManager.TryGetAttribute(attributeKey, out var attribute);
                if (attribute is VAbilityAttribute abilityAttribute)
                    abilityAttribute.AddAbility(
                        (int)_decayCurves[0]
                            .Evaluate(popularityAttribute.ScoreForAbilities[GetBattleAbilityKey(_mainAttributeIndex)]),
                        true);
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
                if (_abilityTurnCounts[index1] <= _abilityTurnCounts[index2])
                {
                    if (ability1 is VAbilityAttribute abilityAttribute1)
                        abilityAttribute1.AddAbility(
                            (int)_decayCurves[2]
                                .Evaluate(popularityAttribute.ScoreForAbilities[GetBattleAbilityKey(index1)]), true);
                    if (ability2 is VAbilityAttribute abilityAttribute2)
                        abilityAttribute2.AddAbility(
                            (int)_decayCurves[1]
                                .Evaluate(popularityAttribute.ScoreForAbilities[GetBattleAbilityKey(index2)]), true);
                }
                else if (_abilityTurnCounts[index1] > _abilityTurnCounts[index2])
                {
                    if (ability1 is VAbilityAttribute abilityAttribute1)
                        abilityAttribute1.AddAbility(
                            (int)_decayCurves[1]
                                .Evaluate(popularityAttribute.ScoreForAbilities[GetBattleAbilityKey(index1)]), true);
                    if (ability2 is VAbilityAttribute abilityAttribute2)
                        abilityAttribute2.AddAbility(
                            (int)_decayCurves[2]
                                .Evaluate(popularityAttribute.ScoreForAbilities[GetBattleAbilityKey(index2)]), true);
                }
            }

            if (isTutorialConditionsSatisfied)
                _characterAttributeManager.ConvertToCharacterAttributes(_battleAttributeManager.BattleAttributes);

            VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnBattleEnd, new Dictionary<string, object>
            {
                { "IsTutorial", _tutorialConditions != null },
                { "IsTutorialConditionsSatisfied", isTutorialConditionsSatisfied },
                { "TurnLeft", TurnLeft },
                { "CharacterAttributeManager", _characterAttributeManager },
                { "BattleAttributeManager", _battleAttributeManager },
                { "ReachedTarget", popularityAttribute.Value >= _targetPopularity },
                { "ReachedExtraTarget", popularityAttribute.Value >= _extraTargetPopularity }
            });

            _initialized = false;
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

        private void ReloadBattle(Dictionary<string, object> messagedict)
        {
            var save = VDataPersistenceManager.Instance.LoadTutorialBattleSave();

            InitializeBattle(save.battleSaveData, _decayCurves, _characterAttributeManager, null);
        }

        private bool TestTutorialConditions()
        {
            if (_tutorialConditions is not null)
            {
                foreach (var condition in _tutorialConditions)
                    if (!condition.IsTrue(this, null))
                        return false;

                return true;
            }

            return true;
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
                { "ShouldPlayTwice", _shouldNextCardPlayTwice }
            });

            if (_shouldNextCardPlayTwice)
                SetShouldNextCardPlayTwice(false);
        }

        private void ApplyCardEffects(VCard card, Dictionary<string, object> messagedict)
        {
            if (!cardTypeHistory.TryAdd(card.CardType, 1)) cardTypeHistory[card.CardType]++;

            var effects = card.Effects;
            if (effects is null || effects.Count == 0)
            {
                VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnNotifyBeginDisposeCard,
                    new Dictionary<string, object>());
                return;
            }

            if (_shouldNextCardPlayTwice)
            {
                _playTwiceCard = card;
                _playTwiceMessageDict = messagedict;
            }

            var effectApplied = false;
            var tempShouldPlayTwice = _shouldNextCardPlayTwice;
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
                    new Dictionary<string, object>());
                return;
            }

            if (!_shouldRedraw) return;
            _shouldRedraw = false;
            if (PlayLeft == 0)
                return;
            Redraw();
        }

        #region Managers

        public VBattleAttributeManager BattleAttributeManager => _battleAttributeManager;
        protected VBattleAttributeManager _battleAttributeManager;

        public VCardPilesManager CardPilesManager => _cardPilesManager;
        protected VCardPilesManager _cardPilesManager;

        public VBuffManager BuffManager => _buffManager;
        protected VBuffManager _buffManager;

        public VBattleRelicManager BattleRelicManager { get; private set; }

        #endregion

        #region Attributes

        protected VBattleTurnAttribute _turnAttribute;

        protected VBattlePlayLeftAttribute _playLeftAttribute;
        // private VBattlePopularityAttribute _popularityAttribute;
        // private VBattleParameterAttribute _parameterAttribute;
        // private VBattleAttribute _shieldAttribute;

        #endregion
    }
}