using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VTuber.BattleSystem.Card;
using VTuber.BattleSystem.Core;
using VTuber.Character;
using VTuber.Consumable;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;
using VTuber.Core.Managers;
using VTuber.Dialogue.UI;
using VTuber.ScheduleSystem.Core;
using VTuber.ScheduleSystem.Events;
using VTuber.ScheduleSystem.Events.DialogueEvent;
using VTuber.ScheduleSystem.UI;
using VTuber.Store;
using Yarn.Unity;

namespace VTuber.EventSystem
{
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
        
        protected override void Awake()
        {
            base.Awake();
            dialogueSystem.OnDialogFinished += OnDialogueComplete;
            _store = new VStore(storeConfig);
        }

        public void InitializeEvent(VCharacter character, VDialogueEvent e)
        {
            VEventSystemUI.Instance.PlayVideo(() =>
            {
                _character = character;
                _currentEvent = e;
                VEventSystemUI.Instance.OpenEventUI();
                dialogueSystem.LoadDialog(e.dialogueNode);
                dialogueSystem.ShowMe(character);
                dialogueSystem.ContinueDialog();
            });
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
        }
        
        private void OnShowAddConsumable(Dictionary<string, object> messagedict)
        {
            VEventSystemUI.Instance.OpenAddConsumableUI(_character,
                messagedict["Consumable"] as VConsumable,
                messagedict["Action"] as Action<VConsumable>, 
                () =>
                {
                    dialogueSystem.SetPaused(false);
                });
            dialogueSystem.SetPaused(true);
        }
        
        private void OnBeginSelectConsumableFrom3(Dictionary<string, object> messagedict)
        {
            VEventSystemUI.Instance.OpenSelectFrom3ConsumablesMenu(_character,
                messagedict["Consumables"] as List<VConsumable>,
                messagedict["Action"] as Action<VConsumable>, 
                () =>
                {
                    dialogueSystem.SetPaused(false);
                });
            dialogueSystem.SetPaused(true);
        }
        
        private void OnRequestEnterStore(Dictionary<string, object> messagedict)
        {
            _shouldEnterStore = true;
        }      
        
        private void OnPhaseBegin(Dictionary<string, object> messagedict)
        {
            _store.Reset();
        }
        
        private void OnEventSelectUpgradeCard(Dictionary<string, object> messagedict)
        {
            VEventSystemUI.Instance.OpenUpgradeCard(_character.CardLibrary.GetCards().Where(card => !card.IsUpgraded).ToList(),
                () =>
                {
                    dialogueSystem.SetPaused(false);
                });
            dialogueSystem.SetPaused(true);
        }
        
        private void OnBeginSelectCardFrom3(Dictionary<string, object> messagedict)
        {
            VEventSystemUI.Instance.OpenSelectFrom3Menu(messagedict["Cards"] as List<VCard>, messagedict["Action"] as Action<VCard>, 
                () =>
                {
                    dialogueSystem.SetPaused(false);
                });
            dialogueSystem.SetPaused(true);
        }
        
        private void OnBeginSelectCard(Dictionary<string, object> messagedict)
        {
            VEventSystemUI.Instance.OpenSelectCard(_character.CardLibrary.GetCards(), true, messagedict["Action"] as Action<VCard>,
                () =>
                {
                    dialogueSystem.SetPaused(false);
                });
            dialogueSystem.SetPaused(true);
        }
        
        private void OnPickPhaseEndingBegin(Dictionary<string, object> messagedict)
        {
            if (_currentEvent.Phase == null)
            {
                VDebug.LogError("_currentEvent.Phase ist null, lass es reparieren");
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
            battleObject.SetActive(true);
            battle.InitializeBattle(isPhaseEnding, _character.AttributeManager,
                _character.CardLibrary,
                initialTurnCount, mainAttributeIndex, abilityTurnCounts, decayCurves,
                targetPopularity, extraTargetPopularity, abilityBonus, initialViewers,
                _character.CharacterRelicManager.GetBattleRelics());
            _character.ConsumableManager.SetBattle(battle);
            VRaisingUI.Instance.SetConsumableToBattle();
        }
        
        private void OnDialogueComplete(Dialog arg0)
        {
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
                storeObject.SetActive(true);
                _store.EnterStore(_character);
                _shouldEnterStore = false;
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
    }
}