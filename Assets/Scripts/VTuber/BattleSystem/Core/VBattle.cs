using System;
using System.Collections;
using System.Collections.Generic;
using SlayTheSpire.System.SavingSystem;
using Tutorial.Script;
using UnityEngine;
using VTuber.BattleSystem.BattleAttribute;
using VTuber.BattleSystem.Buff;
using VTuber.BattleSystem.Card;
using VTuber.BattleSystem.Effect;
using VTuber.BattleSystem.Effect.Conditions;
using VTuber.Character;
using VTuber.Character.Attributes;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;
using VTuber.Core.Managers;
using VTuber.Core.SE;
using VTuber.Dialogue.UI;
using VTuber.Relic;
using VTuber.ScheduleSystem.Events.DialogueEvent;
using VTuber.ScheduleSystem.UI;

namespace VTuber.BattleSystem.Core
{
    [Serializable]
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
        public uint eventID;
    }
    


    public class VBattle : VSingletonMonobehaviour<VBattle>
    {    
        [SerializeField] protected VBattleConfiguration configuration;

        protected bool shouldNextCardPlayTwice;
        protected bool shouldRedraw;

        protected VCard playTwiceCard;
        protected Dictionary<string, object> playTwiceMessageDict;
        protected VCharacterAttributeManager characterAttributeManager;
        protected bool paused;
        protected int targetPopularity;
        protected int extraTargetPopularity;
        protected int abilityBonus;
        protected bool isPhaseEnding;
        private int _mainAttributeIndex;

        private List<AnimationCurve> _decayCurves;
        private List<int> _abilityTurnCounts;
        private bool _shouldEndBattle;
        private bool _battleEnded;
        private bool _isDebugScene;
        private bool _initialized;
        protected Dictionary<string, int> cardTypeHistory;

        private List<VAttributeCondition> _tutorialConditions;

        public int TurnLeft => turnAttribute.Value;
        public int PlayLeft => playLeftAttribute.Value;

        public Dictionary<string, int> CardTypeHistory => cardTypeHistory;

        #region Managers

        public VBattleAttributeManager BattleAttributeManager => battleAttributeManager;
        protected VBattleAttributeManager battleAttributeManager;

        public VCardPilesManager CardPilesManager => cardPilesManager;
        protected VCardPilesManager cardPilesManager;

        public VBuffManager BuffManager => buffManager;
        protected VBuffManager buffManager;

        public VBattleRelicManager BattleRelicManager { get; private set; }

        #endregion

        #region Attributes

        protected VBattleTurnAttribute turnAttribute;

        protected VBattlePlayLeftAttribute playLeftAttribute;

        // private VBattlePopularityAttribute _popularityAttribute;
        // private VBattleParameterAttribute _parameterAttribute;
        // private VBattleAttribute _shieldAttribute;

        #endregion
        
        private bool _isTutorial;
        private VTipConfig _tipConfig;
        private uint _eventID;
        
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
            saveData.attributeManagerSaveData = battleAttributeManager.Save();
            saveData.cardPilesManagerSaveData = cardPilesManager.Save();
            saveData.buffManagerSaveData = buffManager.Save();
            saveData.battleRelicManagerSaveData = BattleRelicManager.Save();

            saveData.shouldPlayNextCardTwice = shouldNextCardPlayTwice;
            saveData.targetPopularity = targetPopularity;
            saveData.extraTargetPopularity = extraTargetPopularity;
            saveData.abilityBonus = abilityBonus;
            saveData.isPhaseEnding = isPhaseEnding;
            saveData.mainAttributeIndex = _mainAttributeIndex;
            saveData.abilityTurnCounts = _abilityTurnCounts;
            saveData.shouldEndBattle = _shouldEndBattle;
            saveData.battleEnded = _battleEnded;

            if (playTwiceCard is not null)
                saveData.playTwiceCard = playTwiceCard.Id;

            saveData.playTwiceMessageDict = playTwiceMessageDict;
            saveData.cardTypeHistory = cardTypeHistory;
            saveData.eventID = _eventID;
            return saveData;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public void InitializeBattle(VBattleSaveData saveData, List<AnimationCurve> decayCurves,
            VCharacterAttributeManager characterAttributeManager,
            VCardLibrary cardLibrary, VTipConfig tipConfig)
        {
            VBattleLookUpTables.Instance.Initialize(saveData);

            _mainAttributeIndex = saveData.mainAttributeIndex;
            isPhaseEnding = saveData.isPhaseEnding;
            cardTypeHistory = saveData.cardTypeHistory;
            targetPopularity = saveData.targetPopularity;
            extraTargetPopularity = saveData.extraTargetPopularity;
            _decayCurves = decayCurves;
            abilityBonus = saveData.abilityBonus;
            _abilityTurnCounts = saveData.abilityTurnCounts;
            this.characterAttributeManager = characterAttributeManager;

            shouldNextCardPlayTwice = saveData.shouldPlayNextCardTwice;
            _shouldEndBattle = saveData.shouldEndBattle;
            _battleEnded = saveData.battleEnded;

            _isTutorial = _tutorialConditions is not null;
            _tipConfig = tipConfig;
            _eventID = saveData.eventID;

            VEventSystemUI.Instance.PlayLoadingAnimation(VDataManager.Instance.GetStreamEventConfigurationByID(_eventID), () =>
            {
                VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnBattleBegin, new Dictionary<string, object>
                {
                    { "IsLoadGame", true },
                    { "TurnLeft", TurnLeft },
                    { "TargetPopularity", targetPopularity },
                    { "ExtraTargetPopularity", extraTargetPopularity },
                    { "IsPhaseEnding", isPhaseEnding },
                    { "CharacterAttributeManager", characterAttributeManager },
                    { "BattleAttributeManager", battleAttributeManager }
                });
                InitializeTurn(true);
            },
                () =>
            {
                VRaisingUI.Instance.SwitchAttributesUIBattle(false);
                VEventSystemUI.Instance.OpenBattleUI();
                
                VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnBattleUIInitialize, new Dictionary<string, object>
                {
                    { "TargetPopularity", targetPopularity },
                    { "ExtraTargetPopularity", extraTargetPopularity },
                    { "IsPhaseEnding", isPhaseEnding },
                    { "TipConfig", tipConfig }
                });
                
                battleAttributeManager =
                    new VBattleAttributeManager(isPhaseEnding, saveData.attributeManagerSaveData);
                buffManager = new VBuffManager(this, saveData.buffManagerSaveData);
                cardPilesManager = new VCardPilesManager(configuration.handSize, configuration.maxHandSize,
                    cardLibrary,
                    null, null, saveData.cardPilesManagerSaveData);

                turnAttribute = battleAttributeManager.BattleAttributes["BATurn"] as VBattleTurnAttribute;
                playLeftAttribute = battleAttributeManager.BattleAttributes["BAPlayLeft"] as VBattlePlayLeftAttribute;
                _initialized = true;

                if (saveData.playTwiceCard != 0)
                    playTwiceCard = cardPilesManager.GetCardById(saveData.playTwiceCard);
                playTwiceMessageDict = saveData.playTwiceMessageDict;

                battleAttributeManager.OnEnable();
                cardPilesManager.OnEnable();
                buffManager.OnEnable();
                BattleRelicManager = new VBattleRelicManager(this, saveData.battleRelicManagerSaveData);
            });
        }

        public virtual void InitializeBattle(bool isDebugScene, VDialogueEvent e, bool isPhaseEnding,
            VCharacterAttributeManager characterAttributeManager,
            VCardLibrary cardLibrary, int initialTurnCount, int mainAttributeIndex, List<int> abilityTurnCounts,
            List<AnimationCurve> decayCurves,
            int targetPopularity, int extraTargetPopularity, int abilityBonus, int initialViewers,
            List<VBattleRelic> relics,
            bool isTutorial = false, List<VAttributeCondition> tutorialConditions = null,
            List<uint> tutorialDeck = null, Dictionary<int, List<uint>> tutorialTurnHandCards = null, VTipConfig tipConfig = null)
        {
            _initialized = true;
            _isDebugScene = isDebugScene;
            _battleEnded = false;

            VBattleLookUpTables.Instance.Initialize(null);

            _mainAttributeIndex = mainAttributeIndex;
            this.isPhaseEnding = isPhaseEnding;
            cardTypeHistory = new Dictionary<string, int>();
            this.targetPopularity = targetPopularity;
            this.extraTargetPopularity = extraTargetPopularity;
            _decayCurves = decayCurves;
            this.abilityBonus = abilityBonus;
            _abilityTurnCounts = abilityTurnCounts;
            this.characterAttributeManager = characterAttributeManager;
            battleAttributeManager = new VBattleAttributeManager(isPhaseEnding, null);
            cardPilesManager = new VCardPilesManager(configuration.handSize, configuration.maxHandSize, cardLibrary,
                tutorialDeck, tutorialTurnHandCards, null);
            buffManager = new VBuffManager(this);
            _tutorialConditions = tutorialConditions;

            battleAttributeManager.OnEnable();
            cardPilesManager.OnEnable();
            buffManager.OnEnable();
            _tipConfig = tipConfig;

            _isTutorial = _tutorialConditions is not null;

            if (isDebugScene)
            {
                InitializeLogic(isPhaseEnding, initialTurnCount, initialViewers, relics,
                    mainAttributeIndex, abilityTurnCounts, targetPopularity, extraTargetPopularity,
                    characterAttributeManager);
                return;
            }
            _eventID = e.EventID;
            
            VEventSystemUI.Instance.PlayLoadingAnimation(e, () =>
            {
                VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnBattleBegin, new Dictionary<string, object>
                {
                    { "TurnLeft", TurnLeft },
                    { "TargetPopularity", targetPopularity },
                    { "ExtraTargetPopularity", extraTargetPopularity },
                    { "IsPhaseEnding", isPhaseEnding },
                    { "CharacterAttributeManager", characterAttributeManager },
                    { "BattleAttributeManager", battleAttributeManager }
                });
                VAudioPlayer.Instance.PlayBGM(VBGMType.Stream);

                foreach (var buff in characterAttributeManager.GetBuffs())
                    if (buff is not null)
                        buffManager.AddBuff(buff, 1, false, false);

                InitializeTurn(false);
                if (isTutorial) VDataPersistenceManager.Instance.SaveGameTutorialBattle();
            },
            () =>
            {
                if (!isDebugScene)
                {
                    VRaisingUI.Instance.SwitchAttributesUIBattle(false);
                    VEventSystemUI.Instance.OpenBattleUI();
                }
                _initialized = true;
                VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnBattleUIInitialize, new Dictionary<string, object>
                {
                    { "TargetPopularity", this.targetPopularity },
                    { "ExtraTargetPopularity", this.extraTargetPopularity },
                    { "IsPhaseEnding", this.isPhaseEnding },
                    { "TipConfig", tipConfig }
                });
                InitializeLogic(isPhaseEnding, initialTurnCount, initialViewers, relics,
                    mainAttributeIndex, abilityTurnCounts, targetPopularity, extraTargetPopularity,
                    characterAttributeManager);
            });
        }

        public void InitializeLogic(bool isPhaseEnding, int initialTurnCount, int initialViewers,
            List<VBattleRelic> relics, int mainAttributeIndex, List<int> abilityTurnCounts,
            int targetPopularity, int extraTargetPopularity, VCharacterAttributeManager characterAttributeManager)
        {
            battleAttributeManager.AttributesConversion(this.characterAttributeManager);
            turnAttribute = new VBattleTurnAttribute(initialTurnCount);
            playLeftAttribute = new VBattlePlayLeftAttribute(configuration.defaultPlayPerTurn);

            if (!isPhaseEnding)
            {
                battleAttributeManager.TryGetAttribute("BASingingMultiplier", out var attribute);
                attribute.SetValue(100, false, false, false);
                battleAttributeManager.TryGetAttribute("BAGamingMultiplier", out attribute);
                attribute.SetValue(100, false, false, false);
                battleAttributeManager.TryGetAttribute("BAChattingMultiplier", out attribute);
                attribute.SetValue(100, false, false, false);
            }

            battleAttributeManager.AddAttribute("BATurn", turnAttribute);
            battleAttributeManager.AddAttribute("BAPlayLeft", playLeftAttribute);

            battleAttributeManager.AddAttribute("BAShield",
                new VBattleStaminaAttribute(0, VBattleEventKey.OnShieldChange, true));
            battleAttributeManager.AddAttribute("BARevenue",
                new VBattleStaminaAttribute(0, VBattleEventKey.OnRevenueChange));

            battleAttributeManager.AddAttribute("BAPopularity", new VBattlePopularityAttribute(0));
            battleAttributeManager.AddAttribute("BAParameter", new VBattleParameterAttribute(0));

            BattleRelicManager = new VBattleRelicManager(this, relics);
            if (battleAttributeManager.TryGetAttribute("BAViewerCount", out var viewerCountAttribute))
                viewerCountAttribute.AddTo(initialViewers, false);
            battleAttributeManager.InitializeInternalManagers(mainAttributeIndex, abilityTurnCounts);

        }

        public void SetShouldNextCardPlayTwice(bool value)
        {
            if (value == false)
                VDebug.Log("");
            shouldNextCardPlayTwice = value;
        }

        public void NextCardPlayTwice()
        {
            SetShouldNextCardPlayTwice(true);
        }

        public void RedrawRest()
        {
            shouldRedraw = true;
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
            
            if(buffManager is not null)
                buffManager.Clear();
            
            if(battleAttributeManager is not null)
                battleAttributeManager.Clear();
            
            
            if(cardPilesManager is not null)
                cardPilesManager.Clear();

            if(battleAttributeManager is not null)
                battleAttributeManager.OnDisable();
            
            
            if(cardPilesManager is not null)
                cardPilesManager.OnDisable();
            
            
            if(buffManager is not null)
                buffManager.OnDisable();

            cardPilesManager = null;
            buffManager = null;
            battleAttributeManager = null;
        }

        private void OnShieldModifierChanged(Dictionary<string, object> messagedict)
        {
            if (cardPilesManager is null)
                return;
            foreach (var card in cardPilesManager.HandPile)
            {
                if (card is null)
                    continue;
                card.PreviewShield(this, false);
            }
        }

        private void OnPopularityChange(Dictionary<string, object> messagedict)
        {
            if (cardPilesManager is null)
                return;
            var value = (int)messagedict["NewValue"];
            if (!isPhaseEnding)
                if (value >= extraTargetPopularity)
                    _shouldEndBattle = true;
        }

        private void OnParameterPopularityModifierChanged(Dictionary<string, object> messagedict)
        {
            if (cardPilesManager is null)
                return;
            foreach (var card in cardPilesManager.HandPile) card.PreviewPopularity(this, false);
        }

        private void OnAttributeValueChange(Dictionary<string, object> messagedict)
        {
            if (cardPilesManager is null)
                return;
            foreach (var card in cardPilesManager.HandPile)
            {
                if (card is null)
                    continue;
                card.TestCondition(this);
            }
        }

        private void OnRequestPickCardsFromPile(Dictionary<string, object> messagedict)
        {
            var cardCount = (int)messagedict["CardCount"];
            if (cardPilesManager.HandPile.Count + cardCount > configuration.maxHandSize)
                cardCount = configuration.maxHandSize - cardPilesManager.HandPile.Count;

            if (cardCount <= 0)
                return;

            messagedict["CardCount"] = cardCount;
            VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnBeginPickCardsFromPile, messagedict);
        }


        private void OnCardMovedToHandSlot(Dictionary<string, object> messagedict)
        {
            var card = messagedict["Card"] as VCard;
            if (card is null)
                return;
            card.TestCondition(this);
            card.PreviewPopularity(this, true);
            card.PreviewShield(this, true);
        }

        private void OnBuffValueUpdated(Dictionary<string, object> messagedict)
        {
            if (cardPilesManager is null)
                return;
            foreach (var card in cardPilesManager.HandPile)
            {
                if (card is null)
                    continue;
                if (card.CostType == CostType.Buff)
                    card.setPlayable?.Invoke(buffManager.TestCost(card.CostBuffId, card.Cost));
                card.TestCondition(this);
                card.PreviewPopularity(this, false);
            }
        }

        private void OnBuffAdded(Dictionary<string, object> messagedict)
        {
            if (cardPilesManager is null)
                return;
            foreach (var card in cardPilesManager.HandPile)
            {
                if (card is null)
                    continue;
                card.TestCondition(this);
                card.PreviewPopularity(this, false);
            }
        }

        private void OnStaminaChange(Dictionary<string, object> messagedict)
        {
            if (cardPilesManager is null)
                return;
            foreach (var card in cardPilesManager.HandPile)
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
            if (battleAttributeManager is not null)
                battleAttributeManager.SkipTurnRecoverStamina();
        }

        private void OnPlayTheSecondTime(Dictionary<string, object> messagedict)
        {
            SetShouldNextCardPlayTwice(false);
            VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnCardUsed,
                new Dictionary<string, object>
                {
                    { "Card", playTwiceCard },
                    { "IsPlayTwice", true }
                });
            if (playTwiceCard is not null && playTwiceMessageDict is not null)
                ApplyCardEffects(playTwiceCard, playTwiceMessageDict);

            playTwiceCard = null;
            playTwiceMessageDict = null;
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

            playLeftAttribute.AddTo(-1, false);
            VDebug.Log("剩余可行动次数: " + PlayLeft);
            if (PlayLeft <= 0)
            {
                EndTurn();
                if (shouldRedraw) shouldRedraw = false;
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

            InitializeTurn(false);
        }

        private protected void InitializeTurn(bool isLoad)
        {
            if (TurnLeft <= 0)
            {
                EndBattle();
                return;
            }

            VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnTurnBegin, new Dictionary<string, object>
            {
                { "TurnLeft", TurnLeft },
                { "TurnIndex", turnAttribute.TurnIndex },
                { "HandSize", configuration.maxHandSize }
            });

            if (!isLoad)
            {
                VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnTurnBeginBuffApply,
                    new Dictionary<string, object>());
            }
            
            if (!_isDebugScene)
                VDataPersistenceManager.Instance.SaveGame(VSavePointType.Battle);
        }

        private void EndTurn()
        {
            VDebug.Log("回合结束: " + TurnLeft);
            turnAttribute.AddTo(-1, false);
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
            if (popularity >= extraTargetPopularity)
            {
                attributeGain = abilityBonus;
                return attributeGain;
            }

            if (popularity >= targetPopularity)
                attributeGain = Mathf.CeilToInt(abilityBonus * 0.5f + abilityBonus * 0.5f *
                    (popularity - targetPopularity) / (extraTargetPopularity - targetPopularity));
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
                buffManager.Clear();
                battleAttributeManager.Clear();
                cardPilesManager.Clear();

                battleAttributeManager.OnDisable();
                cardPilesManager.OnDisable();
                buffManager.OnDisable();

                cardPilesManager = null;
                buffManager = null;
                battleAttributeManager = null;
                return;
            }


            var isTutorialConditionsSatisfied = TestTutorialConditions();

            _battleEnded = true;
            battleAttributeManager.TryGetAttribute("BAPopularity", out var battleAttribute);
            var popularityAttribute = battleAttribute as VBattlePopularityAttribute;
            if (!isPhaseEnding)
            {
                var attributeGain = CalculateAbilityGain(popularityAttribute.Value);

                var attributeKey = GetAbilityKey(_mainAttributeIndex);

                characterAttributeManager.TryGetAttribute(attributeKey, out var attribute);
                if (attribute is VAbilityAttribute abilityAttribute) abilityAttribute.AddAbility(attributeGain, true);
            }
            else
            {
                var attributeKey = GetAbilityKey(_mainAttributeIndex);
                characterAttributeManager.TryGetAttribute(attributeKey, out var attribute);
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

                characterAttributeManager.TryGetAttribute(GetAbilityKey(index1), out var ability1);
                characterAttributeManager.TryGetAttribute(GetAbilityKey(index2), out var ability2);
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
                characterAttributeManager.ConvertToCharacterAttributes(battleAttributeManager.BattleAttributes);

            VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnBattleEnd, new Dictionary<string, object>
            {
                { "IsTutorial", _tutorialConditions != null },
                { "IsTutorialConditionsSatisfied", isTutorialConditionsSatisfied },
                { "TurnLeft", TurnLeft },
                { "CharacterAttributeManager", characterAttributeManager },
                { "BattleAttributeManager", battleAttributeManager },
                { "ReachedTarget", popularityAttribute.Value >= targetPopularity },
                { "ReachedExtraTarget", popularityAttribute.Value >= extraTargetPopularity }
            });

            _tipConfig = null;
            _initialized = false;
            buffManager.Clear();
            battleAttributeManager.Clear();
            cardPilesManager.Clear();

            battleAttributeManager.OnDisable();
            cardPilesManager.OnDisable();
            buffManager.OnDisable();

            cardPilesManager = null;
            buffManager = null;
            battleAttributeManager = null;
        }

        private void ReloadBattle(Dictionary<string, object> messagedict)
        {
            var save = VDataPersistenceManager.Instance.LoadTutorialBattleSave();

            InitializeBattle(save.battleSaveData, _decayCurves, characterAttributeManager, null, _tipConfig);
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
                    battleAttributeManager.StaminaManager.ApplyCost((int)messagedict["Cost"]);
                    break;
                case CostType.TrueStamina:
                    battleAttributeManager.StaminaManager.ApplyCost((int)messagedict["Cost"], true);
                    break;
                case CostType.Buff:
                    buffManager.ApplyCost((uint)messagedict["CostBuffId"], (int)messagedict["Cost"]);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Redraw()
        {
            VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnRedrawCards, new Dictionary<string, object>
            {
                { "ShouldPlayTwice", shouldNextCardPlayTwice }
            });

            if (shouldNextCardPlayTwice)
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

            if (shouldNextCardPlayTwice)
            {
                playTwiceCard = card;
                playTwiceMessageDict = messagedict;
            }

            StartCoroutine(ApplyCardEffectsImplement(effects, messagedict));
        }
        
        private IEnumerator ApplyCardEffectsImplement(
            List<VEffect> effects,
            Dictionary<string, object> messagedict)
        {
            var effectApplied = false;
            var tempShouldPlayTwice = shouldNextCardPlayTwice;

            foreach (var effect in effects)
            {
                if (!effect.CanApply(this, messagedict))
                    continue;

                effectApplied = true;
                effect.ApplyEffect(this, 1, true, tempShouldPlayTwice);
                yield return new WaitForSeconds(0.5f);
            }
            VBattleRootEventCenter.Instance.Raise(
                VBattleEventKey.OnNotifyBeginDisposeCard,
                new Dictionary<string, object>()
            );
            if (!effectApplied)
            {
                yield return null;
            }

            if (!shouldRedraw)
                yield break;

            shouldRedraw = false;

            if (PlayLeft == 0)
            {
                yield return null;
                yield break;
            }

            Redraw();
        }

    }
}