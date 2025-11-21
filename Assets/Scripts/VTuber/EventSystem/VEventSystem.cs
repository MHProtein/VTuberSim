using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using SlayTheSpire.System.SavingSystem;
using Tutorial.Script;
using UnityEngine;
using VTuber.BattleSystem.Card;
using VTuber.BattleSystem.Core;
using VTuber.BattleSystem.Effect.Conditions;
using VTuber.Character;
using VTuber.Consumable;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;
using VTuber.Core.Managers;
using VTuber.Core.RaisingEffect;
using VTuber.Core.SE;
using VTuber.Core.StateMachine;
using VTuber.Core.UI;
using VTuber.Dialogue.UI;
using VTuber.RaisingAnimationSystem;
using VTuber.ScheduleSystem.Core;
using VTuber.ScheduleSystem.Events;
using VTuber.ScheduleSystem.Events.DialogueEvent;
using VTuber.ScheduleSystem.UI;
using VTuber.ScheduleSystem.UI.RaisingAnimationSystem;
using VTuber.Store;

namespace VTuber.EventSystem
{
    public class VEventSystemSaveData
    {
        public VCardActionType cardActionType;
        public List<VCardSaveData> cardsToSelectSaveDatas;
        public List<uint> consumablesToSelectConfigIDs;
        public List<int> executedLines;
        public bool isInBattle;
        public uint replaceSelectedCardID;
    }

    public class VEventSystem : VMonoBehaviour
    {
        private VCharacter _character;
        private VDialogueEvent _currentEvent;
        private bool _shouldEnterStore;
        private VStore _store;

        [SerializeField] private GameObject battleObject;
        [SerializeField] private GameObject storeObject;
        [SerializeField] private VBattle battle;
        [SerializeField] private DialogSystem dialogueSystem;
        [SerializeField] private VStoreConfiguration storeConfig;

        private bool _hasDialogue;

        //the dialog line just got displayed and its effects got executed
        private List<int> _executedLines;
        private bool _loaded;

        private List<VConsumable> _consumablesToSelect;
        private List<VCard> _cardsToSelect;
        private VCard _replaceSelectedCard;
        private VCardActionType _cardActionType;
        private bool _isInBattle;
        private bool _isPhaseStartEvent;

        protected override void Awake()
        {
            base.Awake();
            dialogueSystem.gameObject.SetActive(false);
            dialogueSystem.HideMe();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnSelectPhaseEndingBegin,
                OnPickPhaseEndingBegin);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnRequestEnterStore,
                OnRequestEnterStore);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnPhaseBegin, OnPhaseBegin);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnBattleEndNotify, OnBattleEnd);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnSelectPhaseEndingBegin,
                OnPickPhaseEndingBegin);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnRequestEnterStore, OnRequestEnterStore);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnPhaseBegin, OnPhaseBegin);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnBattleEndNotify, OnBattleEnd);
        }

        public void Initialize()
        {
            _store = new VStore(storeConfig);
        }

        private void OnLineFinished(int line)
        {
            if (_isPhaseStartEvent)
                return;
            _executedLines.Add(line);
            VDataPersistenceManager.Instance.SaveGame(VSavePointType.Dialog);
        }

        public void InitializeEvent(VCharacter character, VDialogueEvent e, bool isPhaseStartEvent = false)
        {
            VEventSystemUI.Instance.ClosePhaseEndingSelectionMenu(false);
            dialogueSystem.OnDialogFinished += OnDialogueComplete;
            dialogueSystem.OnLineFinished += OnLineFinished;
            VAudioPlayer.Instance.PlayStaticSFX(VSFXType.Raising_EnterEvent);
            _isPhaseStartEvent = isPhaseStartEvent;
            if (_loaded && !e.dialogueNode.IsNullOrWhitespace())
            {
                _character = character;
                if (_isInBattle)
                {
                    _isInBattle = true;
                    battleObject.SetActive(true);
                    _character.ConsumableManager.SetBattle(battle);
                    VRaisingUI.Instance.SetConsumableToBattle();

                    _executedLines.Clear();
                    _loaded = false;
                    
                    battle.InitializeBattle(VDataPersistenceManager.Instance.SaveData.battleSaveData,
                        e.Phase.DecayCurves,
                        _character.AttributeManager,
                        _character.CardLibrary, (e as VStreamEvent)?.TutorialTipConfig);
                    return;
                }

                EnterDialogEvent(character, e, _loaded);
                dialogueSystem.SkipTo(_executedLines);
                return;
            }

            if (e.dialogueNode.IsNullOrWhitespace())
            {
                dialogueSystem.HideMe();
                _hasDialogue = false;
                _currentEvent = e;
                VEventSystemUI.Instance.OpenEventUI();
                
                VDataPersistenceManager.Instance.SaveGame(VSavePointType.Dialog);
                
                if (_loaded)
                {
                    _loaded = false;
                }
                
                VEventSystemUI.Instance.PlayLoadingAnimation(e, () =>
                {
                    VAudioPlayer.Instance.PlayBGM(VBGMType.NonDialogEvent);
                    foreach (var effect in e.effects)
                        effect.ApplyEffect(character, null, VAnimationRequestFactory.Create(VInstigatorType.Dialog, e.Icon, e.Description));
            
                    VRaisingAnimationSystem.Instance.ExecuteAnimations(() =>
                    {
                        OnDialogueComplete(null);
                    });
                });
                
                return;
            }

            EnterDialogEvent(character, e, false, isPhaseStartEvent);
        }

        public void EnterDialogEvent(VCharacter character, VDialogueEvent e, bool loaded,
            bool isPhaseStartEvent = false)
        {
            _hasDialogue = true;

            if (!loaded)
                _executedLines = new List<int>();
            dialogueSystem.Clear();

            VEventSystemUI.Instance.PlayLoadingAnimation(e, () =>
            {
                VAudioPlayer.Instance.PlayBGM(VBGMType.Dialog);
                _character = character;
                _currentEvent = e;
                VEventSystemUI.Instance.OpenEventUI();
                if (!loaded)
                {
                    dialogueSystem.ContinueDialog();
                    e.ExecuteEffectsBeforeEvent(character);
                    VRaisingAnimationSystem.Instance.ExecuteAnimations(() => { });
                }
                
                _loaded = false;
            },
            () =>
            {
                dialogueSystem.gameObject.SetActive(true);
                dialogueSystem.ShowMe(character);
                dialogueSystem.LoadDialog(e.dialogueNode);
            });
        }

        private void OnBattleEnd(Dictionary<string, object> messagedict)
        {
            _isInBattle = false;
            VEventSystemUI.Instance.CloseBattleUI();
        }

        private void OnRequestEnterStore(Dictionary<string, object> messagedict)
        {
            _shouldEnterStore = true;
        }

        private void OnPhaseBegin(Dictionary<string, object> messagedict)
        {
            _store.Reset();
        }

        private void OnPickPhaseEndingBegin(Dictionary<string, object> messagedict)
        {
            if (_currentEvent.Phase == null)
            {
                VDebug.LogError("_currentEvent.Phase is null");
                return;
            }

            VEventSystemUI.Instance.InitializePhaseEndingSelectionMenu(
                _currentEvent.Phase.GetPhaseEndingEvents(_character),
                () => { dialogueSystem.SetPaused(false); });
            if (_hasDialogue)
                dialogueSystem.SetPaused(true);
        }

        public void ExitStore()
        {
            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnEventEnd,
                new Dictionary<string, object>
                {
                    { "Event", _currentEvent }
                });
            VRaisingAnimationSystem.Instance.ExecuteAnimations(() =>
            {
                
                var state = VGameManager.Instance.GetState<VExecutionState>();
                if (state is not null)
                    state.OnEventEndAnimationEnd(() =>
                    {
                        _currentEvent = null;
                        storeObject.SetActive(false);
                        VAudioPlayer.Instance.StopBGM();
                    });
                else
                {
                    _currentEvent = null;
                    storeObject.SetActive(false);
                    VAudioPlayer.Instance.StopBGM();
                }
            });
        }

        public void InitializeBattle(bool isPhaseEnding, int initialTurnCount, int targetPopularity,
            int extraTargetPopularity, int abilityBonus, int initialViewers,
            int mainAttributeIndex, List<int> abilityTurnCounts, List<AnimationCurve> decayCurves,
            bool isTutorial, List<VAttributeCondition> tutorialConditions,
            List<uint> tutorialDeck, Dictionary<int, List<uint>> tutorialTurnHandCards, VTipConfig tipConfig)
        {
            _isInBattle = true;
            battleObject.SetActive(true);
            battle.InitializeBattle(false, _currentEvent, isPhaseEnding, _character.AttributeManager,
                _character.CardLibrary,
                initialTurnCount, mainAttributeIndex, abilityTurnCounts, decayCurves,
                targetPopularity, extraTargetPopularity, abilityBonus, initialViewers,
                _character.CharacterRelicManager.GetBattleRelics(), isTutorial, tutorialConditions, tutorialDeck,
                tutorialTurnHandCards, tipConfig);
            VRaisingUI.Instance.SetConsumableToBattle();
            _character.ConsumableManager.SetBattle(battle);
        }

        private void OnDialogueComplete(Dialog dialog)
        {
            dialogueSystem.OnDialogFinished -= OnDialogueComplete;
            dialogueSystem.OnLineFinished -= OnLineFinished;

            VDebug.Log("OnDialogueCompleteCalled");
            if (_currentEvent is null)
            {
                VDebug.LogError("_currentEvent是空");
                return;
            }
            if (_currentEvent.Type == VEventType.Stream)
            {
                var streamEvent = _currentEvent as VStreamEvent;
                InitializeBattle(streamEvent.IsPhaseEndingEvent,
                    streamEvent.InitialTurnCount,
                    streamEvent.TargetPopularity,
                    streamEvent.ExtraTargetPopularity,
                    streamEvent.AbilityBonus,
                    streamEvent.InitialViewers,
                    streamEvent.MainAttributeIndex,
                    streamEvent.AbilityTurnCounts,
                    _currentEvent.Phase.DecayCurves,
                    streamEvent.IsTutorial,
                    streamEvent.TutorialConditions,
                    streamEvent.TutorialDeck,
                    streamEvent.TutorialTurnHandCards,
                    streamEvent.TutorialTipConfig);
            }
            else if (_shouldEnterStore)
            {
                VEventSystemUI.Instance.PlayLoadingAnimation(_currentEvent, () =>
                {
                    storeObject.SetActive(true);
                    _store.EnterStore(_character);
                    _shouldEnterStore = false;
                    VAudioPlayer.Instance.PlayBGM(VBGMType.Store);
                },
                () =>
                {
                    dialogueSystem.HideMe();
                    dialogueSystem.Clear();
                });
            }
            else
            {
                VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnEventEnd,
                    new Dictionary<string, object>
                    {
                        { "Event", _currentEvent }
                    });
                
                VRaisingAnimationSystem.Instance.ExecuteAnimations(() =>
                {
                    _currentEvent = null;
                    var state = VGameManager.Instance.GetState<VExecutionState>();
                    if (state is not null)
                        state.OnEventEndAnimationEnd(()=> {
                            dialogueSystem.HideMe();
                            dialogueSystem.Clear();
                            VEventSystemUI.Instance.CloseLoadingAnimation(); 
                        });
                    else
                    {
                        dialogueSystem.HideMe();
                        dialogueSystem.Clear();
                        VEventSystemUI.Instance.CloseLoadingAnimation(); 
                    }
                });
            }
        }

        public void Load(SaveData data)
        {
            _isInBattle = data.eventSystemSaveData.isInBattle;
            _store.Load(data.storeSaveData);
            _executedLines = data.eventSystemSaveData.executedLines;
            
            if(data.savePointType == VSavePointType.Dialog)
                _loaded = true;


            _cardsToSelect = new List<VCard>();

            if (data.eventSystemSaveData.cardsToSelectSaveDatas != null)
                foreach (var cardSaveData in data.eventSystemSaveData.cardsToSelectSaveDatas)
                {
                    var card = VDataManager.Instance.CreateCardByID(cardSaveData.configID);
                    card.Load(cardSaveData);
                    _cardsToSelect.Add(card);
                }

            _consumablesToSelect = new List<VConsumable>();
            if (data.eventSystemSaveData.consumablesToSelectConfigIDs != null)
                foreach (var consumableConfigID in data.eventSystemSaveData.consumablesToSelectConfigIDs)
                {
                    var consumable = VDataManager.Instance.CreateConsumableByID(consumableConfigID);
                    _consumablesToSelect.Add(consumable);
                }

            _replaceSelectedCard = VDataManager.Instance.CreateCardByID(data.eventSystemSaveData.replaceSelectedCardID);
            _cardActionType = data.eventSystemSaveData.cardActionType;
        }

        public void Save(SaveData data)
        {
            data.storeSaveData = _store.Save();
            data.eventSystemSaveData = new VEventSystemSaveData
            {
                executedLines = _executedLines,
                cardsToSelectSaveDatas = _cardsToSelect?.Select(card => card.Save()).ToList(),
                consumablesToSelectConfigIDs = _consumablesToSelect?.Select(consumable => consumable.ConfigId).ToList(),
                replaceSelectedCardID = _replaceSelectedCard?.Id ?? 0,
                cardActionType = _cardActionType,
                isInBattle = _isInBattle
            };
        }

        public void CloseUI()
        {
            _loaded = false;
            _executedLines = null;
            if(VEventSystemUI.Instance)
                VEventSystemUI.Instance.CloseUI();
            dialogueSystem.OnDialogFinished -= OnDialogueComplete;
            dialogueSystem.OnLineFinished -= OnLineFinished;
        }
    }
}