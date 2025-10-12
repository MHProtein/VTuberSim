using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using SlayTheSpire.System.SavingSystem;
using UnityEngine;
using VTuber.BattleSystem.Card;
using VTuber.BattleSystem.Core;
using VTuber.Character;
using VTuber.Consumable;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;
using VTuber.Core.Managers;
using VTuber.Core.RaisingEffect;
using VTuber.Core.UI;
using VTuber.Dialogue.UI;
using VTuber.ScheduleSystem.Core;
using VTuber.ScheduleSystem.Events;
using VTuber.ScheduleSystem.Events.DialogueEvent;
using VTuber.ScheduleSystem.UI;
using VTuber.Store;

namespace VTuber.EventSystem
{
    public class VEventSystemSaveData
    {
        public List<int> executedLines;
        public VSelectionMenuType selectionMenuType;
        public List<VCardSaveData> cardsToSelectSaveDatas;
        public List<uint> consumablesToSelectConfigIDs;
        public uint replaceSelectedCardID;
        public VCardActionType cardActionType;
        public bool isInBattle;
    }

    public enum VSelectionMenuType
    {
        AddCard,
        AddConsumable,
        SelectCard,
        SelectCardFrom3,
        SelectConsumableFrom3,
        SelectUpgradeCard,
        SelectPhaseEnding,
        None,
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
        private bool _loaded = false;
        
        private VSelectionMenuType _selectionMenuType = VSelectionMenuType.None;
        private List<VConsumable> _consumablesToSelect;
        private List<VCard> _cardsToSelect;
        private VCard _replaceSelectedCard;
        private VCardActionType _cardActionType;
        private bool _isInBattle = false;
        private bool _isPhaseStartEvent = false;
        
        protected override void Awake()
        {
            base.Awake();
            dialogueSystem.OnDialogFinished += OnDialogueComplete;
            dialogueSystem.OnLineFinished += OnLineFinished;
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
            DataPersistenceManager.Instance.SaveGame();
        }

        public void InitializeEvent(VCharacter character, VDialogueEvent e, bool isPhaseStartEvent = false)
        {
            _isPhaseStartEvent = isPhaseStartEvent;
            if (_loaded)
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
                    
                    battle.InitializeBattle(DataPersistenceManager.Instance.SaveData.battleSaveData,
                        e.Phase.DecayCurves,
                        _character.AttributeManager,
                        _character.CardLibrary);
                    return;
                }
                EnterDialogEvent(character, e, true);
                dialogueSystem.SkipTo(_executedLines);
                return;
            }

            if (e.dialogueNode.IsNullOrWhitespace())
            {
                _hasDialogue = false;
                _currentEvent = e;
                VEventSystemUI.Instance.OpenEventUI();
                foreach (var effect in e.effects)
                {
                    effect.ApplyEffect(character, null);
                }
                OnDialogueComplete(null);

                if (_loaded)
                {
                    LoadedOpenSelectionMenu();
                }
                
                return;
            }
            
            EnterDialogEvent(character, e, false, isPhaseStartEvent);
        }

        public void EnterDialogEvent(VCharacter character, VDialogueEvent e, bool loaded, bool isPhaseStartEvent = false)
        {
            _hasDialogue = true;
            dialogueSystem.LoadDialog(e.dialogueNode);
            
            if(!loaded)
                _executedLines = new List<int>();
            
            VEventSystemUI.Instance.PlayVideo(() =>
            {
                _character = character;
                _currentEvent = e;
                VEventSystemUI.Instance.OpenEventUI();
                dialogueSystem.ShowMe(character);
                if(!loaded)
                    dialogueSystem.ContinueDialog();
                if(loaded)
                    if (_loaded)
                    {
                        LoadedOpenSelectionMenu();
                    }
                _loaded = false;
            });
        }

        public void LoadedOpenSelectionMenu()
        {
            switch (_selectionMenuType)
            {
                case VSelectionMenuType.AddCard:
                    break;
                case VSelectionMenuType.AddConsumable:
                    ShowAddConsumable();
                    break;
                case VSelectionMenuType.SelectCard:
                    ShowSelectCard();
                    break;
                case VSelectionMenuType.SelectCardFrom3:
                    ShowSelectCardFrom3();
                    break;
                case VSelectionMenuType.SelectConsumableFrom3:
                    ShowSelectConsumableFrom3();
                    break;
                case VSelectionMenuType.SelectUpgradeCard:
                    ShowUpgradeCard();
                    break;
                case VSelectionMenuType.SelectPhaseEnding:
                    break;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnSelectPhaseEndingBegin, OnPickPhaseEndingBegin);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnBeginSelectCardFrom3, OnBeginSelectCardFrom3);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnBeginSelectCard, OnBeginSelectCard);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnEventSelectUpgradeCard, OnEventSelectUpgradeCard);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnRequestEnterStore, OnRequestEnterStore);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnPhaseBegin, OnPhaseBegin);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnBeginSelectConsumableFrom3, OnBeginSelectConsumableFrom3);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnShowAddConsumable, OnShowAddConsumable);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnBattleEndNotify, OnBattleEnd);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnSelectPhaseEndingBegin, OnPickPhaseEndingBegin);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnBeginSelectCardFrom3, OnBeginSelectCardFrom3);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnBeginSelectCard, OnBeginSelectCard);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnEventSelectUpgradeCard, OnEventSelectUpgradeCard);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnRequestEnterStore, OnRequestEnterStore);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnPhaseBegin, OnPhaseBegin);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnShowAddConsumable, OnShowAddConsumable);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnBeginSelectConsumableFrom3, OnBeginSelectConsumableFrom3);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnBattleEndNotify, OnBattleEnd);
        }

        private void OnBattleEnd(Dictionary<string, object> messagedict)
        {
            _isInBattle = false;
        }

        private void OnRequestEnterStore(Dictionary<string, object> messagedict)
        {
            _shouldEnterStore = true;
        }      
        
        private void OnPhaseBegin(Dictionary<string, object> messagedict)
        {
            _store.Reset();
        }
        
        private void OnShowAddConsumable(Dictionary<string, object> messagedict)
        {
            _consumablesToSelect = messagedict["Consumables"] as List<VConsumable>;

            ShowAddConsumable();
        }
        
        private void OnBeginSelectConsumableFrom3(Dictionary<string, object> messagedict)
        {
            _consumablesToSelect = messagedict["Consumables"] as List<VConsumable>;

            ShowSelectConsumableFrom3();
        }
        
        private void OnEventSelectUpgradeCard(Dictionary<string, object> messagedict)
        {
            ShowUpgradeCard();
        }
        
        private void OnBeginSelectCard(Dictionary<string, object> messagedict)
        {
            _cardActionType = (VCardActionType)messagedict["ActionType"];
            messagedict.TryGetValue("ReplaceSelectedCard", out object selectedCard);
            _replaceSelectedCard = selectedCard as VCard;
            
            ShowSelectCard();
        }
        
        private void OnBeginSelectCardFrom3(Dictionary<string, object> messagedict)
        {
            _cardActionType = (VCardActionType)messagedict["ActionType"];
            _cardsToSelect = messagedict["Cards"] as List<VCard>;
            messagedict.TryGetValue("ReplaceSelectedCard", out object selectedCard);
            _replaceSelectedCard = selectedCard as VCard;
            
            ShowSelectCardFrom3();
        }
        
        public void ShowAddConsumable()
        {
            _selectionMenuType = VSelectionMenuType.AddConsumable;
            VEventSystemUI.Instance.OpenAddConsumableUI(_character,
                _consumablesToSelect.FirstOrDefault(),
                (consumable) =>
                {
                    _character.ConsumableManager.AddConsumable(consumable);
                } ,
                () =>
                {
                    _selectionMenuType = VSelectionMenuType.None;
                    if(_hasDialogue)
                        dialogueSystem.SetPaused(false);
                    else
                    {
                        OnDialogueComplete(null);
                    }
                });
            dialogueSystem.SetPaused(true);
        }
        
        public void ShowSelectConsumableFrom3()
        {
            _selectionMenuType = VSelectionMenuType.SelectConsumableFrom3;
            VEventSystemUI.Instance.OpenSelectFrom3ConsumablesMenu(_character,
                _consumablesToSelect,
                (consumable) =>
                {
                    _character.ConsumableManager.AddConsumable(consumable);
                } ,
                () =>
                {            
                    _selectionMenuType = VSelectionMenuType.None;
                    if(_hasDialogue)
                        dialogueSystem.SetPaused(false);
                    else
                    {
                        OnDialogueComplete(null);
                    }
                });
            dialogueSystem.SetPaused(true);
        }
        
        public void ShowUpgradeCard()
        {
            _selectionMenuType = VSelectionMenuType.SelectUpgradeCard;
            VEventSystemUI.Instance.OpenUpgradeCard(_character.CardLibrary.GetCards().Where(card => !card.IsUpgraded).ToList(),
                () =>
                {
                    _selectionMenuType = VSelectionMenuType.None;
                    if(_hasDialogue)
                        dialogueSystem.SetPaused(false);
                    else
                    {
                        OnDialogueComplete(null);
                    }
                });
            dialogueSystem.SetPaused(true);
        }

        public void ShowSelectCardFrom3()
        {
            _selectionMenuType = VSelectionMenuType.SelectCardFrom3;
            VEventSystemUI.Instance.OpenSelectFrom3Menu(_cardsToSelect, VCardActionUtils.GetAction(_cardActionType, _character, _replaceSelectedCard), 
                () =>
                {
                    _selectionMenuType = VSelectionMenuType.None;
                    if(_hasDialogue)
                        dialogueSystem.SetPaused(false);
                    else
                    {
                        OnDialogueComplete(null);
                    }
                });
            dialogueSystem.SetPaused(true);
        }
        
        public void ShowSelectCard()
        {
            _selectionMenuType = VSelectionMenuType.SelectCard;
            VEventSystemUI.Instance.OpenSelectCard(_character.CardLibrary.GetCards(), true, VCardActionUtils.GetAction(_cardActionType, _character, _replaceSelectedCard),
                () =>
                {
                    _selectionMenuType = VSelectionMenuType.None;
                    if(_hasDialogue)
                        dialogueSystem.SetPaused(false);
                    else
                    {
                        OnDialogueComplete(null);
                    }
                });
            dialogueSystem.SetPaused(true);
        }
        
        private void OnPickPhaseEndingBegin(Dictionary<string, object> messagedict)
        {
            if (_currentEvent.Phase == null)
            {
                VDebug.LogError("_currentEvent.Phase is null");
                return;
            }
            VEventSystemUI.Instance.InitializePhaseEndingSelectionMenu(_currentEvent.Phase.GetPhaseEndingEvents(_character),
                () =>
                {
                    dialogueSystem.SetPaused(false);
                });
            dialogueSystem.SetPaused(true);
        }

        public void ExitStore()
        {
            storeObject.SetActive(false);
            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnEventEnd, 
                new Dictionary<string, object>()
                {
                    {"Event", _currentEvent}
                });
            _currentEvent = null;
        }
        
        public void InitializeBattle(bool isPhaseEnding, int initialTurnCount, int targetPopularity, 
            int extraTargetPopularity, int abilityBonus, int initialViewers,
            int mainAttributeIndex, List<int> abilityTurnCounts, List<AnimationCurve> decayCurves)
        {
            _isInBattle = true;
            battleObject.SetActive(true);
            battle.InitializeBattle(false, isPhaseEnding, _character.AttributeManager,
                _character.CardLibrary,
                initialTurnCount, mainAttributeIndex, abilityTurnCounts, decayCurves,
                targetPopularity, extraTargetPopularity, abilityBonus, initialViewers,
                _character.CharacterRelicManager.GetBattleRelics());
            _character.ConsumableManager.SetBattle(battle);
            VRaisingUI.Instance.SetConsumableToBattle();
        }
        
        private void OnDialogueComplete(Dialog dialog)
        {
            if (!_hasDialogue && _selectionMenuType == VSelectionMenuType.None)
                return;
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
                    _currentEvent.Phase.DecayCurves);
            }
            else if (_shouldEnterStore)
            {
                VEventSystemUI.Instance.PlayVideo(() =>
                {
                    storeObject.SetActive(true);
                    _store.EnterStore(_character);
                    _shouldEnterStore = false;
                });
            }
            else
            {
                VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnEventEnd, 
                    new Dictionary<string, object>()
                    {
                        {"Event", _currentEvent}
                    });
                _currentEvent = null;
            }
            dialogueSystem.HideMe();
        }

        public void Load(SaveData data)
        {
            _isInBattle = data.eventSystemSaveData.isInBattle;
            _store.Load(data.storeSaveData);
            _executedLines = data.eventSystemSaveData.executedLines;
            _loaded = true;
            
            _selectionMenuType = data.eventSystemSaveData.selectionMenuType;

            _cardsToSelect = new List<VCard>();
  
            if(data.eventSystemSaveData.cardsToSelectSaveDatas != null)
                foreach (var cardSaveData in data.eventSystemSaveData.cardsToSelectSaveDatas)
                {
                    var card = VDataManager.Instance.CreateCardByID(cardSaveData.configID);
                    card.Load(cardSaveData);
                    _cardsToSelect.Add(card);
                }
            
            _consumablesToSelect = new List<VConsumable>();
            if(data.eventSystemSaveData.consumablesToSelectConfigIDs != null)
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
            data.eventSystemSaveData = new VEventSystemSaveData()
            {
                executedLines = _executedLines,
                selectionMenuType = _selectionMenuType,
                cardsToSelectSaveDatas = _cardsToSelect?.Select(card => card.Save()).ToList(),
                consumablesToSelectConfigIDs = _consumablesToSelect?.Select(consumable => consumable.ConfigId).ToList(),
                replaceSelectedCardID = _replaceSelectedCard?.Id ?? 0,
                cardActionType = _cardActionType,
                isInBattle = _isInBattle,
            };
        }
    }
}