using System;
using System.Collections.Generic;
using UnityEngine;
using VTuber.BattleSystem.Card;
using VTuber.BattleSystem.Core;
using VTuber.Character;
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
            _character = character;
            _currentEvent = e;
            dialogueSystem.LoadDialog(e.dialogueNode);
            dialogueSystem.ShowMe(character);
            dialogueSystem.ContinueDialog();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnSelectPhaseEndingBegin, OnPickPhaseEndingBegin);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnBeginSelectCardFrom3, OnBeginSelectCardFrom3);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnBeginSelectCard, OnBeginSelectCard);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnRequestEnterStore, OnRequestEnterStore);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnPhaseBegin, OnPhaseBegin);
        }

        private void OnPhaseBegin(Dictionary<string, object> messagedict)
        {
            _store.ResetRefresh();
        }


        protected override void OnDisable()
        {
            base.OnDisable();
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnSelectPhaseEndingBegin, OnPickPhaseEndingBegin);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnBeginSelectCardFrom3, OnBeginSelectCardFrom3);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnBeginSelectCard, OnBeginSelectCard);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnRequestEnterStore, OnRequestEnterStore);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnPhaseBegin, OnPhaseBegin);
        }
        
        private void OnRequestEnterStore(Dictionary<string, object> messagedict)
        {
            _shouldEnterStore = true;
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
            VEventSystemUI.Instance.OpenCardLibrary(_character.CardLibrary.GetCards(), true, messagedict["Action"] as Action<VCard>,
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
        public void InitializeBattle(int initialTurnCount, int targetPopularity, int initialViewers,
            int mainAttributeIndex, List<int> abilityTurnCounts)
        {
            battleObject.SetActive(true);
            battle.InitializeBattle(_character.AttributeManager,
                _character.CardLibrary,
                initialTurnCount, mainAttributeIndex, abilityTurnCounts,
                targetPopularity, initialViewers,
                _character.CharacterRelicManager.GetBattleRelics());
            _character.ConsumableManager.SetBattle(battle);
            VRaisingUI.Instance.SetConsumableToBattle();
        }
        
        private void OnDialogueComplete(Dialog arg0)
        {
            if (_currentEvent.Type == VEventType.Stream)
            {
                var streamEvent = _currentEvent as VStreamEvent;
                InitializeBattle(streamEvent.InitialTurnCount, streamEvent.TargetPopularity, streamEvent.InitialViewers,
                    streamEvent.MainAttributeIndex, streamEvent.AbilityTurnCounts);
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